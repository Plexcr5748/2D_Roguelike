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
    [Tooltip("적용할 접촉 데미지 (수신자에 의해 재정의됨)")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    #region Tooltip
    [Tooltip("접촉 데미지를 받을 오브젝트의 레이어를 지정합니다.")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    // 충돌체에 진입했을 때 접촉 데미지 트리거
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 다른 것과 충돌 중이면 반환
        if (isColliding) return;

        ContactDamage(collision);
    }

    // 충돌체 내부에 머무를 때 접촉 데미지 트리거
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 이미 다른 것과 충돌 중이면 반환
        if (isColliding) return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        // 충돌 객체가 지정된 레이어에 속해있지 않으면 반환 (비트 연산을 사용)
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionObjectLayerMask) == 0)
            return;

        // 접촉 데미지를 받아야 하는지 확인
        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if (receiveContactDamage != null)
        {
            isColliding = true;

            // 일정 시간 후 접촉 충돌 초기화
            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);

            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    /// 접촉 충돌을 초기화
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
