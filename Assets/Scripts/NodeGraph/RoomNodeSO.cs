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

    // �Ʒ� �ڵ�� ����Ƽ �����Ϳ����� ����Ǿ�� ��.
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// ��� �ʱ�ȭ
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // �� ��� Ÿ�� ����Ʈ �ε�
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// ��带 ��� ��Ÿ�Ϸ� �׸�
    public void Draw(GUIStyle nodeStyle)
    {
        // Begin Area�� ����Ͽ� ��� �ڽ� �׸���
        GUILayout.BeginArea(rect, nodeStyle);

        // �˾� ���� ������ �����ϱ� ���� ���� ����
        EditorGUI.BeginChangeCheck();

        // ���� �� ��尡 �θ� ������ �ְų� �Ա� Ÿ���̸� ���̺��� ǥ���ϰ�, �׷��� ������ �˾� ǥ��
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // ������ �� ���� ���̺� ǥ��
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // RoomNodeType �̸� �� ����Ͽ� ������ �� �ִ� �˾� ǥ�� (���� ������ roomNodeType���� �⺻ ����)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // �� Ÿ�� ������ ����Ǿ� �ڽ� ������ ��ȿȭ�� �� ������
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // �̹� �ڽ��� �ִ� ��� �θ�-�ڽ� ��ũ ����
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // �ڽ� �� ��� ��������
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        // �ڽ� �� ��尡 null�� �ƴ� ���
                        if (childRoomNode != null)
                        {
                            // �θ� ��忡�� childID ����
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            // �ڽ� �� ��忡�� �θ�ID ����
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

    /// ���� ������ �� ��� Ÿ�Ե��� ���ڿ� �迭�� ����
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

    /// ��� �̺�Ʈ ó��
    public void ProcessEvents(Event currentEvent)
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

    /// ���콺 �ٿ� �̺�Ʈ ó��
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // ���� Ŭ�� �ٿ�
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // ������ Ŭ�� �ٿ�
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    /// ���� Ŭ�� �ٿ� �̺�Ʈ ó��
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // ��� ���� ���
        if (isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    /// ������ Ŭ�� �ٿ� ó��
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// ���콺 �� �̺�Ʈ ó��
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // ���� Ŭ�� ���� ���
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    /// ���� Ŭ�� �� �̺�Ʈ ó��
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    /// ���콺 �巡�� �̺�Ʈ ó��
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // ���� Ŭ�� �巡�� �̺�Ʈ ó��
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    /// ���� ���콺 �巡�� �̺�Ʈ ó��
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// ��� �巡��
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// �ڽ�ID�� ��忡 �߰�. �߰��Ǿ����� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // �θ� ��ȿ�ϰ� �ڽ� ��带 �߰��� �� �ִ��� Ȯ��
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// �ڽ� ��带 �θ� ��忡 �߰��� �� �ִ��� Ȯ��. �߰� �����ϸ� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        // ��� �׷����� �̹� ����� ���� ���� �ִ��� Ȯ��
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        // ���� �ڽ� ��尡 ���� �� Ÿ���̰� �̹� ����� ���� �� ��尡 �ִٸ� false ��ȯ
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        // ���� �ڽ� ��尡 None Ÿ���̸� false ��ȯ
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        // �̹� �� ��忡 �� �ڽ� ID�� ������ false ��ȯ
        if (childRoomNodeIDList.Contains(childID))
            return false;

        // �� ��� ID�� �ڽ� ID�� ������ false ��ȯ
        if (id == childID)
            return false;

        // �� �ڽ� ID�� �̹� �θ� ID ��Ͽ� �ִ� ��� false ��ȯ
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // �ڽ� ��尡 �̹� �θ� ������ �ִٸ� false ��ȯ
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // �ڽ��� �ڸ����̰� �� ��嵵 �ڸ����� ��� false ��ȯ
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // �ڽ��� �ڸ����� �ƴϰ� �� ��嵵 �ڸ����� �ƴ� ��� false ��ȯ
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // �ڸ����� �߰��ϴ� ��� �� ��尡 ����ϴ� �ִ� �ڽ� �ڸ��� ���� Ȯ��
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        // ���� �ڽ� ��尡 �Ա��� ��� false ��ȯ - �Ա��� �׻� �ֻ��� �θ� ���
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        // �ڸ����� ���� �߰��ϴ� ��� �� �ڸ��� ��忡 �̹� ���� �߰��� ��� false ��ȯ
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    /// �θ�ID�� ��忡 �߰�. �߰��Ǿ����� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// �ڽ�ID�� ��忡�� ����. ���ŵǾ����� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        // ��尡 child ID�� �����ϰ� �ִٸ� ����
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// �θ�ID�� ��忡�� ����. ���ŵǾ����� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        // ��尡 parent ID�� �����ϰ� �ִٸ� ����
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