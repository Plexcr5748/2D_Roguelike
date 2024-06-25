using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    /// 마우스의 월드 좌표를 가져옴
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // 마우스 위치를 화면 크기에 맞춰 제한합니다.
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    /// 카메라 뷰포트의 하단 좌표와 상단 좌표를 가져옴
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    /// 방향 벡터로부터 각도를 돌려 받음
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    /// 각도로부터 방향 벡터를 얻음
    /// <returns></returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// 주어진 각도에 따른 AimDirection 열거형 값을 가져옴
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        // 플레이어 방향 설정
        // 위-오른쪽
        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        // 위
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        // 위-왼쪽
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        // 왼쪽
        else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180 && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        // 아래
        else if ((angleDegrees > -135f && angleDegrees <= -45f))
        {
            aimDirection = AimDirection.Down;
        }
        // 오른쪽
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

    /// 선형 볼륨 스케일을 데시벨로 변환
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;

        // 선형 스케일을 로그 스케일 데시벨로 변환하는 공식
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    /// 빈 문자열 디버그 체크
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "이(가) 비어 있어야 하며 객체 " + thisObject.name.ToString() + "에 값을 포함해야 합니다.");
            return true;
        }
        return false;
    }

    /// null 값 디버그 체크
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + "이(가) null이어서 객체 " + thisObject.name.ToString() + "에 값을 포함해야 합니다.");
            return true;
        }
        return false;
    }

    /// 리스트가 비어 있거나 null 값을 포함하는지 체크 오류가 있으면 true를 반환
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "이(가) null입니다. 객체 " + thisObject.name.ToString() + "에서 값을 포함해야 합니다.");
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 null 값을 포함합니다.");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 값이 없습니다.");
            error = true;
        }

        return error;
    }

    /// 양수 값 디버그 체크 - zeroAllowed가 true이면 0을 허용 오류가 있으면 true를 반환
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 양수 값 또는 0을 포함해야 합니다.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 양수 값이어야 합니다.");
                error = true;
            }
        }

        return error;
    }

    /// 양수 값 디버그 체크 - zeroAllowed가 true이면 0을 허용 오류가 있으면 true를 반환
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 양수 값 또는 0을 포함해야 합니다.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + "이(가) 객체 " + thisObject.name.ToString() + "에서 양수 값이어야 합니다.");
                error = true;
            }
        }

        return error;
    }

    /// 양수 범위 디버그 체크 - min과 max 범위 값이 모두 0일 수 있으면 isZeroAllowed를 true로 설정 오류가 있으면 true를 반환
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + "이(가) " + fieldNameMaximum + "보다 작거나 같아야 합니다. 객체 " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// 양수 범위 디버그 체크 - min과 max 범위 값이 모두 0일 수 있으면 isZeroAllowed를 true로 설정 오류가 있으면 true를 반환
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, int valueToCheckMinimum, string fieldNameMaximum, int valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + "이(가) " + fieldNameMaximum + "보다 작거나 같아야 합니다. 객체 " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// 플레이어에게 가장 가까운 스폰 위치를 얻음
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        // 방의 스폰 위치를 반복
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            // 스폰 그리드 위치를 월드 위치로 변환
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
}
