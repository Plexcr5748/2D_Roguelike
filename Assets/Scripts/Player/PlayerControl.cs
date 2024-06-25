using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]

public class PlayerControl : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO 스크립터블 오브젝트, 이동 속도와 같은 이동 세부 정보를 포함합니다.")]

    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float playerRollCooldownTimer = 0f;
    private bool isPlayerMovementDisabled = false;

    [HideInInspector] public bool isPlayerRolling = false;

    private void Awake()
    {
        // 컴포넌트 로드
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // Fixed Update를 기다리는 용도로 WaitForFixedUpdate 생성
        waitForFixedUpdate = new WaitForFixedUpdate();

        // 시작 무기 설정
        SetStartingWeapon();

        // 플레이어 애니메이션 속도 설정
        SetPlayerAnimationSpeed();
    }

    /// 플레이어 시작 무기 설정
    private void SetStartingWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    /// 플레이어 애니메이션 속도를 이동 속도에 맞춰 설정
    private void SetPlayerAnimationSpeed()
    {
        // 애니메이터 속도를 이동 속도에 맞춰 설정
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        // 플레이어 이동이 비활성화된 경우 반환
        if (isPlayerMovementDisabled)
            return;

        // 플레이어가 구르는 중인 경우 반환
        if (isPlayerRolling) return;

        // 플레이어 이동 입력 처리
        MovementInput();

        // 플레이어 무기 입력 처리
        WeaponInput();

        // 아이템 사용 입력 처리
        UseItemInput();

        // 플레이어 구르기 쿨타임 타이머
        PlayerRollCooldownTimer();
    }

    /// 플레이어 이동 입력
    private void MovementInput()
    {
        // 이동 입력 받기
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        // 입력을 기반으로 방향 벡터 생성
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // 대각선 이동 시 거리 조정 (피타고라스 근사치)
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        // 이동이 있는 경우 이동하거나 구르기
        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                // 이동 이벤트 호출
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            // 그렇지 않으면 구르기, 쿨다운 중이 아닌 경우에만
            else if (playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }

        }
        // 그렇지 않으면 대기 상태 이벤트 호출
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    /// 플레이어 구르기
    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    /// 플레이어 구르기 코루틴
    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        // 최소 거리, 코루틴 루프 종료 기준
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            // 고정 업데이트 기다리기
            yield return waitForFixedUpdate;

        }

        isPlayerRolling = false;

        // 쿨타임 타이머 설정
        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;

    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    /// 무기 입력
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        // 무기 조준 입력
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        // 무기 발사 입력
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        // 무기 전환 입력
        SwitchWeaponInput();

        // 무기 재장전 입력
        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // 마우스 월드 위치 가져오기
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // 무기 발사 위치에서 마우스 커서까지의 방향 벡터 계산
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        // 플레이어 위치에서 마우스 커서까지의 방향 벡터 계산
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        // 무기에서 커서까지의 각도 계산
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // 플레이어에서 커서까지의 각도 계산
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        // 플레이어 조준 방향 설정
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        // 무기 조준 이벤트 호출
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        // 왼쪽 마우스 버튼 클릭 시 발사
        if (Input.GetMouseButton(0))
        {
            // 무기 발사 이벤트 호출
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
    {
        // 마우스 휠 스크롤로 무기 전환
        if (Input.mouseScrollDelta.y < 0f)
        {
            PreviousWeapon();
        }

        if (Input.mouseScrollDelta.y > 0f)
        {
            NextWeapon();
        }

        // 숫자 키로 무기 전환
        for (int i = 1; i <= 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SetWeaponByIndex(i);
                break;
            }
        }

        // 마이너스 키로 첫 번째 무기로 설정
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetCurrentWeaponToFirstInTheList();
        }
    }


    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;

        if (currentWeaponIndex > player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        // 현재 무기가 재장전 중이면 반환
        if (currentWeapon.isWeaponReloading) return;

        // 잔여 탄약이 탄창 용량보다 적고 무한 탄약이 아니면 반환
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

        // 탄창에 탄약이 가득 찼으면 반환
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 재장전 이벤트 호출
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
    }

    /// 플레이어 주변 2 유니티 유닛 이내에서 가장 가까운 아이템 사용
    private void UseItemInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float useItemRadius = 2f;

            // 플레이어 근처의 '사용 가능' 아이템 가져오기
            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

            // 감지된 아이템을 순회하며 '사용 가능' 여부 확인
            foreach (Collider2D collider2D in collider2DArray)
            {
                IUseable iUseable = collider2D.GetComponent<IUseable>();

                if (iUseable != null)
                {
                    iUseable.UseItem();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 무언가와 충돌하면 플레이어 구르기 코루틴 중지
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 충돌 중이면 플레이어 구르기 코루틴 중지
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    /// 플레이어 이동 활성화
    public void EnablePlayer()
    {
        isPlayerMovementDisabled = false;
    }

    /// 플레이어 이동 비활성화
    public void DisablePlayer()
    {
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }

    /// 플레이어 무기 목록에서 현재 무기를 첫 번째로 설정
    private void SetCurrentWeaponToFirstInTheList()
    {
        // 새로운 임시 목록 생성
        List<Weapon> tempWeaponList = new List<Weapon>();

        // 현재 무기를 임시 목록의 첫 번째로 추가
        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        // 기존 무기 목록을 순회하며 현재 무기를 제외하고 추가
        int index = 2;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        // 새 목록 할당
        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;

        // 현재 무기 설정
        SetWeaponByIndex(currentWeaponIndex);
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif

    #endregion Validation
}
