using System;
using GridSystem;
using ResourceSystem;
using UnityEngine;

namespace BuildingSystem.Facilities
{
    [Serializable]
    public class Facility : IResourceUpdater, ICellModifier
    {
        public virtual void OnUpdateResources(ResourceDeck resources)
        {
            
        }

        public virtual void OnAddedToCell(CellData cell)
        {
            
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            
        }
    }
}