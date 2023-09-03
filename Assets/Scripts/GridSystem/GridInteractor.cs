using GridSystem.UI;
using Interactions;
using ToolTipSystem;
using UnityEngine;
using Utils;

namespace GridSystem
{
    public class GridInteractor : Singleton<GridInteractor>, IInteractionMode
    {
        public bool isActive { get; private set; }

        private void Update()
        {
            if (!isActive) return;

            if (GridManager.HoveredCell != null)
            {
                ToolTip.Sub(GridManager.HoveredCell.terrain);
                
                if (Input.GetMouseButtonUp(0))
                {
                    CellInfoPanel.Instance.OpenPanel();    
                }
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