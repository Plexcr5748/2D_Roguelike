using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    /// ���� �߻� �̺�Ʈ�� ȣ���մϴ�.
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

/// ���� �߻� �̺�Ʈ�� �μ� Ŭ����
public class FireWeaponEventArgs : EventArgs
{
    public bool fire;                       // ���� �����ӿ��� �߻� ��ư�� ���ȴ��� ����
    public bool firePreviousFrame;          // ���� �����ӿ��� �߻� ��ư�� ���ȴ��� ����
    public AimDirection aimDirection;       // ��ǥ ����
    public float aimAngle;                  // ��ǥ ����
    public float weaponAimAngle;            // ������ ��ǥ ����
    public Vector3 weaponAimDirectionVector; // ������ ��ǥ ���� ����
}