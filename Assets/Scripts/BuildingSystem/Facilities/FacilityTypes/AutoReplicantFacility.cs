using System;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BuildingSystem.Facilities.FacilityTypes
{
    [Serializable]
    public class AutoReplicantFacility : ProducerFacility
    {
        [SerializeField] [Range(0,1)] private float _givenToCloneHealth;
            
        protected override void OnHealthHitsMax()
        {
            base.OnHealthHitsMax();
            Replicate();
        }

        private void Replicate()
        {
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
                    FacilityType clone = Copy();
                    clone.health = health * _givenToCloneHealth;
                    health -= clone.health;
                    
                    if (neighbours[alea].TryAddFacility(clone)) return;
                }
                
                neighbours.RemoveAt(alea);
                
            } while (neighbours.Count > 0);
        }

        #region CONSTRUCTORS

        public AutoReplicantFacility(AutoReplicantFacility template) : base(template)
        {
            _givenToCloneHealth = template._givenToCloneHealth;
        }
        
        public override FacilityType Copy()
        {
            return new AutoReplicantFacility(this);
        }

        #endregion
    }
}