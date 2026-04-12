using System;
using System.Collections.Generic;
using UnityEngine;

public static class G //ServiseLocator
{
    private static Dictionary<Type, object> _services = new();

    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            Debug.Log($"Serbice {type} already registered");
            return;
        }
        _services[type] = service;
    }

    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
            return service as T;

        throw new Exception($"Serivce {type} not registered");
    }

    public static void Unregister<T>(T service) where T : class
    {
        _services.Remove(typeof(T));
    }

    public static void Clear()
    {
        _services.Clear();
    }
}
