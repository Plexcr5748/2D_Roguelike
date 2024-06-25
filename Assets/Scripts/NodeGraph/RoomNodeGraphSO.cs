using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList; // 방 노드 타입 목록 참조
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>(); // 방 노드 목록
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>(); // 방 노드 딕셔너리

    private void Awake()
    {
        LoadRoomNodeDictionary(); // 방 노드 딕셔너리 초기화
    }

    /// 방 노드 딕셔너리를 방 노드 목록에서 로드
    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        // 딕셔너리 채우기
        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// 주어진 방 노드 타입에 해당하는 방 노드를 반환
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

    /// 주어진 방 노드 ID에 해당하는 방 노드를 반환
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }

    /// 주어진 부모 방 노드에 대한 자식 방 노드 목록을 반환
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childNodeID);
        }
    }

    #region Editor Code

    // 아래 코드는 Unity 편집기에서만 실행되어야 함.
#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null; // 연결선을 그릴 방 노드
    [HideInInspector] public Vector2 linePosition; // 연결선의 위치

    // 에디터에서 변경 사항이 있을 때마다 노드 딕셔너리를 다시 채움
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    // 연결선을 그릴 노드와 위치를 설정
    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code

}