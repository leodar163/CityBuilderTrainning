using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using TimeSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class AutoReplicantFacility : ProducerFacility
    {
        private List<ResourceQuantity> _needs;
        [Tooltip("how many time a market have to have a needed resource to reproduce")]
        [Min(0)] public float relativeExtraToReproduce = 2;

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            _needs = producerSelf.GetNeeds();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TimeManager.onMonthBegins += Replicate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            TimeManager.onNewMonth -= Replicate;
        }

        private void Replicate()
        {
            foreach (var quantity in _needs)
            {
                if (market.GetResourceExtraAmount(quantity.resource) < quantity.quantity * relativeExtraToReproduce)
                    return;
            }

            if (CanBePlaced(cell, out _) && Instantiate(gameObject).TryGetComponent(out FacilityType clone))
            {
                if (!cell.TryAddFacility(clone))
                {
                    Destroy(clone.gameObject);
                }
                else return;
            }
            
            foreach (var neighbour in cell.neighbours)
            {
                if (CanBePlaced(neighbour, out _) && Instantiate(gameObject).TryGetComponent(out clone))
                {
                    neighbour.TryAddFacility(clone);
                    return;
                }
            }
        }
    }
}