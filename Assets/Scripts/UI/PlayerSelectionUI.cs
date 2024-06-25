using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Child ���ӿ�����Ʈ WeaponAnchorPosition/WeaponRotationPoint/Hand�� �ִ� Sprite Renderer�� ä��ϴ�.")]
    #endregion
    public SpriteRenderer playerHandSpriteRenderer;
    #region Tooltip
    [Tooltip("Child ���ӿ�����Ʈ HandNoWeapon�� �ִ� Sprite Renderer�� ä��ϴ�.")]
    #endregion
    public SpriteRenderer playerHandNoWeaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Child ���ӿ�����Ʈ WeaponAnchorPosition/WeaponRotationPoint/Weapon�� �ִ� Sprite Renderer�� ä��ϴ�.")]
    #endregion
    public SpriteRenderer playerWeaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Animator ������Ʈ�� ä��ϴ�.")]
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