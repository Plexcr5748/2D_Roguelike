using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("�����Ϳ� ǥ�õ� RoomNodeType�鸸 �÷��׷� ����")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;

    #region Header
    [Header("�� Ÿ���� Corridor���� ��")]
    #endregion Header
    public bool isCorridor;

    #region Header
    [Header("�� Ÿ���� CorridorNS���� ��")]
    #endregion Header
    public bool isCorridorNS;

    #region Header
    [Header("�� Ÿ���� CorridorEW���� ��")]
    #endregion Header
    public bool isCorridorEW;

    #region Header
    [Header("�� Ÿ���� Entrance���� ��")]
    #endregion Header
    public bool isEntrance;

    #region Header
    [Header("�� Ÿ���� BossRoom�̾�� ��")]
    #endregion Header
    public bool isBossRoom;

    #region Header
    [Header("�� Ÿ���� None (������)�̾�� ��")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
