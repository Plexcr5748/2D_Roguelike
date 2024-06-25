using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom; // 현재 인스턴스화된 방
    private Grid grid; // 유니티 그리드 시스템
    private Tilemap frontTilemap; // 앞면 타일맵
    private Tilemap pathTilemap; // 경로를 표시할 타일맵
    private Vector3Int startGridPosition; // 시작 위치의 그리드 좌표
    private Vector3Int endGridPosition; // 종료 위치의 그리드 좌표
    private TileBase startPathTile; // 시작 타일
    private TileBase finishPathTile; // 종료 타일

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999); // 초기화되지 않은 위치를 나타내는 값
    private Stack<Vector3> pathStack; // 경로를 저장하는 스택

    private void OnEnable()
    {
        // 방이 변경되었을 때 이벤트에 구독
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // 방 변경 이벤트에서 구독 해제
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        // 시작 및 종료 타일을 게임 리소스에서 가져오기
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    // 방 변경 이벤트 핸들러
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        // 경로 타일맵 설정
        SetUpPathTilemap();
    }

    /// 앞면 타일맵의 클론을 경로 타일맵으로 사용
    /// 생성되지 않았다면 생성하고, 그렇지 않으면 기존 것을 사용
    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        // 앞면 타일맵이 클론되지 않았다면 클론 생성
        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        // 클론이 이미 있다면 그것을 사용
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    // 매 프레임마다 호출
    private void Update()
    {
        // 방이나 타일 또는 그리드가 설정되지 않았다면 업데이트 종료
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        // I 키가 눌리면 경로 초기화 후 시작 위치 설정
        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        // O 키가 눌리면 경로 초기화 후 종료 위치 설정
        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        // P 키가 눌리면 경로 표시
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    /// 시작 위치를 설정하고 앞면 타일맵에 시작 타일을 설정
    private void SetStartPosition()
    {
        // 시작 위치가 설정되지 않은 경우
        if (startGridPosition == noValue)
        {
            // 마우스 위치를 그리드 좌표로 변환하여 시작 위치로 설정
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            // 시작 위치가 방의 경계를 벗어나면 무효화
            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            // 타일맵에 시작 타일을 설정
            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            // 기존 시작 타일을 제거하고 시작 위치 무효화
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    /// 종료 위치를 설정하고 앞면 타일맵에 종료 타일을 설정
    private void SetEndPosition()
    {
        // 종료 위치가 설정되지 않은 경우
        if (endGridPosition == noValue)
        {
            // 마우스 위치를 그리드 좌표로 변환하여 종료 위치로 설정
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            // 종료 위치가 방의 경계를 벗어나면 무효화
            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            // 타일맵에 종료 타일을 설정
            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            // 기존 종료 타일을 제거하고 종료 위치 무효화
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    /// 위치가 방의 경계 내에 있는지 확인
    private bool IsPositionWithinBounds(Vector3Int position)
    {
        // 위치가 그리드 밖이면 false 반환
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


    /// 경로를 지우고 시작 및 종료 위치를 초기화
    private void ClearPath()
    {
        // 경로가 없다면 종료
        if (pathStack == null) return;

        // 경로 상의 모든 타일을 제거
        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        // 시작 및 종료 위치 초기화
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    /// 시작 위치와 종료 위치 사이의 A* 경로를 생성하고 표시
    private void DisplayPath()
    {
        // 시작 위치 또는 종료 위치가 설정되지 않았으면 종료
        if (startGridPosition == noValue || endGridPosition == noValue) return;

        // A* 알고리즘을 사용하여 경로를 생성
        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);

        // 경로가 없으면 종료
        if (pathStack == null) return;

        // 경로를 따라 타일맵에 타일을 설정
        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }
}