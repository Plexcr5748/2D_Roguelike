using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    /// 무기 발사 이벤트를 호출합니다.
    public void CallFireWeaponEvent(bool fire, bool firePreviousFrame, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponEventArgs()
        {
            fire = fire,
            firePreviousFrame = firePreviousFrame,
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponAimDirectionVector = weaponAimDirectionVector
        });
    }
}

/// 무기 발사 이벤트의 인수 클래스
public class FireWeaponEventArgs : EventArgs
{
    public bool fire;                       // 현재 프레임에서 발사 버튼이 눌렸는지 여부
    public bool firePreviousFrame;          // 이전 프레임에서 발사 버튼이 눌렸는지 여부
    public AimDirection aimDirection;       // 목표 방향
    public float aimAngle;                  // 목표 각도
    public float weaponAimAngle;            // 무기의 목표 각도
    public Vector3 weaponAimDirectionVector; // 무기의 목표 방향 벡터
}