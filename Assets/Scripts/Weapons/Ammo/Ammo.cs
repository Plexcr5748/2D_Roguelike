using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("�ڽ� TrailRenderer ������Ʈ�� ä��ϴ�.")]
    #endregion Tooltip
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; // �� �Ѿ��� ���� �Ÿ�
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false;

    private void Awake()
    {
        // ��������Ʈ ������ ĳ��
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // �Ѿ� ���� ȿ��
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // �̵��� �����ǵ� ��� �Ѿ��� �������� X - ��: �Ѿ� ������ �Ϻ��� ���
        if (!overrideAmmoMovement)
        {
            // �Ѿ��� �̵��� �Ÿ� ���� ���
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;

            // �ִ� ���� �Ÿ� ���� �� ��Ȱ��ȭ
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    // ���� ����
                    StaticEventHandler.CallMultiplierEvent(false);
                }

                DisableAmmo();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �̹� �浹 ���� ��� ��ȯ
        if (isColliding) return;

        // �浹 ��ü�� ���� ������ ó��
        DealDamage(collision);

        // �Ѿ� ��Ʈ ȿ�� ǥ��
        AmmoHitEffect();

        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        bool enemyHit = false;

        if (health != null)
        {
            // �Ѿ��� ���� �� �������� ������ �ʵ��� isColliding ����
            isColliding = true;

            health.TakeDamage(ammoDetails.ammoDamage);

            // ���� �¾��� ���
            if (health.enemy != null)
            {
                enemyHit = true;
            }
        }

        // �÷��̾� �Ѿ��� ��� ���� ������Ʈ
        if (ammoDetails.isPlayerAmmo)
        {
            if (enemyHit)
            {
                // ����
                StaticEventHandler.CallMultiplierEvent(true);
            }
            else
            {
                // ���� ����
                StaticEventHandler.CallMultiplierEvent(false);
            }
        }

    }


    /// �߻�� �Ѿ� �ʱ�ȭ - ammodetails, aimangle, weaponAngle �� weaponAimDirectionVector�� ���
    /// �� �Ѿ��� ������ �Ϻ��� ��� overrideAmmoMovement�� true�� �����Ͽ� �Ѿ� �̵��� �������� �� ����
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;

        // isColliding �ʱ�ȭ
        isColliding = false;

        // �߻� ���� ����
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // �Ѿ� ��������Ʈ ����
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // �Ѿ� ���� �ð��� �ִ� ��� �ʱ� �Ѿ� ��� ����
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // �Ѿ� ���� Ÿ�̸� ����
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // �Ѿ� ���� �Ÿ� ����
        ammoRange = ammoDetails.ammoRange;

        // �Ѿ� �ӵ� ����
        this.ammoSpeed = ammoSpeed;

        // �Ѿ� �̵� ������
        this.overrideAmmoMovement = overrideAmmoMovement;

        // �Ѿ� ���� ������Ʈ Ȱ��ȭ
        gameObject.SetActive(true);

        #endregion Ammo


        #region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Trail

    }

    /// �Էµ� ������ ���⿡ ���� �Ѿ� �߻� ����� ���� ����
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // �ּҿ��� �ִ� ������ ���� ���� ���� ���
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // 1 �Ǵ� -1�� ���� ���� ��� ��������
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // ���� �������� �Ѿ� �߻� ���� ����
        fireDirectionAngle += spreadToggle * randomSpread;

        // �Ѿ� ȸ�� ����
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // �Ѿ� �߻� ���� ����
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);

    }

    /// �Ѿ� ��Ȱ��ȭ - ��ü Ǯ�� ��ȯ
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// �Ѿ� ��Ʈ ȿ�� ǥ��
    private void AmmoHitEffect()
    {
        // ��Ʈ ȿ���� ������ ��� ó��
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            // Ǯ���� ��Ʈ ȿ�� ���� ������Ʈ �������� (��ƼŬ �ý��� ���� ��� ����)
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

            // ��Ʈ ȿ�� ����
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);

            // ���� ������Ʈ Ȱ��ȭ (��ƼŬ �ý����� �Ϸ�Ǹ� �ڵ����� ��Ȱ��ȭ)
            ammoHitEffect.gameObject.SetActive(true);
        }
    }


    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }

#endif
    #endregion Validation

}
