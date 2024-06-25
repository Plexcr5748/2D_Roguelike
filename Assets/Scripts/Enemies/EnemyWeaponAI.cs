using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("�� �Ѿ��� ���� ���̾ �����ϼ���.")]
    #endregion Tooltip
    [SerializeField] private LayerMask layerMask;

    #region Tooltip
    [Tooltip("WeaponShootPosition �ڽ� ���� ������Ʈ�� Transform�� �����ϼ���.")]
    #endregion Tooltip
    [SerializeField] private Transform weaponShootPosition;

    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        // ������Ʈ �ε�
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
        // Ÿ�̸� ������Ʈ
        firingIntervalTimer -= Time.deltaTime;

        // �߻� ���� Ÿ�̸�
        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                // Ÿ�̸� �ʱ�ȭ
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// �ּҰ��� �ִ밪 ������ ������ �߻� ���� �ð� ���
    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    /// �ּҰ��� �ִ밪 ������ ������ �߻� ���� ���
    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    /// ���� �߻�
    private void FireWeapon()
    {
        // �÷��̾� ����
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        // ���� �߻� ��ġ���� �÷��̾� ���� ���� ���
        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

        // ���⿡�� �÷��̾���� ���� ���
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // ������ �÷��̾���� ���� ���
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        // �� ��ǥ ���� ����
        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        // ���� ��ǥ �̺�Ʈ ȣ��
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        // ���� ���⸦ ������ �ִ� ��쿡�� �߻�
        if (enemyDetails.enemyWeapon != null)
        {
            // ź�� ���� �Ÿ�
            float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            // �÷��̾ ���� �Ÿ� ���� �ִ��� Ȯ��
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                // �߻� ���� ���� �÷��̾ �� �� �ִ��� ���� Ȯ��
                if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

                // ���� �߻� �̺�Ʈ ȣ��
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
