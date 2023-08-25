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

            _mainCamera = Camera.main;
        }

        private void Start()
        {
            InitResourceSliders();
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
            ClearFacilityLayout();
            
            _coordinates.text = $"{currentCell.cellCoordinates.x}:{currentCell.cellCoordinates.y}";
            _terrainType.text = currentCell.terrain.terrainName;
            _facilityCapacity.text = $"{currentCell.terrain.facilityCount}/{currentCell.terrain.maxFacilityCount}";

            for (int i = 0; i < currentCell.terrain.facilityCount; i++)
            {
                Facility facilityToDisplay = currentCell.terrain.GetFacility(i);

                TryAddFacilityInfo(facilityToDisplay);
            }

            AssignSliders();
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
        
        private bool TryAddResourceSlider(ResourceType resource)
        {
            if (Instantiate(_resourceSliderUITemplate, _resourcesLayout)
                .TryGetComponent(out ResourceSliderUI resourceSliderUI))
            {
                resourceSliderUI.resource = resource;
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
        
        private void InitResourceSliders()
        {
            ClearResourceLayout();
            
            foreach (var resource in ResourceSet.Default.resources)
            {
                TryAddResourceSlider(resource);
            }
        }

        private void AssignSliders()
        {
            foreach (var sliderUi in _sliderUIs)
            {
                sliderUi.resourceSlider = currentCell.terrain.resourceDeck.GetSlider(sliderUi.resource);
            }
        }
    }
}