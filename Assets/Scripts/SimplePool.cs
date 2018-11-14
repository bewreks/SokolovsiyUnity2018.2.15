using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
    private static Queue<GameObject> _objects;

    static SimplePool()
    {
        _objects = new Queue<GameObject>();
    }

    public static GameObject GetSphere(GameObject prefab)
    {
        if ( _objects.Count == 0 ) {
            return Object.Instantiate(prefab);
        } else {
            var gameObject = _objects.Dequeue();
            gameObject.SetActive(true);
            return gameObject;
        }
    }

    public static void Realize(GameObject gameObject)
    {
        gameObject.SetActive(false);
        _objects.Enqueue(gameObject);
    }
}