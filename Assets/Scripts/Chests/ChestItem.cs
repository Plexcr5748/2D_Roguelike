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

    /// ���� �������� �ʱ�ȭ
    public void Initialize(Sprite sprite, string text, Vector3 spawnPosition, Color materializeColor)
    {
        // Sprite�� ����
        spriteRenderer.sprite = sprite;
        // ���� ��ġ�� ����
        transform.position = spawnPosition;

        // �������� ����ȭ
        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    /// ���� �������� ����ȭ�մϴ�.
    private IEnumerator MaterializeItem(Color materializeColor, string text)
    {
        // SpriteRenderer �迭�� �����ϰ� ���� SpriteRenderer�� ����
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        // MaterializeEffect�� ����Ͽ� �������� ����ȭ
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, 1f, spriteRendererArray, GameResources.Instance.litMaterial));

        // �������� ����ȭ�Ǿ����� ǥ��
        isItemMaterialized = true;

        // TextMeshPro�� �ؽ�Ʈ�� ����
        textTMP.text = text;
    }
}