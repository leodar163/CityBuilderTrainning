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

        public virtual bool CanPlace(CellData cell, out string format, int indentLevel = 0)
        {
            format = _localizedCondition.GetLocalizedString();
            return true;
        }

        public virtual string GetNeutralPlacementFormat(int indentLevel = 0)
        {
            return $"<indent={indentLevel}em>" + _localizedCondition.GetLocalizedString();
        }
    }
}