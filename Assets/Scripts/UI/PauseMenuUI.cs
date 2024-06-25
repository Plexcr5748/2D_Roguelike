using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("음악 볼륨 레벨을 설정하세요.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI musicLevelText;
    #region Tooltip
    [Tooltip("사운드 볼륨 레벨을 설정하세요.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI soundsLevelText;

    private void Start()
    {
        // 일단 일시 정지 메뉴를 숨김
        gameObject.SetActive(false);
    }

    /// UI 텍스트 초기화
    private IEnumerator InitializeUI()
    {
        // 이전의 음악과 사운드 레벨이 설정된 것을 확인하기 위해 한 프레임 기다림
        yield return null;

        // UI 텍스트 초기화
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        // UI 텍스트 초기화
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    // 메인 메뉴로 나가기 - 일시 정지 메뉴 UI 버튼에서 호출됨
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// 음악 볼륨 증가 - UI의 음악 볼륨 증가 버튼에서 호출됨
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// 음악 볼륨 감소 - UI의 음악 볼륨 감소 버튼에서 호출됨
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// 사운드 볼륨 증가 - UI의 사운드 볼륨 증가 버튼에서 호출됨
    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    /// 사운드 볼륨 감소 - UI의 사운드 볼륨 감소 버튼에서 호출됨
    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }


    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
    }

#endif
    #endregion Validation
}