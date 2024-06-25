using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("적 기본 정보")]
    #endregion

    #region Tooltip
    [Tooltip("적의 이름")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("적의 프리팹")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [Tooltip("적이 플레이어를 쫓기 시작하는 거리")]
    #endregion
    public float chaseDistance = 50f;

    #region Header ENEMY MATERIAL
    [Space(10)]
    [Header("적의 재질")]
    #endregion
    #region Tooltip
    [Tooltip("적이 물질화 후 사용하는 표준 라이트 쉐이더 재질")]
    #endregion
    public Material enemyStandardMaterial;

    #region Header ENEMY MATERIALIZE SETTINGS
    [Space(10)]
    [Header("적 물질화 설정")]
    #endregion
    #region Tooltip
    [Tooltip("적이 물질화하는 데 걸리는 시간(초)")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("적이 물질화할 때 사용되는 쉐이더")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("적이 물질화할 때 사용할 색상. HDR 색상이므로 강도를 조절하여 빛이나 반짝임을 표현할 수 있습니다.")]
    #endregion
    public Color enemyMaterializeColor;

    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("적 무기 설정")]
    #endregion
    #region Tooltip
    [Tooltip("적의 무기 - 무기가 없으면 없음")]
    #endregion
    public WeaponDetailsSO enemyWeapon;
    #region Tooltip
    [Tooltip("적이 발사하는 연사 간격의 최소 시간(초). 이 값은 0보다 커야 합니다. 최소 값과 최대 값 사이의 임의의 값이 선택됩니다.")]
    #endregion
    public float firingIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("적이 발사하는 연사 간격의 최대 시간(초). 최소 값과 최대 값 사이의 임의의 값이 선택됩니다.")]
    #endregion
    public float firingIntervalMax = 1f;
    #region Tooltip
    [Tooltip("적이 발사하는 연사 지속 시간의 최소 값(초). 이 값은 0보다 커야 합니다. 최소 값과 최대 값 사이의 임의의 값이 선택됩니다.")]
    #endregion
    public float firingDurationMin = 1f;
    #region Tooltip
    [Tooltip("적이 발사하는 연사 지속 시간의 최대 값(초). 최소 값과 최대 값 사이의 임의의 값이 선택됩니다.")]
    #endregion
    public float firingDurationMax = 2f;
    #region Tooltip
    [Tooltip("플레이어의 시야가 필요한지 여부를 선택하여 적이 발사하기 전에 플레이어의 시야를 확인합니다. 시야가 필요하지 않으면 적은 장애물을 무시하고 플레이어가 '범위 내'에 있으면 발사합니다.")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY HEALTH
    [Space(10)]
    [Header("적 체력")]
    #endregion
    #region Tooltip
    [Tooltip("각 레벨에 대한 적의 체력")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;
    #region Tooltip
    [Tooltip("적이 적중 후 바로 면역 기간이 있는지 여부를 선택합니다. 면역 시간(초)을 다음 필드에 지정합니다.")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("적이 적중 후 면역되는 시간(초)")]
    #endregion
    public float hitImmunityTime;
    #region Tooltip
    [Tooltip("적의 체력 바를 표시할지 여부를 선택합니다.")]
    #endregion
    public bool isHealthBarDisplayed = false;



    #region Validation
#if UNITY_EDITOR
    // 스크립터블 오브젝트에 입력된 세부 정보 유효성 검사
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }

#endif
    #endregion

}