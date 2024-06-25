using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("룸 프리팹")]

    #endregion Header ROOM PREFAB

    #region Tooltip

    [Tooltip("룸의 게임오브젝트 프리팹입니다. 이 프리팹에는 룸의 모든 타일맵과 환경 게임 오브젝트가 포함되어 있어야 합니다.")]

    #endregion Tooltip

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // 이전 프리팹을 변경할 경우 GUID를 재생성하는 데 사용

    #region Header ROOM MUSIC

    [Space(10)]
    [Header("룸 음악")]

    #endregion Header ROOM MUSIC

    #region Tooltip

    [Tooltip("적들이 모두 클리어되지 않았을 때 재생되는 전투 음악 SO")]

    #endregion Tooltip

    public MusicTrackSO battleMusic;

    #region Tooltip

    [Tooltip("적들이 모두 클리어되었을 때 재생되는 앰비언트 음악 SO")]

    #endregion Tooltip

    public MusicTrackSO ambientMusic;

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("룸 설정")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip("룸 노드 타입 SO입니다. 룸 노드 타입은 룸 노드 그래프에서 사용되는 노드 유형에 해당합니다. 단, 코리도어는 예외입니다. 룸 노드 그래프에는 '코리도어'라는 하나의 코리도어 타입만 있지만, 룸 템플릿에는 'CorridorNS' 및 'CorridorEW'라는 2개의 코리도어 노드 타입이 있습니다.")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip("룸 타일맵을 완전히 포함하는 사각형을 생각해 보면, 룸 하한선은 해당 사각형의 왼쪽 아래 모서리를 나타냅니다. 이는 룸의 타일맵에서 결정해야 합니다. (좌표 브러시 포인터를 사용하여 해당 왼쪽 아래 모서리의 타일맵 그리드 위치를 얻습니다. 이는 로컬 타일맵 위치이며 월드 위치가 아닙니다)")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip("룸 타일맵을 완전히 포함하는 사각형을 생각해 보면, 룸 상한선은 해당 사각형의 오른쪽 위 모서리를 나타냅니다. 이는 룸의 타일맵에서 결정해야 합니다. (좌표 브러시 포인터를 사용하여 해당 오른쪽 위 모서리의 타일맵 그리드 위치를 얻습니다. 이는 로컬 타일맵 위치이며 월드 위치가 아닙니다)")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip("룸마다 최대 네 개의 문이 있어야 합니다 - 각각의 나침반 방향에 하나씩입니다. 이들은 중앙 타일 위치가 문의 좌표 'position'인 일관된 3 타일 개방 크기를 가져야 합니다.")]

    #endregion Tooltip

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip

    [Tooltip("룸의 타일맵 좌표에서 적 및 상자가 소환될 수 있는 각 가능한 스폰 위치를 이 배열에 추가해야 합니다.")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    #region Header ENEMY DETAILS

    [Space(10)]
    [Header("적 세부 정보")]

    #endregion Header ENEMY DETAILS

    #region Tooltip

    [Tooltip("이 룸에서 던전 레벨별로 소환될 수 있는 모든 적을 포함하며, 이 적 유형의 소환 비율 (랜덤)을 포함해야 합니다.")]

    #endregion Tooltip

    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;

    #region Tooltip

    [Tooltip("적의 소환 매개변수를 리스트에 추가해야 합니다.")]

    #endregion Tooltip

    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    /// 룸 템플릿의 입구 리스트를 반환
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // SO 필드 유효성 검사
    private void OnValidate()
    {
        // 프리팹이 변경되거나 GUID가 비어 있으면 고유 GUID 설정
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // 적 및 룸 스폰 매개변수 레벨 검사
        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn), roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn), roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval), roomEnemySpawnParameters.minSpawnInterval, nameof(roomEnemySpawnParameters.maxSpawnInterval), roomEnemySpawnParameters.maxSpawnInterval, true);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies), roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies), roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = false;

                // 적 유형 목록 유효성 검사
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectsByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectsByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel && dungeonObjectsByLevel.spawnableObjectRatioList.Count > 0)
                        isEnemyTypesListForDungeonLevel = true;

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectsByLevel.dungeonLevel), dungeonObjectsByLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);

                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }

                }

                if (isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("게임오브젝트 " + this.name.ToString() + "에서 던전 레벨 " + roomEnemySpawnParameters.dungeonLevel.levelName + "에 대한 적 유형이 지정되지 않았습니다.");
                }
            }
        }

        // 스폰 위치가 채워져 있는지 확인
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}