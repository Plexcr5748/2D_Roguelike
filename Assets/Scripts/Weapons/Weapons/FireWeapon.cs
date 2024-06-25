using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        // 컴포넌트 로드
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        // 무기 발사 이벤트에 구독
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        // 무기 발사 이벤트에서 구독 해지
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        // 쿨다운 타이머 감소
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    /// 무기 발사 이벤트 처리
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// 무기 발사 처리
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // 무기 선충전 타이머 처리
        WeaponPreCharge(fireWeaponEventArgs);

        // 무기 발사
        if (fireWeaponEventArgs.fire)
        {
            // 무기 발사 준비 여부 확인
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    /// 무기 선충전 처리
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // 무기 선충전
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // 이전 프레임에 발사 버튼을 누르고 있으면 선충전 타이머 감소
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // 그렇지 않으면 선충전 타이머 초기화
            ResetPrechargeTimer();
        }
    }

    /// 무기 발사 준비 여부를 반환합니다.
    private bool IsWeaponReadyToFire()
    {
        // 탄약이 없고 무기가 무한 탄약을 가지고 있지 않으면 false를 반환
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        // 무기가 재장전 중이면 false를 반환
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        // 무기가 선충전되지 않았거나 쿨다운 중이면 false를 반환
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        // 탄창에 탄약이 없고 무기가 무한 탄창 용량을 가지고 있지 않으면 false를 반환
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            // 재장전 이벤트 호출
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }

        // 무기가 발사 준비 상태임을 나타내는 true를 반환
        return true;
    }

    /// 총알을 발사
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            // 총알 발사 루틴 시작
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    /// 지정된 총알 세트당 여러 개의 총알을 생성하는 코루틴
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        // 한 발당 발사할 총알 수
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        // 총알 사이의 랜덤 간격
        float ammoSpawnInterval;

        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        // 발사할 총알 수 만큼 반복
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            // 총알 프리팹을 배열에서 가져옴
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // 무기 발사 위치에서 총알 발사
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax), weaponAimDirectionVector);

            // 총알 발사 간격만큼 대기
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // 탄창의 탄약 수 감소 (무한 탄창 용량이 아닌 경우)
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        // 무기 발사 이벤트 호출
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        // 무기 발사 효과 표시
        WeaponShootEffect(aimAngle);

        // 무기 발사 사운드 효과 재생
        WeaponSoundEffect();
    }

    /// 쿨다운 타이머를 재설정
    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    /// 선충전 타이머를 재설정
    private void ResetPrechargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    /// 무기 발사 효과를 표시
    private void WeaponShootEffect(float aimAngle)
    {
        // 발사 효과가 있는 경우 처리
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null && activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            // 오브젝트 풀에서 무기 발사 효과 게임오브젝트 가져옴
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab, activeWeapon.GetShootEffectPosition(), Quaternion.identity);

            // 발사 효과 설정
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);

            // 게임오브젝트 활성화 (파티클 시스템이 자동으로 완료되면 게임오브젝트를 비활성화)
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    /// 무기 발사 사운드 효과를 재생
    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
