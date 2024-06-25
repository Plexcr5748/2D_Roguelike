using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition; // �׸��� ���� ��ġ
    public int gCost = 0; // ���� ���κ����� �Ÿ�
    public int hCost = 0; // ��ǥ ���κ����� �Ÿ�
    public Node parentNode; // �θ� ���

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    // FCost�� gCost�� hCost�� �հ�
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    // �ٸ� ���� ���Ͽ� FCost�� ������, ū��, �������� ��ȯ
    public int CompareTo(Node nodeToCompare)
    {
        // �� ����� �� �ν��Ͻ��� FCost�� nodeToCompare.FCost���� ������ < 0,
        // ũ�� > 0, ������ == 0�� ��ȯ
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        // ���� FCost�� ���ٸ�, hCost�� ��
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;
    }
}
