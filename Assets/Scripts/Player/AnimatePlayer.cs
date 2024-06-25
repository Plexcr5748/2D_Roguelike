using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // ������Ʈ �ε�
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        // �ӵ��� ���� �̵� �̺�Ʈ ����
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        // ��ġ�� �̵��ϴ� �̺�Ʈ ����
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // ��� ���� �̺�Ʈ ����
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        // ���� ���� �̺�Ʈ ����
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        // �ӵ��� ���� �̵� �̺�Ʈ ���� ���
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        // ��ġ�� �̵��ϴ� �̺�Ʈ ���� ���
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // ��� ���� �̺�Ʈ ���� ���
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // ���� ���� �̺�Ʈ ���� ���
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// �ӵ��� ���� �̵� �̺�Ʈ �ڵ鷯
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        InitializeRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    /// ��ġ�� �̵� �̺�Ʈ �ڵ鷯
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetMovementToPositionAnimationParameters(movementToPositionArgs);
    }

    /// ��� ���� �̺�Ʈ �ڵ鷯
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitializeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    /// ���� ���� �̺�Ʈ �ڵ鷯
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }

    /// ���� �ִϸ��̼� �Ű������� �ʱ�ȭ
    private void InitializeAimAnimationParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    /// ȸ�� �ִϸ��̼� �Ű������� �ʱ�ȭ
    private void InitializeRollAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollUp, false);
    }

    /// �̵� �ִϸ��̼� �Ű������� ����
    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    /// ��ġ�� �̵� �ִϸ��̼� �Ű������� ����
    private void SetMovementToPositionAnimationParameters(MovementToPositionArgs movementToPositionArgs)
    {
        // ������ �ִϸ��̼� ����
        if (movementToPositionArgs.isRolling)
        {
            if (movementToPositionArgs.moveDirection.x > 0f)
            {
                player.animator.SetBool(Settings.rollRight, true);
            }
            else if (movementToPositionArgs.moveDirection.x < 0f)
            {
                player.animator.SetBool(Settings.rollLeft, true);
            }
            else if (movementToPositionArgs.moveDirection.y > 0f)
            {
                player.animator.SetBool(Settings.rollUp, true);
            }
            else if (movementToPositionArgs.moveDirection.y < 0f)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }

    /// ��� ���� �ִϸ��̼� �Ű������� ����
    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    /// ���� �ִϸ��̼� �Ű������� ����
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        // ���� ���� ����
        switch (aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;

            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;

            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;

            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;

            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;

            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }
}
