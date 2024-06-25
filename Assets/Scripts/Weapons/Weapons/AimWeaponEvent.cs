using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    // ���� ���� �̺�Ʈ�� ���� �׼� �̺�Ʈ
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    /// ���� ���� �̺�Ʈ�� ȣ���Ͽ� ���� ���� �޼���鿡�� ����
    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }

}


public class AimWeaponEventArgs : EventArgs
{
    // ���� ���� �̺�Ʈ�� ���� Ŭ����
    public AimDirection aimDirection;                 // ���� ����
    public float aimAngle;                            // ���� ����
    public float weaponAimAngle;                      // ���� ���� ����
    public Vector3 weaponAimDirectionVector;          // ���� ���� ���� ����
}