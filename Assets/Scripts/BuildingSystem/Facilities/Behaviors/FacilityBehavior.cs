using GridSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities.Behaviors
{
    public abstract class FacilityBehavior : ScriptableObject
    {
        [SerializeField] protected LocalizedString _behaviorName;
        [SerializeField] protected LocalizedString _format;
        [SerializeField] protected LocalizedString _desc;

        public string BehaviorName => _behaviorName.GetLocalizedString();

        public virtual void OnInit(FacilityType facility)
        {
            
        }
        
        public virtual void OnUpdate(FacilityType facility)
        {
            
        }

        public virtual void OnRemoved(FacilityType facility)
        {
            
        }

        public virtual void OnAddedToCell(FacilityType facility, CellData cell)
        {
            
        }

        public virtual void OnRemovedFromCell(FacilityType facility, CellData cell)
        {
            
        }

        public virtual string Format(FacilityType facility)
        {
            return _format.IsEmpty ? "no_format" : _format.GetLocalizedString();
        }

        public virtual string Description(FacilityType facility)
        {
            return _format.IsEmpty ? "no_format" : _format.GetLocalizedString();
        }
    }
}