using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("�� �⺻ ����")]
    #endregion

    #region Tooltip
    [Tooltip("���� �̸�")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("���� ������")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [Tooltip("���� �÷��̾ �ѱ� �����ϴ� �Ÿ�")]
    #endregion
    public float chaseDistance = 50f;

    #region Header ENEMY MATERIAL
    [Space(10)]
    [Header("���� ����")]
    #endregion
    #region Tooltip
    [Tooltip("���� ����ȭ �� ����ϴ� ǥ�� ����Ʈ ���̴� ����")]
    #endregion
    public Material enemyStandardMaterial;

    #region Header ENEMY MATERIALIZE SETTINGS
    [Space(10)]
    [Header("�� ����ȭ ����")]
    #endregion
    #region Tooltip
    [Tooltip("���� ����ȭ�ϴ� �� �ɸ��� �ð�(��)")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("���� ����ȭ�� �� ���Ǵ� ���̴�")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("���� ����ȭ�� �� ����� ����. HDR �����̹Ƿ� ������ �����Ͽ� ���̳� ��¦���� ǥ���� �� �ֽ��ϴ�.")]
    #endregion
    public Color enemyMaterializeColor;

    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("�� ���� ����")]
    #endregion
    #region Tooltip
    [Tooltip("���� ���� - ���Ⱑ ������ ����")]
    #endregion
    public WeaponDetailsSO enemyWeapon;
    #region Tooltip
    [Tooltip("���� �߻��ϴ� ���� ������ �ּ� �ð�(��). �� ���� 0���� Ŀ�� �մϴ�. �ּ� ���� �ִ� �� ������ ������ ���� ���õ˴ϴ�.")]
    #endregion
    public float firingIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("���� �߻��ϴ� ���� ������ �ִ� �ð�(��). �ּ� ���� �ִ� �� ������ ������ ���� ���õ˴ϴ�.")]
    #endregion
    public float firingIntervalMax = 1f;
    #region Tooltip
    [Tooltip("���� �߻��ϴ� ���� ���� �ð��� �ּ� ��(��). �� ���� 0���� Ŀ�� �մϴ�. �ּ� ���� �ִ� �� ������ ������ ���� ���õ˴ϴ�.")]
    #endregion
    public float firingDurationMin = 1f;
    #region Tooltip
    [Tooltip("���� �߻��ϴ� ���� ���� �ð��� �ִ� ��(��). �ּ� ���� �ִ� �� ������ ������ ���� ���õ˴ϴ�.")]
    #endregion
    public float firingDurationMax = 2f;
    #region Tooltip
    [Tooltip("�÷��̾��� �þ߰� �ʿ����� ���θ� �����Ͽ� ���� �߻��ϱ� ���� �÷��̾��� �þ߸� Ȯ���մϴ�. �þ߰� �ʿ����� ������ ���� ��ֹ��� �����ϰ� �÷��̾ '���� ��'�� ������ �߻��մϴ�.")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY HEALTH
    [Space(10)]
    [Header("�� ü��")]
    #endregion
    #region Tooltip
    [Tooltip("�� ������ ���� ���� ü��")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;
    #region Tooltip
    [Tooltip("���� ���� �� �ٷ� �鿪 �Ⱓ�� �ִ��� ���θ� �����մϴ�. �鿪 �ð�(��)�� ���� �ʵ忡 �����մϴ�.")]
    #endregion
    public bool isImmuneAfterHit = false;
    #region Tooltip
    [Tooltip("���� ���� �� �鿪�Ǵ� �ð�(��)")]
    #endregion
    public float hitImmunityTime;
    #region Tooltip
    [Tooltip("���� ü�� �ٸ� ǥ������ ���θ� �����մϴ�.")]
    #endregion
    public bool isHealthBarDisplayed = false;



    #region Validation
#if UNITY_EDITOR
    // ��ũ���ͺ� ������Ʈ�� �Էµ� ���� ���� ��ȿ�� �˻�
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }

#endif
    #endregion

}