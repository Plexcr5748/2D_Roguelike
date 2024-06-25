using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    #region Header AMMO HIT EFFECT DETAILS
    [Space(10)]
    [Header("AMMO HIT EFFECT DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("��Ʈ ����Ʈ�� ���� �׷����Ʈ�Դϴ�. �� �׷����Ʈ�� ��ƼŬ�� ���� ������ ������ �����ݴϴ� - ���ʿ��� ����������")]
    #endregion
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("��ƼŬ �ý����� ���ڸ� �����ϴ� �ð�")]
    #endregion
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ���� ���� ũ��")]
    #endregion
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ���� ���� �ӵ�")]
    #endregion
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("��ƼŬ�� ����")]
    #endregion
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("����� �ִ� ���� ��")]
    #endregion
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("�ʴ� ����Ǵ� ���� ���Դϴ�. 0�̸� ����Ʈ ��ȣ�� ���˴ϴ�.")]
    #endregion
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ�� ����Ʈ�� ����� ���� ��")]
    #endregion
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("���ڿ� �ۿ��ϴ� �߷� - ���� ���� ���� ���ڰ� ���� �������� �մϴ�.")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("��ƼŬ ȿ���� ��������Ʈ�Դϴ�. �������� ���� ��� �⺻ ���� ��������Ʈ�� ���˴ϴ�.")]
    #endregion
    public Sprite sprite;

    #region Tooltip
    [Tooltip("��ƼŬ�� ���� ������ �ּ� �ӵ� �����Դϴ�. �ּҰ��� �ִ밪 ������ ���� ���� �����˴ϴ�.")]
    #endregion
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("��ƼŬ�� ���� ������ �ִ� �ӵ� �����Դϴ�. �ּҰ��� �ִ밪 ������ ���� ���� �����˴ϴ�.")]
    #endregion
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("��Ʈ ����Ʈ ��ƼŬ �ý����� �����ϴ� �������Դϴ� - �ش��ϴ� ammoHitEffectSO�� ���ǵǾ�� �մϴ�.")]
    #endregion
    public GameObject ammoHitEffectPrefab;

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
    }
#endif
}