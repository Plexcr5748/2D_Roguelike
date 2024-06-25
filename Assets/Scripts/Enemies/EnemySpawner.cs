using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;                          // ������ ���� ��
    private int currentEnemyCount;                       // ���� �����ϴ� ���� ��
    private int enemiesSpawnedSoFar;                     // ���ݱ��� ������ ���� ��
    private int enemyMaxConcurrentSpawnNumber;           // ���ÿ� ������ �� �ִ� �ִ� �� ��
    private Room currentRoom;                            // ���� ��
    private RoomEnemySpawnParameters roomEnemySpawnParameters;  // ���� �� ���� �Ű�����

    private void OnEnable()
    {
        // �� ���� �̺�Ʈ ����
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // �� ���� �̺�Ʈ ���� ����
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// ���� ����� �� ó��
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // �� ���� ������Ʈ
        MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

        // ������ �Ա��� ��� ó�� �ߴ�
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // �̹� ���� Ŭ����� ���� ��� ó�� �ߴ�
        if (currentRoom.isClearedOfEnemies) return;

        // �������� ������ ���� �� ����
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // ���� �� ���� �Ű����� ��������
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // ������ ���� ������ �� Ŭ���� ó�� �� ����
        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearedOfEnemies = true;
            return;
        }

        // ���ÿ� ������ �ִ� �� �� ����
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // ���� �������� ����
        MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);

        // �� ���
        currentRoom.instantiatedRoom.LockDoors();

        // �� ����
        SpawnEnemies();
    }

    /// �� ����
    private void SpawnEnemies()
    {
        // ������ �¼� �ο�� ���·� ����
        if (GameManager.Instance.gameState == GameState.bossStage)
        {
            GameManager.Instance.previousGameState = GameState.bossStage;
            GameManager.Instance.gameState = GameState.engagingBoss;
        }
        // �Ϲ� ���� �¼� �ο�� ���·� ����
        else if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// �� ���� �ڷ�ƾ
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // ������ �� ������ ���� ����� Ŭ���� �ν��Ͻ� ����
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // ���� ������ ��ġ�� �ִ��� Ȯ��
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            // ��� ���� �����ϱ� ���� �ݺ�
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // ���� ������ �� ���� �ִ� ���� ���� ���� �� ������ ���� ������ ���
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // �� ���� - ���� ������ �� Ÿ�� ��������
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// �ּҰ��� �ִ밪 ������ ������ ���� ���� ��ȯ
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    /// �ּҰ��� �ִ밪 ������ ������ ���� ���� ���� �� �� ��ȯ
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    /// ������ ��ġ�� �� ����
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // ���ݱ��� ������ �� �� ����
        enemiesSpawnedSoFar++;

        // ���� �� �� ���� - ���� �ı��� �� ���ҵ�
        currentEnemyCount++;

        // ���� ���� ���� ��������
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // �� ����
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // �� �ʱ�ȭ
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // �� �ı� �̺�Ʈ ����
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }

    /// <summary>
    /// ���� �ı����� �� ó��
    /// </summary>
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // �̺�Ʈ ���� ����
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // ���� �� �� ����
        currentEnemyCount--;

        // ���� ���� - ���� �̺�Ʈ ȣ��
        StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

        // ���� ���� ���� ���ݱ��� ������ �� ���� ������ �� ���� ���� ���
        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            // ���� Ŭ����� ���·� ǥ��
            currentRoom.isClearedOfEnemies = true;

            // ���� ���� ����
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

            // �� ��� ����
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            // �� �� Ŭ���� �̺�Ʈ ȣ��
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }

}