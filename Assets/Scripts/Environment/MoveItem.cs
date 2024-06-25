using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    #region SOUND EFFECT
    [Header("���� ȿ��")]
    #endregion SOUND EFFECT
    #region Tooltip
    [Tooltip("�� �������� �̵��� �� ����Ǵ� ���� ȿ��")]
    #endregion Tooltip
    [SerializeField] private SoundEffectSO moveSoundEffect;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        // ������Ʈ ���� ��������
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        // �̵� ���� �������� ������ ��ֹ� �迭�� �߰�
        instantiatedRoom.moveableItemsList.Add(this);
    }

    /// �浹 �߻� �� ��ֹ� ��ġ ������Ʈ
    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    /// ��ֹ� ��ġ ������Ʈ
    private void UpdateObstacles()
    {
        // �������� �� ��� ���� �ӹ������� ��
        ConfineItemToRoomBounds();

        // ��ֹ� �迭�� �̵� ���� ������ ������Ʈ
        instantiatedRoom.UpdateMoveableObstacles();

        // �浹 �� �� ��ġ ĸó
        previousPosition = transform.position;

        // �̵� ���� �� �Ҹ� ��� (�ӵ��� �ణ �ִ� ���)
        if (Mathf.Abs(rigidBody2D.velocity.x) > 0.001f || Mathf.Abs(rigidBody2D.velocity.y) > 0.001f)
        {
            // �� 10�����Ӹ��� �̵� �Ҹ� ���
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
            }
        }
    }

    /// �������� �� ��� ���� �ӹ������� ��
    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;

        // �������� �� ��踦 �Ѿ�� ���� ��ġ�� ����
        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.y <= roomBounds.min.y ||
            itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }

    }

}
