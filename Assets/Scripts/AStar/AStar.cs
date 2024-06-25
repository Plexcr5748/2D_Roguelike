using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// 시작 위치(startGridPosition)에서 종료 위치(endGridPosition)까지의 경로를 찾아 스택에 추가
    /// 경로가 없으면 null을 반환
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // 시작 및 종료 위치를 템플릿의 하한값만큼 조정
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // 오픈 리스트와 클로즈드 해시셋 생성
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // 경로 찾기를 위한 그리드 노드 생성
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    /// 가장 짧은 경로를 찾고, 경로가 있으면 종료 노드를 반환하고, 없으면 null을 반환
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // 시작 노드를 오픈 리스트에 추가
        openNodeList.Add(startNode);

        // 오픈 리스트가 비어있지 않을 때까지 반복
        while (openNodeList.Count > 0)
        {
            // 오픈 리스트를 정렬합니다.
            openNodeList.Sort();

            // 현재 노드를 오픈 리스트에서 가장 낮은 fCost를 가진 노드로 설정
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // 현재 노드가 목표 노드와 같다면 종료
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // 현재 노드를 클로즈드 리스트에 추가
            closedNodeHashSet.Add(currentNode);

            // 현재 노드의 이웃 노드를 평가
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }

        return null;
    }

    /// 이동 경로를 포함하는 Stack<Vector3>을 생성
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        // 셀의 중앙 점을 가져옴
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // 그리드 위치를 월드 위치로 변환
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x, nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            // 그리드 셀의 중간 위치로 설정
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// 이웃 노드를 평가
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // 모든 방향을 반복
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    // 이웃에 대한 새로운 gCost 계산
                    int newCostToNeighbour;

                    // 이동 페널티를 가져옴
                    // 이동할 수 없는 경로는 값이 0, 기본 이동 페널티는 설정에서 적용
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }


    /// nodeA와 nodeB 사이의 거리(int)를 반환
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // 10은 1 대신 사용되고, 14는 피타고라스의 근사치(SQRT(10*10 + 10*10) - float 사용을 피하기 위해)
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// gridNodes, closedNodeHashSet, 그리고 instantiated room을 사용하여 (neighbourNodeXPosition, neighbourNodeYPosition)에 있는 이웃 노드를 평가
    /// 유효하지 않으면 null을 반환
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // 이웃 노드의 위치가 그리드 밖이면 null을 반환
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // 이웃 노드를 가져옴
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // 해당 위치에서의 장애물 확인
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // 해당 위치에서의 이동 가능한 장애물 확인
        int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

        // 이웃이 장애물이거나 이웃이 클로즈드 리스트에 있으면 건너뜀
        if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}