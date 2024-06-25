using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS
    [Space(10)]
    [Header("사운드 이펙트 세부 정보")]
    #endregion
    #region Tooltip
    [Tooltip("사운드 이펙트의 이름")]
    #endregion
    public string soundEffectName;
    #region Tooltip
    [Tooltip("사운드 이펙트의 프리팹")]
    #endregion
    public GameObject soundPrefab;
    #region Tooltip
    [Tooltip("사운드 이펙트의 오디오 클립")]
    #endregion
    public AudioClip soundEffectClip;
    #region Tooltip
    [Tooltip("사운드 이펙트의 최소 피치 변동. 최소 및 최대 값 사이에서 랜덤 피치 변동이 생성됩니다. 랜덤 피치 변동은 사운드 이펙트를 자연스럽게 만듭니다.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;
    #region Tooltip
    [Tooltip("사운드 이펙트의 최대 피치 변동. 최소 및 최대 값 사이에서 랜덤 피치 변동이 생성됩니다. 랜덤 피치 변동은 사운드 이펙트를 자연스럽게 만듭니다.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;
    #region Tooltip
    [Tooltip("사운드 이펙트의 볼륨.")]
    #endregion
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
