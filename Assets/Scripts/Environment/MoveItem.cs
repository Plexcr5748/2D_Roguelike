using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    #region SOUND EFFECT
    [Header("사운드 효과")]
    #endregion SOUND EFFECT
    #region Tooltip
    [Tooltip("이 아이템을 이동할 때 재생되는 사운드 효과")]
    #endregion Tooltip
    [SerializeField] private SoundEffectSO moveSoundEffect;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        // 컴포넌트 참조 가져오기
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        // 이동 가능 아이템을 아이템 장애물 배열에 추가
        instantiatedRoom.moveableItemsList.Add(this);
    }

    /// 충돌 발생 시 장애물 위치 업데이트
    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    /// 장애물 위치 업데이트
    private void UpdateObstacles()
    {
        // 아이템이 방 경계 내에 머무르도록 함
        ConfineItemToRoomBounds();

        // 장애물 배열의 이동 가능 아이템 업데이트
        instantiatedRoom.UpdateMoveableObstacles();

        // 충돌 후 새 위치 캡처
        previousPosition = transform.position;

        // 이동 중일 때 소리 재생 (속도가 약간 있는 경우)
        if (Mathf.Abs(rigidBody2D.velocity.x) > 0.001f || Mathf.Abs(rigidBody2D.velocity.y) > 0.001f)
        {
            // 매 10프레임마다 이동 소리 재생
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
            }
        }
    }

    /// 아이템이 방 경계 내에 머무르도록 함
    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;

        // 아이템이 방 경계를 넘어가면 이전 위치로 설정
        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.y <= roomBounds.min.y ||
            itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }

    }

}
