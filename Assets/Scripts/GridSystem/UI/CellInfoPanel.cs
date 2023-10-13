using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.UI;
using Interactions;
using ResourceSystem.Markets.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace GridSystem.UI
{
    public class CellInfoPanel : PanelUI<CellInfoPanel>
    {
        [SerializeField] private RectTransform _child;

        [Header("Market")] 
        [SerializeField] private TextMeshProUGUI _marketName;
        [SerializeField] private Button _marketLink;
        
        [Header("Terrain")]
        [SerializeField] private TextMeshProUGUI _coordinates;
        [SerializeField] private TextMeshProUGUI _terrainType;
        
        [Header("Facility")]
        [SerializeField] private TextMeshProUGUI _facilityCapacity;
        [SerializeField] private RectTransform _facilitiesLayout;
        [SerializeField] private FacilityInfoUI _facilityInfoUITemplate;
        private readonly List<FacilityInfoUI> _facilityInfoUIs = new ();
        
        public CellData currentCell { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _marketLink.onClick.AddListener(OpenCellMarket);
        }

        private void OpenCellMarket()
        {
            if (currentCell != null)
            {
                InteractionManager.SwitchInteractionMode(InteractionMode.MarketVue);
                MarketInteractor.SelectMarket(currentCell.market);
            }
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
            
            _marketName.SetText(currentCell.market.name);
            
            _coordinates.SetText($"{currentCell.cellCoordinates.x}:{currentCell.cellCoordinates.y}");
            _terrainType.SetText(currentCell.terrain.terrainName);
            _facilityCapacity.SetText($"{currentCell.facilityCount}/{currentCell.maxFacilityCount}");

            for (int i = 0; i < currentCell.facilityCount; i++)
            {
                FacilityType facilityTypeToDisplay = currentCell.GetFacility(i);

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
            if (_facilityInfoUIs.Count != currentCell.facilityCount)
            {
                ClearFacilityLayout();
                
                for (int i = 0; i < currentCell.facilityCount; i++)
                {
                    TryAddFacilityInfo(currentCell.GetFacility(i));
                }
            }
            
            _facilityCapacity.SetText($"{currentCell.facilityCount}/{currentCell.maxFacilityCount}");
        }
    }
}