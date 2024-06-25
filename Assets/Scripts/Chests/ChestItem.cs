using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textTMP;
    private MaterializeEffect materializeEffect;
    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textTMP = GetComponentInChildren<TextMeshPro>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    /// 상자 아이템을 초기화
    public void Initialize(Sprite sprite, string text, Vector3 spawnPosition, Color materializeColor)
    {
        // Sprite를 설정
        spriteRenderer.sprite = sprite;
        // 생성 위치를 설정
        transform.position = spawnPosition;

        // 아이템을 소재화
        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    /// 상자 아이템을 소재화합니다.
    private IEnumerator MaterializeItem(Color materializeColor, string text)
    {
        // SpriteRenderer 배열을 생성하고 현재 SpriteRenderer를 포함
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        // MaterializeEffect를 사용하여 아이템을 소재화
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, 1f, spriteRendererArray, GameResources.Instance.litMaterial));

        // 아이템이 소재화되었음을 표시
        isItemMaterialized = true;

        // TextMeshPro에 텍스트를 설정
        textTMP.text = text;
    }
}