using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("자식 TrailRenderer 컴포넌트로 채웁니다.")]
    #endregion Tooltip
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; // 각 총알의 사정 거리
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
        // 스프라이트 렌더러 캐시
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 총알 충전 효과
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

        // 이동이 재정의된 경우 총알을 움직이지 X - 예: 총알 패턴의 일부인 경우
        if (!overrideAmmoMovement)
        {
            // 총알을 이동할 거리 벡터 계산
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;

            // 최대 사정 거리 도달 시 비활성화
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    // 배율 없음
                    StaticEventHandler.CallMultiplierEvent(false);
                }

                DisableAmmo();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 충돌 중인 경우 반환
        if (isColliding) return;

        // 충돌 객체에 대한 데미지 처리
        DealDamage(collision);

        // 총알 히트 효과 표시
        AmmoHitEffect();

        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        bool enemyHit = false;

        if (health != null)
        {
            // 총알이 여러 번 데미지를 입히지 않도록 isColliding 설정
            isColliding = true;

            health.TakeDamage(ammoDetails.ammoDamage);

            // 적이 맞았을 경우
            if (health.enemy != null)
            {
                enemyHit = true;
            }
        }

        // 플레이어 총알인 경우 배율 업데이트
        if (ammoDetails.isPlayerAmmo)
        {
            if (enemyHit)
            {
                // 배율
                StaticEventHandler.CallMultiplierEvent(true);
            }
            else
            {
                // 배율 없음
                StaticEventHandler.CallMultiplierEvent(false);
            }
        }

    }


    /// 발사된 총알 초기화 - ammodetails, aimangle, weaponAngle 및 weaponAimDirectionVector를 사용
    /// 이 총알이 패턴의 일부인 경우 overrideAmmoMovement를 true로 설정하여 총알 이동을 재정의할 수 있음
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;

        // isColliding 초기화
        isColliding = false;

        // 발사 방향 설정
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // 총알 스프라이트 설정
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // 총알 충전 시간이 있는 경우 초기 총알 재료 설정
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // 총알 충전 타이머 설정
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

        // 총알 사정 거리 설정
        ammoRange = ammoDetails.ammoRange;

        // 총알 속도 설정
        this.ammoSpeed = ammoSpeed;

        // 총알 이동 재정의
        this.overrideAmmoMovement = overrideAmmoMovement;

        // 총알 게임 오브젝트 활성화
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

    /// 입력된 각도와 방향에 따라 총알 발사 방향과 각도 설정
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // 최소에서 최대 사이의 랜덤 퍼짐 각도 계산
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // 1 또는 -1의 랜덤 퍼짐 토글 가져오기
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // 랜덤 퍼짐으로 총알 발사 각도 조정
        fireDirectionAngle += spreadToggle * randomSpread;

        // 총알 회전 설정
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // 총알 발사 방향 설정
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);

    }

    /// 총알 비활성화 - 객체 풀로 반환
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// 총알 히트 효과 표시
    private void AmmoHitEffect()
    {
        // 히트 효과가 지정된 경우 처리
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            // 풀에서 히트 효과 게임 오브젝트 가져오기 (파티클 시스템 구성 요소 포함)
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

            // 히트 효과 설정
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);

            // 게임 오브젝트 활성화 (파티클 시스템은 완료되면 자동으로 비활성화)
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
