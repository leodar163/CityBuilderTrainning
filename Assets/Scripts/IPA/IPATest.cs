using TMPro;
using UnityEngine;

namespace IPA
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class IPATest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private char char1;
        [SerializeField] private char char2;
        [SerializeField] private string result;

        private void OnValidate()
        {
            result = $"{char1}{char2}";
            if (!_text) TryGetComponent(out _text);
            _text.text =  ": " + char1 + char2;
        }
    }
}
