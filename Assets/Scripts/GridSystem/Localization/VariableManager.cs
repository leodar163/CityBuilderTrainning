using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace GridSystem.Localization
{
    public class VariableManager : Singleton<VariableManager>
    {
        [Header("Resource Variables")] 
        [SerializeField] private LocalizedString _localizedMaxQuantity;
        [SerializeField] private LocalizedString _localizedQuantity;
        [SerializeField] private LocalizedString _localizedBase;

        public static string maxQuantityName => Instance._localizedMaxQuantity.GetLocalizedString();
        public static string quantityName => Instance._localizedQuantity.GetLocalizedString();
        public static string baseName => Instance._localizedBase.GetLocalizedString();
    }
}