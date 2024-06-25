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
        // Dimmed 재질을 비활성화
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        // Dimmed 재질을 완전히 표시
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        // Room 노드 타입 리스트를 로드
        LoadRoomNodeTypeList();
    }

    /// Room 노드 타입 리스트를 로드
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// 랜덤 던전을 생성. 던전이 성공적으로 생성되면 true를 반환하고, 실패하면 false를 반환
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // 스크립터블 오브젝트 룸 템플릿을 딕셔너리에 로드
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            // 리스트에서 랜덤한 Room 노드 그래프를 선택
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // 던전이 성공적으로 생성될 때까지 또는 Room 노드 그래프의 최대 재시도 횟수를 초과할 때까지 반복
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                // 던전 Room 게임 오브젝트 및 던전 Room 딕셔너리를 초기화
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // 선택된 Room 노드 그래프에 대해 랜덤 던전을 시도
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }


            if (dungeonBuildSuccessful)
            {
                // Room 게임 오브젝트를 생성
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// Room 템플릿을 딕셔너리에 로드
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Room 템플릿 딕셔너리를 초기화
        roomTemplateDictionary.Clear();

        // Room 템플릿 리스트를 딕셔너리에 로드
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Room 템플릿 리스트에 중복된 키가 있습니다: " + roomTemplateList);
            }
        }
    }

    /// 지정된 Room 노드 그래프에 대해 랜덤 던전을 생성하려고 시도
    /// 성공적인 랜덤 레이아웃이 생성되면 true를 반환하고, 문제가 발생하여 추가 시도가 필요하면 false를 반환
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {

        // 오픈 Room 노드 큐를 생성
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Room 노드 그래프에서 Entrance 노드를 가져와 Room 노드 큐에 추가
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("Entrance 노드가 없습니다.");
            return false;  // 던전 생성 실패
        }

        // 시작 시 Room이 겹치지 않도록 함
        bool noRoomOverlaps = true;


        // 오픈 Room 노드 큐를 처리
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // 모든 Room 노드가 처리되고 Room이 겹치지 않았다면 true를 반환
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// 오픈 Room 노드 큐에서 Room을 처리하고, Room이 겹치지 않으면 true를 반환
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {

        // 오픈 Room 노드 큐에 Room 노드가 있고, Room이 겹치지 않았다면 반복
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // 오픈 Room 노드 큐에서 다음 Room 노드를 가져옴
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Room 노드 그래프에서 자식 노드를 가져와 Queue에 추가 (부모 Room과 연결됨)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // Room이 Entrance인 경우 위치가 지정되었음을 표시하고 Room 딕셔너리에 추가
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // Room을 Room 딕셔너리에 추가
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // Room 타입이 Entrance가 아닌 경우
            else
            {
                // 노드의 부모 Room을 가져옴
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // Room이 겹치지 않도록 배치할 수 있는지 확인
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }

        }

        return noRoomOverlaps;

    }


    /// 던전에 방 노드를 배치를 시도합니다 - 방을 배치할 수 있으면 방을 반환하고, 그렇지 않으면 null을 반환
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {

        // 초기화 및 오버랩을 가정합니다.
        bool roomOverlaps = true;

        // 방이 오버랩될 때까지 반복 - 부모의 모든 사용 가능한 문에 대해 방을 배치하려고 시도
        // 방이 중복되지 않게 성공적으로 배치될 때까지.
        while (roomOverlaps)
        {
            // 부모의 연결되지 않은 사용 가능한 문 중에서 무작위로 선택
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // 더 이상 시도할 문이 없으면 오버랩 실패.
                return false; // 방이 중복
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // 부모 문의 방향과 일치하는 방 노드에 대한 무작위 방 템플릿을 가져옴
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // 방을 생성
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // 방을 배치합니다 - 방이 중복되지 않으면 true를 반환
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // 방이 중복되지 않으면 while 루프를 종료할 수 있도록 false로 설정
                roomOverlaps = false;

                // 방을 위치로 표시
                room.isPositioned = true;

                // 사전에 방을 추가
                dungeonBuilderRoomDictionary.Add(room.id, room);

            }
            else
            {
                roomOverlaps = true;
            }

        }

        return true;  // 방이 중복되지 않음

    }

    /// 부모 문의 방향과 일치하는 방 노드에 대한 무작위 방 템플릿을 가져옴
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // 방 노드가 복도이면 부모 문의 방향에 따라 올바른 복도 방 템플릿을 무작위로 선택
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
        // 그렇지 않으면 무작위 방 템플릿을 선택
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomtemplate;
    }


    /// 방을 배치 - 방이 중복되지 않으면 true를 반환하고, 그렇지 않으면 false를 반환
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {

        // 현재 방 문 위치를 가져옴
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // 방의 문이 없으면 반환
        if (doorway == null)
        {
            // 부모 문을 사용할 수 없게 마킹
            doorwayParent.isUnavailable = true;

            return false;
        }

        // 'world' 그리드 부모 문 위치를 계산
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // 부모 문과 연결하려는 방 문 위치에 따라 조정 위치 오프셋을 계산
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

        // 부모 문과 일치하도록 방 문 위치를 기준으로 하여 방의 하한과 상한을 계산
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // 문을 연결 및 사용할 수 없음으로 표시
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // 중복 없이 방이 연결되었음을 나타내기 위해 true를 반환
            return true;
        }
        else
        {
            // 부모 문을 사용할 수 없게 마킹
            doorwayParent.isUnavailable = true;

            return false;
        }

    }

    /// 문의 방향이 부모 문과 반대인 문을 문 목록에서 가져옴
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


    /// 상위 및 하위 경계 매개변수와 겹치는 방을 확인하고, 겹치는 방이 있으면 해당 방을 반환하고 그렇지 않으면 null을 반환
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // 모든 방을 반복
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // 테스트할 방과 동일한 방이거나 방이 아직 위치되지 않았으면 건너뜀
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            // 방이 겹치는지 확인
            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        // 반환
        return null;
    }

    /// 두 방이 서로 겹치는지 확인 - 겹치면 true를 반환하고, 그렇지 않으면 false를 반환
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

    /// 간격 1이 간격 2와 겹치는지 확인 - 이 메서드는 IsOverlappingRoom 메서드에서 사용
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

    /// roomType과 일치하는 roomtemplatelist에서 무작위 방 템플릿을 가져와 반환
    /// (일치하는 방 템플릿이 없으면 null을 반환)
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // room 템플릿 목록을 반복
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // 일치하는 room 템플릿을 추가
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // 목록이 비어있으면 null을 반환
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // 목록에서 무작위로 방 템플릿을 선택하고 반환
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    /// 연결되지 않은 doorway를 가져옴
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // doorway 목록을 반복
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    /// roomTemplate 및 layoutNode를 기반으로 방을 생성하고 생성된 방을 반환
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // 템플릿에서 방을 초기화
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

        // 방의 부모 ID를 설정
        if (roomNode.parentRoomNodeIDList.Count == 0) // 입구
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // 게임 매니저에서 현재 방을 설정
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        // 스폰할 적이 없으면 방을 기본적으로 적 없는 상태로 설정
        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearedOfEnemies = true;
        }

        return room;
    }

    /// roomNodeGraph 목록에서 무작위 room node graph를 선택
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("room node graph가 목록에 없습니다.");
            return null;
        }
    }

    /// doorway 목록의 깊은 복사본을 생성
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

    /// 문자열 목록의 깊은 복사본을 생성
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// prefab에서 던전 방 gameobject를 인스턴스화
    private void InstantiateRoomGameobjects()
    {
        // 모든 던전 방을 반복
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // 방 위치를 계산 (인스턴스화된 방 위치는 방 템플릿의 하한에 의해 조정되어야 함).
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // 방을 인스턴스화
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // 인스턴스화된 프리팹에서 인스턴스화된 방 컴포넌트를 가져옴
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // 인스턴스화된 방을 초기화
            instantiatedRoom.Initialise(roomGameobject);

            // 게임 오브젝트 참조를 저장
            room.instantiatedRoom = instantiatedRoom;

        }
    }

    /// roomTemplate ID로 room template을 가져옴. ID가 존재하지 않으면 null을 반환
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

    /// roomID로 room을 가져옴. 해당 ID로 된 room이 없으면 null을 반환
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

    /// 던전 방 gameobject와 던전 방 사전을 모두 지웁니다.
    private void ClearDungeon()
    {
        // 인스턴스화된 던전 gameobject를 제거하고 던전 매니저 방 사전을 지움.
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