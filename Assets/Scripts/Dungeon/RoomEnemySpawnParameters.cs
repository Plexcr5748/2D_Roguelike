using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("�� ���� ���� ������ �����մϴ�. �� �������� ��ȯ�� �� �� ���� �����մϴ�.")]
    #endregion Tooltip
    public DungeonLevelSO dungeonLevel;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ��ȯ�� �ּ� �� ���Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int minTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ��ȯ�� �ִ� �� ���Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int maxTotalEnemiesToSpawn;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ���ÿ� ��ȯ�� �ּ� �� ���Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int minConcurrentEnemies;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ���ÿ� ��ȯ�� �ִ� �� ���Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int maxConcurrentEnemies;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ������ ��ȯ�Ǵ� �ּ� ����(��)�Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int minSpawnInterval;
    #region Tooltip
    [Tooltip("�� �濡�� �� ���� ������ ������ ��ȯ�Ǵ� �ִ� ����(��)�Դϴ�. ���� ���� �ּ� �� �ִ� �� ������ ������ ���Դϴ�.")]
    #endregion Tooltip
    public int maxSpawnInterval;
}