using GridSystem;
using ResourceSystem;
using UnityEngine;
using TerrainData = TerrainSystem.TerrainData;

namespace BuildingSystem.Facilities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Facility : MonoBehaviour, IResourceUpdater, ICellModifier
    {
        [SerializeField] private BoxCollider _collider;
        public Sprite icon;
        
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
            
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            
        }

        public virtual bool CanBePlaced(TerrainData terrain)
        {
            return true;
        }
    }
}