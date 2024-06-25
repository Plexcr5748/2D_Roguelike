using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("������Ʈ ����")]
    #endregion

    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        // �⺻������ �� �浹ü�� ��Ȱ��ȭ
        doorCollider.enabled = false;

        // ������Ʈ�� �ε�
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �÷��̾� ���Ⱑ �浹�ϸ� ���� ��
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // �θ� ���� ������Ʈ�� ��Ȱ��ȭ�� ��(�÷��̾ �濡�� ����� �־��� ��),
        // �ִϸ����� ���°� �缳��. ���� �ִϸ����� ���¸� ����
        animator.SetBool(Settings.open, isOpen);
    }

    /// ���� ��
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            // �ִϸ������� ���� �Ű������� ����
            animator.SetBool(Settings.open, true);

            // �Ҹ� ȿ���� ���
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    /// ���� ���
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // �ִϸ������� ���� �Ű������� false�� �����Ͽ� ���� �ݱ�
        animator.SetBool(Settings.open, false);
    }

    /// ���� ��� ����
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        // ������ ���� ���� �־��ٸ� �ٽ� ���� ��
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
        // doorCollider�� null���� Ȯ��
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion

}