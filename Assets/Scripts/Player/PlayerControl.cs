using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]

public class PlayerControl : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO ��ũ���ͺ� ������Ʈ, �̵� �ӵ��� ���� �̵� ���� ������ �����մϴ�.")]

    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float playerRollCooldownTimer = 0f;
    private bool isPlayerMovementDisabled = false;

    [HideInInspector] public bool isPlayerRolling = false;

    private void Awake()
    {
        // ������Ʈ �ε�
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // Fixed Update�� ��ٸ��� �뵵�� WaitForFixedUpdate ����
        waitForFixedUpdate = new WaitForFixedUpdate();

        // ���� ���� ����
        SetStartingWeapon();

        // �÷��̾� �ִϸ��̼� �ӵ� ����
        SetPlayerAnimationSpeed();
    }

    /// �÷��̾� ���� ���� ����
    private void SetStartingWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    /// �÷��̾� �ִϸ��̼� �ӵ��� �̵� �ӵ��� ���� ����
    private void SetPlayerAnimationSpeed()
    {
        // �ִϸ����� �ӵ��� �̵� �ӵ��� ���� ����
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        // �÷��̾� �̵��� ��Ȱ��ȭ�� ��� ��ȯ
        if (isPlayerMovementDisabled)
            return;

        // �÷��̾ ������ ���� ��� ��ȯ
        if (isPlayerRolling) return;

        // �÷��̾� �̵� �Է� ó��
        MovementInput();

        // �÷��̾� ���� �Է� ó��
        WeaponInput();

        // ������ ��� �Է� ó��
        UseItemInput();

        // �÷��̾� ������ ��Ÿ�� Ÿ�̸�
        PlayerRollCooldownTimer();
    }

    /// �÷��̾� �̵� �Է�
    private void MovementInput()
    {
        // �̵� �Է� �ޱ�
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        // �Է��� ������� ���� ���� ����
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // �밢�� �̵� �� �Ÿ� ���� (��Ÿ��� �ٻ�ġ)
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        // �̵��� �ִ� ��� �̵��ϰų� ������
        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                // �̵� �̺�Ʈ ȣ��
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            // �׷��� ������ ������, ��ٿ� ���� �ƴ� ��쿡��
            else if (playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }

        }
        // �׷��� ������ ��� ���� �̺�Ʈ ȣ��
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    /// �÷��̾� ������
    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    /// �÷��̾� ������ �ڷ�ƾ
    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        // �ּ� �Ÿ�, �ڷ�ƾ ���� ���� ����
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            // ���� ������Ʈ ��ٸ���
            yield return waitForFixedUpdate;

        }

        isPlayerRolling = false;

        // ��Ÿ�� Ÿ�̸� ����
        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;

    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    /// ���� �Է�
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        // ���� ���� �Է�
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        // ���� �߻� �Է�
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        // ���� ��ȯ �Է�
        SwitchWeaponInput();

        // ���� ������ �Է�
        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // ���콺 ���� ��ġ ��������
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // ���� �߻� ��ġ���� ���콺 Ŀ�������� ���� ���� ���
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        // �÷��̾� ��ġ���� ���콺 Ŀ�������� ���� ���� ���
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        // ���⿡�� Ŀ�������� ���� ���
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // �÷��̾�� Ŀ�������� ���� ���
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        // �÷��̾� ���� ���� ����
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        // ���� ���� �̺�Ʈ ȣ��
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        // ���� ���콺 ��ư Ŭ�� �� �߻�
        if (Input.GetMouseButton(0))
        {
            // ���� �߻� �̺�Ʈ ȣ��
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
    {
        // ���콺 �� ��ũ�ѷ� ���� ��ȯ
        if (Input.mouseScrollDelta.y < 0f)
        {
            PreviousWeapon();
        }

        if (Input.mouseScrollDelta.y > 0f)
        {
            NextWeapon();
        }

        // ���� Ű�� ���� ��ȯ
        for (int i = 1; i <= 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SetWeaponByIndex(i);
                break;
            }
        }

        // ���̳ʽ� Ű�� ù ��° ����� ����
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetCurrentWeaponToFirstInTheList();
        }
    }


    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;

        if (currentWeaponIndex > player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        // ���� ���Ⱑ ������ ���̸� ��ȯ
        if (currentWeapon.isWeaponReloading) return;

        // �ܿ� ź���� źâ �뷮���� ���� ���� ź���� �ƴϸ� ��ȯ
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

        // źâ�� ź���� ���� á���� ��ȯ
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // ������ �̺�Ʈ ȣ��
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
    }

    /// �÷��̾� �ֺ� 2 ����Ƽ ���� �̳����� ���� ����� ������ ���
    private void UseItemInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float useItemRadius = 2f;

            // �÷��̾� ��ó�� '��� ����' ������ ��������
            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

            // ������ �������� ��ȸ�ϸ� '��� ����' ���� Ȯ��
            foreach (Collider2D collider2D in collider2DArray)
            {
                IUseable iUseable = collider2D.GetComponent<IUseable>();

                if (iUseable != null)
                {
                    iUseable.UseItem();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���𰡿� �浹�ϸ� �÷��̾� ������ �ڷ�ƾ ����
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �浹 ���̸� �÷��̾� ������ �ڷ�ƾ ����
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    /// �÷��̾� �̵� Ȱ��ȭ
    public void EnablePlayer()
    {
        isPlayerMovementDisabled = false;
    }

    /// �÷��̾� �̵� ��Ȱ��ȭ
    public void DisablePlayer()
    {
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }

    /// �÷��̾� ���� ��Ͽ��� ���� ���⸦ ù ��°�� ����
    private void SetCurrentWeaponToFirstInTheList()
    {
        // ���ο� �ӽ� ��� ����
        List<Weapon> tempWeaponList = new List<Weapon>();

        // ���� ���⸦ �ӽ� ����� ù ��°�� �߰�
        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        // ���� ���� ����� ��ȸ�ϸ� ���� ���⸦ �����ϰ� �߰�
        int index = 2;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        // �� ��� �Ҵ�
        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;

        // ���� ���� ����
        SetWeaponByIndex(currentWeaponIndex);
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif

    #endregion Validation
}
