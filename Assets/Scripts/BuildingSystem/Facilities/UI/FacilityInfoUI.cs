using TMPro;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityInfoUI : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        private FacilityType _facility;
        
        public FacilityType Facility
        {
            get => _facility;
            set
            {
                if (_facility != null)
                {
                    _deleteButton.onClick.RemoveAllListeners();
                }
                _facility = value;
                DisplayFacility();  
            } 
        }

        private ConstructionSite _constructionSite;

        [SerializeField] private Image _facilityIcon;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Slider _constructionSlider;
        [SerializeField] private TextMeshProUGUI _progressionText;
        
        [Header("Tooltip")]
        [SerializeField] private FacilityTooltipMessageUI _facilityTooltipMessageTemplate;
        public TooltipMessageUI tooltipMessage => _facilityTooltipMessageTemplate;
        public ITooltipMessenger tooltipMessengerSelf => this;

        private void Awake()
        {
            if (_deleteButton)
                _deleteButton.onClick.AddListener(DestroyFacility);
        }

        private void Update()
        {
            if (_constructionSite != null)
            {
                _constructionSlider.maxValue = Mathf.RoundToInt(_constructionSite.constructionCost * 100) / 100f;
                
                _constructionSlider.value = Mathf.RoundToInt(_constructionSite.constructionInvestment * 100) / 100f;

                _progressionText.text = $"{_constructionSlider.value}/{_constructionSlider.maxValue}";
                
                _constructionSlider.gameObject.SetActive(true);
                _progressionText.gameObject.SetActive(true);
            }
            else
            {
                _constructionSlider.gameObject.SetActive(false);
                _progressionText.gameObject.SetActive(false);
            }
        }

        private void DisplayFacility()
        {
            if (_facility == null)return;

            _constructionSite = _facility as ConstructionSite;

            DisplayFacility(_constructionSite == null ? _facility : _constructionSite.facilityToBuild);
        }

        private void DisplayFacility(FacilityType facility)
        {
            _facilityIcon.sprite = facility.icon;
        }

        private void DestroyFacility()
        {
            _facility.cell.RemoveFacility(_facility);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }

        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is FacilityTooltipMessageUI facilityUI)
            {
                facilityUI.SetFacility(_constructionSite == null ? _facility : _constructionSite.facilityToBuild);
            }
        }
    }
}