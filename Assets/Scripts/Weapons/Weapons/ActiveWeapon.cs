using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    #region Tooltip
    [Tooltip("�ڽ� ���� ���ӿ�����Ʈ�� �ִ� SpriteRenderer�� ä��ϴ�.")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    #region Tooltip
    [Tooltip("�ڽ� ���� ���ӿ�����Ʈ�� �ִ� PolygonCollider2D�� ä��ϴ�.")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;

    #region Tooltip
    [Tooltip("WeaponShootPosition ���ӿ�����Ʈ�� �ִ� Transform���� ä��ϴ�.")]
    #endregion
    [SerializeField] private Transform weaponShootPositionTransform;

    #region Tooltip
    [Tooltip("WeaponEffectPosition ���ӿ�����Ʈ�� �ִ� Transform���� ä��ϴ�.")]
    #endregion
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        // ������Ʈ �ε�
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        // ���� ������ ��������Ʈ ����
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        // ���⿡ ������ �ݶ��̴��� ��������Ʈ�� ������ ���� ����� ����
        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            // ��������Ʈ�� ���� ��� �������� - �̴� Vector2 ����Ʈ�� ��������Ʈ�� ���� ��� ������ ��ȯ
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            // ������ ������ �ݶ��̴��� ��������Ʈ�� ���� ��� ����
            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }

        // ���� �߻� ��ġ ����
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion
}
