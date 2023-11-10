using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Events.UI
{
    public class GameEventWindow : Singleton<GameEventWindow>
    {
        private static GameEvent _currentGameEvent;
        [SerializeField] private RectTransform _child;
        [SerializeField] private Image _illustration;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private TextMeshProUGUI _effects;
        [SerializeField] private Button _validationButton;

        private void OnEnable()
        {
            GameEvent.onEventFired += OpenEvent;
            _validationButton.onClick.AddListener(CloseCurrentEvent);
        }

        private void OnDisable()
        {
            GameEvent.onEventFired -= OpenEvent;
            _validationButton.onClick.RemoveListener(CloseCurrentEvent);
        }


        public static void OpenEvent(GameEvent gameEvent)
        {
            if (_currentGameEvent != null)
                CloseCurrentEvent();
            _currentGameEvent = gameEvent;

            if (Instance._illustration) Instance._illustration.sprite = gameEvent.illustration;

            Instance._title.SetText(gameEvent.Title);
            Instance._desc.SetText(gameEvent.Desc);

            string effectFormat = "";

            foreach (var effect in gameEvent.Effects)
            {
                effectFormat += effect.GetEffectFormat() + '\n';
            }
            
            Instance._effects.SetText(effectFormat);
            
            Instance._child.gameObject.SetActive(true);
        }

        public static void CloseCurrentEvent()
        {
            if (_currentGameEvent == null) return;
            Instance._child.gameObject.SetActive(false);
        }
        
        
    }
}