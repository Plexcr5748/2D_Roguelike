using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        // ������Ʈ ��������
        door = GetComponentInParent<Door>();
    }

    /// ���� ������ �巯���� ��
    public void FadeInDoor(Door door)
    {
        // ������ �巯���� �� ���ο� ������ ����
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

    /// ���� ������ �巯���� �ϴ� �ڷ�ƾ
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

    // Ʈ���Ű� �۵��ϸ� ���� ������ �巯���� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}