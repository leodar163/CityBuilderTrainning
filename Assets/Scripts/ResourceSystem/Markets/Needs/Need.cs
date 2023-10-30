using System;

namespace ResourceSystem.Markets.Needs
{
    [Serializable]
    public struct Need
    {
        public ResourceType resource;
        public float quantity;
        public float surplusMult;
        public float shortageMult;
    }
}