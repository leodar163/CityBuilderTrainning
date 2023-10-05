using System.Collections.Generic;
using ResourceSystem.Transactions;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace ResourceSystem.Scriptables
{
    [CreateAssetMenu(menuName = "Resources/Resource Deck", fileName = "NewResourceDeck")]
    public class ResourceDeck : DefaultableScriptableObject<ResourceDeck>
    {
        [SerializeField] public ResourceContainer[] containers;
    }
}