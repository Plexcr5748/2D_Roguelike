using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header WEAPON SHOOT EFFECT DETAILS

    #region Tooltip
    [Tooltip("Shoot 효과의 색상 그라데이션입니다. 이 그라데이션은 파티클의 수명 동안 색상을 보여줍니다 - 왼쪽에서 오른쪽으로")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("파티클 시스템이 입자를 방출하는 시간 길이입니다")]
    #endregion Tooltip
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("파티클 효과의 시작 입자 크기입니다")]
    #endregion Tooltip
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("파티클 효과의 시작 입자 속도입니다")]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("파티클의 수명입니다")]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("방출될 최대 입자 수입니다")]
    #endregion Tooltip
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("초당 발생하는 입자 수입니다. 0이면 burst 숫자만 사용됩니다")]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("파티클 효과의 폭발 중 입자 수입니다")]
    #endregion Tooltip
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("입자의 중력입니다. 작은 음수 값은 입자가 위로 떠오르게 만듭니다")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("파티클 효과의 스프라이트입니다. 지정하지 않으면 기본 파티클 스프라이트가 사용됩니다")]
    #endregion Tooltip
    public Sprite sprite;

    #region Tooltip
    [Tooltip("파티클의 수명 동안의 최소 속도입니다. 최소값과 최대값 사이에서 랜덤 값이 생성됩니다")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("파티클의 수명 동안의 최대 속도입니다. 최소값과 최대값 사이에서 랜덤 값이 생성됩니다")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("weaponShootEffectPrefab은 shoot 효과의 파티클 시스템을 포함하고 있으며 weaponShootEffect SO에 의해 구성됩니다")]
    #endregion Tooltip
    public GameObject weaponShootEffectPrefab;


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);
    }

#endif

    #endregion Validation
}
