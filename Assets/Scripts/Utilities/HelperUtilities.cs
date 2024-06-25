using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    /// ���콺�� ���� ��ǥ�� ������
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // ���콺 ��ġ�� ȭ�� ũ�⿡ ���� �����մϴ�.
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    /// ī�޶� ����Ʈ�� �ϴ� ��ǥ�� ��� ��ǥ�� ������
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    /// ���� ���ͷκ��� ������ ���� ����
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    /// �����κ��� ���� ���͸� ����
    /// <returns></returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// �־��� ������ ���� AimDirection ������ ���� ������
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        // �÷��̾� ���� ����
        // ��-������
        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        // ��
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        // ��-����
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        // ����
        else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180 && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        // �Ʒ�
        else if ((angleDegrees > -135f && angleDegrees <= -45f))
        {
            aimDirection = AimDirection.Down;
        }
        // ������
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    /// ���� ���� �������� ���ú��� ��ȯ
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;

        // ���� �������� �α� ������ ���ú��� ��ȯ�ϴ� ����
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    /// �� ���ڿ� ����� üũ
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "��(��) ��� �־�� �ϸ� ��ü " + thisObject.name.ToString() + "�� ���� �����ؾ� �մϴ�.");
            return true;
        }
        return false;
    }

    /// null �� ����� üũ
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + "��(��) null�̾ ��ü " + thisObject.name.ToString() + "�� ���� �����ؾ� �մϴ�.");
            return true;
        }
        return false;
    }

    /// ����Ʈ�� ��� �ְų� null ���� �����ϴ��� üũ ������ ������ true�� ��ȯ
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "��(��) null�Դϴ�. ��ü " + thisObject.name.ToString() + "���� ���� �����ؾ� �մϴ�.");
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� null ���� �����մϴ�.");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� ���� �����ϴ�.");
            error = true;
        }

        return error;
    }

    /// ��� �� ����� üũ - zeroAllowed�� true�̸� 0�� ��� ������ ������ true�� ��ȯ
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� ��� �� �Ǵ� 0�� �����ؾ� �մϴ�.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� ��� ���̾�� �մϴ�.");
                error = true;
            }
        }

        return error;
    }

    /// ��� �� ����� üũ - zeroAllowed�� true�̸� 0�� ��� ������ ������ true�� ��ȯ
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� ��� �� �Ǵ� 0�� �����ؾ� �մϴ�.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + "��(��) ��ü " + thisObject.name.ToString() + "���� ��� ���̾�� �մϴ�.");
                error = true;
            }
        }

        return error;
    }

    /// ��� ���� ����� üũ - min�� max ���� ���� ��� 0�� �� ������ isZeroAllowed�� true�� ���� ������ ������ true�� ��ȯ
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + "��(��) " + fieldNameMaximum + "���� �۰ų� ���ƾ� �մϴ�. ��ü " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// ��� ���� ����� üũ - min�� max ���� ���� ��� 0�� �� ������ isZeroAllowed�� true�� ���� ������ ������ true�� ��ȯ
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, int valueToCheckMinimum, string fieldNameMaximum, int valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + "��(��) " + fieldNameMaximum + "���� �۰ų� ���ƾ� �մϴ�. ��ü " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// �÷��̾�� ���� ����� ���� ��ġ�� ����
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        // ���� ���� ��ġ�� �ݺ�
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            // ���� �׸��� ��ġ�� ���� ��ġ�� ��ȯ
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
}
