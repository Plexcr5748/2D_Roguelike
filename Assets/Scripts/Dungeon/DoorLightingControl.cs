using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        // 컴포넌트 가져오기
        door = GetComponentInParent<Door>();
    }

    /// 문을 서서히 드러나게 함
    public void FadeInDoor(Door door)
    {
        // 서서히 드러나게 할 새로운 재질을 생성
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }

            isLit = true;
        }
    }

    /// 문을 서서히 드러나게 하는 코루틴
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    // 트리거가 작동하면 문을 서서히 드러나게 함
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}