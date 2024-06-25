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
        // ������Ʈ �ε�
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        // ���� �߻� �̺�Ʈ�� ����
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        // ���� �߻� �̺�Ʈ���� ���� ����
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        // ��ٿ� Ÿ�̸� ����
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    /// ���� �߻� �̺�Ʈ ó��
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// ���� �߻� ó��
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // ���� ������ Ÿ�̸� ó��
        WeaponPreCharge(fireWeaponEventArgs);

        // ���� �߻�
        if (fireWeaponEventArgs.fire)
        {
            // ���� �߻� �غ� ���� Ȯ��
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    /// ���� ������ ó��
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // ���� ������
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // ���� �����ӿ� �߻� ��ư�� ������ ������ ������ Ÿ�̸� ����
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // �׷��� ������ ������ Ÿ�̸� �ʱ�ȭ
            ResetPrechargeTimer();
        }
    }

    /// ���� �߻� �غ� ���θ� ��ȯ�մϴ�.
    private bool IsWeaponReadyToFire()
    {
        // ź���� ���� ���Ⱑ ���� ź���� ������ ���� ������ false�� ��ȯ
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        // ���Ⱑ ������ ���̸� false�� ��ȯ
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        // ���Ⱑ ���������� �ʾҰų� ��ٿ� ���̸� false�� ��ȯ
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        // źâ�� ź���� ���� ���Ⱑ ���� źâ �뷮�� ������ ���� ������ false�� ��ȯ
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            // ������ �̺�Ʈ ȣ��
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }

        // ���Ⱑ �߻� �غ� �������� ��Ÿ���� true�� ��ȯ
        return true;
    }

    /// �Ѿ��� �߻�
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            // �Ѿ� �߻� ��ƾ ����
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    /// ������ �Ѿ� ��Ʈ�� ���� ���� �Ѿ��� �����ϴ� �ڷ�ƾ
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        // �� �ߴ� �߻��� �Ѿ� ��
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        // �Ѿ� ������ ���� ����
        float ammoSpawnInterval;

        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        // �߻��� �Ѿ� �� ��ŭ �ݺ�
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            // �Ѿ� �������� �迭���� ������
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // ���� �߻� ��ġ���� �Ѿ� �߻�
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax), weaponAimDirectionVector);

            // �Ѿ� �߻� ���ݸ�ŭ ���
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // źâ�� ź�� �� ���� (���� źâ �뷮�� �ƴ� ���)
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        // ���� �߻� �̺�Ʈ ȣ��
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        // ���� �߻� ȿ�� ǥ��
        WeaponShootEffect(aimAngle);

        // ���� �߻� ���� ȿ�� ���
        WeaponSoundEffect();
    }

    /// ��ٿ� Ÿ�̸Ӹ� �缳��
    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    /// ������ Ÿ�̸Ӹ� �缳��
    private void ResetPrechargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    /// ���� �߻� ȿ���� ǥ��
    private void WeaponShootEffect(float aimAngle)
    {
        // �߻� ȿ���� �ִ� ��� ó��
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null && activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            // ������Ʈ Ǯ���� ���� �߻� ȿ�� ���ӿ�����Ʈ ������
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab, activeWeapon.GetShootEffectPosition(), Quaternion.identity);

            // �߻� ȿ�� ����
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);

            // ���ӿ�����Ʈ Ȱ��ȭ (��ƼŬ �ý����� �ڵ����� �Ϸ�Ǹ� ���ӿ�����Ʈ�� ��Ȱ��ȭ)
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    /// ���� �߻� ���� ȿ���� ���
    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
