using UnityEngine;

public class AmmoPattern : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("자식 총알 게임 오브젝트들을 배열에 추가합니다.")]
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

        // 총알 방향 설정
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // 총알 사정거리 설정
        ammoRange = ammoDetails.ammoRange;

        // 총알 패턴 게임 오브젝트 활성화
        gameObject.SetActive(true);

        // 모든 자식 총알 초기화
        foreach (Ammo ammo in ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }

        // 총알 충전 타이머 설정 - 발사 후 잠시 홀드하는 시간
        ammoChargeTimer = Mathf.Max(ammoDetails.ammoChargeTime, 0f);
    }

    private void Update()
    {
        // 총알 충전 효과
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }

        // 총알 이동할 거리 벡터 계산
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        // 총알 회전
        transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

        // 최대 사정거리 이후 비활성화
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }

    }

    /// 입력된 각도와 방향을 기반으로 총알 발사 방향 설정
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // 최소 및 최대 사이의 랜덤 퍼짐 각도 계산
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // 1 또는 -1을 랜덤하게 선택하여 퍼짐 각도 토글 설정
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // 퍼짐 각도로 발사 각도 조정
        fireDirectionAngle += spreadToggle * randomSpread;

        // 총알 발사 방향 설정
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// 총알 비활성화 - 객체 풀로 반환
    private void DisableAmmo()
    {
        // 총알 패턴 게임 오브젝트 비활성화
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
