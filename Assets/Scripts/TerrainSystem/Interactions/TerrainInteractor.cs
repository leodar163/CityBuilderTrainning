using GridSystem;
using GridSystem.Interaction;
using GridSystem.UI;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using Utils;

namespace TerrainSystem.Interactions
{
    public class TerrainInteractor : Singleton<TerrainInteractor>, IGridInteractor, ITooltipMessenger
    {
        
        [Header("Tooltip")]
        [SerializeField] private TextTooltipMessageUI _terrainTooltipMessage;
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _terrainTooltipMessage;
        public GridInteractorType type => GridInteractorType.Terrain;
        public bool isActive { get; set; }
        public bool cancelable => false;

        public void OnHoveredCellChanged(CellData cellData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
            if (cellData != null)
                tooltipMessengerSelf.SubToTooltip();
        }

        void IGridInteractor.OnActivated()
        {
            
        }

        void IGridInteractor.OnDeactivated()
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && GridEventSystem.HoveredCell != null)
            {
                CellInfoPanel.Instance.OpenPanel();
            }
        }

        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textMessageUI)
            {
                textMessageUI.SetTexts(GridEventSystem.HoveredCell.terrain.terrainName, "");
            }
        }
    }
}