using System;
using System.Collections.Generic;
using Format;
using GridSystem;
using Localization;
using ResourceSystem;
using TerrainSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class FacilityType : MonoBehaviour , ICellModifier, IToolTipSpeaker, IResourceModifier, IResourceBorrower
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;

        [Header("Production")] 
        [SerializeField] protected ProductionLine[] _productionLines;
       
        
        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [SerializeField] protected LocalizedString _resourceModifications;
        [SerializeField] protected LocalizedString _placementConditions;

        public Dictionary<ResourceSlider, float> loaners { get; } = new();
        public string borrowerName => facilityName;
        
        public CellData cell { get; private set; }
        public BoxCollider Collider => _collider;

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.GetLocalizedString();
        public string modifierName => facilityName;

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
        }

        public virtual void OnAddedToCell(CellData cell)
        {
            this.cell = cell;
            cell.resourceDeck.Sub(this);
            onFacilityBuild?.Invoke(this);
            
            foreach (var line in _productionLines)
            {
                line.Init(cell, this);
            }
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            this.cell = null;
            cell.resourceDeck.Unsub(this);
            onFacilityDestroyed?.Invoke(this);
            
            foreach (var line in _productionLines)
            {
                line.Init(null, null);
            }
        }

        public virtual bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            conditionsFormat = _placementConditions.GetLocalizedString();
            return true;
        }

        protected void FormatCondition(string conditionVariableName, bool condition)
        {
            ((StringVariable)_placementConditions[conditionVariableName]).Value = $" : " +
                $"<color=#{ ColorUtility.ToHtmlStringRGBA(condition ? Color.green : Color.red)}>" + 
                (condition ? "O" : "X") +
                "</color>";
        }

        #region TOOLTIP

        private string FormatProductionLines()
        {
            string format = $"<b><uppercase>{VariableNameManager.ProductionName}</uppercase></b>";

            foreach (var line in _productionLines)
            {
                format += $"\n<indent=5%>{line.GetProductionFormat()}</indent>\n";
            }

            return format;
        }

        private string FormatProductionLinesDelta()
        {
            string format = $"<b><uppercase>{VariableNameManager.ProductionName}</uppercase></b>";

            foreach (var line in _productionLines)
            {
                format += $"\n<indent=5%>{ProductionLine.GetProductionFormat(line, line.GetResourceDeltaIn(), line.GetResourceDeltaOut(), true)}</indent>\n";
            }

            return format;
        }

        public virtual ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = _facilityName.GetLocalizedString(),
                message = _facilityDescription.GetLocalizedString() +
                          $"\n{FormatManager.separator}\n{(cell == null ? FormatProductionLines() : FormatProductionLinesDelta())}" + 
                          $"\n{FormatManager.separator}\n{_placementConditions.GetLocalizedString()}"
            };
        }
        #endregion
        

        #region PRODUCTION

        public virtual ResourceDelta[] GetResourceDelta()
        {
            ResourceDelta[] deltas = Array.Empty<ResourceDelta>();

            foreach (var line in _productionLines)
            {
                ResourceDelta[] lineDeltas = line.GetResourceDelta();

                ResourceDelta[] newDeltas = new ResourceDelta[deltas.Length + lineDeltas.Length];
                deltas.CopyTo(newDeltas, 0);
                lineDeltas.CopyTo(newDeltas, deltas.Length);

                deltas = newDeltas;
            }
            return deltas;
        }
        
        

        #endregion

        
    }
}