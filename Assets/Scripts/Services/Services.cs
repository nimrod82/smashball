using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Smashball
{
    public static class Services
    {
        private static readonly Dictionary<Type, object> registry = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStartPlayMode() => Clear();

        public static void Clear()
        {
            registry.Clear();
        }

        public static void Register<T>(T service)
        {
            if (service == null)
            {
                Debug.LogError($"[Services] Attempt to register null for {typeof(T).Name}");
                return;
            }

            registry[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            if (registry.TryGetValue(typeof(T), out var instance))
                return (T)instance;

            var services = string.Join(", ", registry.Keys.Select(t => t.Name));
            Debug.LogError($"[Services] No service of type {typeof(T).Name} registered. Registered services: {services}");
            return default;
        }
    }
}