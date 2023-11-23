using System;
using Conditions.Placement;
using Format;
using GridSystem;
using Rendering;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace BuildingSystem.Facilities
{
    [Serializable]
    public abstract class FacilityType : IBatchRendered
    {
        public Sprite icon;

        [Header("Rendering")]
        protected Vector3 _position = Vector3.zero;
        protected Quaternion _rotation = Quaternion.Euler(Vector3.zero);
        protected Vector3 _scale = Vector3.one;
        [SerializeField] protected InstanceRenderData _renderData;
        [SerializeField] protected float _scaleMultiplier = 0.3f;
        [Header("Construction")]
        public float constructionCost;
        [SerializeField] protected float _sizeRadius = 0.3f;
        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [Header("Placement Description")] 
        [SerializeField] protected PlacementCondition _placementCondition;

        public CellData cell { get; private set; }

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.IsEmpty ? "no_facility_name" : _facilityName.GetLocalizedString();
        public string facilityDesc => _facilityDescription.IsEmpty ? "no_facility_desc" : _facilityDescription.GetLocalizedString();

        public float sizeRadius => _sizeRadius * _scale.magnitude;
        
        public IBatchRendered RenderingSelf => this;
        public InstanceRenderData RenderData => _renderData;

        Vector3 IBatchRendered.Position
        {
            get => _position;
            set => _position = value;
        }

        Quaternion IBatchRendered.Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        Vector3 IBatchRendered.Scale
        {
            get => _scale;
            set => _scale = value;
        }

        #region CONSTRUCTORS

        public virtual FacilityType Copy()
        {
            return null;
        }

        #endregion
        
        public virtual void OnAddedToCell(CellData cellAddedTo)
        {
            cell = cellAddedTo;
            PlaceInCell();
            RenderingSelf.OnCreated();
            onFacilityBuild?.Invoke(this);
        }

        public virtual void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            cell = null;
            RenderingSelf.OnDestroyed();
            onFacilityDestroyed?.Invoke(this);
        }

        private void PlaceInCell()
        {
            bool isPositionValid;

            Vector3 facilityPosition;
            _rotation = quaternion.Euler(new float3(0, Random.Range(-175, 175),0));
            _scale = Vector3.one * (Random.Range(0.9f, 1.1f) * _scaleMultiplier);
            
            do
            {
                facilityPosition = cell.position + new Vector3
                {
                    x = Random.Range(-0.5f, 0.5f),
                    z = Random.Range(-0.5f, 0.5f)
                };

                isPositionValid = true;
                
                foreach (var facility in cell.facilities)
                {
                    if (facility == this ||
                        Vector3.Distance(facilityPosition, facility._position) 
                          < sizeRadius + facility.sizeRadius) continue;

                    isPositionValid = false;
                    break;
                }
                
            } while (!isPositionValid);

            _position = cell.position + facilityPosition;
        }
        
        public bool CanBePlaced(CellData cellData, out string conditionsFormat)
        {
            if (cellData.freeFacilityPlacements <= 0)
            {
                conditionsFormat = FormatManager.NoPlaceException;
                return false;
            }
            if (_placementCondition)
                return _placementCondition.CanPlace(cellData,out conditionsFormat);
            
            conditionsFormat = FormatManager.NoPlacementCondition;
            return true;
        }

        public string GetPlacementConditions()
        {
            if (_placementCondition == null)
                return FormatManager.NoPlacementCondition;
            return _placementCondition.GetNeutralPlacementFormat();
        }
    }
}