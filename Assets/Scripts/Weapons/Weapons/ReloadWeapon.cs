using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Coroutine reloadWeaponCoroutine;

    private void Awake()
    {
        // ������Ʈ �ε�
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        // ������ ���� �̺�Ʈ ����
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        // Ȱ�� ���� ���� �̺�Ʈ ����
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        // ������ ���� �̺�Ʈ ���� ����
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        // Ȱ�� ���� ���� �̺�Ʈ ���� ����
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    /// ������ ���� �̺�Ʈ ó��
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    /// ���⸦ ������ ����
    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.topUpAmmoPercent));
    }

    /// ���� ������ �ڷ�ƾ
    private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent)
    {
        // ������ �Ҹ� ���
        if (weapon.weaponDetails.weaponReloadingSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.weaponReloadingSoundEffect);
        }

        // ���⸦ ������ ������ ����
        weapon.isWeaponReloading = true;

        // ������ ���� Ÿ�̸� ������Ʈ
        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        // ��ü ź���� �����ؾ� �ϴ� ��� ó��
        if (topUpAmmoPercent != 0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.weaponAmmoCapacity * topUpAmmoPercent) / 100f);
            int totalAmmo = weapon.weaponRemainingAmmo + ammoIncrease;

            if (totalAmmo > weapon.weaponDetails.weaponAmmoCapacity)
            {
                weapon.weaponRemainingAmmo = weapon.weaponDetails.weaponAmmoCapacity;
            }
            else
            {
                weapon.weaponRemainingAmmo = totalAmmo;
            }
        }

        // ���Ⱑ ���� ź���� ������ ������ Ŭ���� �ٽ� ä��
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        // ���� ź���� �ƴ϶�� ���� ź���� Ŭ���� ä��� �� �ʿ��� �纸�� ũ�� Ŭ���� ������ ä��
        else if (weapon.weaponRemainingAmmo >= weapon.weaponDetails.weaponClipAmmoCapacity)
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        // �׷��� ������ Ŭ���� ���� ź������ ����
        else
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponRemainingAmmo;
        }

        // ���� ������ Ÿ�̸� �ʱ�ȭ
        weapon.weaponReloadTimer = 0f;

        // ���⸦ ������ ���� �ƴ����� ����
        weapon.isWeaponReloading = false;

        // ���� ������ �Ϸ� �̺�Ʈ ȣ��
        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }

    /// Ȱ�� ���� ���� �̺�Ʈ �ڵ鷯
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon.isWeaponReloading)
        {
            if (reloadWeaponCoroutine != null)
            {
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(setActiveWeaponEventArgs.weapon, 0));
        }
    }
}
