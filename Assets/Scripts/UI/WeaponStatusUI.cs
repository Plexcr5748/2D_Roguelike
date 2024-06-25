using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("��ü ����")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("WeaponImage ���ӿ�����Ʈ�� Image ������Ʈ�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private Image weaponImage;
    #region Tooltip
    [Tooltip("AmmoHolder ���ӿ�����Ʈ�� Transform�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private Transform ammoHolderTransform;
    #region Tooltip
    [Tooltip("ReloadText ���ӿ�����Ʈ�� TextMeshPro-Text ������Ʈ�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI reloadText;
    #region Tooltip
    [Tooltip("AmmoRemainingText ���ӿ�����Ʈ�� TextMeshPro-Text ������Ʈ�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    #region Tooltip
    [Tooltip("WeaponNameText ���ӿ�����Ʈ�� TextMeshPro-Text ������Ʈ�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI weaponNameText;
    #region Tooltip
    [Tooltip("ReloadBar ���ӿ�����Ʈ�� RectTransform�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private Transform reloadBar;
    #region Tooltip
    [Tooltip("BarImage ���ӿ�����Ʈ�� Image ������Ʈ�� ���⿡ ��������.")]
    #endregion Tooltip
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        // �÷��̾ ������
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        // Ȱ�� ���� ���� �̺�Ʈ�� ����
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        // ���� �߻� �̺�Ʈ�� ����
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        // ���� ������ �̺�Ʈ�� ����
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReload;

        // ���� ������ �Ϸ� �̺�Ʈ�� ����
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        // Ȱ�� ���� ���� �̺�Ʈ ������ ����
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        // ���� �߻� �̺�Ʈ ������ ����
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        // ���� ������ �̺�Ʈ ������ ����
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReload;

        // ���� ������ �Ϸ� �̺�Ʈ ������ ����
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        // UI���� Ȱ�� ���� ���¸� ������Ʈ
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    /// UI���� Ȱ�� ���� ���� �̺�Ʈ�� ó��
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    /// UI���� ���� �߻� �̺�Ʈ�� ó��
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    /// ���� �߻�� UI�� ������Ʈ
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    /// UI���� ���� ������ �̺�Ʈ�� ó��
    private void ReloadWeaponEvent_OnWeaponReload(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// UI���� ���� ������ �Ϸ� �̺�Ʈ�� ó��
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    /// ���Ⱑ �������Ǿ��� �� UI�� ������Ʈ
    private void WeaponReloaded(Weapon weapon)
    {
        // �������� ���Ⱑ ���� ������ ��쿡�� UI�� ������Ʈ
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    /// UI���� Ȱ�� ���⸦ ����
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // ������ ���Ⱑ ���� ������ ���̶�� ������ �ٸ� ������Ʈ
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    /// Ȱ�� ���� �̹����� ������Ʈ
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    /// Ȱ�� ���� �̸��� ������Ʈ
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    /// UI���� ���� ź�� �ؽ�Ʈ�� ������Ʈ
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    /// UI���� ������ ź�� �������� ������Ʈ
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            // ź�� ������ �������� �ν��Ͻ�ȭ
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    /// ź�� �������� ��� ����
    private void ClearAmmoLoadedIcons()
    {
        // ������ ���ӿ�����Ʈ�� ��� ����
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// ���� ������ �� UI�� ������ �ٸ� ������Ʈ
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// ���� ������ �� �ִϸ��̼��� ó���ϴ� �ڷ�ƾ�Դϴ�.

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        // ������ �ٸ� ���������� ����
        barImage.color = Color.red;

        // ���� ������ �ٸ� �ִϸ���Ʈ
        while (currentWeapon.isWeaponReloading)
        {
            // ������ �ٸ� ������Ʈ
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            // �ٸ� ä��
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    /// UI�� ���� ������ �ٸ� �ʱ�ȭ
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        // �ٸ� ������� ����
        barImage.color = Color.green;

        // �� �������� 1�� ����
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// ���� ������ ���� �� ������Ʈ �ڷ�ƾ�� ����
    private void StopReloadWeaponCoroutine()
    {
        // Ȱ�� ���� ������ �� �ڷ�ƾ�� ����
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    /// ���� ������ �ؽ�Ʈ�� �����̰� ������Ʈ
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            // ������ �ٸ� ���������� ����
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// ���� ������ �ؽ�Ʈ �������� �����ϴ� �ڷ�ƾ
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// ���� ������ �ؽ�Ʈ �������� ����
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    /// ���� ������ �ؽ�Ʈ ������ �ڷ�ƾ�� ����
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
    #endregion Validation
}

