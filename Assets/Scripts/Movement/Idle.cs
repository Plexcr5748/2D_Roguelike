using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    #region Header
    [Header("Reference")]
    #endregion
    [SerializeField] private Rigidbody2D rigidBody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        // ������Ʈ �ε�
        rigidBody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();

    }

    private void OnEnable()
    {
        // idle �̺�Ʈ�� ����
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // idle �̺�Ʈ���� ���� ����
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    /// idle �̺�Ʈ �ڵ鷯
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    ///  rigidBody�� idle ���·� �̵�
    private void MoveRigidBody()
    {
        //  rigidBody �ӵ� �ʱ�ȭ
        rigidBody2D.velocity = Vector2.zero;
    }
}