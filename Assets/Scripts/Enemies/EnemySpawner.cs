using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;                          // 스폰할 적의 수
    private int currentEnemyCount;                       // 현재 존재하는 적의 수
    private int enemiesSpawnedSoFar;                     // 지금까지 스폰된 적의 수
    private int enemyMaxConcurrentSpawnNumber;           // 동시에 스폰할 수 있는 최대 적 수
    private Room currentRoom;                            // 현재 방
    private RoomEnemySpawnParameters roomEnemySpawnParameters;  // 방의 적 스폰 매개변수

    private void OnEnable()
    {
        // 방 변경 이벤트 구독
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // 방 변경 이벤트 구독 해지
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// 방이 변경될 때 처리
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // 방 음악 업데이트
        MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

        // 복도나 입구인 경우 처리 중단
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // 이미 적이 클리어된 방인 경우 처리 중단
        if (currentRoom.isClearedOfEnemies) return;

        // 무작위로 스폰할 적의 수 결정
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // 방의 적 스폰 매개변수 가져오기
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // 스폰할 적이 없으면 방 클리어 처리 후 종료
        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearedOfEnemies = true;
            return;
        }

        // 동시에 스폰할 최대 적 수 결정
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // 전투 음악으로 변경
        MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);

        // 문 잠금
        currentRoom.instantiatedRoom.LockDoors();

        // 적 스폰
        SpawnEnemies();
    }

    /// 적 스폰
    private void SpawnEnemies()
    {
        // 보스와 맞서 싸우는 상태로 설정
        if (GameManager.Instance.gameState == GameState.bossStage)
        {
            GameManager.Instance.previousGameState = GameState.bossStage;
            GameManager.Instance.gameState = GameState.engagingBoss;
        }
        // 일반 적과 맞서 싸우는 상태로 설정
        else if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// 적 스폰 코루틴
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // 무작위 적 선택을 위한 도우미 클래스 인스턴스 생성
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // 적을 스폰할 위치가 있는지 확인
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            // 모든 적을 생성하기 위해 반복
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // 현재 스폰된 적 수가 최대 동시 스폰 가능 적 수보다 작을 때까지 대기
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // 적 생성 - 다음 스폰할 적 타입 가져오기
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// 최소값과 최대값 사이의 무작위 스폰 간격 반환
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    /// 최소값과 최대값 사이의 무작위 동시 스폰 가능 적 수 반환
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    /// 지정된 위치에 적 생성
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // 지금까지 스폰된 적 수 증가
        enemiesSpawnedSoFar++;

        // 현재 적 수 증가 - 적이 파괴될 때 감소됨
        currentEnemyCount++;

        // 현재 던전 레벨 가져오기
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // 적 생성
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // 적 초기화
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // 적 파괴 이벤트 구독
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }

    /// <summary>
    /// 적이 파괴됐을 때 처리
    /// </summary>
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // 이벤트 구독 해제
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // 현재 적 수 감소
        currentEnemyCount--;

        // 점수 증가 - 점수 이벤트 호출
        StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

        // 현재 적이 없고 지금까지 스폰된 적 수가 스폰할 적 수와 같을 경우
        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            // 방을 클리어된 상태로 표시
            currentRoom.isClearedOfEnemies = true;

            // 게임 상태 설정
            if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.previousGameState = GameState.engagingBoss;
            }

            // 문 잠금 해제
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            // 방 적 클리어 이벤트 호출
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }

}