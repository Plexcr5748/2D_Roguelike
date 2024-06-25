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
    private HealthEvent healthEvent; // ü�� �̺�Ʈ
    private Health health; // ü��
    [HideInInspector] public AimWeaponEvent aimWeaponEvent; // ���� ���� �̺�Ʈ
    [HideInInspector] public FireWeaponEvent fireWeaponEvent; // ���� �߻� �̺�Ʈ
    private FireWeapon fireWeapon; // ���� �߻�
    private SetActiveWeaponEvent setActiveWeaponEvent; // Ȱ�� ���� ���� �̺�Ʈ
    private EnemyMovementAI enemyMovementAI; // �� AI
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent; // ��ġ �̵� �̺�Ʈ
    [HideInInspector] public IdleEvent idleEvent; // ��� �̺�Ʈ
    private MaterializeEffect materializeEffect; // ����ȭ ȿ��
    private CircleCollider2D circleCollider2D; // ���� �浹ü
    private PolygonCollider2D polygonCollider2D; // �ٰ��� �浹ü
    [HideInInspector] public SpriteRenderer[] spriteRendererArray; // ��������Ʈ ������ �迭
    [HideInInspector] public Animator animator; // �ִϸ�����

    private void Awake()
    {
        // ������Ʈ �ε�
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
        // ü�� �̺�Ʈ ����
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        // ü�� �̺�Ʈ ���� ����
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    /// ü�� �ս� �̺�Ʈ ó��
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    /// �� �ı� ó��
    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }


    /// �� �ʱ�ȭ
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        // �� ����ȭ
        StartCoroutine(MaterializeEnemy());
    }

    /// ���� ������ ������Ʈ ������ ����
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        // ���� ������Ʈ�� ������ ��ȣ ����
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }


    /// ���� ���� ü�� ����
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        // �ش� ���� ������ �� ü�� ��������
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

    /// ���� ���� ���� ����
    private void SetEnemyStartingWeapon()
    {
        // ���� ���⸦ ������ �ִ��� Ȯ��
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

            // ������ ���� ����
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    /// ���� �ִϸ��̼� �ӵ� ����
    private void SetEnemyAnimationSpeed()
    {
        // �ִϸ����� �ӵ��� ���� ������ �ӵ��� ����
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        // �浹ü, �̵� AI, ���� AI ��Ȱ��ȭ
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        // �浹ü, �̵� AI, ���� AI Ȱ��ȭ
        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnabled)
    {
        // �浹ü Ȱ��ȭ/��Ȱ��ȭ
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;

        // �̵� AI Ȱ��ȭ/��Ȱ��ȭ
        enemyMovementAI.enabled = isEnabled;

        // ���� �߻� Ȱ��ȭ/��Ȱ��ȭ
        fireWeapon.enabled = isEnabled;
    }
}