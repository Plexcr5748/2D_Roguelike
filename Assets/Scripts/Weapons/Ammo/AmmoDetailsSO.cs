using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("�Ѿ��� �̸�")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("�Ѿ˿� ����� ��������Ʈ")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("�Ѿ˿� ����� �������� ä�켼��. �迭�� ���� �������� ������ ��� �迭���� �������� ���õ˴ϴ�. �������� IFireable �������̽��� �ؼ��ؾ� �մϴ�.")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("�Ѿ˿� ����� ���׸���")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("�Ѿ��� �߻�� �� �����̱� ���� ��� '����'�ؾ� �ϴ� ���, �߻� �� �����Ǵ� �ð�(��)�� �����ϼ���.")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("�Ѿ��� �����Ǵ� ���� ����� ���׸���")]
    #endregion
    public Material ammoChargeMaterial;

    #region Header AMMO HIT EFFECT
    [Space(10)]
    [Header("AMMO HIT EFFECT")]
    #endregion
    #region Tooltip
    [Tooltip("�Ѿ� ��Ʈ ȿ�� �����տ� ���� �Ű������� �����ϴ� ��ũ���ͺ� ������Ʈ")]
    #endregion
    public AmmoHitEffectSO ammoHitEffect;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    #endregion
    #region Tooltip
    [Tooltip("�� �Ѿ��� ������ ������")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("�Ѿ��� �ּ� �ӵ� - �ӵ��� �ּ� �� �ִ� �� ������ ���� ���� �˴ϴ�.")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("�Ѿ��� �ִ� �ӵ� - �ӵ��� �ּ� �� �ִ� �� ������ ���� ���� �˴ϴ�.")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("�Ѿ��� ���� �Ÿ�(����Ƽ ���� ����)")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("�Ѿ� ������ ȸ�� �ӵ�(�ʴ� ����)")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("�Ѿ��� �ּ� ���� �����Դϴ�. ���� ������ ��Ȯ���� �����ϴ�. �ּ� �� �ִ� �� ������ ���� ������ ���˴ϴ�.")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip("�Ѿ��� �ִ� ���� �����Դϴ�. ���� ������ ��Ȯ���� �����ϴ�. �ּ� �� �ִ� �� ������ ���� ������ ���˴ϴ�.")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("�� ���� �߻�Ǵ� �ּ� �Ѿ� ���Դϴ�. �ּ� �� �ִ� �� ������ ���� �Ѿ� ���� �߻�˴ϴ�.")]
    #endregion
    public int ammoSpawnAmountMin = 1;
    #region Tooltip
    [Tooltip("�� ���� �߻�Ǵ� �ִ� �Ѿ� ���Դϴ�. �ּ� �� �ִ� �� ������ ���� �Ѿ� ���� �߻�˴ϴ�.")]
    #endregion
    public int ammoSpawnAmountMax = 1;
    #region Tooltip
    [Tooltip("�ּ� �߻� ���� �ð��Դϴ�. �ּ� �� �ִ� �� ������ ���� �ð� ����(��)�� �����˴ϴ�.")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("�ִ� �߻� ���� �ð��Դϴ�. �ּ� �� �ִ� �� ������ ���� �ð� ����(��)�� �����˴ϴ�.")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;


    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("�Ѿ˿� �Ѿ� ������ �ʿ��� ��� �����ϼ���. ������ ��� ���� �Ѿ� ���� ���� ä���� �־�� �մϴ�.")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("�Ѿ� ������ ���� �ð�(��)")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("�Ѿ� ������ ���׸���")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("�Ѿ� ������ ���� �ʺ�")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("�Ѿ� ������ �� �ʺ�")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    // �Էµ� ��ũ���ͺ� ������Ʈ ���� ���� ��ȿ�� �˻�
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }

#endif
    #endregion
}
