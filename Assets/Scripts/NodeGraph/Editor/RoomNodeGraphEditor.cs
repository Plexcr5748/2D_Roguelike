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

    // ��� ���̾ƿ� ��
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // ���ἱ ��
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // �׸��� ����
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        // �ν����� ���� ���� �̺�Ʈ ����
        Selection.selectionChanged += InspectorSelectionChanged;

        // ��� ���̾ƿ� ��Ÿ�� ����
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // ���õ� ��� ��Ÿ�� ����
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // �� ��� ���� �ҷ�����
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        // �ν����� ���� ���� �̺�Ʈ ���� ����
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// �ν����Ϳ��� RoomNodeGraphSO ��ũ���ͺ� ������Ʈ �ڻ��� ���� Ŭ���Ǹ� �� ��� �׷��� ������ â�� ��
    [OnOpenAsset(0)] // UnityEditor.Callbacks ���ӽ����̽� �ʿ�
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

    /// ������ GUI �׸���
    private void OnGUI()
    {
        // RoomNodeGraphSO Ÿ���� ��ũ���ͺ� ������Ʈ�� ���õ� ��� ó��
        if (currentRoomNodeGraph != null)
        {
            // �׸��� �׸���
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            // �巡�� ���� ��� �� �׸���
            DrawDraggedLine();

            // �̺�Ʈ ó��
            ProcessEvents(Event.current);

            // �� ��� ���� ���� �׸���
            DrawRoomConnections();

            // �� ��� �׸���
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    /// �� ��� �׷��� �������� ��� �׸��� �׸���
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

    /// �巡�� ���� �� �׸���
    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            // ��忡�� ���� ��ġ�� �� �׸���
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    /// �̺�Ʈ ó��
    private void ProcessEvents(Event currentEvent)
    {
        // �׷��� �巡�� �ʱ�ȭ
        graphDrag = Vector2.zero;

        // ���� �巡�� ���� �ƴϰų� null�� ��� ���콺�� �ִ� ��� ��������
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // ���콺�� �� ��� ���� ���ų� ���� ��忡�� �巡�� ���̶�� �׷��� �̺�Ʈ ó��
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        // �׷��� ������ �� ��� �̺�Ʈ ó��
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// ���콺�� �� ��� ���� �ִ��� Ȯ�� - �׷��ٸ� �� ��� ��ȯ, �ƴϸ� null ��ȯ
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

    /// �� ��� �׷��� �̺�Ʈ ó��
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // ���콺 �ٿ� �̺�Ʈ ó��
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // ���콺 �� �̺�Ʈ ó��
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // ���콺 �巡�� �̺�Ʈ ó��
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);

                break;

            default:
                break;
        }
    }

    /// �� ��� �׷����� ���콺 �ٿ� �̺�Ʈ ó�� (��� ���� ���� ����)
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // �׷������� ������ ���콺 Ŭ�� �� ���ؽ�Ʈ �޴� ǥ��
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // �׷������� ���� ���콺 Ŭ�� ��
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    /// ���ؽ�Ʈ �޴� ǥ��
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

    /// ���콺 ��ġ�� �� ��� ����
    private void CreateRoomNode(object mousePositionObject)
    {
        // ���� ��� �׷����� ��������� ���� �Ա� �� ��� �߰�
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// ���콺 ��ġ�� �� ��带 ���� - RoomNodeType�� �����ϴ� �����ε�� �޼���
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // �� ��� ��ũ���ͺ� ������Ʈ �ڻ� ����
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // ������ �� ��带 ���� �� ��� �׷����� �� ��� ��Ͽ� �߰�
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // �� ����� �� ����
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // �� ��带 �� ��� �׷��� ��ũ���ͺ� ������Ʈ �ڻ� �����ͺ��̽��� �߰�
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // �׷��� ��� ���� ����
        currentRoomNodeGraph.OnValidate();
    }

    /// ���õ� �� ��� ����
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        // ��� ��带 ��ȸ
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                // �ڽ� �� ��� ID�� ��ȸ
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // �ڽ� �� ��� ��������
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        // �ڽ� �� ��忡�� �θ� ID ����
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                // �θ� �� ��� ID�� ��ȸ
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    // �θ� ��� ��������
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        // �θ� ��忡�� �ڽ� ID ����
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // ť�� �ִ� �� ��� ����
        while (roomNodeDeletionQueue.Count > 0)
        {
            // ť���� �� ��� ��������
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            // �������� ��� ����
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            // ����Ʈ���� ��� ����
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            // �ڻ� �����ͺ��̽����� ��� ����
            DestroyImmediate(roomNodeToDelete, true);

            // �ڻ� �����ͺ��̽� ����
            AssetDatabase.SaveAssets();
        }
    }

    /// ���õ� �� ���� ���� ��ũ ����
    private void DeleteSelectedRoomNodeLinks()
    {
        // ��� �� ��带 ��ȸ
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    // �ڽ� �� ��� ��������
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    // �ڽ� �� ��尡 ���õ� ���
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        // �θ� �� ��忡�� �ڽ� ID ����
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        // �ڽ� �� ��忡�� �θ� ID ����
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // ��� ���õ� �� ��� ���� ����
        ClearAllSelectedRoomNodes();
    }

    /// ��� �� ��忡�� ���� ����
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

    /// ��� �� ��� ����
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// ���콺 �� �̺�Ʈ ó��
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // ������ ���콺 ��ư�� ���� ���� �巡�� ���� ���
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            // �� ��� ���� �ִ��� Ȯ��
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // �θ� �� ��忡 �ڽ��� �߰��� �� �ִ� ��� �ڽ����� ����
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // �ڽ� �� ��忡 �θ� ID ����
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// ���콺 �巡�� �̺�Ʈ ó��
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // ������ Ŭ�� �巡�� �̺�Ʈ ó�� - �� �׸���
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        // ���� Ŭ�� �巡�� �̺�Ʈ ó�� - ��� �׷��� �巡��
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// ������ ���콺 �巡�� �̺�Ʈ ó�� - �� �׸���
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// ���� ���콺 �巡�� �̺�Ʈ ó�� - �� ��� �׷��� �巡��
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    /// �� ��忡�� ����� ���� �巡��
    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// �� ��忡�� �� �巡�� �ʱ�ȭ
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// �� ���� ���� ������ �׷��� â�� �׸���
    private void DrawRoomConnections()
    {
        // ��� �� ��带 ��ȸ
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // �ڽ� �� ������ ��ȸ
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // �ڽ� �� ��带 �������� ��������
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }
    /// �θ� �� ���� �ڽ� �� ��� ���̿� ���ἱ�� �׸��ϴ�.
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        // ���� ���� ��ġ�� �� ��ġ�� �����ɴϴ�.
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // �߰� ��ġ�� ����մϴ�.
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // ���� ���� ��ġ���� �� ��ġ���� ���� ���͸� ����մϴ�.
        Vector2 direction = endPosition - startPosition;

        // �߰� ������ ����ȭ�� ���� ��ġ�� ����մϴ�.
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // ȭ��ǥ �Ӹ��� �߰� �� ������ ��ġ�� ����մϴ�.
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // ȭ��ǥ�� �׸��ϴ�.
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // ���� �׸��ϴ�.
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// �׷��� â���� �� ��带 �׸��ϴ�.
    private void DrawRoomNodes()
    {
        // ��� �� ��带 ��ȸ�ϸ� �׸��ϴ�.
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

    /// �ν����Ϳ��� ������ ����Ǿ����ϴ�.
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