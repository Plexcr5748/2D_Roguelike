using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("���̺��� ����. Ǫ���� �� �̵� �ӵ��� �����մϴ�.")]
    #endregion
    [SerializeField] private float itemMass;
    private BoxCollider2D boxCollider2D;
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (!itemUsed)
        {
            // �������� �浹ü ��� ��������
            Bounds bounds = boxCollider2D.bounds;

            // �÷��̾�� ���� ����� ���� ���
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            // �÷��̾ ���̺��� �����ʿ� ������ �������� ������
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }

            // �÷��̾ ���̺��� ���ʿ� ������ ���������� ������
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            // �÷��̾ ���̺� �Ʒ��� ������ ���� ������
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            else
            {
                animator.SetBool(Settings.flipDown, true);
            }

            // ���̾ Environment�� ���� - �Ѿ��� ���� ���̺�� �浹��.
            gameObject.layer = LayerMask.NameToLayer("Environment");

            // ��ü�� ������ ������ ������ �����Ͽ� �÷��̾ �������� ������ �� �ְ� ��
            rigidBody2D.mass = itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlip);

            itemUsed = true;

        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion
}