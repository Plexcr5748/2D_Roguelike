using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("���� ������Ʈ ����")]
    #endregion Header GAMEOBJECT REFERENCES

    #region Tooltip

    [Tooltip("���� �������� �Ͻ� ���� �޴� ���ӿ�����Ʈ�� ä��ϴ�")]

    #endregion Tooltip

    [SerializeField] private GameObject pauseMenu;

    #region Tooltip
    [Tooltip("FadeScreenUI�� MessageText TextMeshPro ������Ʈ�� ä��ϴ�")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextTMP;

    #region Tooltip
    [Tooltip("FadeScreenUI�� FadeImage CanvasGroup ������Ʈ�� ä��ϴ�")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;

    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("���� ����")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("���� ���� ScriptableObject�� ä��ϴ�")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("�׽�Ʈ�� ���� ���� ���� ������ ä��ϴ�, ù ��° ���� = 0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;
    private bool isFading = false;

    protected override void Awake()
    {
        // ���̽� Ŭ���� ȣ��
        base.Awake();

        // �÷��̾� ���� ���� - ���� �޴����� ����� ���� �÷��̾� ScriptableObject����
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // �÷��̾� ����
        InstantiatePlayer();

    }

    /// ������ �÷��̾ ����
    private void InstantiatePlayer()
    {
        // �÷��̾� ����
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // �÷��̾� �ʱ�ȭ
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);

    }

    private void OnEnable()
    {
        // �� ���� �̺�Ʈ�� ����
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // ���� ���� ��� óġ�� �̺�Ʈ�� ����
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        // ���� ȹ�� �̺�Ʈ�� ����
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        // ���� ��� �̺�Ʈ�� ����
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        // �÷��̾� �ı� �̺�Ʈ�� ����
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        // �� ���� �̺�Ʈ ������ ����
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // ���� ���� ��� óġ�� �̺�Ʈ ������ ����
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        // ���� ȹ�� �̺�Ʈ ������ ����
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        // ���� ��� �̺�Ʈ ������ ����
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;

        // �÷��̾� �ı� �̺�Ʈ ������ ����
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;

    }

    /// �� ���� �̺�Ʈ�� ó���մϴ�.
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    /// ���� ���� ��� óġ�� �̺�Ʈ�� ó���մϴ�.
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    /// ���� ȹ�� �̺�Ʈ�� ó��
    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        // ���� ����
        gameScore += pointsScoredArgs.points * scoreMultiplier;

        // ���� ���� �̺�Ʈ ȣ��
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// ���� ��� �̺�Ʈ�� ó��
    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier--;
        }

        // 1���� 30 ���̷� Ŭ����
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        // ���� ���� �̺�Ʈ ȣ��
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// �÷��̾� �ı� �̺�Ʈ�� ó��
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    // Start is called before the first frame update
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        // ������ 0���� ����
        gameScore = 0;

        // ����� 1�� ����
        scoreMultiplier = 1;

        // ȭ���� �������� ����
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

    }
    /// ���� ���¸� ó���մϴ�.
    private void HandleGameState()
    {
        // ���� ���� ó��
        switch (gameState)
        {
            case GameState.gameStarted:

                // ù ��° ������ ����
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                // �Ա����� ���� �����Ƿ� ���� ���� ��� óġ���� Ʈ����
                RoomEnemiesDefeated();

                break;

            // ������ ���� ���� �� ���� ���� ���� ���� �� Ű�� ó��
            case GameState.playingLevel:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }

                break;

            // ���� ���� ���� �� �Ͻ� ���� �޴��� ó��
            case GameState.engagingEnemies:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;


            // ���� ���� �ʿ����� �� Ű�� ���� ���� ���� �� ����
            case GameState.dungeonOverviewMap:

                // Ű�� ������ ��
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    // ���� ���� ���� ����
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }

                break;

            // ������ ���� ���̰� ������ ���� ���� ���� ���� ���� ���� �� Ű�� ó��
            case GameState.bossStage:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }

                break;

            // ������ ���� ���� �� �Ͻ� ���� �޴��� ó��
            case GameState.engagingBoss:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;

            // ���� �Ϸ� ���¸� ó��
            case GameState.levelCompleted:

                // ���� �Ϸ� �ؽ�Ʈ�� ǥ��
                StartCoroutine(LevelCompleted());

                break;

            // ���� �¸� ���¸� ó�� (�� ���� Ʈ���� - ���� ���� ���¸� �׽�Ʈ�Ͽ� ����)
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());

                break;

            // ���� �й� ���¸� ó�� (�� ���� Ʈ���� - ���� ���� ���¸� �׽�Ʈ�Ͽ� ����)
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); // �״� ���� ������ Ŭ�����ϸ� �޽����� ǥ�õ��� �ʵ��� ��
                    StartCoroutine(GameLost());
                }

                break;

            // ������ �ٽ� ����
            case GameState.restartGame:

                RestartGame();

                break;

            // ������ �Ͻ� �����Ǿ� �Ͻ� ���� �޴��� ǥ�õ� ��, �ٽ� ESC Ű�� ������ �Ͻ� ���� �޴��� ���� �� ����
            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
        }

    }

    /// �÷��̾ ���� �ִ� ���� ����
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;

        //// �����
        //Debug.Log(room.prefab.name.ToString());
    }

    /// ���� ���� ��� óġ�Ǿ����� Ȯ���ϰ�, ��� ���� ���� �����κ��� Ŭ����Ǿ����� �׽�Ʈ
    /// ��� ���� Ŭ����Ǿ��ٸ� ���� ���� ������ �ε�
    private void RoomEnemiesDefeated()
    {
        // ������ Ŭ����Ǿ����� �ʱ�ȭ
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        // ��� ���� ���� Ȯ���Ͽ� ���� Ŭ����Ǿ����� Ȯ��
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            // ���� ���� �ϴ� �ǳʶ�
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            // �ٸ� ���� �����κ��� Ŭ������� �ʾҴ��� Ȯ��
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        // ���� ���� ����
        // ���� ��� ���� ���� Ŭ����Ǿ��� ���� ���� ���ų� (������ Ŭ����Ǿ��� ���� �浵 Ŭ����Ǿ��ٸ�)
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            // �� ���� ���� ������ �ִ��� Ȯ��
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        // �׷��� �ʰ� ���� ���� ������ ���� ������ Ŭ����� ���
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }

    }

    /// �Ͻ� ���� �޴��� ó���մϴ�. �Ͻ� ���� �޴� ��ư �Ǵ� �簳 ��ư���� ȣ��
    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            // �Ͻ� ���� �޴� Ȱ��ȭ
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            // ���� ���� ����
            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            // �Ͻ� ���� �޴� ��Ȱ��ȭ
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            // ���� ���� ����
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
    }

    /// ���� ���� �� ȭ���� ǥ���մϴ�.
    private void DisplayDungeonOverviewMap()
    {
        // ���̵� ���� ��� ��ȯ
        if (isFading)
            return;

        // ���� ���� �� ǥ��
        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }

    /// ���� ������ �����մϴ�.
    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // ������ �°� ������ �����մϴ�.
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("������ ��� ��� �׷����κ��� ������ ������ �� �����ϴ�.");
        }

        // ���� ����Ǿ����� �˸��� ���� �̺�Ʈ ȣ��
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // �÷��̾ ���� �߾ӿ� ��ġ��ŵ�ϴ�.
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // �÷��̾� ��ó���� ���� ����� ���� ������ ������
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        // ���� ���� �ؽ�Ʈ ǥ��
        StartCoroutine(DisplayDungeonLevelText());

    }

    /// ���� ���� �ؽ�Ʈ�� ǥ��
    private IEnumerator DisplayDungeonLevelText()
    {
        // ȭ���� ���������� ����
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        // ���̵� ��
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    /// �޽��� �ؽ�Ʈ�� ǥ��. displaySeconds�� 0�̸� Enter Ű�� ���� ������ �޽����� ǥ��
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        // �ؽ�Ʈ ����
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // �־��� �ð� ���� �޽��� ǥ��
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Enter ��ư�� ���� ������ �޽��� ǥ��
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        // �ؽ�Ʈ �����
        messageTextTMP.SetText("");
    }

    /// ���� ���������� ����
    private IEnumerator BossStage()
    {
        // ���� �� Ȱ��ȭ
        bossRoom.gameObject.SetActive(true);

        // ���� �� ��� ����
        bossRoom.UnlockDoors(0f);

        // 2�� ���
        yield return new WaitForSeconds(2f);

        // ĵ������ ���̵� ���Ͽ� �ؽ�Ʈ �޽��� ǥ��
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // ���� �޽��� ǥ��
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE  " + GameResources.Instance.currentPlayer.playerName + "!  YOU'VE SURVIVED ....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!", Color.white, 5f));

        // ĵ���� ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    /// ���� �ϷḦ ǥ���ϰ� ���� ������ �ε�
    private IEnumerator LevelCompleted()
    {
        // ���� ���� ����
        gameState = GameState.playingLevel;

        // 2�� ���
        yield return new WaitForSeconds(2f);

        // ĵ������ ���̵� ���Ͽ� �ؽ�Ʈ �޽��� ǥ��
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // ���� �Ϸ� �޽��� ǥ��
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));

        // ĵ���� ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // �÷��̾ Return Ű�� ���� ������ ����Ͽ� ���� ������ ����
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // Enter�� �� �� �����Ǵ� ���� �����ϱ� ����

        // ���� ���� �ε��� ����
        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    /// ĵ���� �׷��� ���̵� ó��
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }

        isFading = false;
    }

    /// ���� �¸�
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        // �÷��̾� ��Ȱ��ȭ
        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;

        // ������ ��ŷ�� ������ �׽�Ʈ
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }

            // ���� ������Ʈ
            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), playerScore = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        // 1�� ���
        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // ���� �¸� �޽��� ǥ��
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // ���� ���¸� ��������� ����
        gameState = GameState.restartGame;
    }

    /// ���� �й�
    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        // �÷��̾� ��Ȱ��ȭ
        GetPlayer().playerControl.DisablePlayer();

        // ��ŷ ��������
        int rank = HighScoreManager.Instance.GetRank(gameScore);
        string rankText;

        // ������ ��ŷ�� ������ �׽�Ʈ
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }

            // ���� ������Ʈ
            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), playerScore = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        // 1�� ���
        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // �� ��Ȱ��ȭ
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        // ���� �й� �޽��� ǥ��
        yield return StartCoroutine(DisplayMessageRoutine("BAD LUCK " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // ���� ���¸� ��������� ����
        gameState = GameState.restartGame;
    }

    /// ������ �����
    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// �÷��̾ ������
    public Player GetPlayer()
    {
        return player;
    }

    /// �÷��̾� �̴ϸ� �������� ������.
    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    /// ���� �÷��̾ �ִ� ���� ������
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// ���� ���� ������ ������
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation
}