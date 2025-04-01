using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
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

    public int CountInactive => pool.Count;
}