using System;
using System.Collections.Generic;
using BuildingSystem.Facilities.Behaviors;
using BuildingSystem.Facilities.HealthManagement;
using Conditions.Placement;
using Format;
using GridSystem;
using Rendering;
using ResourceSystem;
using ResourceSystem.Markets;
using TimeSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace BuildingSystem.Facilities
{
    [Serializable]
    public class FacilityType : IBatchRendered
    {
        public Sprite icon;

        [Header("Rendering")]
        protected Vector3 _position = Vector3.zero;
        protected Quaternion _rotation = Quaternion.Euler(Vector3.zero);
        protected Vector3 _scale = Vector3.one;
        [SerializeField] protected InstanceRenderData _renderData;
        [SerializeField] protected float _scaleMultiplier = 0.3f;
        [Header("Growth")]
        [SerializeField] protected float _naturalGrowth;
        [SerializeField] protected float _maxHealth;
        public float health;
        [SerializeField] protected float _growthFromNeeds;
        [SerializeField] protected List<ResourceType> _resourceNeeds;
        protected List<FacilityNeed> _needs;
        protected float _growth;
        [Header("Construction")]
        public float constructionCost;
        [SerializeField] protected float _sizeRadius = 0.3f;
        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [Header("Placement Description")] 
        [SerializeField] protected PlacementCondition _placementCondition;
        [Header("Behavior")] 
        [SerializeField] protected List<FacilityBehavior> _behaviors;

        protected FacilityType _template;
        
        public CellData cell { get; private set; }

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.IsEmpty ? "no_facility_name" : _facilityName.GetLocalizedString();
        public string facilityDesc => _facilityDescription.IsEmpty ? "no_facility_desc" : _facilityDescription.GetLocalizedString();

        public float sizeRadius => _sizeRadius * _scale.magnitude;

        private readonly List<IGrowthEffector> _growthEffectors = new();

        public List<IGrowthEffector> GrowthEffectors => _growthEffectors;

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

        public List<ResourceType> Needs => _resourceNeeds;

        
        public float MaxHealth => _maxHealth;
        
        public float Growth
        {
            get
            {
                CalculateGrowth();
                return _growth;
            }
        }

        #region GROWTH

        public void AddGrowthEffector(IGrowthEffector effector)
        {
            if (_growthEffectors.Contains(effector)) return;
            _growthEffectors.Add(effector);
            effector.OnDestroyed += RemoveGrowthEffector;
        }

        public void RemoveAllGrowthEffector()
        {
            foreach (var effector in _growthEffectors)
            {
                RemoveGrowthEffector(effector);
            }
        }
        
        public void RemoveGrowthEffector(IGrowthEffector effector)
        {
            if (!_growthEffectors.Contains(effector)) return;
            _growthEffectors.Remove(effector);
            effector.OnDestroyed -= RemoveGrowthEffector;
        }

        private void CalculateGrowth()
        {
            _growth = 0;

            foreach (var need in _needs)
            {
                need.Growth = CalculateGrowthFromNeed(need.resource);
            }

            foreach (var effector in _growthEffectors)
            {
                _growth += effector.Growth;
            }
        }

        public float CalculateGrowthFromNeed(ResourceType resource)
        {
            if (cell.market == null) return 0;

            return cell.market.GetResourceAvailabilityState(resource) switch
            {
                ResourceAvailability.Shortage => -_growthFromNeeds * 2,
                ResourceAvailability.Missing => -_growthFromNeeds,
                ResourceAvailability.Average => 0,
                ResourceAvailability.InExcess => _growthFromNeeds,
                ResourceAvailability.InAbundance => _growthFromNeeds * 2,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ApplyGrowth()
        {
            health = Mathf.Clamp(health + Growth, 0, _maxHealth);
            if (health <= 0)
            {
                OnHealthHitsZero();
            }
            else if (health >= _maxHealth)
            {
                OnHealthHitsMax();
            }
        }

        private void InitNeeds()
        {
            _needs = new List<FacilityNeed>();
            
            foreach (var need in _resourceNeeds)
            {
                FacilityNeed facilityNeed = new FacilityNeed { resource = need };
                _needs.Add(facilityNeed);
                AddGrowthEffector(facilityNeed);
            }
        }

        private void InitNaturalGrowth()
        {
            NaturalGrowth naturalGrowth = new NaturalGrowth { Growth = _naturalGrowth };
            AddGrowthEffector(naturalGrowth);
        }
        
        protected virtual void OnHealthHitsZero()
        {
            
        }

        protected virtual void OnHealthHitsMax()
        {
            
        }

        #endregion

        #region BEHAVIORS

        private void InitBehaviors()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnInit(this);
            }
        }
        
        private void UpdateBehaviors()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnUpdate(this);
            }
        }

        public bool TryAddBehavior(FacilityBehavior behavior)
        {
            if (_behaviors.Contains(behavior)) return false;

            _behaviors.Add(behavior);
            behavior.OnInit(this);
            
            return true;
        }

        public bool TryRemoveBehavior(FacilityBehavior behavior)
        {
            if (!_behaviors.Remove(behavior)) return false;
            
            behavior.OnRemoved(this);
            return true;
        }

        public void RemoveAllBehaviors()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.OnRemoved(this);
            }
            _behaviors.Clear();
        }
        
        #endregion
        
        #region CONSTRUCTORS

        protected FacilityType(FacilityType template)
        {
            _template = template._template ?? template;
            
            _renderData = template._renderData;
            _scaleMultiplier = template._scaleMultiplier;
            
            _facilityName = template._facilityName;
            _facilityDescription = template._facilityDescription;
            
            _placementCondition = template._placementCondition;
            constructionCost = template.constructionCost;
            _sizeRadius = template._sizeRadius;

            _naturalGrowth = template._naturalGrowth;
            _growthFromNeeds = template._growthFromNeeds;
            health = template.health;
            _maxHealth = template._maxHealth;
            _resourceNeeds = new List<ResourceType>(template._resourceNeeds);
            
            InitNaturalGrowth();
            InitNeeds();
        }

        public virtual FacilityType Copy()
        {
            return new FacilityType(this);
        }

        #endregion

        #region CELL_PLACEMENT

        public virtual void OnAddedToCell(CellData cellAddedTo)
        {
            cell = cellAddedTo;
            PlaceInCell();
            RenderingSelf.OnCreated();
            onFacilityBuild?.Invoke(this);
            TimeManager.onNewMonth += ApplyGrowth;
        }

        public virtual void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            cell = null;
            RenderingSelf.OnDestroyed();
            RemoveAllGrowthEffector();
            onFacilityDestroyed?.Invoke(this);
            TimeManager.onNewMonth -= ApplyGrowth;
        }

        private void PlaceInCell()
        {
            bool isPositionValid;

            Vector3 facilityPosition;
            _rotation = quaternion.Euler(new float3(0, Random.Range(-175, 175),0));
            _scale = Vector3.one * (Random.Range(0.9f, 1.1f) * _scaleMultiplier);

            int safetyLoopCount = 100;
            
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
                    if (facility == this) continue;
                    if (Vector3.Distance(facilityPosition, facility._position)
                        < sizeRadius + facility.sizeRadius)
                    {
                        isPositionValid = false;
                        break;
                    }
                }

                safetyLoopCount--;
                
            } while (!isPositionValid && safetyLoopCount > 0);

            _position = facilityPosition;
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

        #endregion
        

        #region OPPERATIONS

        public static bool AreSameType(FacilityType a, FacilityType b)
        {
            return (a?._template ?? a) == (b?._template ?? b);
        }

        public bool IsSameType(FacilityType other)
        {
            return AreSameType(this, other);
        }
        
        #endregion
    }
}