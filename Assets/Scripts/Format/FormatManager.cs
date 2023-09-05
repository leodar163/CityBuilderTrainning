using UnityEngine;
using Utils;

namespace Format
{
    public class FormatManager : Singleton<FormatManager>
    {
        [Header("Formatting")] 
        [SerializeField] private string _separatorFormat;

        [Header("Colors")] 
        [SerializeField] private Color _negativeColor = Color.red;
        [SerializeField] private Color _positiveColor = Color.green;
        [SerializeField] private Color _separatorColor = Color.gray;
        
        public static string negativeColor => ColorUtility.ToHtmlStringRGBA(Instance._negativeColor);
        public static string positiveColor => ColorUtility.ToHtmlStringRGBA(Instance._positiveColor);
        public static string separatorColor => ColorUtility.ToHtmlStringRGBA(Instance._separatorColor);
        public static string separatorColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._separatorColor);
        public static string separator => $"<color=#{separatorColorHTML}>{Instance._separatorFormat}</color>";
        
        
    }
}