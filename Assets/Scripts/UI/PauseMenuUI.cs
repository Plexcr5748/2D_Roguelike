using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("���� ���� ������ �����ϼ���.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI musicLevelText;
    #region Tooltip
    [Tooltip("���� ���� ������ �����ϼ���.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI soundsLevelText;

    private void Start()
    {
        // �ϴ� �Ͻ� ���� �޴��� ����
        gameObject.SetActive(false);
    }

    /// UI �ؽ�Ʈ �ʱ�ȭ
    private IEnumerator InitializeUI()
    {
        // ������ ���ǰ� ���� ������ ������ ���� Ȯ���ϱ� ���� �� ������ ��ٸ�
        yield return null;

        // UI �ؽ�Ʈ �ʱ�ȭ
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        // UI �ؽ�Ʈ �ʱ�ȭ
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    // ���� �޴��� ������ - �Ͻ� ���� �޴� UI ��ư���� ȣ���
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// ���� ���� ���� - UI�� ���� ���� ���� ��ư���� ȣ���
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// ���� ���� ���� - UI�� ���� ���� ���� ��ư���� ȣ���
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// ���� ���� ���� - UI�� ���� ���� ���� ��ư���� ȣ���
    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    /// ���� ���� ���� - UI�� ���� ���� ���� ��ư���� ȣ���
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