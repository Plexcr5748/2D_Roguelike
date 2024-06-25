using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        // Dimmed ������ ��Ȱ��ȭ
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        // Dimmed ������ ������ ǥ��
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        // Room ��� Ÿ�� ����Ʈ�� �ε�
        LoadRoomNodeTypeList();
    }

    /// Room ��� Ÿ�� ����Ʈ�� �ε�
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// ���� ������ ����. ������ ���������� �����Ǹ� true�� ��ȯ�ϰ�, �����ϸ� false�� ��ȯ
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // ��ũ���ͺ� ������Ʈ �� ���ø��� ��ųʸ��� �ε�
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            // ����Ʈ���� ������ Room ��� �׷����� ����
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // ������ ���������� ������ ������ �Ǵ� Room ��� �׷����� �ִ� ��õ� Ƚ���� �ʰ��� ������ �ݺ�
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                // ���� Room ���� ������Ʈ �� ���� Room ��ųʸ��� �ʱ�ȭ
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // ���õ� Room ��� �׷����� ���� ���� ������ �õ�
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }


            if (dungeonBuildSuccessful)
            {
                // Room ���� ������Ʈ�� ����
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// Room ���ø��� ��ųʸ��� �ε�
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Room ���ø� ��ųʸ��� �ʱ�ȭ
        roomTemplateDictionary.Clear();

        // Room ���ø� ����Ʈ�� ��ųʸ��� �ε�
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Room ���ø� ����Ʈ�� �ߺ��� Ű�� �ֽ��ϴ�: " + roomTemplateList);
            }
        }
    }

    /// ������ Room ��� �׷����� ���� ���� ������ �����Ϸ��� �õ�
    /// �������� ���� ���̾ƿ��� �����Ǹ� true�� ��ȯ�ϰ�, ������ �߻��Ͽ� �߰� �õ��� �ʿ��ϸ� false�� ��ȯ
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {

        // ���� Room ��� ť�� ����
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Room ��� �׷������� Entrance ��带 ������ Room ��� ť�� �߰�
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("Entrance ��尡 �����ϴ�.");
            return false;  // ���� ���� ����
        }

        // ���� �� Room�� ��ġ�� �ʵ��� ��
        bool noRoomOverlaps = true;


        // ���� Room ��� ť�� ó��
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // ��� Room ��尡 ó���ǰ� Room�� ��ġ�� �ʾҴٸ� true�� ��ȯ
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// ���� Room ��� ť���� Room�� ó���ϰ�, Room�� ��ġ�� ������ true�� ��ȯ
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {

        // ���� Room ��� ť�� Room ��尡 �ְ�, Room�� ��ġ�� �ʾҴٸ� �ݺ�
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // ���� Room ��� ť���� ���� Room ��带 ������
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Room ��� �׷������� �ڽ� ��带 ������ Queue�� �߰� (�θ� Room�� �����)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // Room�� Entrance�� ��� ��ġ�� �����Ǿ����� ǥ���ϰ� Room ��ųʸ��� �߰�
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // Room�� Room ��ųʸ��� �߰�
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // Room Ÿ���� Entrance�� �ƴ� ���
            else
            {
                // ����� �θ� Room�� ������
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // Room�� ��ġ�� �ʵ��� ��ġ�� �� �ִ��� Ȯ��
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }

        }

        return noRoomOverlaps;

    }


    /// ������ �� ��带 ��ġ�� �õ��մϴ� - ���� ��ġ�� �� ������ ���� ��ȯ�ϰ�, �׷��� ������ null�� ��ȯ
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {

        // �ʱ�ȭ �� �������� �����մϴ�.
        bool roomOverlaps = true;

        // ���� �������� ������ �ݺ� - �θ��� ��� ��� ������ ���� ���� ���� ��ġ�Ϸ��� �õ�
        // ���� �ߺ����� �ʰ� ���������� ��ġ�� ������.
        while (roomOverlaps)
        {
            // �θ��� ������� ���� ��� ������ �� �߿��� �������� ����
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // �� �̻� �õ��� ���� ������ ������ ����.
                return false; // ���� �ߺ�
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // �θ� ���� ����� ��ġ�ϴ� �� ��忡 ���� ������ �� ���ø��� ������
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // ���� ����
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // ���� ��ġ�մϴ� - ���� �ߺ����� ������ true�� ��ȯ
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // ���� �ߺ����� ������ while ������ ������ �� �ֵ��� false�� ����
                roomOverlaps = false;

                // ���� ��ġ�� ǥ��
                room.isPositioned = true;

                // ������ ���� �߰�
                dungeonBuilderRoomDictionary.Add(room.id, room);

            }
            else
            {
                roomOverlaps = true;
            }

        }

        return true;  // ���� �ߺ����� ����

    }

    /// �θ� ���� ����� ��ġ�ϴ� �� ��忡 ���� ������ �� ���ø��� ������
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // �� ��尡 �����̸� �θ� ���� ���⿡ ���� �ùٸ� ���� �� ���ø��� �������� ����
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // �׷��� ������ ������ �� ���ø��� ����
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomtemplate;
    }


    /// ���� ��ġ - ���� �ߺ����� ������ true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {

        // ���� �� �� ��ġ�� ������
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // ���� ���� ������ ��ȯ
        if (doorway == null)
        {
            // �θ� ���� ����� �� ���� ��ŷ
            doorwayParent.isUnavailable = true;

            return false;
        }

        // 'world' �׸��� �θ� �� ��ġ�� ���
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // �θ� ���� �����Ϸ��� �� �� ��ġ�� ���� ���� ��ġ �������� ���
        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // �θ� ���� ��ġ�ϵ��� �� �� ��ġ�� �������� �Ͽ� ���� ���Ѱ� ������ ���
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // ���� ���� �� ����� �� �������� ǥ��
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // �ߺ� ���� ���� ����Ǿ����� ��Ÿ���� ���� true�� ��ȯ
            return true;
        }
        else
        {
            // �θ� ���� ����� �� ���� ��ŷ
            doorwayParent.isUnavailable = true;

            return false;
        }

    }

    /// ���� ������ �θ� ���� �ݴ��� ���� �� ��Ͽ��� ������
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {

        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;

    }


    /// ���� �� ���� ��� �Ű������� ��ġ�� ���� Ȯ���ϰ�, ��ġ�� ���� ������ �ش� ���� ��ȯ�ϰ� �׷��� ������ null�� ��ȯ
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // ��� ���� �ݺ�
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // �׽�Ʈ�� ��� ������ ���̰ų� ���� ���� ��ġ���� �ʾ����� �ǳʶ�
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            // ���� ��ġ���� Ȯ��
            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        // ��ȯ
        return null;
    }

    /// �� ���� ���� ��ġ���� Ȯ�� - ��ġ�� true�� ��ȯ�ϰ�, �׷��� ������ false�� ��ȯ
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// ���� 1�� ���� 2�� ��ġ���� Ȯ�� - �� �޼���� IsOverlappingRoom �޼��忡�� ���
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// roomType�� ��ġ�ϴ� roomtemplatelist���� ������ �� ���ø��� ������ ��ȯ
    /// (��ġ�ϴ� �� ���ø��� ������ null�� ��ȯ)
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // room ���ø� ����� �ݺ�
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // ��ġ�ϴ� room ���ø��� �߰�
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // ����� ��������� null�� ��ȯ
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // ��Ͽ��� �������� �� ���ø��� �����ϰ� ��ȯ
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    /// ������� ���� doorway�� ������
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // doorway ����� �ݺ�
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    /// roomTemplate �� layoutNode�� ������� ���� �����ϰ� ������ ���� ��ȯ
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // ���ø����� ���� �ʱ�ȭ
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.battleMusic = roomTemplate.battleMusic;
        room.ambientMusic = roomTemplate.ambientMusic;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        // ���� �θ� ID�� ����
        if (roomNode.parentRoomNodeIDList.Count == 0) // �Ա�
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // ���� �Ŵ������� ���� ���� ����
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        // ������ ���� ������ ���� �⺻������ �� ���� ���·� ����
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearedOfEnemies = true;
        }

        return room;
    }

    /// roomNodeGraph ��Ͽ��� ������ room node graph�� ����
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("room node graph�� ��Ͽ� �����ϴ�.");
            return null;
        }
    }

    /// doorway ����� ���� ���纻�� ����
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    /// ���ڿ� ����� ���� ���纻�� ����
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// prefab���� ���� �� gameobject�� �ν��Ͻ�ȭ
    private void InstantiateRoomGameobjects()
    {
        // ��� ���� ���� �ݺ�
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // �� ��ġ�� ��� (�ν��Ͻ�ȭ�� �� ��ġ�� �� ���ø��� ���ѿ� ���� �����Ǿ�� ��).
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // ���� �ν��Ͻ�ȭ
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // �ν��Ͻ�ȭ�� �����տ��� �ν��Ͻ�ȭ�� �� ������Ʈ�� ������
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // �ν��Ͻ�ȭ�� ���� �ʱ�ȭ
            instantiatedRoom.Initialise(roomGameobject);

            // ���� ������Ʈ ������ ����
            room.instantiatedRoom = instantiatedRoom;

        }
    }

    /// roomTemplate ID�� room template�� ������. ID�� �������� ������ null�� ��ȯ
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// roomID�� room�� ������. �ش� ID�� �� room�� ������ null�� ��ȯ
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// ���� �� gameobject�� ���� �� ������ ��� ����ϴ�.
    private void ClearDungeon()
    {
        // �ν��Ͻ�ȭ�� ���� gameobject�� �����ϰ� ���� �Ŵ��� �� ������ ����.
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}