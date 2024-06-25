using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header WEAPON SHOOT EFFECT DETAILS

    #region Tooltip
    [Tooltip("Shoot ȿ���� ���� �׶��̼��Դϴ�. �� �׶��̼��� ��ƼŬ�� ���� ���� ������ �����ݴϴ� - ���ʿ��� ����������")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("��ƼŬ �ý����� ���ڸ� �����ϴ� �ð� �����Դϴ�")]
    #endregion Tooltip
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ���� ���� ũ���Դϴ�")]
    #endregion Tooltip
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ���� ���� �ӵ��Դϴ�")]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("��ƼŬ�� �����Դϴ�")]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("����� �ִ� ���� ���Դϴ�")]
    #endregion Tooltip
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("�ʴ� �߻��ϴ� ���� ���Դϴ�. 0�̸� burst ���ڸ� ���˴ϴ�")]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ���� �� ���� ���Դϴ�")]
    #endregion Tooltip
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("������ �߷��Դϴ�. ���� ���� ���� ���ڰ� ���� �������� ����ϴ�")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ��������Ʈ�Դϴ�. �������� ������ �⺻ ��ƼŬ ��������Ʈ�� ���˴ϴ�")]
    #endregion Tooltip
    public Sprite sprite;

    #region Tooltip
    [Tooltip("��ƼŬ�� ���� ������ �ּ� �ӵ��Դϴ�. �ּҰ��� �ִ밪 ���̿��� ���� ���� �����˴ϴ�")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("��ƼŬ�� ���� ������ �ִ� �ӵ��Դϴ�. �ּҰ��� �ִ밪 ���̿��� ���� ���� �����˴ϴ�")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("weaponShootEffectPrefab�� shoot ȿ���� ��ƼŬ �ý����� �����ϰ� ������ weaponShootEffect SO�� ���� �����˴ϴ�")]
    #endregion Tooltip
    public GameObject weaponShootEffectPrefab;


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);
    }

#endif

    #endregion Validation
}
