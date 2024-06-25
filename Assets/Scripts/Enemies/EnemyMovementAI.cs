using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO 스크립터블 오브젝트는 속도와 같은 움직임 세부 정보를 포함합니다.")]
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
    [HideInInspector] public int updateFrameNumber = 1; // 기본 값. 이는 적 스포너에서 설정
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        // 컴포넌트 로드
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // 사용할 Fixed Update를 위한 대기
        waitForFixedUpdate = new WaitForFixedUpdate();

        // 플레이어 참조 위치 초기화
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

    }

    private void Update()
    {
        MoveEnemy();
    }


    /// AStar 경로 탐색을 사용하여 플레이어에게 이동 경로를 구축하고 적을 각 그리드 위치로 이동
    private void MoveEnemy()
    {
        // 움직임 쿨다운 타이머
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        // 적이 추격을 시작해야 하는 거리에 있는지 확인
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        // 플레이어를 추격할 거리에 있지 않으면 반환
        if (!chasePlayer)
            return;

        // 일정한 프레임에서만 A Star 경로 재구성을 처리하여 부하를 분산.
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        // 움직임 쿨다운 타이머가 만료되었거나 플레이어가 필요한 거리보다 더 멀리 움직였으면
        // 적 경로를 재구성하고 적을 이동
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            // 경로 재구성 쿨다운 타이머 초기화
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            // 플레이어 참조 위치 재설정
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            // AStar 경로 탐색을 사용하여 적을 이동. - 플레이어에게 경로 재구성 트리거
            CreatePath();

            // 경로를 찾았다면 적을 이동
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    // 휴식 이벤트 트리거
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // 코루틴을 사용하여 경로를 따라 적을 이동
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

            }
        }
    }


    /// <summary>
    /// 적을 경로의 다음 위치로 이동시키는 코루틴
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            // 매우 가까울 때까지 이동을 계속 가까워지면 다음 단계로 이동
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                // 움직임 이벤트 트리거
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;  // 2D 물리를 사용하여 적을 이동시키므로 다음 고정된 업데이트까지 기다림

            }

            yield return waitForFixedUpdate;
        }

        // 경로 단계의 끝 - 적 휴식 이벤트 트리거
        enemy.idleEvent.CallIdleEvent();

    }

    /// 적을 위한 AStar 정적 클래스를 사용하여 경로를 생성합니다.
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        // 그리드에서 플레이어 위치를 가져옴
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);


        // 그리드에서 적 위치를 가져옴
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        // 적이 이동할 경로를 구축
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // 첫 번째 단계를 제거. 이는 적이 이미 있는 그리드 사각형
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            // 경로가 없으면 휴식 이벤트 트리거
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// 적의 경로를 재계산할 프레임 번호를 설정 - 성능 급증을 방지하기 위해
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// 플레이어와 가장 가까운 장애물이 없는 위치를 가져옵니다.
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y]);

        // 플레이어가 장애물이 표시된 셀 사각형에 있지 않으면 해당 위치를 반환
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // 반경 내에 장애물이 없는 셀을 찾아야 합니다. '반 충돌' 타일과 테이블 때문에
        // 플레이어는 장애물로 표시된 그리드 사각형에 있을 수 있음
        else
        {
            // 주변 위치 목록을 비움
            surroundingPositionList.Clear();

            // 주변 위치 목록을 채움. 이는 (0,0) 그리드 사각형 주위의 8개의 가능한 벡터 위치를 보유
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }


            // 모든 위치를 반복
            for (int l = 0; l < 8; l++)
            {
                // 목록에서 임의의 인덱스를 생성
                int index = Random.Range(0, surroundingPositionList.Count);

                // 선택된 주변 위치에 장애물이 있는지 확인
                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);

                    // 장애물이 없으면 이동할 셀 위치를 반환
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }

                }
                // 그리드 밖의 주변 위치에 대한 오류를 잡음
                catch
                {

                }

                // 장애물이 있는 주변 위치를 제거하고 다시 시도할 수 있도록 함
                surroundingPositionList.RemoveAt(index);
            }


            // 플레이어 주변에 장애물이 없으면 적을 적 위치로 보냄
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
