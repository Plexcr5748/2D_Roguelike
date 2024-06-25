using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room; // 방 정보
    [HideInInspector] public Grid grid; // 그리드
    [HideInInspector] public Tilemap groundTilemap; // 지면 타일맵
    [HideInInspector] public Tilemap decoration1Tilemap; // 장식 1 타일맵
    [HideInInspector] public Tilemap decoration2Tilemap; // 장식 2 타일맵
    [HideInInspector] public Tilemap frontTilemap; // 전면 타일맵
    [HideInInspector] public Tilemap collisionTilemap; // 충돌 타일맵
    [HideInInspector] public Tilemap minimapTilemap; // 미니맵 타일맵
    [HideInInspector] public int[,] aStarMovementPenalty; // AStar 길찾기에 사용되는 이동 패널티 2차원 배열
    [HideInInspector] public int[,] aStarItemObstacles; // 이동 가능한 아이템이 장애물인 위치를 저장하는 배열
    [HideInInspector] public Bounds roomColliderBounds; // 방 콜라이더 경계
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>(); // 이동 가능한 아이템 목록

    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("오브젝트 참조")]

    #endregion Header OBJECT REFERENCES

    #region Tooltip

    [Tooltip("환경 자식 플레이스홀더 게임 오브젝트로 채웁니다.")]

    #endregion Tooltip

    [SerializeField] private GameObject environmentGameObject; // 환경 게임 오브젝트

    private BoxCollider2D boxCollider2D; // 박스 콜라이더

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // 방 콜라이더 경계 저장
        roomColliderBounds = boxCollider2D.bounds;
    }

    private void Start()
    {
        // 이동 가능한 아이템 장애물 배열 업데이트
        UpdateMoveableObstacles();
    }

    // 플레이어가 방에 진입하면 방 변경 이벤트 트리거
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 콜라이더를 트리거했을 때
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            // 방을 방문한 것으로 표시
            this.room.isPreviouslyVisited = true;

            // 방 변경 이벤트 호출
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    /// 인스턴스화된 방 초기화
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// 타일맵 및 그리드 멤버 변수 채우기
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // 그리드 컴포넌트 가져오기
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // 자식에서 타일맵 가져오기
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }

        }

    }

    /// 사용되지 않는 문을 막습니다.
    private void BlockOffUnusedDoorWays()
    {
        // 모든 문을 순회
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;

            // 연결되지 않은 문을 타일맵을 이용해 막음
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// 타일맵 레이어에서 문을 막습니다.
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }

    }

    /// 수평 방향의 문을 막습니다.
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // 모든 타일을 복사하여 반복
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // 복사 중인 타일의 회전 정보 가져오기
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // 타일 복사
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // 복사된 타일의 회전 설정
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// 수직 방향의 문을 막습니다.
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // 모든 타일을 복사하여 반복
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {

            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // 복사 중인 타일의 회전 정보 가져오기
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // 타일 복사
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // 복사된 타일의 회전 설정
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);

            }

        }
    }

    // 장애물 업데이트 - AStar 경로 탐색에서 사용됨.
    private void AddObstaclesAndPreferredPaths()
    {
        // 이 배열은 벽 장애물로 채워질 것입니다.
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        // 모든 그리드 정사각형을 반복함
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // 그리드 정사각형의 기본 이동 패널티 설정
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // 적이 걸어다닐 수 없는 충돌 타일에 대한 장애물 추가
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // 적에 대한 선호 경로 추가 (1은 선호 경로 값, 그리드 위치의 기본 값은 설정에 명시됨).
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    // 만약 이 방이 복도가 아니라면 문을 열어줍니다.
    private void AddDoorsToRooms()
    {
        // 방이 동서방향 또는 남북방향 복도라면 반환합니다.
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        // 문 위치에 문 프리팹을 생성합니다.
        foreach (Doorway doorway in room.doorWayList)
        {
            // 문 프리팹이 null이 아니고 문이 연결되어 있다면
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    // 방을 부모로 하는 문 생성
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    // 방을 부모로 하는 문 생성
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    // 방을 부모로 하는 문 생성
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    // 방을 부모로 하는 문 생성
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                // 문 컴포넌트 가져오기
                Door doorComponent = door.GetComponent<Door>();

                // 이 문이 보스 방의 일부인지 설정
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // 방 접근 방지를 위해 문 잠그기
                    doorComponent.LockDoor();

                    // 문 옆 미니맵에 해골 아이콘 생성
                    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab, gameObject.transform);
                    skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    // 충돌 타일맵 렌더러 비활성화
    private void DisableCollisionTilemapRenderer()
    {
        // 충돌 타일맵 렌더러 비활성화
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    // 플레이어가 방에 들어갈 때 트리거되는 방 트리거 콜라이더 비활성화
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    // 플레이어가 방에 들어갈 때 트리거되는 방 트리거 콜라이더 활성화
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    // 환경 게임 오브젝트 활성화
    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(true);
    }

    // 환경 게임 오브젝트 비활성화
    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(false);
    }

    // 방 문 잠그기
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        // 문 잠그기 트리거
        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        // 방 트리거 콜라이더 비활성화
        DisableRoomCollider();
    }

    // 방 문 잠금 해제
    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    // 방 문 잠금 해제 루틴
    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        // 문 열기 트리거
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        // 방 트리거 콜라이더 활성화
        EnableRoomCollider();
    }

    // 아이템 장애물 배열 생성
    private void CreateItemObstaclesArray()
    {
        // 이 배열은 게임 플레이 중에 이동 가능한 장애물로 채워질 것입니다.
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }

    // 기본 AStar 이동 패널티 값으로 아이템 장애물 배열 초기화
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // 그리드 정사각형의 기본 이동 패널티 설정
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    // 이동 가능한 장애물 배열 업데이트
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);

            // 이동 가능한 항목의 콜라이더 경계를 반복하며 장애물 배열에 추가
            for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        // 환경 게임 오브젝트가 null인지 확인하여 유효성 검사
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }

#endif

    #endregion Validation
}