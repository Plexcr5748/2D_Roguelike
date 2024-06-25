using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        // ������Ʈ �ε�
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        playerControl = GetComponent<PlayerControl>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// �÷��̾� �ʱ�ȭ
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        // �÷��̾� ���� ���� ����
        CreatePlayerStartingWeapons();

        // �÷��̾� ���� ü�� ����
        SetPlayerHealth();
    }

    private void OnEnable()
    {
        // �÷��̾� ü�� �̺�Ʈ ����
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        // �÷��̾� ü�� �̺�Ʈ ���� ���
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    /// ü�� ���� �̺�Ʈ ó��
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        // �÷��̾ ����� ���
        if (healthEventArgs.healthAmount <= 0f)
        {
            destroyedEvent.CallDestroyedEvent(true, 0);
        }
    }

    /// �÷��̾� ���� ���� ����
    private void CreatePlayerStartingWeapons()
    {
        // ����Ʈ �ʱ�ȭ
        weaponList.Clear();

        // ���� ���� ����Ʈ���� ���� �߰�
        foreach (WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList)
        {
            // �÷��̾ ���� �߰�
            AddWeaponToPlayer(weaponDetails);
        }
    }

    /// �÷��̾� ü�� ����
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    /// �÷��̾� ��ġ ��ȯ
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    /// �÷��̾ ���� �߰�
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };

        // ����Ʈ�� ���� �߰�
        weaponList.Add(weapon);

        // ����Ʈ���� ���� ��ġ ����
        weapon.weaponListPosition = weaponList.Count;

        // �߰��� ���⸦ Ȱ��ȭ ���·� ����
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }

    /// �÷��̾ ���⸦ ���� ������ Ȯ��
    public bool IsWeaponHeldByPlayer(WeaponDetailsSO weaponDetails)
    {
        foreach (Weapon weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetails) return true;
        }

        return false;
    }
}