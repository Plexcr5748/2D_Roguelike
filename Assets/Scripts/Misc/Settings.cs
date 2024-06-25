using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Settings
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region ���� ���� ����
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region �� ����
    public const float fadeInTime = 0.5f; // ���� ���̵� �εǴ� �ð�
    public const int maxChildCorridors = 3; // �濡�� ������� �ִ� ���� ������ ��. - �ִ밪�� 3������ �̷��� �����ϸ� ���� ���尡 ������ ���ɼ��� �����Ƿ� ���� X ���� ���� ���� ���� ���ɼ��� ������
    public const float doorUnlockDelay = 1f;
    #endregion

    #region �ִϸ����� �Ű�����
    // �ִϸ����� �Ű����� - �÷��̾�
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static int use = Animator.StringToHash("use");
    public static float baseSpeedForPlayerAnimations = 8f;

    // �ִϸ����� �Ű����� - ��
    public static float baseSpeedForEnemyAnimations = 3f;

    // �ִϸ����� �Ű����� - ��
    public static int open = Animator.StringToHash("open");

    // �ִϸ����� �Ű����� - ������ ������ ���
    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroyed = "Destroyed";
    #endregion

    #region ���� ������Ʈ �±�
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region �����
    public const float musicFadeOutTime = 0.5f;  // �⺻ ���� ���̵� �ƿ� ��ȯ �ð�
    public const float musicFadeInTime = 0.5f;  // �⺻ ���� ���̵� �� ��ȯ �ð�
    #endregion

    #region �߻� ����
    public const float useAimAngleDistance = 3.5f; // ��� �Ÿ��� �� ������ ������ �÷��̾�� ���� ���� ������ ����ϰ�, �� �̻��̸� ���⿡�� ���� ���� ������ ���
    #endregion

    #region ASTAR ��� ã�� �Ű�����
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region �� �Ű�����
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI �Ű�����
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region ���� ������ �Ű�����
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion

    #region ���̽��ھ�
    public const int numberOfHighScoresToSave = 100;
    #endregion
}