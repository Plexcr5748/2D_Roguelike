using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room; // �� ����
    [HideInInspector] public Grid grid; // �׸���
    [HideInInspector] public Tilemap groundTilemap; // ���� Ÿ�ϸ�
    [HideInInspector] public Tilemap decoration1Tilemap; // ��� 1 Ÿ�ϸ�
    [HideInInspector] public Tilemap decoration2Tilemap; // ��� 2 Ÿ�ϸ�
    [HideInInspector] public Tilemap frontTilemap; // ���� Ÿ�ϸ�
    [HideInInspector] public Tilemap collisionTilemap; // �浹 Ÿ�ϸ�
    [HideInInspector] public Tilemap minimapTilemap; // �̴ϸ� Ÿ�ϸ�
    [HideInInspector] public int[,] aStarMovementPenalty; // AStar ��ã�⿡ ���Ǵ� �̵� �г�Ƽ 2���� �迭
    [HideInInspector] public int[,] aStarItemObstacles; // �̵� ������ �������� ��ֹ��� ��ġ�� �����ϴ� �迭
    [HideInInspector] public Bounds roomColliderBounds; // �� �ݶ��̴� ���
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>(); // �̵� ������ ������ ���

    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("������Ʈ ����")]

    #endregion Header OBJECT REFERENCES

    #region Tooltip

    [Tooltip("ȯ�� �ڽ� �÷��̽�Ȧ�� ���� ������Ʈ�� ä��ϴ�.")]

    #endregion Tooltip

    [SerializeField] private GameObject environmentGameObject; // ȯ�� ���� ������Ʈ

    private BoxCollider2D boxCollider2D; // �ڽ� �ݶ��̴�

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // �� �ݶ��̴� ��� ����
        roomColliderBounds = boxCollider2D.bounds;
    }

    private void Start()
    {
        // �̵� ������ ������ ��ֹ� �迭 ������Ʈ
        UpdateMoveableObstacles();
    }

    // �÷��̾ �濡 �����ϸ� �� ���� �̺�Ʈ Ʈ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �ݶ��̴��� Ʈ�������� ��
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            // ���� �湮�� ������ ǥ��
            this.room.isPreviouslyVisited = true;

            // �� ���� �̺�Ʈ ȣ��
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    /// �ν��Ͻ�ȭ�� �� �ʱ�ȭ
    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// Ÿ�ϸ� �� �׸��� ��� ���� ä���
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // �׸��� ������Ʈ ��������
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // �ڽĿ��� Ÿ�ϸ� ��������
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

    /// ������ �ʴ� ���� �����ϴ�.
    private void BlockOffUnusedDoorWays()
    {
        // ��� ���� ��ȸ
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;

            // ������� ���� ���� Ÿ�ϸ��� �̿��� ����
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

    /// Ÿ�ϸ� ���̾�� ���� �����ϴ�.
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

    /// ���� ������ ���� �����ϴ�.
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // ��� Ÿ���� �����Ͽ� �ݺ�
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // ���� ���� Ÿ���� ȸ�� ���� ��������
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Ÿ�� ����
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // ����� Ÿ���� ȸ�� ����
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// ���� ������ ���� �����ϴ�.
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // ��� Ÿ���� �����Ͽ� �ݺ�
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {

            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // ���� ���� Ÿ���� ȸ�� ���� ��������
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Ÿ�� ����
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // ����� Ÿ���� ȸ�� ����
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);

            }

        }
    }

    // ��ֹ� ������Ʈ - AStar ��� Ž������ ����.
    private void AddObstaclesAndPreferredPaths()
    {
        // �� �迭�� �� ��ֹ��� ä���� ���Դϴ�.
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        // ��� �׸��� ���簢���� �ݺ���
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // �׸��� ���簢���� �⺻ �̵� �г�Ƽ ����
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // ���� �ɾ�ٴ� �� ���� �浹 Ÿ�Ͽ� ���� ��ֹ� �߰�
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // ���� ���� ��ȣ ��� �߰� (1�� ��ȣ ��� ��, �׸��� ��ġ�� �⺻ ���� ������ ��õ�).
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    // ���� �� ���� ������ �ƴ϶�� ���� �����ݴϴ�.
    private void AddDoorsToRooms()
    {
        // ���� �������� �Ǵ� ���Ϲ��� ������� ��ȯ�մϴ�.
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        // �� ��ġ�� �� �������� �����մϴ�.
        foreach (Doorway doorway in room.doorWayList)
        {
            // �� �������� null�� �ƴϰ� ���� ����Ǿ� �ִٸ�
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    // ���� �θ�� �ϴ� �� ����
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    // ���� �θ�� �ϴ� �� ����
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    // ���� �θ�� �ϴ� �� ����
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    // ���� �θ�� �ϴ� �� ����
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                // �� ������Ʈ ��������
                Door doorComponent = door.GetComponent<Door>();

                // �� ���� ���� ���� �Ϻ����� ����
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // �� ���� ������ ���� �� ��ױ�
                    doorComponent.LockDoor();

                    // �� �� �̴ϸʿ� �ذ� ������ ����
                    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab, gameObject.transform);
                    skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    // �浹 Ÿ�ϸ� ������ ��Ȱ��ȭ
    private void DisableCollisionTilemapRenderer()
    {
        // �浹 Ÿ�ϸ� ������ ��Ȱ��ȭ
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    // �÷��̾ �濡 �� �� Ʈ���ŵǴ� �� Ʈ���� �ݶ��̴� ��Ȱ��ȭ
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }

    // �÷��̾ �濡 �� �� Ʈ���ŵǴ� �� Ʈ���� �ݶ��̴� Ȱ��ȭ
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    // ȯ�� ���� ������Ʈ Ȱ��ȭ
    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(true);
    }

    // ȯ�� ���� ������Ʈ ��Ȱ��ȭ
    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(false);
    }

    // �� �� ��ױ�
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        // �� ��ױ� Ʈ����
        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        // �� Ʈ���� �ݶ��̴� ��Ȱ��ȭ
        DisableRoomCollider();
    }

    // �� �� ��� ����
    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    // �� �� ��� ���� ��ƾ
    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        // �� ���� Ʈ����
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        // �� Ʈ���� �ݶ��̴� Ȱ��ȭ
        EnableRoomCollider();
    }

    // ������ ��ֹ� �迭 ����
    private void CreateItemObstaclesArray()
    {
        // �� �迭�� ���� �÷��� �߿� �̵� ������ ��ֹ��� ä���� ���Դϴ�.
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }

    // �⺻ AStar �̵� �г�Ƽ ������ ������ ��ֹ� �迭 �ʱ�ȭ
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // �׸��� ���簢���� �⺻ �̵� �г�Ƽ ����
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    // �̵� ������ ��ֹ� �迭 ������Ʈ
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);

            // �̵� ������ �׸��� �ݶ��̴� ��踦 �ݺ��ϸ� ��ֹ� �迭�� �߰�
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
        // ȯ�� ���� ������Ʈ�� null���� Ȯ���Ͽ� ��ȿ�� �˻�
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }

#endif

    #endregion Validation
}