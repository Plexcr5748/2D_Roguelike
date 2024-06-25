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
        // 컴포넌트 로드
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        // 방 변경 이벤트 구독
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // 방 변경 이벤트 구독 해제
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    /// 방 변경 이벤트 처리
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // 방이 현재 방이고, 방이 이미 조명이 켜져 있지 않은 경우 방 조명을 서서히 킴
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            // 방 서서히 켜기
            FadeInRoomLighting();

            // 방 환경 장식 게임 오브젝트 활성화
            instantiatedRoom.ActivateEnvironmentGameObjects();

            // 환경 장식 게임오브젝트 조명 서서히 켜기
            FadeInEnvironmentLighting();

            // 방 문 조명 서서히 켜기
            FadeInDoors();

            instantiatedRoom.room.isLit = true;

        }
    }

    /// 방 조명 서서히 켜기
    private void FadeInRoomLighting()
    {
        // 방 타일맵의 조명 서서히 켜기
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    /// 방 조명 서서히 켜기 코루틴
    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // 서서히 켤 새로운 머티리얼 생성
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

        // 머티리얼을 다시 기본 머티리얼로 설정
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    /// 환경 장식 게임 오브젝트 조명 서서히 켜기
    private void FadeInEnvironmentLighting()
    {
        // 서서히 켤 새로운 머티리얼 생성
        Material material = new Material(GameResources.Instance.variableLitShader);

        // 방 안의 모든 환경 컴포넌트 가져오기
        Environment[] environmentComponents = GetComponentsInChildren<Environment>();

        // 반복문 실행
        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = material;
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environmentComponents));
    }

    /// 환경 장식 게임 오브젝트 조명 서서히 켜기 코루틴
    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environmentComponents)
    {
        // 조명 서서히 켜기
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // 환경 컴포넌트 머티리얼을 기본 머티리얼로 설정
        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
                environmentComponent.spriteRenderer.material = GameResources.Instance.litMaterial;
        }
    }

    /// 문 서서히 켜기
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
