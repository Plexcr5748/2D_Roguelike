using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom; // ���� �ν��Ͻ�ȭ�� ��
    private Grid grid; // ����Ƽ �׸��� �ý���
    private Tilemap frontTilemap; // �ո� Ÿ�ϸ�
    private Tilemap pathTilemap; // ��θ� ǥ���� Ÿ�ϸ�
    private Vector3Int startGridPosition; // ���� ��ġ�� �׸��� ��ǥ
    private Vector3Int endGridPosition; // ���� ��ġ�� �׸��� ��ǥ
    private TileBase startPathTile; // ���� Ÿ��
    private TileBase finishPathTile; // ���� Ÿ��

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999); // �ʱ�ȭ���� ���� ��ġ�� ��Ÿ���� ��
    private Stack<Vector3> pathStack; // ��θ� �����ϴ� ����

    private void OnEnable()
    {
        // ���� ����Ǿ��� �� �̺�Ʈ�� ����
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // �� ���� �̺�Ʈ���� ���� ����
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        // ���� �� ���� Ÿ���� ���� ���ҽ����� ��������
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    // �� ���� �̺�Ʈ �ڵ鷯
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        // ��� Ÿ�ϸ� ����
        SetUpPathTilemap();
    }

    /// �ո� Ÿ�ϸ��� Ŭ���� ��� Ÿ�ϸ����� ���
    /// �������� �ʾҴٸ� �����ϰ�, �׷��� ������ ���� ���� ���
    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        // �ո� Ÿ�ϸ��� Ŭ�е��� �ʾҴٸ� Ŭ�� ����
        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        // Ŭ���� �̹� �ִٸ� �װ��� ���
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    // �� �����Ӹ��� ȣ��
    private void Update()
    {
        // ���̳� Ÿ�� �Ǵ� �׸��尡 �������� �ʾҴٸ� ������Ʈ ����
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        // I Ű�� ������ ��� �ʱ�ȭ �� ���� ��ġ ����
        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        // O Ű�� ������ ��� �ʱ�ȭ �� ���� ��ġ ����
        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        // P Ű�� ������ ��� ǥ��
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    /// ���� ��ġ�� �����ϰ� �ո� Ÿ�ϸʿ� ���� Ÿ���� ����
    private void SetStartPosition()
    {
        // ���� ��ġ�� �������� ���� ���
        if (startGridPosition == noValue)
        {
            // ���콺 ��ġ�� �׸��� ��ǥ�� ��ȯ�Ͽ� ���� ��ġ�� ����
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            // ���� ��ġ�� ���� ��踦 ����� ��ȿȭ
            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            // Ÿ�ϸʿ� ���� Ÿ���� ����
            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            // ���� ���� Ÿ���� �����ϰ� ���� ��ġ ��ȿȭ
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    /// ���� ��ġ�� �����ϰ� �ո� Ÿ�ϸʿ� ���� Ÿ���� ����
    private void SetEndPosition()
    {
        // ���� ��ġ�� �������� ���� ���
        if (endGridPosition == noValue)
        {
            // ���콺 ��ġ�� �׸��� ��ǥ�� ��ȯ�Ͽ� ���� ��ġ�� ����
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            // ���� ��ġ�� ���� ��踦 ����� ��ȿȭ
            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            // Ÿ�ϸʿ� ���� Ÿ���� ����
            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            // ���� ���� Ÿ���� �����ϰ� ���� ��ġ ��ȿȭ
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    /// ��ġ�� ���� ��� ���� �ִ��� Ȯ��
    private bool IsPositionWithinBounds(Vector3Int position)
    {
        // ��ġ�� �׸��� ���̸� false ��ȯ
        if (position.x < instantiatedRoom.room.templateLowerBounds.x || position.x > instantiatedRoom.room.templateUpperBounds.x
            || position.y < instantiatedRoom.room.templateLowerBounds.y || position.y > instantiatedRoom.room.templateUpperBounds.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// ��θ� ����� ���� �� ���� ��ġ�� �ʱ�ȭ
    private void ClearPath()
    {
        // ��ΰ� ���ٸ� ����
        if (pathStack == null) return;

        // ��� ���� ��� Ÿ���� ����
        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        // ���� �� ���� ��ġ �ʱ�ȭ
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    /// ���� ��ġ�� ���� ��ġ ������ A* ��θ� �����ϰ� ǥ��
    private void DisplayPath()
    {
        // ���� ��ġ �Ǵ� ���� ��ġ�� �������� �ʾ����� ����
        if (startGridPosition == noValue || endGridPosition == noValue) return;

        // A* �˰����� ����Ͽ� ��θ� ����
        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);

        // ��ΰ� ������ ����
        if (pathStack == null) return;

        // ��θ� ���� Ÿ�ϸʿ� Ÿ���� ����
        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }
}