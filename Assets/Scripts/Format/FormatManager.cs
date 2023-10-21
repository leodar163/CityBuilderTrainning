using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
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
    }
}