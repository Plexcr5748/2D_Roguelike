using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS

    [Space(10)]
    [Header("기본 레벨 세부 정보")]

    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip

    [Tooltip("레벨 이름")]

    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL

    [Space(10)]
    [Header("레벨용 방 템플릿")]

    #endregion Header ROOM TEMPLATES FOR LEVEL

    #region Tooltip

    [Tooltip("레벨에 포함될 방 템플릿 목록입니다. 모든 방 노드 유형이 레벨의 방 노드 그래프에 포함되어 있는지 확인해야 합니다.")]

    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL

    [Space(10)]
    [Header("레벨용 방 노드 그래프")]

    #endregion Header ROOM NODE GRAPHS FOR LEVEL

    #region Tooltip

    [Tooltip("레벨에서 무작위로 선택할 방 노드 그래프 목록입니다.")]

    #endregion Tooltip

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation

#if UNITY_EDITOR

    // 스크립터블 오브젝트에 입력된 세부 정보를 유효성 검사
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        // 지정된 노드 그래프의 모든 노드 유형에 대해 방 템플릿이 지정되었는지 확인

        // 먼저 북/남 쪽 복도, 동/서 쪽 복도 및 입구 유형이 지정되었는지 확인
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // 모든 방 템플릿을 반복하여 이 노드 유형이 지정되었는지 확인
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log(this.name.ToString() + "에서: 동/서 복도 방 유형이 지정되지 않았습니다.");
        }

        if (isNSCorridor == false)
        {
            Debug.Log(this.name.ToString() + "에서: 남/북 복도 방 유형이 지정되지 않았습니다.");
        }

        if (isEntrance == false)
        {
            Debug.Log(this.name.ToString() + "에서: 입구 복도 방 유형이 지정되지 않았습니다.");
        }

        // 모든 노드 그래프를 반복
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // 노드 그래프 내의 모든 노드를 반복
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // 각 roomNode 유형에 대해 방 템플릿이 지정되었는지 확인

                // 복도와 입구는 이미 확인
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                // 모든 방 템플릿을 반복하여 이 노드 유형이 지정되었는지 확인
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {

                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }

                }

                if (!isRoomNodeTypeFound)
                    Debug.Log(this.name.ToString() + "에서: " + roomNodeGraph.name.ToString() + " 노드 그래프에 " + roomNodeSO.roomNodeType.name.ToString() + " 방 템플릿을 찾을 수 없습니다.");


            }
        }
    }

#endif

    #endregion
}