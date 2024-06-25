using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    // 노드 레이아웃 값
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // 연결선 값
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // 그리드 간격
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        // 인스펙터 선택 변경 이벤트 구독
        Selection.selectionChanged += InspectorSelectionChanged;

        // 노드 레이아웃 스타일 정의
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // 선택된 노드 스타일 정의
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // 방 노드 유형 불러오기
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        // 인스펙터 선택 변경 이벤트 구독 해제
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// 인스펙터에서 RoomNodeGraphSO 스크립터블 오브젝트 자산이 더블 클릭되면 방 노드 그래프 에디터 창을 엶
    [OnOpenAsset(0)] // UnityEditor.Callbacks 네임스페이스 필요
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    /// 에디터 GUI 그리기
    private void OnGUI()
    {
        // RoomNodeGraphSO 타입의 스크립터블 오브젝트가 선택된 경우 처리
        if (currentRoomNodeGraph != null)
        {
            // 그리드 그리기
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            // 드래그 중인 경우 선 그리기
            DrawDraggedLine();

            // 이벤트 처리
            ProcessEvents(Event.current);

            // 방 노드 간의 연결 그리기
            DrawRoomConnections();

            // 방 노드 그리기
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    /// 방 노드 그래프 에디터의 배경 그리드 그리기
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    /// 드래그 중인 선 그리기
    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            // 노드에서 라인 위치로 선 그리기
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    /// 이벤트 처리
    private void ProcessEvents(Event currentEvent)
    {
        // 그래프 드래그 초기화
        graphDrag = Vector2.zero;

        // 현재 드래그 중이 아니거나 null인 경우 마우스가 있는 노드 가져오기
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // 마우스가 방 노드 위에 없거나 현재 노드에서 드래그 중이라면 그래프 이벤트 처리
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        // 그렇지 않으면 방 노드 이벤트 처리
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// 마우스가 방 노드 위에 있는지 확인 - 그렇다면 방 노드 반환, 아니면 null 반환
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    /// 방 노드 그래프 이벤트 처리
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
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

    /// 방 노드 그래프의 마우스 다운 이벤트 처리 (노드 위에 있지 않음)
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // 그래프에서 오른쪽 마우스 클릭 시 컨텍스트 메뉴 표시
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // 그래프에서 왼쪽 마우스 클릭 시
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    /// 컨텍스트 메뉴 표시
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    /// 마우스 위치에 방 노드 생성
    private void CreateRoomNode(object mousePositionObject)
    {
        // 현재 노드 그래프가 비어있으면 먼저 입구 방 노드 추가
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// 마우스 위치에 방 노드를 생성 - RoomNodeType을 전달하는 오버로드된 메서드
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // 방 노드 스크립터블 오브젝트 자산 생성
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // 생성된 방 노드를 현재 방 노드 그래프의 방 노드 목록에 추가
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // 방 노드의 값 설정
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // 방 노드를 방 노드 그래프 스크립터블 오브젝트 자산 데이터베이스에 추가
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // 그래프 노드 사전 갱신
        currentRoomNodeGraph.OnValidate();
    }

    /// 선택된 방 노드 삭제
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        // 모든 노드를 순회
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                // 자식 방 노드 ID를 순회
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // 자식 방 노드 가져오기
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        // 자식 방 노드에서 부모 ID 제거
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                // 부모 방 노드 ID를 순회
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    // 부모 노드 가져오기
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        // 부모 노드에서 자식 ID 제거
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // 큐에 있는 방 노드 삭제
        while (roomNodeDeletionQueue.Count > 0)
        {
            // 큐에서 방 노드 가져오기
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            // 사전에서 노드 제거
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            // 리스트에서 노드 제거
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            // 자산 데이터베이스에서 노드 제거
            DestroyImmediate(roomNodeToDelete, true);

            // 자산 데이터베이스 저장
            AssetDatabase.SaveAssets();
        }
    }

    /// 선택된 방 노드들 간의 링크 삭제
    private void DeleteSelectedRoomNodeLinks()
    {
        // 모든 방 노드를 순회
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    // 자식 방 노드 가져오기
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    // 자식 방 노드가 선택된 경우
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        // 부모 방 노드에서 자식 ID 제거
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        // 자식 방 노드에서 부모 ID 제거
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // 모든 선택된 방 노드 선택 해제
        ClearAllSelectedRoomNodes();
    }

    /// 모든 방 노드에서 선택 해제
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// 모든 방 노드 선택
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// 마우스 업 이벤트 처리
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // 오른쪽 마우스 버튼을 떼고 선을 드래그 중인 경우
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            // 방 노드 위에 있는지 확인
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // 부모 방 노드에 자식을 추가할 수 있는 경우 자식으로 설정
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // 자식 방 노드에 부모 ID 설정
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// 마우스 드래그 이벤트 처리
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // 오른쪽 클릭 드래그 이벤트 처리 - 선 그리기
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        // 왼쪽 클릭 드래그 이벤트 처리 - 노드 그래프 드래그
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// 오른쪽 마우스 드래그 이벤트 처리 - 선 그리기
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// 왼쪽 마우스 드래그 이벤트 처리 - 방 노드 그래프 드래그
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    /// 방 노드에서 연결된 선을 드래그
    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// 방 노드에서 선 드래그 초기화
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// 방 노드들 간의 연결을 그래프 창에 그리기
    private void DrawRoomConnections()
    {
        // 모든 방 노드를 순회
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // 자식 방 노드들을 순회
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // 자식 방 노드를 사전에서 가져오기
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }
    /// 부모 방 노드와 자식 방 노드 사이에 연결선을 그립니다.
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        // 선의 시작 위치와 끝 위치를 가져옵니다.
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // 중간 위치를 계산합니다.
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // 선의 시작 위치에서 끝 위치로의 방향 벡터를 계산합니다.
        Vector2 direction = endPosition - startPosition;

        // 중간 점에서 정상화된 수직 위치를 계산합니다.
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // 화살표 머리의 중간 점 오프셋 위치를 계산합니다.
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // 화살표를 그립니다.
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // 선을 그립니다.
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// 그래프 창에서 방 노드를 그립니다.
    private void DrawRoomNodes()
    {
        // 모든 방 노드를 순회하며 그립니다.
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    /// 인스펙터에서 선택이 변경되었습니다.
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}