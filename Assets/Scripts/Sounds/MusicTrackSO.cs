
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DETAILS
    [Space(10)]
    [Header("���� Ʈ�� ���� ����")]
    #endregion

    #region Tooltip
    [Tooltip("���� Ʈ���� �̸�")]
    #endregion
    public string musicName;

    #region Tooltip
    [Tooltip("���� Ʈ���� ����� Ŭ��")]
    #endregion
    public AudioClip musicClip;

    #region Tooltip
    [Tooltip("���� Ʈ���� ����")]
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