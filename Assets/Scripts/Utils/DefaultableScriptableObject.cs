using System;
using UnityEngine;

namespace Utils
{
    public class DefaultableScriptableObject<T> : ScriptableObject 
        where T : ScriptableObject
    {
        private static T s_default;
        
        public static T Default
        {
            get
            {
                if (s_default == null) s_default = Resources.Load($"{typeof(T).Name}Default", typeof(T)) as T;
                if (s_default == null) throw new NullReferenceException($"No {typeof(T)} with name '{typeof(T).Name}Default'" +
                                                                        " found in a 'Resources' folder");
                return s_default;
            }
        }
    }
}