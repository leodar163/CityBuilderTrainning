﻿using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.UI;
using ResourceSystem;
using ResourceSystem.Markets;
using ResourceSystem.UI;
using TMPro;
using UnityEngine;
using Utils.UI;

namespace GridSystem.UI
{
    public class CellInfoPanel : PanelUI<CellInfoPanel>
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
        [SerializeField] private ResourceSliderUI _populationSlider;
        [SerializeField] private ResourceSliderUI _habitationSlider;
        [SerializeField] private ResourceSliderUI _workforceSlider;
        [SerializeField] private ResourceSliderUI _environmentSlider;
        [SerializeField] private ResourceSliderUI _foodSlider;
        [SerializeField] private ResourceSliderUI _woodSlider; 
        [SerializeField] private ResourceSliderUI _mineralsSlider;

        private Camera _mainCamera;
        public CellData currentCell { get; private set; }
        

        protected override void Awake()
        {
            base.Awake();

            _populationSlider.resourceSlider = new ResourceSlider("resource_population",Mathf.Infinity);
            
            _mainCamera = Camera.main;
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

            if (currentCell != null)
            {
                CheckForFacilities();
                _populationSlider.resourceSlider.nativeQuantity = 
                    currentCell.market.GetResourceValueAmount(_populationSlider.resource, OrderType.Offer);
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


        private void DisplayCellInfos()
        {
            ClearFacilityLayout();
            
            _coordinates.SetText($"{currentCell.cellCoordinates.x}:{currentCell.cellCoordinates.y}");
            _terrainType.SetText(currentCell.terrain.terrainName);
            _facilityCapacity.SetText($"{currentCell.terrain.facilityCount}/{currentCell.terrain.maxFacilityCount}");

            for (int i = 0; i < currentCell.terrain.facilityCount; i++)
            {
                FacilityType facilityTypeToDisplay = currentCell.terrain.GetFacility(i);

                TryAddFacilityInfo(facilityTypeToDisplay);
            }
        }

        private bool TryAddFacilityInfo(FacilityType facilityType)
        {
            if (Instantiate(_facilityInfoUITemplate, _facilitiesLayout)
                .TryGetComponent(out FacilityInfoUI facilityInfo))
            {
                facilityInfo.Facility = facilityType;
                _facilityInfoUIs.Add(facilityInfo);
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
            
            _facilityCapacity.SetText($"{currentCell.terrain.facilityCount}/{currentCell.terrain.maxFacilityCount}");
        }
    }
}