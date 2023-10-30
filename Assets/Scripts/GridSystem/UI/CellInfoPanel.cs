using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.UI;
using GridSystem.Interaction;
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
                MapFilter.ShowMapFilter(TileMapType.Market);
                GridEventSystem.SwitchInteractor(GridInteractorType.Market);
                MarketInteractor.SelectMarket(currentCell.market);
            }
        }
        
        public override void OpenPanel()
        {
            if (GridEventSystem.HoveredCell == null)
                return;

            currentCell = GridEventSystem.HoveredCell;
            
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
                DisplayFacilityLayout();
            }
        }

        private void DisplayCellInfos()
        {
           DisplayFacilityLayout();
            
            _marketName.SetText(currentCell.market.name);
            
            _coordinates.SetText($"{currentCell.cellCoordinates.x}:{currentCell.cellCoordinates.y}");
            _terrainType.SetText(currentCell.terrain.terrainName);
            _facilityCapacity.SetText($"{currentCell.facilityCount}/{currentCell.maxFacilityCount}");
        }

        private void AddFacilityInfoUI(FacilityType facilityType)
        {
            if (!Instantiate(_facilityInfoUITemplate, _facilitiesLayout)
                    .TryGetComponent(out FacilityInfoUI facilityInfo)) return;
            
            facilityInfo.Facility = facilityType;
            _facilityInfoUIs.Add(facilityInfo);
        }

        //Too much heavy to be called every frames... Need to find another way to update
        private void DisplayFacilityLayout()
        {
            List<FacilityInfoUI> facilityUIsToRemove = new();
            List<FacilityType> facilityToDisplay = new(currentCell.facilities);

            foreach (var facilityInfoUI in _facilityInfoUIs)
            {
                if (facilityInfoUI.Facility == null || !currentCell.facilities.Contains(facilityInfoUI.Facility))
                {
                    facilityUIsToRemove.Add(facilityInfoUI);
                }
                else
                {
                    facilityToDisplay.Remove(facilityInfoUI.Facility);
                }
            }

            foreach (var facility in facilityToDisplay)
            {
                if (facilityUIsToRemove.Count > 0)
                {
                    facilityUIsToRemove[0].Facility = facility;
                    facilityUIsToRemove.RemoveAt(0);
                    continue;
                }
                
                AddFacilityInfoUI(facility);
            }

            foreach (var facilityUI in facilityUIsToRemove)
            {
                _facilityInfoUIs.Remove(facilityUI);
                Destroy(facilityUI.gameObject);
            }

            _facilityCapacity.SetText($"{currentCell.facilityCount}/{currentCell.maxFacilityCount}");
        }
    }
}