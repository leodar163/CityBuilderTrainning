using System;
using UnityEngine;

namespace Effects
{
    public abstract class ScriptableEffect : ScriptableObject
    {
        public virtual Effect GetEffectCopy()
        {
            throw new NotImplementedException();
        }

        public virtual string GetEffectFormat()
        {
            throw new NotImplementedException();
        }
    }
}