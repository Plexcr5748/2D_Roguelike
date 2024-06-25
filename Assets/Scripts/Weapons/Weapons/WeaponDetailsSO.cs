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
    [Tooltip("무기 스프라이트 - 스프라이트에 'generate physics shape' 옵션이 선택되어 있어야 함")]
    #endregion Tooltip
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    #endregion Header WEAPON CONFIGURATION
    #region Tooltip
    [Tooltip("무기 발사 위치 - 스프라이트 피벗으로부터 무기 끝의 오프셋 위치")]
    #endregion Tooltip
    public Vector3 weaponShootPosition;
    #region Tooltip
    [Tooltip("현재 무기의 탄약 정보")]
    #endregion Tooltip
    public AmmoDetailsSO weaponCurrentAmmo;
    #region Tooltip
    [Tooltip("무기 발사 효과 SO - 무기 발사 효과 프리팹과 함께 사용할 입자 효과 매개변수를 포함합니다")]
    #endregion Tooltip
    public WeaponShootEffectSO weaponShootEffect;
    #region Tooltip
    [Tooltip("무기 발사 사운드 효과 SO")]
    #endregion Tooltip
    public SoundEffectSO weaponFiringSoundEffect;
    #region Tooltip
    [Tooltip("무기 재장전 사운드 효과 SO")]
    #endregion Tooltip
    public SoundEffectSO weaponReloadingSoundEffect;
    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("무기 작동 값")]
    #endregion Header WEAPON OPERATING VALUES
    #region Tooltip
    [Tooltip("무기가 무한 탄약을 가지고 있는지 선택합니다")]
    #endregion Tooltip
    public bool hasInfiniteAmmo = false;
    #region Tooltip
    [Tooltip("무기가 무한 탄창 용량을 가지고 있는지 선택합니다")]
    #endregion Tooltip
    public bool hasInfiniteClipCapacity = false;
    #region Tooltip
    [Tooltip("무기 탄창 용량 - 재장전 전에 발사할 수 있는 최대 총알 수")]
    #endregion Tooltip
    public int weaponClipAmmoCapacity = 6;
    #region Tooltip
    [Tooltip("무기 탄약 용량 - 이 무기가 보유할 수 있는 최대 총알 수")]
    #endregion Tooltip
    public int weaponAmmoCapacity = 100;
    #region Tooltip
    [Tooltip("무기 발사 속도 - 0.2는 초당 5발의 발사를 의미합니다")]
    #endregion Tooltip
    public float weaponFireRate = 0.2f;
    #region Tooltip
    [Tooltip("무기 선사 시간 - 발사 전에 버튼을 길게 누르는 시간(초)")]
    #endregion Tooltip
    public float weaponPrechargeTime = 0f;
    #region Tooltip
    [Tooltip("무기 재장전 시간(초)")]
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
