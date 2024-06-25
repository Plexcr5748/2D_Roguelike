using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private HealthEvent healthEvent; // 체력 이벤트
    private Health health; // 체력
    [HideInInspector] public AimWeaponEvent aimWeaponEvent; // 무기 조준 이벤트
    [HideInInspector] public FireWeaponEvent fireWeaponEvent; // 무기 발사 이벤트
    private FireWeapon fireWeapon; // 무기 발사
    private SetActiveWeaponEvent setActiveWeaponEvent; // 활성 무기 설정 이벤트
    private EnemyMovementAI enemyMovementAI; // 적 AI
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent; // 위치 이동 이벤트
    [HideInInspector] public IdleEvent idleEvent; // 대기 이벤트
    private MaterializeEffect materializeEffect; // 물질화 효과
    private CircleCollider2D circleCollider2D; // 원형 충돌체
    private PolygonCollider2D polygonCollider2D; // 다각형 충돌체
    [HideInInspector] public SpriteRenderer[] spriteRendererArray; // 스프라이트 렌더러 배열
    [HideInInspector] public Animator animator; // 애니메이터

    private void Awake()
    {
        // 컴포넌트 로드
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 체력 이벤트 구독
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        // 체력 이벤트 구독 해제
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    /// 체력 손실 이벤트 처리
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    /// 적 파괴 처리
    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }


    /// 적 초기화
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        // 적 물질화
        StartCoroutine(MaterializeEnemy());
    }

    /// 적의 움직임 업데이트 프레임 설정
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        // 적이 업데이트할 프레임 번호 설정
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }


    /// 적의 시작 체력 설정
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        // 해당 던전 레벨의 적 체력 가져오기
        foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }
        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    /// 적의 시작 무기 설정
    private void SetEnemyStartingWeapon()
    {
        // 적이 무기를 가지고 있는지 확인
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = enemyDetails.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponClipRemainingAmmo = enemyDetails.enemyWeapon.weaponClipAmmoCapacity,
                weaponRemainingAmmo = enemyDetails.enemyWeapon.weaponAmmoCapacity,
                isWeaponReloading = false
            };

            // 적에게 무기 설정
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    /// 적의 애니메이션 속도 설정
    private void SetEnemyAnimationSpeed()
    {
        // 애니메이터 속도를 적의 움직임 속도에 맞춤
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        // 충돌체, 이동 AI, 무기 AI 비활성화
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        // 충돌체, 이동 AI, 무기 AI 활성화
        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnabled)
    {
        // 충돌체 활성화/비활성화
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;

        // 이동 AI 활성화/비활성화
        enemyMovementAI.enabled = isEnabled;

        // 무기 발사 활성화/비활성화
        fireWeapon.enabled = isEnabled;
    }
}