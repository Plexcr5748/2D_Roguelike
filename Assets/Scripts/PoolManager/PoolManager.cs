using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region Tooltip
    [Tooltip("������ ���� �����Ϸ��� ���� ������Ʈ�� ���� �����ϰ� �� �迭�� �������� ä��ϴ�.")]
    #endregion
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        // �̱��� ���� ������Ʈ�� ��ü Ǯ�� �θ� ��
        objectPoolTransform = this.gameObject.transform;

        // ������ �� ��ü Ǯ ����
        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    /// ������ �����հ� ������ ������ Ǯ ũ��� ��ü Ǯ�� �����մϴ�.
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name; // ������ �̸� ��������

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // �ڽ� ��ü�� �θ�� ����� ���� ���� ������Ʈ ����

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;

                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    /// ��ü Ǯ���� ������Ʈ�� ���� 'prefab'�� ������Ʈ�� ������ ������ ���� ������Ʈ. 'position'�� Ȱ��ȭ�� �� ��Ÿ�� ���� ������Ʈ�� ���� ��ġ. 'rotation'�� ���� ������Ʈ�� ȸ���ؾ� �� ��� ����
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            // Ǯ ť���� ��ü ��������
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }
        else
        {
            Debug.Log("�����տ� ���� ��ü Ǯ�� �����ϴ�: " + prefab);
            return null;
        }
    }

    /// 'poolKey'�� ����Ͽ� ��ü Ǯ���� ���� ������Ʈ�� ������Ʈ�� ������.
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf)
        {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    /// ���� ������Ʈ�� �ʱ�ȭ
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}
