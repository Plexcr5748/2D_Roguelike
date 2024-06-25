
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DETAILS
    [Space(10)]
    [Header("음악 트랙 세부 정보")]
    #endregion

    #region Tooltip
    [Tooltip("음악 트랙의 이름")]
    #endregion
    public string musicName;

    #region Tooltip
    [Tooltip("음악 트랙의 오디오 클립")]
    #endregion
    public AudioClip musicClip;

    #region Tooltip
    [Tooltip("음악 트랙의 볼륨")]
    #endregion
    [Range(0, 1)]
    public float musicVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
    #endregion
}