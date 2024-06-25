using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject References
    [Space(10)]
    [Header("GameObject References")]
    #endregion
    #region Tooltip
    [Tooltip("자식 Bar 게임오브젝트로 채우기")]
    #endregion
    [SerializeField] private GameObject healthBar;

    /// 체력바 활성화
    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    /// 체력바 비활성화
    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// 체력바 값 설정 (0과 1 사이의 체력 퍼센트로)
    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}