using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton : MonoBehaviour
{
    private static SceneSingleton instance = null;
    private void Awake()
    {
        instance = this;
    }
    public static T Get<T>() where T: MonoBehaviour
    {
        var onSelf = instance?.GetComponent<T>();
        if(onSelf)
        {
            return onSelf;
        }

        return instance?.GetComponentInChildren<T>();

    }
}
