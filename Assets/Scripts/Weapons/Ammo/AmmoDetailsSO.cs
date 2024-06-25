using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("총알의 이름")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("총알에 사용할 스프라이트")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("총알에 사용할 프리팹을 채우세요. 배열에 여러 프리팹이 지정된 경우 배열에서 랜덤으로 선택됩니다. 프리팹은 IFireable 인터페이스를 준수해야 합니다.")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("총알에 사용할 머테리얼")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("총알이 발사된 후 움직이기 전에 잠시 '충전'해야 하는 경우, 발사 후 충전되는 시간(초)을 설정하세요.")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("총알이 충전되는 동안 사용할 머테리얼")]
    #endregion
    public Material ammoChargeMaterial;

    #region Header AMMO HIT EFFECT
    [Space(10)]
    [Header("AMMO HIT EFFECT")]
    #endregion
    #region Tooltip
    [Tooltip("총알 히트 효과 프리팹에 대한 매개변수를 정의하는 스크립터블 오브젝트")]
    #endregion
    public AmmoHitEffectSO ammoHitEffect;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    #endregion
    #region Tooltip
    [Tooltip("각 총알이 입히는 데미지")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("총알의 최소 속도 - 속도는 최소 및 최대 값 사이의 랜덤 값이 됩니다.")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("총알의 최대 속도 - 속도는 최소 및 최대 값 사이의 랜덤 값이 됩니다.")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("총알의 사정 거리(유니티 유닛 기준)")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("총알 패턴의 회전 속도(초당 각도)")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("총알의 최소 퍼짐 각도입니다. 높은 퍼짐은 정확도가 낮습니다. 최소 및 최대 값 사이의 랜덤 퍼짐이 계산됩니다.")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip("총알의 최대 퍼짐 각도입니다. 높은 퍼짐은 정확도가 낮습니다. 최소 및 최대 값 사이의 랜덤 퍼짐이 계산됩니다.")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("한 번에 발사되는 최소 총알 수입니다. 최소 및 최대 값 사이의 랜덤 총알 수가 발사됩니다.")]
    #endregion
    public int ammoSpawnAmountMin = 1;
    #region Tooltip
    [Tooltip("한 번에 발사되는 최대 총알 수입니다. 최소 및 최대 값 사이의 랜덤 총알 수가 발사됩니다.")]
    #endregion
    public int ammoSpawnAmountMax = 1;
    #region Tooltip
    [Tooltip("최소 발사 간격 시간입니다. 최소 및 최대 값 사이의 랜덤 시간 간격(초)이 지정됩니다.")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("최대 발사 간격 시간입니다. 최소 및 최대 값 사이의 랜덤 시간 간격(초)이 지정됩니다.")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;


    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("총알에 총알 궤적이 필요한 경우 선택하세요. 선택한 경우 다음 총알 궤적 값이 채워져 있어야 합니다.")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("총알 궤적의 지속 시간(초)")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("총알 궤적의 머테리얼")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("총알 궤적의 시작 너비")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("총알 궤적의 끝 너비")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    // 입력된 스크립터블 오브젝트 세부 사항 유효성 검사
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }

#endif
    #endregion
}
