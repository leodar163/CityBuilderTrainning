using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using TerrainSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Facility : MonoBehaviour , ICellModifier, IToolTipSpeaker, IResourceModifier
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;
        
        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [SerializeField] protected LocalizedString _resourceModifications;
        [SerializeField] protected LocalizedString _placementConditions;
        

        public CellData cell { get; private set; }
        public BoxCollider Collider => _collider;

        public static event Action<Facility> onFacilityBuild;
        public static event Action<Facility> onFacilityDestroyed;

        public string facilityName => _facilityName.GetLocalizedString();
        public string modifierName => facilityName;

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
        }
        

        public virtual void OnAddedToCell(CellData cell)
        {
            this.cell = cell;
            cell.terrain.resourceDeck.Sub(this);
            onFacilityBuild?.Invoke(this);
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            this.cell = null;
            cell.terrain.resourceDeck.Unsub(this);
            onFacilityDestroyed?.Invoke(this);
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

        public virtual ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = _facilityName.GetLocalizedString(),
                message = _facilityDescription.GetLocalizedString() +
                          "\n" + _resourceModifications.GetLocalizedString() +
                          "\n\n" + _placementConditions.GetLocalizedString()
            };
        }

        public virtual ResourceDelta[] GetResourceDelta()
        {
            return null;
        }
    }
}