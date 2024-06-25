using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Child 게임오브젝트 WeaponAnchorPosition/WeaponRotationPoint/Hand에 있는 Sprite Renderer로 채웁니다.")]
    #endregion
    public SpriteRenderer playerHandSpriteRenderer;
    #region Tooltip
    [Tooltip("Child 게임오브젝트 HandNoWeapon에 있는 Sprite Renderer로 채웁니다.")]
    #endregion
    public SpriteRenderer playerHandNoWeaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Child 게임오브젝트 WeaponAnchorPosition/WeaponRotationPoint/Weapon에 있는 Sprite Renderer로 채웁니다.")]
    #endregion
    public SpriteRenderer playerWeaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Animator 컴포넌트로 채웁니다.")]
    #endregion
    public Animator animator;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(animator), animator);
    }
#endif
    #endregion
}