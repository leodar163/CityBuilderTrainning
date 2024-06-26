﻿using System;
using ResourceSystem.Markets;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Utils;

namespace Localization
{
    public class VariableNameManager : Singleton<VariableNameManager>
    {
        [Header("Resource Variables")] 
        [SerializeField] private LocalizedString _localizedMaxQuantity;
        public static string MaxQuantityName { get; private set; }
        [SerializeField] private LocalizedString _localizedQuantity;
        public static string QuantityName { get; private set; }
        [SerializeField] private LocalizedString _localizedBase;
        public static string BaseName { get; private set; }
        [SerializeField] private LocalizedString _localizedAvailableQuantity;
        public static string AvailableQuantityName { get; private set; }
        [SerializeField] private LocalizedString _localizedProduction;
        public static string ProductionName { get; private set; }
        [SerializeField] private LocalizedString _localizedEfficiency;
        public static string EfficiencyName { get; private set; }
        
        [Header("Market")] 
        [SerializeField] private LocalizedString _localizedMarket;
        public static string MarketName { get; private set; }
        [SerializeField] private LocalizedString _localizedEcosystem;
        public static string EcosystemName { get; private set; }
        [SerializeField] private LocalizedString _localizedShortage;
        public static string ShortageName { get; private set; }
        [SerializeField] private LocalizedString _localizedOffer;
        public static string OfferName { get; private set; }
        [SerializeField] private LocalizedString _localizedDemand;
        public static string DemandName { get; private set; }
        [SerializeField] private LocalizedString _localizedAvailability;
        public static string AvailabilityName { get; private set; }
        [SerializeField] private LocalizedString _localizedAvailabilityDesc;
        public static string AvailabilityDesc { get; private set; }
        [SerializeField] private AutoLocalizedString _shortageOf;
        [SerializeField] private AutoLocalizedString _missingOf;
        [SerializeField] private AutoLocalizedString _averageOf;
        [SerializeField] private AutoLocalizedString _excessOf;
        [SerializeField] private AutoLocalizedString _abundanceOf;
        
        [Header("Facility")] 
        [SerializeField] private LocalizedString _localizedHealth;
        public static string HealthName { get; private set; }
        [SerializeField] private LocalizedString _localizedNeedFormat;
        public static string NeedFormat { get; private set; }
        [SerializeField] private AutoLocalizedString _destroyFacilityMessage;
        public static AutoLocalizedString DestroyFacilityMessage => Instance._destroyFacilityMessage;
        [SerializeField] private AutoLocalizedString _facilityCantDestroy;
        public static AutoLocalizedString FacilityCantDestroy => Instance._facilityCantDestroy;



        private void Awake()
        {
            InitStrings();
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += InitStrings;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= InitStrings;
        }

        public static AutoLocalizedString GetNeedFormat(ResourceAvailability availability)
        {
            return availability switch
            {
                ResourceAvailability.Shortage => Instance._shortageOf,
                ResourceAvailability.Missing => Instance._missingOf,
                ResourceAvailability.Average => Instance._averageOf,
                ResourceAvailability.InExcess => Instance._excessOf,
                ResourceAvailability.InAbundance => Instance._abundanceOf,
                _ => throw new ArgumentOutOfRangeException(nameof(availability), availability, null)
            };
        }
        
        private void InitStrings(Locale locale = null)
        {
            MaxQuantityName = _localizedMaxQuantity.GetLocalizedString();
            QuantityName = _localizedQuantity.GetLocalizedString();
            BaseName = _localizedBase.GetLocalizedString();
            AvailableQuantityName = _localizedAvailableQuantity.GetLocalizedString();
            ProductionName = _localizedProduction.GetLocalizedString();
            EfficiencyName = _localizedEfficiency.GetLocalizedString();

            MarketName = _localizedMarket.GetLocalizedString();
            EcosystemName = _localizedEcosystem.GetLocalizedString();
            ShortageName = _localizedShortage.GetLocalizedString();
            OfferName = _localizedOffer.GetLocalizedString();
            DemandName = _localizedDemand.GetLocalizedString();
            AvailabilityName = _localizedAvailability.GetLocalizedString();
            AvailabilityDesc = _localizedAvailabilityDesc.GetLocalizedString();
            
            HealthName = _localizedHealth.GetLocalizedString();
            NeedFormat = _localizedNeedFormat.GetLocalizedString();
        }
    }
}