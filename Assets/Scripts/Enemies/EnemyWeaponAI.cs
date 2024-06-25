using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("적 총알이 맞출 레이어를 선택하세요.")]
    #endregion Tooltip
    [SerializeField] private LayerMask layerMask;

    #region Tooltip
    [Tooltip("WeaponShootPosition 자식 게임 오브젝트의 Transform을 설정하세요.")]
    #endregion Tooltip
    [SerializeField] private Transform weaponShootPosition;

    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        // 컴포넌트 로드
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        // 타이머 업데이트
        firingIntervalTimer -= Time.deltaTime;

        // 발사 간격 타이머
        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                // 타이머 초기화
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// 최소값과 최대값 사이의 무작위 발사 지속 시간 계산
    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    /// 최소값과 최대값 사이의 무작위 발사 간격 계산
    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    /// 무기 발사
    private void FireWeapon()
    {
        // 플레이어 방향
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        // 무기 발사 위치에서 플레이어 방향 벡터 계산
        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

        // 무기에서 플레이어로의 각도 계산
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // 적에서 플레이어로의 각도 계산
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        // 적 목표 방향 설정
        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        // 무기 목표 이벤트 호출
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        // 적이 무기를 가지고 있는 경우에만 발사
        if (enemyDetails.enemyWeapon != null)
        {
            // 탄약 사정 거리
            float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            // 플레이어가 사정 거리 내에 있는지 확인
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                // 발사 전에 적이 플레이어를 볼 수 있는지 여부 확인
                if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

                // 무기 발사 이벤트 호출
                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange, layerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }

#endif
    #endregion Validation
}
