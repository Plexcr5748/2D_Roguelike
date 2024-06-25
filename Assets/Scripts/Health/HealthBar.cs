using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject References
    [Space(10)]
    [Header("GameObject References")]
    #endregion
    #region Tooltip
    [Tooltip("�ڽ� Bar ���ӿ�����Ʈ�� ä���")]
    #endregion
    [SerializeField] private GameObject healthBar;

    /// ü�¹� Ȱ��ȭ
    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    /// ü�¹� ��Ȱ��ȭ
    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// ü�¹� �� ���� (0�� 1 ������ ü�� �ۼ�Ʈ��)
    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}