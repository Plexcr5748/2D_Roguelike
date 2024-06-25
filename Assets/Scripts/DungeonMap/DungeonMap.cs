using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    #region Header GameObject References
    [Space(10)]
    [Header("게임오브젝트 참조")]
    #endregion
    #region Tooltip
    [Tooltip("미니맵 UI 게임오브젝트를 추가하세요.")]
    #endregion
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera cameraMain;

    private void Start()
    {
        // 메인 카메라 캐시
        cameraMain = Camera.main;

        // 플레이어 트랜스폼 가져오기
        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        // Cinemachine 카메라 타겟으로 플레이어 설정
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // 던전맵 카메라 가져오기
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 마우스 버튼이 눌리고 게임 상태가 던전 개요 맵일 때 클릭한 방 가져오기
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    /// 클릭한 방 가져오기
    private void GetRoomClicked()
    {
        // 스크린 위치를 월드 위치로 변환
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // 커서 위치에서 충돌체 검사
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        // 충돌체 중 방이 있는지 확인
        foreach (Collider2D collider2D in collider2DArray)
        {
            if (collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                // 클릭한 방이 적 없고 이전에 방문한 방이면 플레이어를 해당 방으로 이동
                if (instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    // 플레이어를 방으로 이동
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    /// 플레이어를 선택한 방으로 이동
    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        // 방 변경 이벤트 호출
        StaticEventHandler.CallRoomChangedEvent(room);

        // 화면을 즉시 검은색으로 페이드 아웃
        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

        // 던전 개요 지도 지우기
        ClearDungeonOverViewMap();

        // 페이드 중에 플레이어 비활성화
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // 플레이어에 가장 가까운 스폰 지점 찾기
        Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);

        // 플레이어를 새 위치로 이동 - 가장 가까운 스폰 지점에서 생성
        GameManager.Instance.GetPlayer().transform.position = spawnPosition;

        // 화면 다시 페이드 인
        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        // 플레이어 활성화
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    /// 던전 개요 지도 UI 표시
    public void DisplayDungeonOverViewMap()
    {
        // 게임 상태 설정
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        // 플레이어 비활성화
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // 메인 카메라 비활성화 및 던전 개요 카메라 활성화
        cameraMain.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        // 표시할 수 있도록 모든 방 활성화
        ActivateRoomsForDisplay();

        // 작은 미니맵 UI 비활성화
        minimapUI.SetActive(false);
    }

    /// 던전 개요 지도 UI 지우기
    public void ClearDungeonOverViewMap()
    {
        // 게임 상태 설정
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        // 플레이어 활성화
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        // 메인 카메라 활성화 및 던전 개요 카메라 비활성화
        cameraMain.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        // 작은 미니맵 UI 활성화
        minimapUI.SetActive(true);
    }

    /// 모든 방이 표시될 수 있도록 활성화
    private void ActivateRoomsForDisplay()
    {
        // 던전 방 반복
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}