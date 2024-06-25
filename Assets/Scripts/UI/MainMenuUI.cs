using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("���� ���� ���� ��ư ���� ������Ʈ�� �����ϼ���.")]
    #endregion Tooltip
    [SerializeField] private GameObject playButton;
    #region Tooltip
    [Tooltip("���� ���� ��ư ���� ������Ʈ�� �����ϼ���.")]
    #endregion
    [SerializeField] private GameObject quitButton;
    #region Tooltip
    [Tooltip("���̽��ھ� ��ư ���� ������Ʈ�� �����ϼ���.")]
    #endregion
    [SerializeField] private GameObject highScoresButton;
    #region Tooltip
    [Tooltip("���� ���� ��ư ���� ������Ʈ�� �����ϼ���.")]
    #endregion
    [SerializeField] private GameObject instructionsButton;
    #region Tooltip
    [Tooltip("���� �޴��� ���ư��� ��ư ���� ������Ʈ�� �����ϼ���.")]
    #endregion
    [SerializeField] private GameObject returnToMainMenuButton;
    private bool isInstructionSceneLoaded = false;
    private bool isHighScoresSceneLoaded = false;

    private void Start()
    {
        // ���� ���
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        // ĳ���� ���� ���� �߰��� �ε�
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }


    /// ���� ���� ��ư Ŭ�� �� ȣ���
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }


    /// ���̽��ھ� ��ư Ŭ�� �� ȣ���
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoresButton.SetActive(false);
        instructionsButton.SetActive(false);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        // ���̽��ھ� ���� �߰��� �ε�
        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    /// ���� �޴��� ���ư��� ��ư Ŭ�� �� ȣ���
    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);
        highScoresButton.SetActive(true);
        instructionsButton.SetActive(true);

        // ĳ���� ���� ���� �߰��� �ε�
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    /// ���� ���� ��ư Ŭ�� �� ȣ���
    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoresButton.SetActive(false);
        instructionsButton.SetActive(false);
        isInstructionSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        // ���� ���� �߰��� �ε�
        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    /// ���� ���� - �� �޼���� �ν����Ϳ��� ������ onClick �̺�Ʈ���� ȣ���
    public void QuitGame()
    {
        Application.Quit();
    }


    #region Validation
#if UNITY_EDITOR
    // ��ũ���ͺ� ������Ʈ ���� ������ ��ȿ�� �˻�
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoresButton), highScoresButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
    #endregion
}