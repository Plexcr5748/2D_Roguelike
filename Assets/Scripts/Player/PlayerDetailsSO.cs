using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("플레이어 캐릭터 이름.")]
    #endregion
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("플레이어를 위한 프리팹 게임 오브젝트")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("플레이어 런타임 애니메이터 컨트롤러")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("플레이어 시작 체력 양")]
    #endregion
    public int playerHealthAmount;
    #region Tooltip
    [Tooltip("피격 후 바로 면역 기간이 있는 경우 선택하십시오. 면역 시간(초)을 다음 필드에 지정하십시오.")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("피격 후 면역 시간(초)")]
    #endregion
    public float hitImmunityTime;

    #region Header WEAPON
    [Space(10)]
    [Header("WEAPON")]
    #endregion
    #region Tooltip
    [Tooltip("플레이어 초기 시작 무기")]
    #endregion
    public WeaponDetailsSO startingWeapon;
    #region Tooltip
    [Tooltip("시작 무기 목록을 포함합니다.")]
    #endregion
    public List<WeaponDetailsSO> startingWeaponList;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion
    #region Tooltip
    [Tooltip("미니맵에서 사용할 플레이어 아이콘 스프라이트")]
    #endregion
    public Sprite playerMiniMapIcon;

    #region Tooltip
    [Tooltip("플레이어 손 스프라이트")]
    #endregion
    public Sprite playerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif
    #endregion

}