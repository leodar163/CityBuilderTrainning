using System;
using System.Collections.Generic;
using Format;
using GridSystem;
using Localization;
using ResourceSystem;
using ResourceSystem.Productions;
using ResourceSystem.Transactions;
using TerrainSystem;
using TimeSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class FacilityType : MonoBehaviour , IToolTipSpeaker, ITransactor, IProducer
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;


        [Header("Description")]
        [SerializeField] protected LocalizedString _facilityName;
        [SerializeField] protected LocalizedString _facilityDescription;
        [SerializeField] protected LocalizedString _resourceModifications;
        [SerializeField] protected LocalizedString _placementConditions;
        [SerializeField] private List<ProductionLine> _productionLines;

        private Action<InGameDate> monthlyProduction;

        public CellData cell { get; private set; }
        public BoxCollider Collider => _collider;

        public static event Action<FacilityType> onFacilityBuild;
        public static event Action<FacilityType> onFacilityDestroyed;

        public string facilityName => _facilityName.GetLocalizedString();
        public string modifierName => facilityName;

        List<ResourceContainer> ITransactor.registry { get; } = new();

        public ITransactor transactorSelf => this;
        public ITransactor transactor => this;
        public IProducer producerSelf => this;

        List<ProductionLine> IProducer.productionLines => _productionLines;

        List<ITransactor> IProducer.creditors { get; } = new();

        List<ITransactor> IProducer.debtors { get; } = new();

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
        }

        protected virtual void Awake()
        {
            monthlyProduction = _ => producerSelf.Produce();
        }

        protected virtual void OnEnable()
        {
            TimeManager.onNewMonth += monthlyProduction;
        }

        protected virtual void OnDisable()
        {
            TimeManager.onNewMonth -= monthlyProduction;
            transactorSelf.ReleaseAll();
        }

        public virtual void OnAddedToCell(CellData cellAddedTo)
        {
            cell = cellAddedTo;
            producerSelf.InitTransactor();
            producerSelf.AddCreditor(cellAddedTo);
            producerSelf.AddDebtor(cellAddedTo);
            onFacilityBuild?.Invoke(this);
        }

        public virtual void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            cell = null;
            onFacilityDestroyed?.Invoke(this);
            Destroy(gameObject);
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
                          $"\n{FormatManager.separator}\n{(cell == null ? FormatProductionLines() : FormatProductionLinesDelta())}" + 
                          $"\n{FormatManager.separator}\n{_placementConditions.GetLocalizedString()}"
            };
        }
        #endregion
        

        #region PRODUCTION

        public virtual ResourceDelta[] GetResourceDelta()
        {
            ResourceDelta[] deltas = Array.Empty<ResourceDelta>();
            
            return deltas;
        }
        
        

        #endregion
    }
}