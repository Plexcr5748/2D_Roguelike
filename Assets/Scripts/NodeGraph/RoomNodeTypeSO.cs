using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("에디터에 표시될 RoomNodeType들만 플래그로 지정")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;

    #region Header
    [Header("한 타입은 Corridor여야 함")]
    #endregion Header
    public bool isCorridor;

    #region Header
    [Header("한 타입은 CorridorNS여야 함")]
    #endregion Header
    public bool isCorridorNS;

    #region Header
    [Header("한 타입은 CorridorEW여야 함")]
    #endregion Header
    public bool isCorridorEW;

    #region Header
    [Header("한 타입은 Entrance여야 함")]
    #endregion Header
    public bool isEntrance;

    #region Header
    [Header("한 타입은 BossRoom이어야 함")]
    #endregion Header
    public bool isBossRoom;

    #region Header
    [Header("한 타입은 None (미지정)이어야 함")]
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
