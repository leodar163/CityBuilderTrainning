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
        [SerializeField] private Color _separatorColor = Color.gray;
        
        public static string negativeColor => ColorUtility.ToHtmlStringRGBA(Instance._negativeColor);
        public static string positiveColor => ColorUtility.ToHtmlStringRGBA(Instance._positiveColor);
        public static string separatorColor => ColorUtility.ToHtmlStringRGBA(Instance._separatorColor);
        public static string separator => $"<color=#{separatorColor}>{Instance._separatorFormat}</color>";

        public static string isFormat => Instance._localizedIs.GetLocalizedString();
        public static string isFormatPlural => Instance._localizedIsPlural.GetLocalizedString();
        public static string isNotFormat => Instance._localizedIsNot.GetLocalizedString();
        public static string isNotFormatPlural => Instance._localizedIsNotPlural.GetLocalizedString();
        public static string conditionFalseFormat => $"<color=#{negativeColor}>X</color>";
        public static string conditionTrueFormat => $"<color=#{positiveColor}>O</color>";
    }
}