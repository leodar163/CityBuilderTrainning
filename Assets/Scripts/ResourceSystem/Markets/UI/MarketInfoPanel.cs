using System.Collections.Generic;
using ResourceSystem.Categories;
using ResourceSystem.Categories.UI;
using ResourceSystem.Markets.Interactions;
using ResourceSystem.Markets.Modifiers.UI;
using TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace ResourceSystem.Markets.UI
{
    public class MarketInfoPanel : PanelUI<MarketInfoPanel>
    {
        [Header("Infos")]
        [SerializeField] private TextMeshProUGUI _marketName;
        [SerializeField] private Image _marketColor;
        [SerializeField] private HappinessSlider _happinessSlider;
        

        [Header("Values")] 
        [SerializeField] private RectTransform _resourceValuesContainer;
        [SerializeField] private ResourceValueUI _resourceValueUITemplate;
        [SerializeField] private List<ResourceValueUI> _resourceValuesUI;
        
        [Header("Filtering")]
        [SerializeField] private ResourceCategorySet _categorySet;
        [SerializeField] private RectTransform _filterButtonContainer;
        [SerializeField] private ResourceCategoryButton _categoryButtonTemplate;
        [SerializeField] private List<ResourceCategoryButton> _categoryButtons;
        private ResourceCategory _currentCategoryFilter;

        [Header("Modifiers")] 
        [SerializeField] private MarketModifierUI _marketModifierUITemplate;
        [SerializeField] private RectTransform _modifierContainer;
        private readonly List<MarketModifierUI> _modifiers = new();


        private Market _market;

        private void Awake()
        {
            _categorySet ??= ResourceCategorySet.Default;
            InitCategoryFilterButtons();
        }

        private void OnEnable()
        {
            TimeManager.onMonthBegins += UpdateDisplay;
            ResourceCategoryButton.onSelect += FilterByResourceCategory;
            ResourceCategoryButton.onUnselect += FilterByResourceCategory;
        }

        private void OnDisable()
        {
            TimeManager.onMonthBegins -= UpdateDisplay;
            ResourceCategoryButton.onSelect -= FilterByResourceCategory;
            ResourceCategoryButton.onUnselect -= FilterByResourceCategory;
        }

        #region FILTERS

        private void InitCategoryFilterButtons()
        {
            foreach (var cate in _categorySet.Categories)
            {
                if (TryGetFilterButton(cate, out ResourceCategoryButton button) ||
                    !Instantiate(_categoryButtonTemplate.gameObject, _filterButtonContainer).TryGetComponent(out button)) 
                    continue;
                _categoryButtons.Add(button);
                button.Category = cate;
            }
        }

        private bool TryGetFilterButton(ResourceCategory category, out ResourceCategoryButton button)
        {
            foreach (var categoryButton in _categoryButtons)
            {
                if (categoryButton.Category != category) continue;
                button = categoryButton;
                return true;
            }

            button = null;
            return false;
        }

        #endregion

        #region MODIFIERS

        private void UpdateModifiers()
        {
            for (int i = 0; i < _market.modifiers.Count; i++)
            {
                if (_modifiers.Count < i + 1 && 
                    Instantiate(_marketModifierUITemplate, _modifierContainer)
                        .TryGetComponent(out MarketModifierUI modifierUI))
                {
                    _modifiers.Add(modifierUI);
                }
                else
                {
                    modifierUI = _modifiers[i];
                }

                modifierUI.Modifier = _market.modifiers[i];
                modifierUI.gameObject.SetActive(true);
            }

            for (int i = 0 ; i < MarketManager.Instance.modifiers.Count; i++)
            {
                if (_modifiers.Count < i + _market.modifiers.Count && 
                    Instantiate(_marketModifierUITemplate, _modifierContainer)
                        .TryGetComponent(out MarketModifierUI modifierUI))
                {
                    _modifiers.Add(modifierUI);
                }
                else
                {
                    modifierUI = _modifiers[_market.modifiers.Count + i];
                }

                modifierUI.Modifier = MarketManager.Instance.modifiers[i];
                modifierUI.gameObject.SetActive(true);
            }

            if (_modifiers.Count > MarketManager.Instance.modifiers.Count + _market.modifiers.Count)
            {
                for (int i = MarketManager.Instance.modifiers.Count + _market.modifiers.Count -1; i < _modifiers.Count; i++)
                {
                    _modifiers[i].gameObject.SetActive(false);
                }
            }
        }

        #endregion
        
        private void UpdateDisplay()
        {
            if (!isOpen) return;
            
            _marketName.SetText(_market.name);
            _marketColor.color = _market.color;

            CheckForMissingValues();
            UpdateValuesDisplaying();

            if (_market.type != MarketType.Ecosystem)
            {
                _happinessSlider.UpdateSlider();

                _happinessSlider.gameObject.SetActive(true);
            }
            else
            {
                _happinessSlider.gameObject.SetActive(false);
            }
            
            UpdateModifiers();
        }

        private void UpdateValuesDisplaying()
        {
            foreach (var value in _resourceValuesUI)
            {
                if (_currentCategoryFilter != null && value.Resource.Category != _currentCategoryFilter)
                    continue;
                
                if (_market.TryGetResourceValue(value.Resource, out _))
                {
                    value.gameObject.SetActive(true);
                    value.UpdateDisplay();
                    continue;
                }

                value.gameObject.SetActive(false);
            }
        }
        
        private void CheckForMissingValues()
        {
            if (_market == null) return;

            foreach (var value in _market._resourceValues)
            {
                if (!TryGetResourceValueUI(value.resource, out ResourceValueUI resourceValueUI)
                    && Instantiate(_resourceValueUITemplate.gameObject, _resourceValuesContainer)
                        .TryGetComponent(out resourceValueUI))
                {
                    _resourceValuesUI.Add(resourceValueUI);
                    resourceValueUI.Resource = value.resource;
                    resourceValueUI.Market = _market;
                }
            }
        }

        private bool TryGetResourceValueUI(ResourceType resource, out ResourceValueUI resourceValueUI)
        {
            foreach (var value in _resourceValuesUI)
            {
                if (value.Resource != resource) continue;
                resourceValueUI = value;
                return true;
            }

            resourceValueUI = null;
            return false;
        }

        private void FilterByResourceCategory(ResourceCategory category)
        {
            _currentCategoryFilter = category;
            foreach (var value in _resourceValuesUI)
            {
                value.gameObject.SetActive(category == null || value.Resource.Category == category);
            }
        }
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            _market = MarketInteractor.SelectedMarket;
            _happinessSlider.market = _market;
       
            foreach (var value in _resourceValuesUI)
            {
                value.Market = _market;
            }
            
            UpdateDisplay();
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            _market = null;
            _happinessSlider.market = null;
            
            foreach (var value in _resourceValuesUI)
            {
                value.Market = null;
            }
        }
    }
}