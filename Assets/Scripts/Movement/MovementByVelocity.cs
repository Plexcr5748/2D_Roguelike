using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    #region Header
    [Header("References")]
    #endregion
    [SerializeField] private Rigidbody2D rigidBody2D;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        // ������Ʈ �ε�
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        // �̵� �̺�Ʈ ����
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        // �̵� �̺�Ʈ ���� ����
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    /// MovementByVelocityEvent �̺�Ʈ �ڵ鷯
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    /// ������ٵ� �̵���ŵ�ϴ�.
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        // ������ٵ� �ӵ� ���� (�浹 ������ �������� ������)
        rigidBody2D.velocity = moveDirection * moveSpeed;
    }
}