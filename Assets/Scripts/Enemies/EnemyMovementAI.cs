using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO ��ũ���ͺ� ������Ʈ�� �ӵ��� ���� ������ ���� ������ �����մϴ�.")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; // �⺻ ��. �̴� �� �����ʿ��� ����
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        // ������Ʈ �ε�
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // ����� Fixed Update�� ���� ���
        waitForFixedUpdate = new WaitForFixedUpdate();

        // �÷��̾� ���� ��ġ �ʱ�ȭ
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

    }

    private void Update()
    {
        MoveEnemy();
    }


    /// AStar ��� Ž���� ����Ͽ� �÷��̾�� �̵� ��θ� �����ϰ� ���� �� �׸��� ��ġ�� �̵�
    private void MoveEnemy()
    {
        // ������ ��ٿ� Ÿ�̸�
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        // ���� �߰��� �����ؾ� �ϴ� �Ÿ��� �ִ��� Ȯ��
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        // �÷��̾ �߰��� �Ÿ��� ���� ������ ��ȯ
        if (!chasePlayer)
            return;

        // ������ �����ӿ����� A Star ��� �籸���� ó���Ͽ� ���ϸ� �л�.
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        // ������ ��ٿ� Ÿ�̸Ӱ� ����Ǿ��ų� �÷��̾ �ʿ��� �Ÿ����� �� �ָ� ����������
        // �� ��θ� �籸���ϰ� ���� �̵�
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            // ��� �籸�� ��ٿ� Ÿ�̸� �ʱ�ȭ
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            // �÷��̾� ���� ��ġ �缳��
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            // AStar ��� Ž���� ����Ͽ� ���� �̵�. - �÷��̾�� ��� �籸�� Ʈ����
            CreatePath();

            // ��θ� ã�Ҵٸ� ���� �̵�
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    // �޽� �̺�Ʈ Ʈ����
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // �ڷ�ƾ�� ����Ͽ� ��θ� ���� ���� �̵�
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

            }
        }
    }


    /// <summary>
    /// ���� ����� ���� ��ġ�� �̵���Ű�� �ڷ�ƾ
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            // �ſ� ����� ������ �̵��� ��� ��������� ���� �ܰ�� �̵�
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                // ������ �̺�Ʈ Ʈ����
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;  // 2D ������ ����Ͽ� ���� �̵���Ű�Ƿ� ���� ������ ������Ʈ���� ��ٸ�

            }

            yield return waitForFixedUpdate;
        }

        // ��� �ܰ��� �� - �� �޽� �̺�Ʈ Ʈ����
        enemy.idleEvent.CallIdleEvent();

    }

    /// ���� ���� AStar ���� Ŭ������ ����Ͽ� ��θ� �����մϴ�.
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        // �׸��忡�� �÷��̾� ��ġ�� ������
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);


        // �׸��忡�� �� ��ġ�� ������
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        // ���� �̵��� ��θ� ����
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // ù ��° �ܰ踦 ����. �̴� ���� �̹� �ִ� �׸��� �簢��
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            // ��ΰ� ������ �޽� �̺�Ʈ Ʈ����
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// ���� ��θ� ������ ������ ��ȣ�� ���� - ���� ������ �����ϱ� ����
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// �÷��̾�� ���� ����� ��ֹ��� ���� ��ġ�� �����ɴϴ�.
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y]);

        // �÷��̾ ��ֹ��� ǥ�õ� �� �簢���� ���� ������ �ش� ��ġ�� ��ȯ
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // �ݰ� ���� ��ֹ��� ���� ���� ã�ƾ� �մϴ�. '�� �浹' Ÿ�ϰ� ���̺� ������
        // �÷��̾�� ��ֹ��� ǥ�õ� �׸��� �簢���� ���� �� ����
        else
        {
            // �ֺ� ��ġ ����� ���
            surroundingPositionList.Clear();

            // �ֺ� ��ġ ����� ä��. �̴� (0,0) �׸��� �簢�� ������ 8���� ������ ���� ��ġ�� ����
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }


            // ��� ��ġ�� �ݺ�
            for (int l = 0; l < 8; l++)
            {
                // ��Ͽ��� ������ �ε����� ����
                int index = Random.Range(0, surroundingPositionList.Count);

                // ���õ� �ֺ� ��ġ�� ��ֹ��� �ִ��� Ȯ��
                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);

                    // ��ֹ��� ������ �̵��� �� ��ġ�� ��ȯ
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }

                }
                // �׸��� ���� �ֺ� ��ġ�� ���� ������ ����
                catch
                {

                }

                // ��ֹ��� �ִ� �ֺ� ��ġ�� �����ϰ� �ٽ� �õ��� �� �ֵ��� ��
                surroundingPositionList.RemoveAt(index);
            }


            // �÷��̾� �ֺ��� ��ֹ��� ������ ���� �� ��ġ�� ����
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

        }
    }


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif

    #endregion Validation
}
