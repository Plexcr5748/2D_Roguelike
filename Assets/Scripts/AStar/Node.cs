using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition; // 그리드 상의 위치
    public int gCost = 0; // 시작 노드로부터의 거리
    public int hCost = 0; // 목표 노드로부터의 거리
    public Node parentNode; // 부모 노드

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    // FCost는 gCost와 hCost의 합계
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    // 다른 노드와 비교하여 FCost가 작은지, 큰지, 같은지를 반환
    public int CompareTo(Node nodeToCompare)
    {
        // 비교 결과는 이 인스턴스의 FCost가 nodeToCompare.FCost보다 작으면 < 0,
        // 크면 > 0, 같으면 == 0을 반환
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        // 만약 FCost가 같다면, hCost를 비교
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;
    }
}
