using System;
using GridSystem.UI;
using Interactions;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using Utils;

namespace GridSystem
{
    public class GridInteractor : Singleton<GridInteractor>, ITooltipMessenger, IInteractor
    {
        [SerializeField] private TextTooltipMessageUI _terrainTooltipMessage;
        public TooltipMessageUI tooltipMessage => _terrainTooltipMessage;
        
        public ITooltipMessenger tooltipMessengerSelf => this;

        public bool isActive { get; private set; }
        public InteractionMode interactionMode => InteractionMode.GridInteraction;
        private static CellData s_hoveredCell;

        private void Update()
        {
            if (!isActive) return;

            if (GridManager.HoveredCell != null)
            {
                if (s_hoveredCell == null)
                    Tooltip.Sub(this);


                if (Input.GetMouseButtonUp(0))
                {
                    CellInfoPanel.Instance.OpenPanel();    
                }
            }
            else
            {
                if (s_hoveredCell != null)
                    Tooltip.Unsub(this);
            }

            s_hoveredCell = GridManager.HoveredCell;
        }

        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textMessageUI)
            {
                textMessageUI.SetTexts(s_hoveredCell.terrain.terrainName, "");
            }
        }

        public void ActivateMode()
        {
            isActive = true;
        }

        public void DeactivateMode()
        {
            isActive = false;
        }
    }
}