using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("�ּ� �̵� �ӵ�. GetMoveSpeed �޼���� �ּ� �� �ִ� �� ���̿��� ������ ���� ����մϴ�.")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("�ִ� �̵� �ӵ�. GetMoveSpeed �޼���� �ּ� �� �ִ� �� ���̿��� ������ ���� ����մϴ�.")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("������ �������� �ִ� ����� ������ �ӵ�")]
    #endregion
    public float rollSpeed; // �÷��̾��
    #region Tooltip
    [Tooltip("������ �������� �ִ� ����� ������ �Ÿ�")]
    #endregion
    public float rollDistance; // �÷��̾��
    #region Tooltip
    [Tooltip("������ �������� �ִ� ����� ������ �׼� ���� ��ٿ� �ð�(��)")]
    #endregion
    public float rollCooldownTime; // �÷��̾��

    /// �ּ� �� �ִ� �� ���̿��� ������ �̵� �ӵ��� ������
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0f || rollSpeed != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }

    }

#endif
    #endregion Validation
}