using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("테이블의 질량. 푸시할 때 이동 속도를 제어합니다.")]
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
            // 아이템의 충돌체 경계 가져오기
            Bounds bounds = boxCollider2D.bounds;

            // 플레이어와 가장 가까운 지점 계산
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            // 플레이어가 테이블의 오른쪽에 있으면 왼쪽으로 뒤집기
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }

            // 플레이어가 테이블의 왼쪽에 있으면 오른쪽으로 뒤집기
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            // 플레이어가 테이블 아래에 있으면 위로 뒤집기
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            else
            {
                animator.SetBool(Settings.flipDown, true);
            }

            // 레이어를 Environment로 설정 - 총알이 이제 테이블과 충돌함.
            gameObject.layer = LayerMask.NameToLayer("Environment");

            // 물체의 질량을 지정된 양으로 설정하여 플레이어가 아이템을 움직일 수 있게 함
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