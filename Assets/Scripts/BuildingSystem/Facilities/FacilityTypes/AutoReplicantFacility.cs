using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BuildingSystem.Facilities.FacilityTypes
{
    [Serializable]
    public class AutoReplicantFacility : ProducerFacility
    {
        private List<ResourceQuantity> _needs;
        [Tooltip("how many time a market have to have a needed resource to reproduce")]
        [Min(0)] public float relativeExtraToReproduce = 2;

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            _needs = producerSelf.GetNeeds();
            TimeManager.onMonthBegins += Replicate;
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            base.OnRemovedFromCell(cellRemovedFrom);
            TimeManager.onNewMonth -= Replicate;
        }

        private void Replicate()
        {
            foreach (var quantity in _needs)
            {
                if (market.GetResourceExtraAmount(quantity.resource) < quantity.quantity * relativeExtraToReproduce)
                    return;
            }

            if (CanBePlaced(cell, out _))
            {
                if(cell.TryAddFacility(Copy())) return;
            }

            List<CellData> neighbours = new List<CellData>(cell.neighbours);
            do
            {
                int alea = Random.Range(0, neighbours.Count);

                if (CanBePlaced(neighbours[alea], out _))
                {
                    if (neighbours[alea].TryAddFacility(Copy())) return;
                }
                
                neighbours.RemoveAt(alea);
                
            } while (neighbours.Count > 0);
        }

        #region CONSTRUCTORS

        public AutoReplicantFacility(AutoReplicantFacility template) : base(template)
        {
            relativeExtraToReproduce = template.relativeExtraToReproduce;
        }
        
        public override FacilityType Copy()
        {
            return new AutoReplicantFacility(this);
        }

        #endregion
    }
}