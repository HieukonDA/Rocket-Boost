using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : class
{
    private readonly Func<T> factoryMethod;
    private readonly Queue<T> pool = new Queue<T>();
    private readonly HashSet<T> allObjects = new HashSet<T>();
    private readonly Transform parent;

    public ObjectPool(Func<T> factory, int initialSize, Transform parent = null)
    {
        this.factoryMethod = factory;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            if (obj is GameObject go) go.SetActive(false);
            pool.Enqueue(obj);
            allObjects.Add(obj);
        }
    }

    private T CreateNewObject()
    {
        T obj = factoryMethod.Invoke();
        if (obj is GameObject go && parent != null) go.transform.parent = parent;
        allObjects.Add(obj);
        return obj;
    }

    public T Get()
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
        if (obj is GameObject go) go.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        if (obj is GameObject go) go.SetActive(false);
        pool.Enqueue(obj);
    }

    public bool Contains(T obj)
    {
        return allObjects.Contains(obj); // Kiểm tra xem obj có trong tất cả object không
    }

    public IEnumerable<T> GetActiveObjects()
    {
        List<T> activeObjects = new List<T>();
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf && child.CompareTag("Coin"))
            {
                activeObjects.Add(child.gameObject as T);
            }
        }
        return activeObjects;
    }

    public int CountInactive => pool.Count;
}