using UnityEngine;
using Utils;

namespace ResourceSystem.Player
{
    public class PlayerResourceDeck : Singleton<PlayerResourceDeck>
    {
        [SerializeField] private ScriptableResourceDeck _resourceDeck;
        public static ResourceDeck deck { get; private set; }

        private void Awake()
        {
            deck = _resourceDeck.GetResourceDeckCopy();
        }
    }
}