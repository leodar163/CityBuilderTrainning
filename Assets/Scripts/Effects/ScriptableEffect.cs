using System;
using UnityEngine;

namespace Effects
{
    public abstract class ScriptableEffect<T> : ScriptableObject where T : Effect
    {
        [SerializeField] protected T _effect;

        public virtual T GetEffectCopy()
        {
            throw new NotImplementedException();
        }
    }
}