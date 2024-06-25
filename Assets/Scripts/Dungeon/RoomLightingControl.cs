using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        // ������Ʈ �ε�
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        // �� ���� �̺�Ʈ ����
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // �� ���� �̺�Ʈ ���� ����
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    /// �� ���� �̺�Ʈ ó��
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // ���� ���� ���̰�, ���� �̹� ������ ���� ���� ���� ��� �� ������ ������ Ŵ
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            // �� ������ �ѱ�
            FadeInRoomLighting();

            // �� ȯ�� ��� ���� ������Ʈ Ȱ��ȭ
            instantiatedRoom.ActivateEnvironmentGameObjects();

            // ȯ�� ��� ���ӿ�����Ʈ ���� ������ �ѱ�
            FadeInEnvironmentLighting();

            // �� �� ���� ������ �ѱ�
            FadeInDoors();

            instantiatedRoom.room.isLit = true;

        }
    }

    /// �� ���� ������ �ѱ�
    private void FadeInRoomLighting()
    {
        // �� Ÿ�ϸ��� ���� ������ �ѱ�
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    /// �� ���� ������ �ѱ� �ڷ�ƾ
    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // ������ �� ���ο� ��Ƽ���� ����
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // ��Ƽ������ �ٽ� �⺻ ��Ƽ����� ����
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    /// ȯ�� ��� ���� ������Ʈ ���� ������ �ѱ�
    private void FadeInEnvironmentLighting()
    {
        // ������ �� ���ο� ��Ƽ���� ����
        Material material = new Material(GameResources.Instance.variableLitShader);

        // �� ���� ��� ȯ�� ������Ʈ ��������
        Environment[] environmentComponents = GetComponentsInChildren<Environment>();

        // �ݺ��� ����
        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = material;
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environmentComponents));
    }

    /// ȯ�� ��� ���� ������Ʈ ���� ������ �ѱ� �ڷ�ƾ
    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environmentComponents)
    {
        // ���� ������ �ѱ�
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // ȯ�� ������Ʈ ��Ƽ������ �⺻ ��Ƽ����� ����
        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = GameResources.Instance.litMaterial;
        }
    }

    /// �� ������ �ѱ�
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

            doorLightingControl.FadeInDoor(door);
        }
    }
}
