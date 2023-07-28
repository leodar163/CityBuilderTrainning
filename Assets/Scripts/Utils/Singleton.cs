using System;
using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (!s_instance)
                {
                    T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);

                    switch (instances.Length)
                    {
                        case 0:
                            throw new NullReferenceException(
                                $"No {nameof(T)} instantiated, while it's needed as a Singleton");
                        case 1:
                            s_instance = instances[0];
                            break;
                        default:
                            throw new IndexOutOfRangeException(
                                $"There is multiple instances of {nameof(T)}, " +
                                $"while as it is a Singleton, there's should be only one.");

                    }
                }

                return s_instance;
            }
        }
    }
}