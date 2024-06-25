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

    #region 던전 빌드 설정
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region 방 설정
    public const float fadeInTime = 0.5f; // 방이 페이드 인되는 시간
    public const int maxChildCorridors = 3; // 방에서 뻗어나가는 최대 하위 복도의 수. - 최대값은 3이지만 이렇게 설정하면 던전 빌드가 실패할 가능성이 있으므로 권장 X 방이 서로 맞지 않을 가능성이 높아짐
    public const float doorUnlockDelay = 1f;
    #endregion

    #region 애니메이터 매개변수
    // 애니메이터 매개변수 - 플레이어
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

    // 애니메이터 매개변수 - 적
    public static float baseSpeedForEnemyAnimations = 3f;

    // 애니메이터 매개변수 - 문
    public static int open = Animator.StringToHash("open");

    // 애니메이터 매개변수 - 데미지 가능한 장식
    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroyed = "Destroyed";
    #endregion

    #region 게임 오브젝트 태그
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region 오디오
    public const float musicFadeOutTime = 0.5f;  // 기본 음악 페이드 아웃 전환 시간
    public const float musicFadeInTime = 0.5f;  // 기본 음악 페이드 인 전환 시간
    #endregion

    #region 발사 제어
    public const float useAimAngleDistance = 3.5f; // 대상 거리가 이 값보다 작으면 플레이어에서 계산된 조준 각도를 사용하고, 그 이상이면 무기에서 계산된 조준 각도를 사용
    #endregion

    #region ASTAR 경로 찾기 매개변수
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region 적 매개변수
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI 매개변수
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region 접촉 데미지 매개변수
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion

    #region 하이스코어
    public const int numberOfHighScoresToSave = 100;
    #endregion
}