using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor; // 재질화 효과에 사용할 색상
    [SerializeField] private float materializeTime = 3f; // 상자가 재질화되는 시간
    [SerializeField] private Transform itemSpawnPoint; // 아이템 생성 지점

    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 캐싱
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 캐싱
        materializeEffect = GetComponent<MaterializeEffect>(); // 재질화 효과 컴포넌트 캐싱
        messageTextTMP = GetComponentInChildren<TextMeshPro>(); // 자식 오브젝트에서 TextMeshPro 컴포넌트 캐싱
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
    {
        this.healthPercent = healthPercent; // 초기화: 체력 백분율
        this.weaponDetails = weaponDetails; // 초기화: 무기 세부 정보
        this.ammoPercent = ammoPercent; // 초기화: 탄약 백분율

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest()); // 상자를 재질화
        }
        else
        {
            EnableChest(); // 상자를 활성화
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest(); // 상자 활성화
    }

    private void EnableChest()
    {
        isEnabled = true; // 사용 가능 상태로 설정
    }

    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;

            case ChestState.healthItem:
                CollectHealthItem();
                break;

            case ChestState.ammoItem:
                CollectAmmoItem();
                break;

            case ChestState.weaponItem:
                CollectWeaponItem();
                break;

            case ChestState.empty:
                return;

            default:
                return;
        }
    }

    private void OpenChest()
    {
        animator.SetBool(Settings.use, true); // 사용 애니메이션 재생

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen); // 상자 열기 효과음 재생

        if (weaponDetails != null)
        {
            // 플레이어가 이미 무기를 보유하고 있는지 확인하고, 보유 중이라면 null로 설정
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
                weaponDetails = null;
        }

        UpdateChestState(); // 상자 상태 업데이트
    }

    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem; // 체력 아이템 상태로 설정
            InstantiateHealthItem(); // 체력 아이템 생성
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem; // 탄약 아이템 상태로 설정
            InstantiateAmmoItem(); // 탄약 아이템 생성
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem; // 무기 아이템 상태로 설정
            InstantiateWeaponItem(); // 무기 아이템 생성
        }
        else
        {
            chestState = ChestState.empty; // 비어있는 상태로 설정
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform); // 상자 아이템 생성
        chestItem = chestItemGameObject.GetComponent<ChestItem>(); // 상자 아이템 컴포넌트 가져오기
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem(); // 아이템 생성
        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor); // 체력 아이템 초기화
    }

    private void CollectHealthItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // 아이템 존재 여부 확인

        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent); // 플레이어 체력 추가
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup); // 체력 획득 효과음 재생
        healthPercent = 0; // 체력 초기화
        Destroy(chestItemGameObject); // 상자 아이템 파괴
        UpdateChestState(); // 상자 상태 업데이트
    }

    private void InstantiateAmmoItem()
    {
        InstantiateItem(); // 아이템 생성
        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor); // 탄약 아이템 초기화
    }

    private void CollectAmmoItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // 아이템 존재 여부 확인

        Player player = GameManager.Instance.GetPlayer();
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent); // 현재 무기의 탄약 업데이트
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup); // 탄약 획득 효과음 재생
        ammoPercent = 0; // 탄약 초기화
        Destroy(chestItemGameObject); // 상자 아이템 파괴
        UpdateChestState(); // 상자 상태 업데이트
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem(); // 아이템 생성
        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor); // 무기 아이템 초기화
    }

    private void CollectWeaponItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // 아이템 존재 여부 확인

        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails); // 플레이어에게 무기 추가
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup); // 무기 획득 효과음 재생
        }
        else
        {
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f)); // 이미 무기 장착되어 있음 메시지 표시
        }
        weaponDetails = null; // 무기 초기화
        Destroy(chestItemGameObject); // 상자 아이템 파괴
        UpdateChestState(); // 상자 상태 업데이트
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText; // 메시지 텍스트 설정

        yield return new WaitForSeconds(messageDisplayTime); // 일정 시간 후

        messageTextTMP.text = ""; // 메시지 텍스트 초기화
    }
}