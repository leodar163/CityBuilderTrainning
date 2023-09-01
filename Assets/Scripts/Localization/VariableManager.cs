using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace Localization
{
    public class VariableManager : Singleton<VariableManager>
    {
        [Header("Resource Variables")] 
        [SerializeField] private LocalizedString _localizedMaxQuantity;
        [SerializeField] private LocalizedString _localizedQuantity;
        [SerializeField] private LocalizedString _localizedBase;
        [SerializeField] private LocalizedString _localizedAvailableQuantity;

        public static string maxQuantityName => Instance._localizedMaxQuantity.GetLocalizedString();
        public static string quantityName => Instance._localizedQuantity.GetLocalizedString();
        public static string baseName => Instance._localizedBase.GetLocalizedString();
        public static string availableQuantityName => Instance._localizedAvailableQuantity.GetLocalizedString();
    }
}