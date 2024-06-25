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
    [Tooltip("최소 이동 속도. GetMoveSpeed 메서드는 최소 및 최대 값 사이에서 무작위 값을 계산합니다.")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("최대 이동 속도. GetMoveSpeed 메서드는 최소 및 최대 값 사이에서 무작위 값을 계산합니다.")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("구르기 움직임이 있는 경우의 구르기 속도")]
    #endregion
    public float rollSpeed; // 플레이어용
    #region Tooltip
    [Tooltip("구르기 움직임이 있는 경우의 구르기 거리")]
    #endregion
    public float rollDistance; // 플레이어용
    #region Tooltip
    [Tooltip("구르기 움직임이 있는 경우의 구르기 액션 간의 쿨다운 시간(초)")]
    #endregion
    public float rollCooldownTime; // 플레이어용

    /// 최소 및 최대 값 사이에서 무작위 이동 속도를 가져옴
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