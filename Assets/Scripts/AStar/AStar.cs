using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    /// ���� ��ġ(startGridPosition)���� ���� ��ġ(endGridPosition)������ ��θ� ã�� ���ÿ� �߰�
    /// ��ΰ� ������ null�� ��ȯ
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // ���� �� ���� ��ġ�� ���ø��� ���Ѱ���ŭ ����
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // ���� ����Ʈ�� Ŭ����� �ؽü� ����
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // ��� ã�⸦ ���� �׸��� ��� ����
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

    /// ���� ª�� ��θ� ã��, ��ΰ� ������ ���� ��带 ��ȯ�ϰ�, ������ null�� ��ȯ
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // ���� ��带 ���� ����Ʈ�� �߰�
        openNodeList.Add(startNode);

        // ���� ����Ʈ�� ������� ���� ������ �ݺ�
        while (openNodeList.Count > 0)
        {
            // ���� ����Ʈ�� �����մϴ�.
            openNodeList.Sort();

            // ���� ��带 ���� ����Ʈ���� ���� ���� fCost�� ���� ���� ����
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // ���� ��尡 ��ǥ ���� ���ٸ� ����
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // ���� ��带 Ŭ����� ����Ʈ�� �߰�
            closedNodeHashSet.Add(currentNode);

            // ���� ����� �̿� ��带 ��
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }

        return null;
    }

    /// �̵� ��θ� �����ϴ� Stack<Vector3>�� ����
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        // ���� �߾� ���� ������
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // �׸��� ��ġ�� ���� ��ġ�� ��ȯ
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x, nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            // �׸��� ���� �߰� ��ġ�� ����
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// �̿� ��带 ��
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // ��� ������ �ݺ�
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    // �̿��� ���� ���ο� gCost ���
                    int newCostToNeighbour;

                    // �̵� ���Ƽ�� ������
                    // �̵��� �� ���� ��δ� ���� 0, �⺻ �̵� ���Ƽ�� �������� ����
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


    /// nodeA�� nodeB ������ �Ÿ�(int)�� ��ȯ
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // 10�� 1 ��� ���ǰ�, 14�� ��Ÿ����� �ٻ�ġ(SQRT(10*10 + 10*10) - float ����� ���ϱ� ����)
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// gridNodes, closedNodeHashSet, �׸��� instantiated room�� ����Ͽ� (neighbourNodeXPosition, neighbourNodeYPosition)�� �ִ� �̿� ��带 ��
    /// ��ȿ���� ������ null�� ��ȯ
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        // �̿� ����� ��ġ�� �׸��� ���̸� null�� ��ȯ
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // �̿� ��带 ������
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // �ش� ��ġ������ ��ֹ� Ȯ��
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // �ش� ��ġ������ �̵� ������ ��ֹ� Ȯ��
        int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

        // �̿��� ��ֹ��̰ų� �̿��� Ŭ����� ����Ʈ�� ������ �ǳʶ�
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