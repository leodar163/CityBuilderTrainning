using GridSystem;
using ResourceSystem;
using TerrainSystem;
using UnityEngine;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Facility : MonoBehaviour, IResourceUpdater, ICellModifier
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;
        public CellData cell { get; private set; }
        
        public BoxCollider Collider => _collider;

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
        }

        public virtual void OnUpdateResources(ResourceDeck resources)
        {
            
        }

        public virtual void OnAddedToCell(CellData cell)
        {
            this.cell = cell;
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            this.cell = null;
        }

        public virtual bool CanBePlaced(TerrainType terrain)
        {
            return true;
        }
    }
}