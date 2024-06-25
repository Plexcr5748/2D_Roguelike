using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS

    [Space(10)]
    [Header("�⺻ ���� ���� ����")]

    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip

    [Tooltip("���� �̸�")]

    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL

    [Space(10)]
    [Header("������ �� ���ø�")]

    #endregion Header ROOM TEMPLATES FOR LEVEL

    #region Tooltip

    [Tooltip("������ ���Ե� �� ���ø� ����Դϴ�. ��� �� ��� ������ ������ �� ��� �׷����� ���ԵǾ� �ִ��� Ȯ���ؾ� �մϴ�.")]

    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL

    [Space(10)]
    [Header("������ �� ��� �׷���")]

    #endregion Header ROOM NODE GRAPHS FOR LEVEL

    #region Tooltip

    [Tooltip("�������� �������� ������ �� ��� �׷��� ����Դϴ�.")]

    #endregion Tooltip

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation

#if UNITY_EDITOR

    // ��ũ���ͺ� ������Ʈ�� �Էµ� ���� ������ ��ȿ�� �˻�
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        // ������ ��� �׷����� ��� ��� ������ ���� �� ���ø��� �����Ǿ����� Ȯ��

        // ���� ��/�� �� ����, ��/�� �� ���� �� �Ա� ������ �����Ǿ����� Ȯ��
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // ��� �� ���ø��� �ݺ��Ͽ� �� ��� ������ �����Ǿ����� Ȯ��
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log(this.name.ToString() + "����: ��/�� ���� �� ������ �������� �ʾҽ��ϴ�.");
        }

        if (isNSCorridor == false)
        {
            Debug.Log(this.name.ToString() + "����: ��/�� ���� �� ������ �������� �ʾҽ��ϴ�.");
        }

        if (isEntrance == false)
        {
            Debug.Log(this.name.ToString() + "����: �Ա� ���� �� ������ �������� �ʾҽ��ϴ�.");
        }

        // ��� ��� �׷����� �ݺ�
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // ��� �׷��� ���� ��� ��带 �ݺ�
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // �� roomNode ������ ���� �� ���ø��� �����Ǿ����� Ȯ��

                // ������ �Ա��� �̹� Ȯ��
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                // ��� �� ���ø��� �ݺ��Ͽ� �� ��� ������ �����Ǿ����� Ȯ��
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {

                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }

                }

                if (!isRoomNodeTypeFound)
                    Debug.Log(this.name.ToString() + "����: " + roomNodeGraph.name.ToString() + " ��� �׷����� " + roomNodeSO.roomNodeType.name.ToString() + " �� ���ø��� ã�� �� �����ϴ�.");


            }
        }
    }

#endif

    #endregion
}