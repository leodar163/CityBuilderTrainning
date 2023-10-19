using System;
using GridSystem.UI;
using Interactions;
using ToolTipSystem;
using UnityEngine;
using Utils;

namespace GridSystem
{
    public class GridInteractor : Singleton<GridInteractor>, ITooltipMessenger
    {
        [SerializeField] private TooltipMessageUI _terrainTooltipMessage;
        public TooltipMessageUI message => _terrainTooltipMessage;
        
        public ITooltipMessenger tooltipMessengerSelf => this;

        public bool isActive { get; private set; }
        public InteractionMode interactionMode => InteractionMode.GridInteraction;

        private void Update()
        {
            if (!isActive) return;

            if (GridManager.HoveredCell != null)
            {
                Tooltip.Sub(this);


                if (Input.GetMouseButtonUp(0))
                {
                    CellInfoPanel.Instance.OpenPanel();    
                }
            }
            else
            {
                Tooltip.Unsub(this);
            }
        }
        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
           
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