using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.UI;
using ResourceSystem;
using ResourceSystem.UI;
using TimeSystem;
using TMPro;
using UnityEngine;
using Utils.UI;

namespace GridSystem.UI
{
    public class CellPanel : PanelUI<CellPanel>
    {
        [SerializeField] private RectTransform _child;

        [Header("Terrain")]
        [SerializeField] private TextMeshProUGUI _coordinates;
        [SerializeField] private TextMeshProUGUI _terrainType;
        
        [Header("Facility")]
        [SerializeField] private TextMeshProUGUI _facilityCapacity;
        [SerializeField] private RectTransform _facilitiesLayout;
        [SerializeField] private FacilityInfoUI _facilityInfoUITemplate;
        private readonly List<FacilityInfoUI> _facilityInfoUIs = new ();
        
        [Header("Resources")]
        [SerializeField] private RectTransform _resourcesLayout;
        [SerializeField] private ResourceSliderUI _resourceSliderUITemplate;
        private readonly List<ResourceSliderUI> _sliderUIs = new ();

        private Camera _mainCamera;
        public CellData currentCell { get; private set; }

        private Action<InGameDate> _resourceCheck;

        protected override void Awake()
        {
            base.Awake();
            _resourceCheck = _ =>
            {
                if(currentCell != null)
                    CheckForResourceSliders();
            };
            
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            TimeManager.onNewMonth += _resourceCheck;
        }

        private void OnDisable()
        {
            TimeManager.onNewMonth -= _resourceCheck;
        }

        public override void OpenPanel()
        {
            if (GridManager.HoveredCell == null)
                return;

            currentCell = GridManager.HoveredCell;
            
            base.OpenPanel();
            
            DisplayCellInfos();
            _child.gameObject.SetActive(true);
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            _child.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(!isOpen) return;
            
            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape))
            {
                ClosePanel();
            }

            if (currentCell != null)
            {
                CheckForFacilities();
            }
        }

        private void Clear()
        {
            ClearFacilityLayout();
            
            ClearResourceLayout();
        }

        private void ClearFacilityLayout()
        {
            foreach (var facilityInfoUI in _facilityInfoUIs)
            {
                Destroy(facilityInfoUI.gameObject);
            }
            
            _facilityInfoUIs.Clear();
        }

        private void ClearResourceLayout()
        {
            foreach (var slider in _sliderUIs)
            {
                Destroy(slider.gameObject);
            }

            _sliderUIs.Clear();
        }
        
        
        private void DisplayCellInfos()
        {
            Clear();
            
            _coordinates.text = $"{currentCell.cellCoordinates.x}:{currentCell.cellCoordinates.y}";
            _terrainType.text = currentCell.terrain.terrainName;
            _facilityCapacity.text = $"{currentCell.terrain.facilityCount}/{currentCell.terrain.maxFacilityCount}";

            for (int i = 0; i < currentCell.terrain.facilityCount; i++)
            {
                Facility facilityToDisplay = currentCell.terrain.GetFacility(i);

                TryAddFacilityInfo(facilityToDisplay);
            }

            ResourceDeck resourceDeck = currentCell.terrain.resourceDeck;
            
            foreach (var sliderToDisplay in resourceDeck.resourceSliders)
            {
                TryAddResourceSlider(sliderToDisplay);
            }
        }

        private bool TryAddFacilityInfo(Facility facility)
        {
            if (Instantiate(_facilityInfoUITemplate, _facilitiesLayout)
                .TryGetComponent(out FacilityInfoUI facilityInfo))
            {
                facilityInfo.facility = facility;
                _facilityInfoUIs.Add(facilityInfo);
                return true;
            }

            return false;
        }
        
        private bool TryAddResourceSlider(ResourceSlider slider)
        {
            if (Instantiate(_resourceSliderUITemplate, _resourcesLayout)
                .TryGetComponent(out ResourceSliderUI resourceSliderUI))
            {
                resourceSliderUI.resourceSlider = slider;
                _sliderUIs.Add(resourceSliderUI);
                return true;
            }

            return false;
        }

        private void CheckForFacilities()
        {
            if (_facilityInfoUIs.Count != currentCell.terrain.facilityCount)
            {
                ClearFacilityLayout();
                
                for (int i = 0; i < currentCell.terrain.facilityCount; i++)
                {
                    TryAddFacilityInfo(currentCell.terrain.GetFacility(i));
                }
            }
            
            _facilityCapacity.text = $"{currentCell.terrain.facilityCount}/{currentCell.terrain.maxFacilityCount}";
        }

        private void CheckForResourceSliders()
        {
            if (_sliderUIs.Count != currentCell.terrain.resourceDeck.resourceSliders.Count)
            {
                ClearResourceLayout();

                foreach (var slider in currentCell.terrain.resourceDeck.resourceSliders)
                {
                    TryAddResourceSlider(slider);
                }
            }
        }
    }
}