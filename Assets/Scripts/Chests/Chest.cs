using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor; // ����ȭ ȿ���� ����� ����
    [SerializeField] private float materializeTime = 3f; // ���ڰ� ����ȭ�Ǵ� �ð�
    [SerializeField] private Transform itemSpawnPoint; // ������ ���� ����

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
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ĳ��
        spriteRenderer = GetComponent<SpriteRenderer>(); // ��������Ʈ ������ ������Ʈ ĳ��
        materializeEffect = GetComponent<MaterializeEffect>(); // ����ȭ ȿ�� ������Ʈ ĳ��
        messageTextTMP = GetComponentInChildren<TextMeshPro>(); // �ڽ� ������Ʈ���� TextMeshPro ������Ʈ ĳ��
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
    {
        this.healthPercent = healthPercent; // �ʱ�ȭ: ü�� �����
        this.weaponDetails = weaponDetails; // �ʱ�ȭ: ���� ���� ����
        this.ammoPercent = ammoPercent; // �ʱ�ȭ: ź�� �����

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest()); // ���ڸ� ����ȭ
        }
        else
        {
            EnableChest(); // ���ڸ� Ȱ��ȭ
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest(); // ���� Ȱ��ȭ
    }

    private void EnableChest()
    {
        isEnabled = true; // ��� ���� ���·� ����
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
        animator.SetBool(Settings.use, true); // ��� �ִϸ��̼� ���

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen); // ���� ���� ȿ���� ���

        if (weaponDetails != null)
        {
            // �÷��̾ �̹� ���⸦ �����ϰ� �ִ��� Ȯ���ϰ�, ���� ���̶�� null�� ����
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
                weaponDetails = null;
        }

        UpdateChestState(); // ���� ���� ������Ʈ
    }

    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem; // ü�� ������ ���·� ����
            InstantiateHealthItem(); // ü�� ������ ����
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem; // ź�� ������ ���·� ����
            InstantiateAmmoItem(); // ź�� ������ ����
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem; // ���� ������ ���·� ����
            InstantiateWeaponItem(); // ���� ������ ����
        }
        else
        {
            chestState = ChestState.empty; // ����ִ� ���·� ����
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform); // ���� ������ ����
        chestItem = chestItemGameObject.GetComponent<ChestItem>(); // ���� ������ ������Ʈ ��������
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem(); // ������ ����
        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor); // ü�� ������ �ʱ�ȭ
    }

    private void CollectHealthItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // ������ ���� ���� Ȯ��

        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent); // �÷��̾� ü�� �߰�
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup); // ü�� ȹ�� ȿ���� ���
        healthPercent = 0; // ü�� �ʱ�ȭ
        Destroy(chestItemGameObject); // ���� ������ �ı�
        UpdateChestState(); // ���� ���� ������Ʈ
    }

    private void InstantiateAmmoItem()
    {
        InstantiateItem(); // ������ ����
        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor); // ź�� ������ �ʱ�ȭ
    }

    private void CollectAmmoItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // ������ ���� ���� Ȯ��

        Player player = GameManager.Instance.GetPlayer();
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent); // ���� ������ ź�� ������Ʈ
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup); // ź�� ȹ�� ȿ���� ���
        ammoPercent = 0; // ź�� �ʱ�ȭ
        Destroy(chestItemGameObject); // ���� ������ �ı�
        UpdateChestState(); // ���� ���� ������Ʈ
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem(); // ������ ����
        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor); // ���� ������ �ʱ�ȭ
    }

    private void CollectWeaponItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return; // ������ ���� ���� Ȯ��

        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails); // �÷��̾�� ���� �߰�
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup); // ���� ȹ�� ȿ���� ���
        }
        else
        {
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f)); // �̹� ���� �����Ǿ� ���� �޽��� ǥ��
        }
        weaponDetails = null; // ���� �ʱ�ȭ
        Destroy(chestItemGameObject); // ���� ������ �ı�
        UpdateChestState(); // ���� ���� ������Ʈ
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText; // �޽��� �ؽ�Ʈ ����

        yield return new WaitForSeconds(messageDisplayTime); // ���� �ð� ��

        messageTextTMP.text = ""; // �޽��� �ؽ�Ʈ �ʱ�ȭ
    }
}