using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        // ������Ʈ �ε�
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        // �̵� �̺�Ʈ�� ����
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // ��� �̺�Ʈ�� ����
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;

        // ���� ���� �̺�Ʈ�� ����
        enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        // �̵� �̺�Ʈ���� ���� ����
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // ��� �̺�Ʈ���� ���� ����
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // ���� ���� �̺�Ʈ���� ���� ����
        enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    /// ���� ���� �̺�Ʈ �ڵ鷯
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        �ʱ�ȭ_����_�ִϸ��̼�_�Ű�����();
        ����_����_�ִϸ��̼�_�Ű�����_����(aimWeaponEventArgs.aimDirection);
    }

    /// �̵� �̺�Ʈ �ڵ鷯
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        �̵�_�ִϸ��̼�_�Ű�����_����();
    }

    /// ��� �̺�Ʈ �ڵ鷯
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        ���_�ִϸ��̼�_�Ű�����_����();
    }

    /// ���� �ִϸ��̼� �Ű����� �ʱ�ȭ
    private void �ʱ�ȭ_����_�ִϸ��̼�_�Ű�����()
    {
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimDown, false);
    }

    /// �̵� �ִϸ��̼� �Ű����� ����
    private void �̵�_�ִϸ��̼�_�Ű�����_����()
    {
        // �̵� ����
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }

    /// ��� �ִϸ��̼� �Ű����� ����
    private void ���_�ִϸ��̼�_�Ű�����_����()
    {
        // ��� ����
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    /// ���� ���� �ִϸ��̼� �Ű����� ����
    private void ����_����_�ִϸ��̼�_�Ű�����_����(AimDirection aimDirection)
    {
        // ���� ���� ����
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