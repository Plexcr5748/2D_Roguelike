using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    #region Header GameObject References
    [Space(10)]
    [Header("���ӿ�����Ʈ ����")]
    #endregion
    #region Tooltip
    [Tooltip("�̴ϸ� UI ���ӿ�����Ʈ�� �߰��ϼ���.")]
    #endregion
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera cameraMain;

    private void Start()
    {
        // ���� ī�޶� ĳ��
        cameraMain = Camera.main;

        // �÷��̾� Ʈ������ ��������
        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        // Cinemachine ī�޶� Ÿ������ �÷��̾� ����
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // ������ ī�޶� ��������
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ���콺 ��ư�� ������ ���� ���°� ���� ���� ���� �� Ŭ���� �� ��������
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    /// Ŭ���� �� ��������
    private void GetRoomClicked()
    {
        // ��ũ�� ��ġ�� ���� ��ġ�� ��ȯ
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // Ŀ�� ��ġ���� �浹ü �˻�
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        // �浹ü �� ���� �ִ��� Ȯ��
        foreach (Collider2D collider2D in collider2DArray)
        {
            if (collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                // Ŭ���� ���� �� ���� ������ �湮�� ���̸� �÷��̾ �ش� ������ �̵�
                if (instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    // �÷��̾ ������ �̵�
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    /// �÷��̾ ������ ������ �̵�
    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        // �� ���� �̺�Ʈ ȣ��
        StaticEventHandler.CallRoomChangedEvent(room);

        // ȭ���� ��� ���������� ���̵� �ƿ�
        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

        // ���� ���� ���� �����
        ClearDungeonOverViewMap();

        // ���̵� �߿� �÷��̾� ��Ȱ��ȭ
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // �÷��̾ ���� ����� ���� ���� ã��
        Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);

        // �÷��̾ �� ��ġ�� �̵� - ���� ����� ���� �������� ����
        GameManager.Instance.GetPlayer().transform.position = spawnPosition;

        // ȭ�� �ٽ� ���̵� ��
        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        // �÷��̾� Ȱ��ȭ
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    /// ���� ���� ���� UI ǥ��
    public void DisplayDungeonOverViewMap()
    {
        // ���� ���� ����
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        // �÷��̾� ��Ȱ��ȭ
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        // ���� ī�޶� ��Ȱ��ȭ �� ���� ���� ī�޶� Ȱ��ȭ
        cameraMain.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        // ǥ���� �� �ֵ��� ��� �� Ȱ��ȭ
        ActivateRoomsForDisplay();

        // ���� �̴ϸ� UI ��Ȱ��ȭ
        minimapUI.SetActive(false);
    }

    /// ���� ���� ���� UI �����
    public void ClearDungeonOverViewMap()
    {
        // ���� ���� ����
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        // �÷��̾� Ȱ��ȭ
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        // ���� ī�޶� Ȱ��ȭ �� ���� ���� ī�޶� ��Ȱ��ȭ
        cameraMain.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        // ���� �̴ϸ� UI Ȱ��ȭ
        minimapUI.SetActive(true);
    }

    /// ��� ���� ǥ�õ� �� �ֵ��� Ȱ��ȭ
    private void ActivateRoomsForDisplay()
    {
        // ���� �� �ݺ�
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}