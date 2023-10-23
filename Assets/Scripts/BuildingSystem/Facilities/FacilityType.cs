using System;
using Conditions.Placement;
using GridSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class FacilityType : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;

        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [Header("Placement Description")] 
        [SerializeField] private PlacementCondition _placementCondition;
        [SerializeField] private LocalizedString _localizeNoPlacementCondition;
        
        public CellData cell { get; private set; }
        public BoxCollider Collider => _collider;

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.IsEmpty ? "no_facility_name" : _facilityName.GetLocalizedString();
        public string facilityDesc => _facilityDescription.IsEmpty ? "no_facility_desc" : _facilityDescription.GetLocalizedString();

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
            if (_placementCondition)
                return _placementCondition.CanPlace(cellData,out conditionsFormat);
            
            conditionsFormat = _localizeNoPlacementCondition.GetLocalizedString();
            return true;
        }

        public string GetPlacementConditions()
        {
            if (_placementCondition == null)
                return _localizeNoPlacementCondition.GetLocalizedString();
            return _placementCondition.GetNeutralPlacementFormat();
        }
    }
}