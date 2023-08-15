using UnityEngine;
using Utils;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Deck", fileName = "NewResourceDeck")]
    public class ScriptableResourceDeck : DefaultableScriptableObject<ScriptableResourceDeck>
    {
        public ResourceDeck resourceDeck;
    }
}