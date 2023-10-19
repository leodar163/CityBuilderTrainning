using System;
using Conditions.Placement;
using Format;
using GridSystem;
using JetBrains.Annotations;
using Localization;
using TerrainSystem;
using TimeSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class FacilityType : MonoBehaviour , IToolTipSpeaker
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;

        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [Header("Placement Description")] 
        public PlacementCondition placementCondition;
        [SerializeField] private LocalizedString _localizeNoPlacementCondition;
        
        public CellData cell { get; private set; }
        public BoxCollider Collider => _collider;

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.GetLocalizedString();
        public string modifierName => facilityName;

        public float constructionCost;
        

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
        }
        

        public virtual void OnAddedToCell(CellData cellAddedTo)
        {
            cell = cellAddedTo;
            
            onFacilityBuild?.Invoke(this);
        }

        public virtual void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            cell = null;
            onFacilityDestroyed?.Invoke(this);
            Destroy(gameObject);
        }

        public bool CanBePlaced(CellData cellData, out string conditionsFormat)
        {
            if (placementCondition)
                return placementCondition.CanPlace(cellData,out conditionsFormat);
            
            conditionsFormat = _localizeNoPlacementCondition.GetLocalizedString();
            return true;
        }

        #region TOOLTIP

        private string FormatProductionLines()
        {
            string format = $"<b><uppercase>{VariableNameManager.ProductionName}</uppercase></b>";
            

            return format;
        }

        private string FormatProductionLinesDelta()
        {
            string format = $"<b><uppercase>{VariableNameManager.ProductionName}</uppercase></b>";
            return format;
        }

        public virtual ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = _facilityName.GetLocalizedString(),
                message = _facilityDescription.GetLocalizedString() +
                          $"\n{FormatManager.separator}\n{(cell == null ? FormatProductionLines() : FormatProductionLinesDelta())}"
                          + $"\n{FormatManager.separator}\n" +
                          $"{(placementCondition ? placementCondition.GetNeutralPlacementFormat() : _localizeNoPlacementCondition.GetLocalizedString())}"
            };
        }
        #endregion
    }
}