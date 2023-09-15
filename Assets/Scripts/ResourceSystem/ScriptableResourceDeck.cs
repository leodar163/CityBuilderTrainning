using System.Collections.Generic;
using ResourceSystem.Transactions;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Deck", fileName = "NewResourceDeck")]
    public class ScriptableResourceDeck : DefaultableScriptableObject<ScriptableResourceDeck>
    {
        [SerializeField] public ResourceContainer[] containers;
    }
}