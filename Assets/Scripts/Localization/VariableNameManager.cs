using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace Localization
{
    public class VariableNameManager : Singleton<VariableNameManager>
    {
        [Header("Resource Variables")] [SerializeField]
        private LocalizedString _localizedMaxQuantity;

        [SerializeField] private LocalizedString _localizedQuantity;
        [SerializeField] private LocalizedString _localizedBase;
        [SerializeField] private LocalizedString _localizedAvailableQuantity;
        [SerializeField] private LocalizedString _localizedProduction;
        [SerializeField] private LocalizedString _localizedEfficiency;
        [Header("Market")] 
        [SerializeField] private LocalizedString _localizedMarket;
        [SerializeField] private LocalizedString _localizedEcosystem;
        [SerializeField] private LocalizedString _localizedShortage;
        [SerializeField] private LocalizedString _localizedOffer;
        [SerializeField] private LocalizedString _localizedDemand;
        [SerializeField] private LocalizedString _localizedAvailability;
        [SerializeField] private LocalizedString _localizedAvailabilityDesc;


        public static string MaxQuantityName => Instance._localizedMaxQuantity.GetLocalizedString();
        public static string QuantityName => Instance._localizedQuantity.GetLocalizedString();
        public static string BaseName => Instance._localizedBase.GetLocalizedString();
        public static string AvailableQuantityName => Instance._localizedAvailableQuantity.GetLocalizedString();
        public static string ProductionName => Instance._localizedProduction.GetLocalizedString();
        public static string EfficiencyName => Instance._localizedEfficiency.GetLocalizedString();
        
        public static string MarketName => Instance._localizedMarket.GetLocalizedString();
        public static string EcosystemName => Instance._localizedEcosystem.GetLocalizedString();
        public static string ShortageName => Instance._localizedShortage.GetLocalizedString();
        public static string OfferName => Instance._localizedOffer.GetLocalizedString();
        public static string DemandName => Instance._localizedDemand.GetLocalizedString();
        public static string AvailabilityName => Instance._localizedAvailability.GetLocalizedString();
        public static string AvailabilityDesc => Instance._localizedAvailabilityDesc.GetLocalizedString();
        
    }
}