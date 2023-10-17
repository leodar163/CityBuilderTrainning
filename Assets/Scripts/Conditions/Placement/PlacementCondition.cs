using GridSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace Conditions.Placement
{
    public abstract class PlacementCondition : ScriptableObject
    {
    [SerializeField] protected LocalizedString _localizedCondition;
    [Space]
    public bool inverseCondition;
    
    public virtual bool CanPlace(CellData cell, out string format)
    {
        format = _localizedCondition.GetLocalizedString();
        return true;
    }
    }
}