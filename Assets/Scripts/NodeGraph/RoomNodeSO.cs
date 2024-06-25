using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    // 아래 코드는 유니티 에디터에서만 실행되어야 함.
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// 노드 초기화
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // 방 노드 타입 리스트 로드
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// 노드를 노드 스타일로 그림
    public void Draw(GUIStyle nodeStyle)
    {
        // Begin Area를 사용하여 노드 박스 그리기
        GUILayout.BeginArea(rect, nodeStyle);

        // 팝업 선택 변경을 감지하기 위한 리전 시작
        EditorGUI.BeginChangeCheck();

        // 만약 방 노드가 부모를 가지고 있거나 입구 타입이면 레이블을 표시하고, 그렇지 않으면 팝업 표시
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // 변경할 수 없는 레이블 표시
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // RoomNodeType 이름 값 사용하여 선택할 수 있는 팝업 표시 (현재 설정된 roomNodeType으로 기본 설정)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // 방 타입 선택이 변경되어 자식 연결이 무효화될 수 있으면
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // 이미 자식이 있는 경우 부모-자식 링크 삭제
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // 자식 방 노드 가져오기
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        // 자식 방 노드가 null이 아닌 경우
                        if (childRoomNode != null)
                        {
                            // 부모 노드에서 childID 제거
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            // 자식 방 노드에서 부모ID 제거
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    /// 선택 가능한 방 노드 타입들의 문자열 배열을 생성
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    /// 노드 이벤트 처리
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // 마우스 다운 이벤트 처리
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // 마우스 업 이벤트 처리
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // 마우스 드래그 이벤트 처리
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// 마우스 다운 이벤트 처리
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // 왼쪽 클릭 다운
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // 오른쪽 클릭 다운
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    /// 왼쪽 클릭 다운 이벤트 처리
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // 노드 선택 토글
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    /// 오른쪽 클릭 다운 처리
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// 마우스 업 이벤트 처리
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // 왼쪽 클릭 업인 경우
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// 왼쪽 클릭 업 이벤트 처리
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    /// 마우스 드래그 이벤트 처리
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // 왼쪽 클릭 드래그 이벤트 처리
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    /// 왼쪽 마우스 드래그 이벤트 처리
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// 노드 드래그
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// 자식ID를 노드에 추가. 추가되었으면 true를 반환하고, 그렇지 않으면 false를 반환
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // 부모에 유효하게 자식 노드를 추가할 수 있는지 확인
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// 자식 노드를 부모 노드에 추가할 수 있는지 확인. 추가 가능하면 true를 반환하고, 그렇지 않으면 false를 반환
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        // 노드 그래프에 이미 연결된 보스 룸이 있는지 확인
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        // 만약 자식 노드가 보스 룸 타입이고 이미 연결된 보스 룸 노드가 있다면 false 반환
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        // 만약 자식 노드가 None 타입이면 false 반환
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        // 이미 이 노드에 이 자식 ID가 있으면 false 반환
        if (childRoomNodeIDList.Contains(childID))
            return false;

        // 이 노드 ID와 자식 ID가 같으면 false 반환
        if (id == childID)
            return false;

        // 이 자식 ID가 이미 부모 ID 목록에 있는 경우 false 반환
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // 자식 노드가 이미 부모를 가지고 있다면 false 반환
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // 자식이 코릿돌이고 이 노드도 코릿돌인 경우 false 반환
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // 자식이 코릿돌이 아니고 이 노드도 코릿돌이 아닌 경우 false 반환
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // 코릿돌을 추가하는 경우 이 노드가 허용하는 최대 자식 코릿돌 수를 확인
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        // 만약 자식 노드가 입구인 경우 false 반환 - 입구는 항상 최상위 부모 노드
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        // 코릿돌에 방을 추가하는 경우 이 코릿돌 노드에 이미 방이 추가된 경우 false 반환
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    /// 부모ID를 노드에 추가. 추가되었으면 true를 반환하고, 그렇지 않으면 false를 반환
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// 자식ID를 노드에서 제거. 제거되었으면 true를 반환하고, 그렇지 않으면 false를 반환
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        // 노드가 child ID를 포함하고 있다면 제거
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// 부모ID를 노드에서 제거. 제거되었으면 true를 반환하고, 그렇지 않으면 false를 반환
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        // 노드가 parent ID를 포함하고 있다면 제거
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}