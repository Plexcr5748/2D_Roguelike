using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region Header DEAL DAMAGE
    [Space(10)]
    [Header("DEAL DAMAGE")]
    #endregion
    #region Tooltip
    [Tooltip("������ ���� ������ (�����ڿ� ���� �����ǵ�)")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    #region Tooltip
    [Tooltip("���� �������� ���� ������Ʈ�� ���̾ �����մϴ�.")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    // �浹ü�� �������� �� ���� ������ Ʈ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �̹� �ٸ� �Ͱ� �浹 ���̸� ��ȯ
        if (isColliding) return;

        ContactDamage(collision);
    }

    // �浹ü ���ο� �ӹ��� �� ���� ������ Ʈ����
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �̹� �ٸ� �Ͱ� �浹 ���̸� ��ȯ
        if (isColliding) return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        // �浹 ��ü�� ������ ���̾ �������� ������ ��ȯ (��Ʈ ������ ���)
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionObjectLayerMask) == 0)
            return;

        // ���� �������� �޾ƾ� �ϴ��� Ȯ��
        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if (receiveContactDamage != null)
        {
            isColliding = true;

            // ���� �ð� �� ���� �浹 �ʱ�ȭ
            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);

            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    /// ���� �浹�� �ʱ�ȭ
    private void ResetContactCollision()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
    #endregion
}
