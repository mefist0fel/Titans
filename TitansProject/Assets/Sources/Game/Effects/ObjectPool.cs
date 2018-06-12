using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool<T> where T: Component {

    private readonly T prototype;
    private readonly Transform parent;
    private List<T> cache = new List<T>();

    public ObjectPool(T objectPrototype, Transform parentTransform = null) {
        prototype = objectPrototype;
        parent = parentTransform;
    }

    public T Get() {
        foreach (var obj in cache) {
            if (obj == null) {
                continue;
            }
            if (!obj.gameObject.activeSelf) {
                return obj;
            }
        }
        var newObject = GameObject.Instantiate(prototype, parent);
        newObject.gameObject.SetActive(false);
        cache.Add(newObject);
        return newObject;
    }
}
