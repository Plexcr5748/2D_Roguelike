using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList; // �� ��� Ÿ�� ��� ����
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>(); // �� ��� ���
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>(); // �� ��� ��ųʸ�

    private void Awake()
    {
        LoadRoomNodeDictionary(); // �� ��� ��ųʸ� �ʱ�ȭ
    }

    /// �� ��� ��ųʸ��� �� ��� ��Ͽ��� �ε�
    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        // ��ųʸ� ä���
        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// �־��� �� ��� Ÿ�Կ� �ش��ϴ� �� ��带 ��ȯ
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach (RoomNodeSO node in roomNodeList)
        {
            if (node.roomNodeType == roomNodeType)
            {
                return node;
            }
        }
        return null;
    }

    /// �־��� �� ��� ID�� �ش��ϴ� �� ��带 ��ȯ
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }

    /// �־��� �θ� �� ��忡 ���� �ڽ� �� ��� ����� ��ȯ
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childNodeID);
        }
    }

    #region Editor Code

    // �Ʒ� �ڵ�� Unity �����⿡���� ����Ǿ�� ��.
#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null; // ���ἱ�� �׸� �� ���
    [HideInInspector] public Vector2 linePosition; // ���ἱ�� ��ġ

    // �����Ϳ��� ���� ������ ���� ������ ��� ��ųʸ��� �ٽ� ä��
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    // ���ἱ�� �׸� ���� ��ġ�� ����
    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code

}