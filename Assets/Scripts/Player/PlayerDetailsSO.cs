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
    [Tooltip("�÷��̾� ĳ���� �̸�.")]
    #endregion
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("�÷��̾ ���� ������ ���� ������Ʈ")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("�÷��̾� ��Ÿ�� �ִϸ����� ��Ʈ�ѷ�")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("�÷��̾� ���� ü�� ��")]
    #endregion
    public int playerHealthAmount;
    #region Tooltip
    [Tooltip("�ǰ� �� �ٷ� �鿪 �Ⱓ�� �ִ� ��� �����Ͻʽÿ�. �鿪 �ð�(��)�� ���� �ʵ忡 �����Ͻʽÿ�.")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("�ǰ� �� �鿪 �ð�(��)")]
    #endregion
    public float hitImmunityTime;

    #region Header WEAPON
    [Space(10)]
    [Header("WEAPON")]
    #endregion
    #region Tooltip
    [Tooltip("�÷��̾� �ʱ� ���� ����")]
    #endregion
    public WeaponDetailsSO startingWeapon;
    #region Tooltip
    [Tooltip("���� ���� ����� �����մϴ�.")]
    #endregion
    public List<WeaponDetailsSO> startingWeaponList;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion
    #region Tooltip
    [Tooltip("�̴ϸʿ��� ����� �÷��̾� ������ ��������Ʈ")]
    #endregion
    public Sprite playerMiniMapIcon;

    #region Tooltip
    [Tooltip("�÷��̾� �� ��������Ʈ")]
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