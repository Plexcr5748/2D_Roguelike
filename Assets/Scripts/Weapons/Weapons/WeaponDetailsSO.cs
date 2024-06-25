using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BASE DETAILS
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    #endregion Header WEAPON BASE DETAILS
    #region Tooltip
    [Tooltip("WEAPON BASE DETAILS")]
    #endregion Tooltip
    public string weaponName;
    #region Tooltip
    [Tooltip("���� ��������Ʈ - ��������Ʈ�� 'generate physics shape' �ɼ��� ���õǾ� �־�� ��")]
    #endregion Tooltip
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    #endregion Header WEAPON CONFIGURATION
    #region Tooltip
    [Tooltip("���� �߻� ��ġ - ��������Ʈ �ǹ����κ��� ���� ���� ������ ��ġ")]
    #endregion Tooltip
    public Vector3 weaponShootPosition;
    #region Tooltip
    [Tooltip("���� ������ ź�� ����")]
    #endregion Tooltip
    public AmmoDetailsSO weaponCurrentAmmo;
    #region Tooltip
    [Tooltip("���� �߻� ȿ�� SO - ���� �߻� ȿ�� �����հ� �Բ� ����� ���� ȿ�� �Ű������� �����մϴ�")]
    #endregion Tooltip
    public WeaponShootEffectSO weaponShootEffect;
    #region Tooltip
    [Tooltip("���� �߻� ���� ȿ�� SO")]
    #endregion Tooltip
    public SoundEffectSO weaponFiringSoundEffect;
    #region Tooltip
    [Tooltip("���� ������ ���� ȿ�� SO")]
    #endregion Tooltip
    public SoundEffectSO weaponReloadingSoundEffect;
    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("���� �۵� ��")]
    #endregion Header WEAPON OPERATING VALUES
    #region Tooltip
    [Tooltip("���Ⱑ ���� ź���� ������ �ִ��� �����մϴ�")]
    #endregion Tooltip
    public bool hasInfiniteAmmo = false;
    #region Tooltip
    [Tooltip("���Ⱑ ���� źâ �뷮�� ������ �ִ��� �����մϴ�")]
    #endregion Tooltip
    public bool hasInfiniteClipCapacity = false;
    #region Tooltip
    [Tooltip("���� źâ �뷮 - ������ ���� �߻��� �� �ִ� �ִ� �Ѿ� ��")]
    #endregion Tooltip
    public int weaponClipAmmoCapacity = 6;
    #region Tooltip
    [Tooltip("���� ź�� �뷮 - �� ���Ⱑ ������ �� �ִ� �ִ� �Ѿ� ��")]
    #endregion Tooltip
    public int weaponAmmoCapacity = 100;
    #region Tooltip
    [Tooltip("���� �߻� �ӵ� - 0.2�� �ʴ� 5���� �߻縦 �ǹ��մϴ�")]
    #endregion Tooltip
    public float weaponFireRate = 0.2f;
    #region Tooltip
    [Tooltip("���� ���� �ð� - �߻� ���� ��ư�� ��� ������ �ð�(��)")]
    #endregion Tooltip
    public float weaponPrechargeTime = 0f;
    #region Tooltip
    [Tooltip("���� ������ �ð�(��)")]
    #endregion Tooltip
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
    #endregion Validation
}
