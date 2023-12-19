using System;
using BuildingSystem.Facilities.FacilityTypes;
using GridSystem;


namespace BuildingSystem.Facilities
{
    [Serializable]
    public class ConstructionSite : ProducerFacility
    {
        private FacilityType _facilityToBuild;
        public FacilityType facilityToBuild => _facilityToBuild;

        public override string EconomicActorName => $"{base.EconomicActorName} ({facilityToBuild.facilityName})";

        #region CONSTRUCTORS

        public ConstructionSite(ConstructionSite template) : base(template)
        {
            _facilityToBuild = template._facilityToBuild;
        }
        
        public override FacilityType Copy()
        {
            return new ConstructionSite(this);
        }

        #endregion

        protected override void OnHealthHitsMax()
        {
            base.OnHealthHitsMax();
            BuildFacility();
        }

        private void BuildFacility()
        {
            CellData currentCell = cell;
            _facilityToBuild = _facilityToBuild.Copy();
            cell.maxFacilityCount++;
            cell.TryAddFacility(facilityToBuild);
            _facilityToBuild.RenderingSelf.SetTransform(_position, _rotation, _facilityToBuild.RenderingSelf.Scale);
            cell.RemoveFacility(this);
            currentCell.maxFacilityCount--;
        }

        public void SetFacilityToBuild(FacilityType facility)
        {
            if (facility == null || facility == facilityToBuild) return;
            _facilityToBuild = facility;
            _placementType = facility.PlacementType;
            constructionCost = facility.constructionCost;
            _maxHealth = constructionCost;
        }
    }
}