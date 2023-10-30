using UnityEngine;
using Utils;

namespace ResourceSystem.Markets.Needs
{
    [CreateAssetMenu(menuName = "Variables/Needs Set", fileName = "NewNeedsSet")]
    public class ScriptableNeedsSet : DefaultableScriptableObject<ScriptableNeedsSet>
    {
        public NeedsSet needsSet;
    }
}