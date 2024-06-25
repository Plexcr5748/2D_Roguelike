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
    [Header("게임 오브젝트 참조")]
    #endregion Header GAMEOBJECT REFERENCES

    #region Tooltip

    [Tooltip("계층 구조에서 일시 정지 메뉴 게임오브젝트를 채웁니다")]

    #endregion Tooltip

    [SerializeField] private GameObject pauseMenu;

    #region Tooltip
    [Tooltip("FadeScreenUI의 MessageText TextMeshPro 컴포넌트를 채웁니다")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextTMP;

    #region Tooltip
    [Tooltip("FadeScreenUI의 FadeImage CanvasGroup 컴포넌트를 채웁니다")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;

    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("던전 레벨")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("던전 레벨 ScriptableObject를 채웁니다")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("테스트를 위한 시작 던전 레벨을 채웁니다, 첫 번째 레벨 = 0")]

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
        // 베이스 클래스 호출
        base.Awake();

        // 플레이어 정보 설정 - 메인 메뉴에서 저장된 현재 플레이어 ScriptableObject에서
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // 플레이어 생성
        InstantiatePlayer();

    }

    /// 씬에서 플레이어를 생성
    private void InstantiatePlayer()
    {
        // 플레이어 생성
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // 플레이어 초기화
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);

    }

    private void OnEnable()
    {
        // 방 변경 이벤트에 구독
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // 방의 적이 모두 처치된 이벤트에 구독
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        // 점수 획득 이벤트에 구독
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        // 점수 배수 이벤트에 구독
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        // 플레이어 파괴 이벤트에 구독
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        // 방 변경 이벤트 구독을 해제
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // 방의 적이 모두 처치된 이벤트 구독을 해제
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        // 점수 획득 이벤트 구독을 해제
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        // 점수 배수 이벤트 구독을 해제
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;

        // 플레이어 파괴 이벤트 구독을 해제
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;

    }

    /// 방 변경 이벤트를 처리합니다.
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    /// 방의 적이 모두 처치된 이벤트를 처리합니다.
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    /// 점수 획득 이벤트를 처리
    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        // 점수 증가
        gameScore += pointsScoredArgs.points * scoreMultiplier;

        // 점수 변경 이벤트 호출
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// 점수 배수 이벤트를 처리
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

        // 1에서 30 사이로 클램프
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        // 점수 변경 이벤트 호출
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// 플레이어 파괴 이벤트를 처리
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

        // 점수를 0으로 설정
        gameScore = 0;

        // 배수를 1로 설정
        scoreMultiplier = 1;

        // 화면을 검정으로 설정
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

    }
    /// 게임 상태를 처리합니다.
    private void HandleGameState()
    {
        // 게임 상태 처리
        switch (gameState)
        {
            case GameState.gameStarted:

                // 첫 번째 레벨을 실행
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                // 입구에는 적이 없으므로 방의 적이 모두 처치됨을 트리거
                RoomEnemiesDefeated();

                break;

            // 레벨을 진행 중일 때 던전 개요 맵을 위한 탭 키를 처리
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

            // 적과 전투 중일 때 일시 정지 메뉴를 처리
            case GameState.engagingEnemies:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;


            // 던전 개요 맵에서는 탭 키를 놓아 맵을 지울 수 있음
            case GameState.dungeonOverviewMap:

                // 키가 놓였을 때
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    // 던전 개요 맵을 지움
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }

                break;

            // 레벨을 진행 중이고 보스와 전투 전에 던전 개요 맵을 위한 탭 키를 처리
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

            // 보스와 전투 중일 때 일시 정지 메뉴를 처리
            case GameState.engagingBoss:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;

            // 레벨 완료 상태를 처리
            case GameState.levelCompleted:

                // 레벨 완료 텍스트를 표시
                StartCoroutine(LevelCompleted());

                break;

            // 게임 승리 상태를 처리 (한 번만 트리거 - 이전 게임 상태를 테스트하여 실행)
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());

                break;

            // 게임 패배 상태를 처리 (한 번만 트리거 - 이전 게임 상태를 테스트하여 실행)
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); // 죽는 순간 레벨을 클리어하면 메시지가 표시되지 않도록 함
                    StartCoroutine(GameLost());
                }

                break;

            // 게임을 다시 시작
            case GameState.restartGame:

                RestartGame();

                break;

            // 게임이 일시 정지되어 일시 정지 메뉴가 표시될 때, 다시 ESC 키를 누르면 일시 정지 메뉴를 지울 수 있음
            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
        }

    }

    /// 플레이어가 현재 있는 방을 설정
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;

        //// 디버그
        //Debug.Log(room.prefab.name.ToString());
    }

    /// 방의 적이 모두 처치되었는지 확인하고, 모든 던전 방이 적으로부터 클리어되었는지 테스트
    /// 모든 방이 클리어되었다면 다음 던전 레벨을 로드
    private void RoomEnemiesDefeated()
    {
        // 던전이 클리어되었는지 초기화
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        // 모든 던전 방을 확인하여 적이 클리어되었는지 확인
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            // 보스 방은 일단 건너뜀
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            // 다른 방이 적으로부터 클리어되지 않았는지 확인
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        // 게임 상태 설정
        // 만약 모든 던전 방이 클리어되었고 보스 방이 없거나 (던전이 클리어되었고 보스 방도 클리어되었다면)
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            // 더 많은 던전 레벨이 있는지 확인
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        // 그렇지 않고 보스 방을 제외한 던전 레벨이 클리어된 경우
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }

    }

    /// 일시 정지 메뉴를 처리합니다. 일시 정지 메뉴 버튼 또는 재개 버튼에서 호출
    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            // 일시 정지 메뉴 활성화
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            // 게임 상태 설정
            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            // 일시 정지 메뉴 비활성화
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            // 게임 상태 복원
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
    }

    /// 던전 개요 맵 화면을 표시합니다.
    private void DisplayDungeonOverviewMap()
    {
        // 페이딩 중일 경우 반환
        if (isFading)
            return;

        // 던전 개요 맵 표시
        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }

    /// 던전 레벨을 실행합니다.
    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // 레벨에 맞게 던전을 생성합니다.
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("지정된 방과 노드 그래프로부터 던전을 생성할 수 없습니다.");
        }

        // 방이 변경되었음을 알리는 정적 이벤트 호출
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // 플레이어를 방의 중앙에 위치시킵니다.
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // 플레이어 근처에서 가장 가까운 스폰 지점을 가져옴
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        // 던전 레벨 텍스트 표시
        StartCoroutine(DisplayDungeonLevelText());

    }

    /// 던전 레벨 텍스트를 표시
    private IEnumerator DisplayDungeonLevelText()
    {
        // 화면을 검은색으로 설정
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        // 페이드 인
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    /// 메시지 텍스트를 표시. displaySeconds가 0이면 Enter 키를 누를 때까지 메시지가 표시
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        // 텍스트 설정
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // 주어진 시간 동안 메시지 표시
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
            // Enter 버튼이 눌릴 때까지 메시지 표시
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        // 텍스트 지우기
        messageTextTMP.SetText("");
    }

    /// 보스 스테이지에 진입
    private IEnumerator BossStage()
    {
        // 보스 방 활성화
        bossRoom.gameObject.SetActive(true);

        // 보스 방 잠금 해제
        bossRoom.UnlockDoors(0f);

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 캔버스를 페이드 인하여 텍스트 메시지 표시
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // 보스 메시지 표시
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE  " + GameResources.Instance.currentPlayer.playerName + "!  YOU'VE SURVIVED ....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!", Color.white, 5f));

        // 캔버스 페이드 아웃
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    /// 레벨 완료를 표시하고 다음 레벨을 로드
    private IEnumerator LevelCompleted()
    {
        // 다음 레벨 실행
        gameState = GameState.playingLevel;

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 캔버스를 페이드 인하여 텍스트 메시지 표시
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // 레벨 완료 메시지 표시
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));

        // 캔버스 페이드 아웃
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // 플레이어가 Return 키를 누를 때까지 대기하여 다음 레벨로 진행
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // Enter가 두 번 감지되는 것을 방지하기 위해

        // 다음 레벨 인덱스 증가
        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    /// 캔버스 그룹을 페이드 처리
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

    /// 게임 승리
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        // 플레이어 비활성화
        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;

        // 점수가 랭킹에 들어가는지 테스트
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }

            // 점수 업데이트
            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), playerScore = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 페이드 아웃
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // 게임 승리 메시지 표시
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // 게임 상태를 재시작으로 설정
        gameState = GameState.restartGame;
    }

    /// 게임 패배
    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        // 플레이어 비활성화
        GetPlayer().playerControl.DisablePlayer();

        // 랭킹 가져오기
        int rank = HighScoreManager.Instance.GetRank(gameScore);
        string rankText;

        // 점수가 랭킹에 들어가는지 테스트
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.playerCharacterName.ToUpper();
            }

            // 점수 업데이트
            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), playerScore = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 페이드 아웃
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // 적 비활성화
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        // 게임 패배 메시지 표시
        yield return StartCoroutine(DisplayMessageRoutine("BAD LUCK " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // 게임 상태를 재시작으로 설정
        gameState = GameState.restartGame;
    }

    /// 게임을 재시작
    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// 플레이어를 가져옴
    public Player GetPlayer()
    {
        return player;
    }

    /// 플레이어 미니맵 아이콘을 가져옴.
    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    /// 현재 플레이어가 있는 방을 가져옴
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// 현재 던전 레벨을 가져옴
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