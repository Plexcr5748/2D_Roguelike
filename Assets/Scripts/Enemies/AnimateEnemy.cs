using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        // 컴포넌트 로드
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        // 이동 이벤트에 구독
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // 대기 이벤트에 구독
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;

        // 무기 조준 이벤트에 구독
        enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        // 이동 이벤트에서 구독 해제
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // 대기 이벤트에서 구독 해제
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // 무기 조준 이벤트에서 구독 해제
        enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// 무기 조준 이벤트 핸들러
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        초기화_조준_애니메이션_매개변수();
        무기_조준_애니메이션_매개변수_설정(aimWeaponEventArgs.aimDirection);
    }

    /// 이동 이벤트 핸들러
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        이동_애니메이션_매개변수_설정();
    }

    /// 대기 이벤트 핸들러
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        대기_애니메이션_매개변수_설정();
    }

    /// 조준 애니메이션 매개변수 초기화
    private void 초기화_조준_애니메이션_매개변수()
    {
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimDown, false);
    }

    /// 이동 애니메이션 매개변수 설정
    private void 이동_애니메이션_매개변수_설정()
    {
        // 이동 설정
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }

    /// 대기 애니메이션 매개변수 설정
    private void 대기_애니메이션_매개변수_설정()
    {
        // 대기 설정
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    /// 무기 조준 애니메이션 매개변수 설정
    private void 무기_조준_애니메이션_매개변수_설정(AimDirection aimDirection)
    {
        // 조준 방향 설정
        switch (aimDirection)
        {
            case AimDirection.Up:
                enemy.animator.SetBool(Settings.aimUp, true);
                break;

            case AimDirection.UpRight:
                enemy.animator.SetBool(Settings.aimUpRight, true);
                break;

            case AimDirection.UpLeft:
                enemy.animator.SetBool(Settings.aimUpLeft, true);
                break;

            case AimDirection.Right:
                enemy.animator.SetBool(Settings.aimRight, true);
                break;

            case AimDirection.Left:
                enemy.animator.SetBool(Settings.aimLeft, true);
                break;

            case AimDirection.Down:
                enemy.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }
}