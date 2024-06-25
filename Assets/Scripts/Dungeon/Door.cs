using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("오브젝트 참조")]
    #endregion

    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        // 기본적으로 문 충돌체를 비활성화
        doorCollider.enabled = false;

        // 컴포넌트를 로드
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어나 플레이어 무기가 충돌하면 문을 염
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // 부모 게임 오브젝트가 비활성화될 때(플레이어가 방에서 충분히 멀어질 때),
        // 애니메이터 상태가 재설정. 따라서 애니메이터 상태를 복원
        animator.SetBool(Settings.open, isOpen);
    }

    /// 문을 염
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            // 애니메이터의 열림 매개변수를 설정
            animator.SetBool(Settings.open, true);

            // 소리 효과를 재생
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    /// 문을 잠금
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // 애니메이터의 열림 매개변수를 false로 설정하여 문을 닫기
        animator.SetBool(Settings.open, false);
    }

    /// 문을 잠금 해제
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        // 이전에 문이 열려 있었다면 다시 문을 염
        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // doorCollider가 null인지 확인
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion

}