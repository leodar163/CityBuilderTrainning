using UnityEngine;
using UnityEngine.Localization;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource", fileName = "NewResource")]
    public class ResourceType : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private LocalizedString _name;
        public Sprite icon;
        [Space] 
        [SerializeField] private bool _borrowable;

        public bool borrowable => _borrowable;
        public string id => _id;
        public string resourceName => _name.GetLocalizedStringAsync().Result;
    }
}