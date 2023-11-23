using System;
using Localization;
using ResourceSystem.Markets;
using ResourceSystem.Markets.Modifiers;
using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace Format
{
    public class FormatManager : Singleton<FormatManager>
    {
        [Header("Conditions")]
        [SerializeField] private LocalizedString _localizedIs;
        [SerializeField] private LocalizedString _localizedIsPlural;
        [SerializeField] private LocalizedString _localizedIsNot;
        [SerializeField] private LocalizedString _localizedIsNotPlural;
        [Header("Market Scope")]
        [SerializeField] private LocalizedString _localizedAllMarket;
        [SerializeField] private LocalizedString _localizedMainMarket;
        [SerializeField] private LocalizedString _localizedRandomMarket;
        [Header("Facilities")]
        [SerializeField] protected LocalizedString _localizeNoPlacementCondition;
        [SerializeField] protected LocalizedString _localizeNoPlaceRemaining;

        [Header("Formatting")] 
        [SerializeField] private string _separatorFormat;

        [Header("Colors")] 
        [SerializeField] private Color _negativeColor = Color.red;
        [SerializeField] private Color _positiveColor = Color.green;
        [SerializeField] private Color _defaultTextColor = Color.white;
        [SerializeField] private Color _separatorColor = Color.gray;
        
        public static Color negativeColor => Instance._negativeColor;
        public static Color positiveColor => Instance._positiveColor;
        public static Color DefaultTextColor => Instance._defaultTextColor;
        public static Color separatorColor => Instance._separatorColor;
        
        public static string negativeColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._negativeColor);
        public static string positiveColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._positiveColor);
        public static string defaultTextColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._defaultTextColor);
        public static string separatorColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._separatorColor);
        public static string separator => $"<color=#{separatorColorHTML}>{Instance._separatorFormat}</color>";

        public static string isFormat => Instance._localizedIs.GetLocalizedString();
        public static string isFormatPlural => Instance._localizedIsPlural.GetLocalizedString();
        public static string isNotFormat => Instance._localizedIsNot.GetLocalizedString();
        public static string isNotFormatPlural => Instance._localizedIsNotPlural.GetLocalizedString();
        public static string conditionFalseFormat => $"<color=#{negativeColorHTML}>X</color>";
        public static string conditionTrueFormat => $"<color=#{positiveColorHTML}>O</color>";
        
        public static string AllMarketFormat => Instance._localizedAllMarket.GetLocalizedString();
        public static string MainMaretFormat => Instance._localizedMainMarket.GetLocalizedString();
        public static string RandomMarketFormat => Instance._localizedRandomMarket.GetLocalizedString();

        public static string NoPlacementCondition => Instance._localizeNoPlacementCondition.GetLocalizedString();
        public static string NoPlaceException => Instance._localizeNoPlaceRemaining.GetLocalizedString();
        
        public static string FormatMarketScope(MarketType marketFilter, MarketModifierScope scope)
        {
            string filterFormat = marketFilter switch
            {
                MarketType.Ecosystem => VariableNameManager.EcosystemName,
                MarketType.Artificial => VariableNameManager.MarketName,
                MarketType.Both => $"{VariableNameManager.MarketName} & {VariableNameManager.EcosystemName}",
                _ => throw new ArgumentOutOfRangeException()
            };
                
            return scope switch
            {
                MarketModifierScope.Global => string.Format(FormatManager.AllMarketFormat, filterFormat),
                MarketModifierScope.Main => string.Format(FormatManager.MainMaretFormat, filterFormat),
                MarketModifierScope.Random => string.Format(FormatManager.RandomMarketFormat, filterFormat),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static string Plus(float value, float threshold = 0)
        {
            return value >= 0 ? "+" : "";
        }
    }
}