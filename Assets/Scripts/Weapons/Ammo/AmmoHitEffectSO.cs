using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    #region Header AMMO HIT EFFECT DETAILS
    [Space(10)]
    [Header("AMMO HIT EFFECT DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("히트 이펙트의 색상 그래디언트입니다. 이 그래디언트는 파티클의 수명 동안의 색상을 보여줍니다 - 왼쪽에서 오른쪽으로")]
    #endregion
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("파티클 시스템이 입자를 방출하는 시간")]
    #endregion
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("파티클 효과의 시작 입자 크기")]
    #endregion
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("파티클 효과의 시작 입자 속도")]
    #endregion
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("파티클의 수명")]
    #endregion
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("방출될 최대 입자 수")]
    #endregion
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("초당 방출되는 입자 수입니다. 0이면 버스트 번호만 사용됩니다.")]
    #endregion
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("파티클 효과 버스트당 방출될 입자 수")]
    #endregion
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("입자에 작용하는 중력 - 작은 음수 값은 입자가 위로 떠오르게 합니다.")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("파티클 효과의 스프라이트입니다. 지정되지 않은 경우 기본 입자 스프라이트가 사용됩니다.")]
    #endregion
    public Sprite sprite;

    #region Tooltip
    [Tooltip("파티클의 생애 동안의 최소 속도 범위입니다. 최소값과 최대값 사이의 랜덤 값이 생성됩니다.")]
    #endregion
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("파티클의 생애 동안의 최대 속도 범위입니다. 최소값과 최대값 사이의 랜덤 값이 생성됩니다.")]
    #endregion
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("히트 이펙트 파티클 시스템을 포함하는 프리팹입니다 - 해당하는 ammoHitEffectSO가 정의되어야 합니다.")]
    #endregion
    public GameObject ammoHitEffectPrefab;

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
    }
#endif
}