using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    // 무기 조준 이벤트에 대한 액션 이벤트
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    /// 무기 조준 이벤트를 호출하여 구독 중인 메서드들에게 전달
    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }

}


public class AimWeaponEventArgs : EventArgs
{
    // 무기 조준 이벤트의 인자 클래스
    public AimDirection aimDirection;                 // 조준 방향
    public float aimAngle;                            // 조준 각도
    public float weaponAimAngle;                      // 무기 조준 각도
    public Vector3 weaponAimDirectionVector;          // 무기 조준 방향 벡터
}