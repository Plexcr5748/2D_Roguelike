using UnityEngine;

public class GridNodes
{
    private int width; // 그리드의 너비
    private int height; // 그리드의 높이

    private Node[,] gridNode; // 그리드 노드 배열

    /// 그리드 노드의 초기화
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        // 그리드 노드 배열을 초기화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    /// 특정 위치의 그리드 노드 반환
    public Node GetGridNode(int xPosition, int yPosition)
    {
        // 요청된 그리드 노드가 범위 내에 있는지 확인
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            // 범위를 벗어난 경우 경고 로그 출력
            Debug.Log("요청된 그리드 노드가 범위를 벗어났습니다");
            return null;
        }
    }
}
