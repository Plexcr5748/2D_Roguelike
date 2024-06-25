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
        // 컴포넌트 로드
        rigidBody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();

    }

    private void OnEnable()
    {
        // idle 이벤트에 구독
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // idle 이벤트에서 구독 해지
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    /// idle 이벤트 핸들러
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    ///  rigidBody를 idle 상태로 이동
    private void MoveRigidBody()
    {
        //  rigidBody 속도 초기화
        rigidBody2D.velocity = Vector2.zero;
    }
}