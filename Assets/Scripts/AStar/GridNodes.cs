using UnityEngine;

public class GridNodes
{
    private int width; // �׸����� �ʺ�
    private int height; // �׸����� ����

    private Node[,] gridNode; // �׸��� ��� �迭

    /// �׸��� ����� �ʱ�ȭ
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        // �׸��� ��� �迭�� �ʱ�ȭ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    /// Ư�� ��ġ�� �׸��� ��� ��ȯ
    public Node GetGridNode(int xPosition, int yPosition)
    {
        // ��û�� �׸��� ��尡 ���� ���� �ִ��� Ȯ��
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            // ������ ��� ��� ��� �α� ���
            Debug.Log("��û�� �׸��� ��尡 ������ ������ϴ�");
            return null;
        }
    }
}
