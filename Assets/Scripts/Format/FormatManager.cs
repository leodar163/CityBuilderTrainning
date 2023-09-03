using UnityEngine;
using Utils;

namespace Format
{
    public class FormatManager : Singleton<FormatManager>
    {
        [Header("Formatting")] 
        [SerializeField] private string _separatorFormat;
        
        [Header("Colors")]
        [SerializeField] private Color _separatorColor = Color.gray;
        
        public static Color separatorColor => Instance._separatorColor;
        public static string separatorColorHTML => ColorUtility.ToHtmlStringRGBA(Instance._separatorColor);
        public static string separator => $"<color=#{separatorColorHTML}>{Instance._separatorFormat}</color>";
    }
}