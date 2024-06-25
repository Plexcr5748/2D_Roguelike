using UnityEngine;

public class AmmoPattern : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("�ڽ� �Ѿ� ���� ������Ʈ���� �迭�� �߰��մϴ�.")]
    #endregion
    [SerializeField] private Ammo[] ammoArray;

    private float ammoRange;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement)
    {
        this.ammoDetails = ammoDetails;
        this.ammoSpeed = ammoSpeed;

        // �Ѿ� ���� ����
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // �Ѿ� �����Ÿ� ����
        ammoRange = ammoDetails.ammoRange;

        // �Ѿ� ���� ���� ������Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        // ��� �ڽ� �Ѿ� �ʱ�ȭ
        foreach (Ammo ammo in ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }

        // �Ѿ� ���� Ÿ�̸� ���� - �߻� �� ��� Ȧ���ϴ� �ð�
        ammoChargeTimer = Mathf.Max(ammoDetails.ammoChargeTime, 0f);
    }

    private void Update()
    {
        // �Ѿ� ���� ȿ��
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }

        // �Ѿ� �̵��� �Ÿ� ���� ���
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        // �Ѿ� ȸ��
        transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

        // �ִ� �����Ÿ� ���� ��Ȱ��ȭ
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }

    }

    /// �Էµ� ������ ������ ������� �Ѿ� �߻� ���� ����
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // �ּ� �� �ִ� ������ ���� ���� ���� ���
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // 1 �Ǵ� -1�� �����ϰ� �����Ͽ� ���� ���� ��� ����
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // ���� ������ �߻� ���� ����
        fireDirectionAngle += spreadToggle * randomSpread;

        // �Ѿ� �߻� ���� ����
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// �Ѿ� ��Ȱ��ȭ - ��ü Ǯ�� ��ȯ
    private void DisableAmmo()
    {
        // �Ѿ� ���� ���� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
    }
#endif
    #endregion
}
