using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace Localization
{
    public class VariableNameManager : Singleton<VariableNameManager>
    {
        [Header("Resource Variables")] 
        [SerializeField] private LocalizedString _localizedMaxQuantity;
        [SerializeField] private LocalizedString _localizedQuantity;
        [SerializeField] private LocalizedString _localizedBase;
        [SerializeField] private LocalizedString _localizedAvailableQuantity;
        [SerializeField] private LocalizedString _localizedIn;
        [SerializeField] private LocalizedString _localizedOut;
        [SerializeField] private LocalizedString _localizedProduction;
        [SerializeField] private LocalizedString _localizedEfficiency;
        

        public static string maxQuantityName => Instance._localizedMaxQuantity.GetLocalizedString();
        public static string quantityName => Instance._localizedQuantity.GetLocalizedString();
        public static string baseName => Instance._localizedBase.GetLocalizedString();
        public static string availableQuantityName => Instance._localizedAvailableQuantity.GetLocalizedString();
        public static string InName => Instance._localizedIn.GetLocalizedString();
        public static string OutName => Instance._localizedOut.GetLocalizedString();
        public static string ProductionName => Instance._localizedProduction.GetLocalizedString();
        public static string EfficiencyName => Instance._localizedEfficiency.GetLocalizedString();

        
        
    }
}