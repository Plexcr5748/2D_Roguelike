using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("이 방의 던전 레벨을 정의합니다. 이 레벨에서 소환될 총 적 수를 지정합니다.")]
    #endregion Tooltip
    public DungeonLevelSO dungeonLevel;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 소환할 최소 적 수입니다. 실제 수는 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int minTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 소환할 최대 적 수입니다. 실제 수는 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int maxTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 동시에 소환할 최소 적 수입니다. 실제 수는 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int minConcurrentEnemies;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 동시에 소환할 최대 적 수입니다. 실제 수는 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int maxConcurrentEnemies;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 적들이 소환되는 최소 간격(초)입니다. 실제 값은 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int minSpawnInterval;
    #region Tooltip
    [Tooltip("이 방에서 이 던전 레벨에 적들이 소환되는 최대 간격(초)입니다. 실제 값은 최소 및 최대 값 사이의 무작위 값입니다.")]
    #endregion Tooltip
    public int maxSpawnInterval;
}