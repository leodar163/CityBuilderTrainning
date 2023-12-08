using System;
using System.Collections.Generic;
using Localization;
using UnityEngine;

namespace Variable
{
    [Serializable]
    public class SubscribableFloat
    {
        [SerializeField] private Sprite _variableIcon;
        public Sprite VariableIcon => _variableIcon;
        
        [SerializeField] private AutoLocalizedString _variableName;
        public AutoLocalizedString VariableName
        {
            get
            {
                if (!_changePipeInit)
                {
                    _variableName.onStringChanged += _ => onVariableChanged?.Invoke(this);
                    _changePipeInit = true;
                }
                return _variableName;
            }
        }
        private bool _changePipeInit; 
        
        private float _baseValue;
        
        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                onVariableChanged?.Invoke(this);
            }
        }
        
        public float Value => GetValue();

        private float _percent;
        private float _mult;
        private float _add;

        public readonly List<IVariableModifier> modifiers = new();

        public event Action<SubscribableFloat> onVariableChanged;
        
        public void AddModifier(IVariableModifier modifier)
        {
            if (modifiers.Contains(modifier)) return;
            modifiers.Add(modifier);

            _add += modifier.Add;
            _mult += modifier.Mult;
            _percent += modifier.Percent;

            onVariableChanged?.Invoke(this);
        }

        public void RemoveModifier(IVariableModifier modifier)
        {
            if (!modifiers.Remove(modifier)) return;

            _add -= modifier.Add;
            _mult -= modifier.Mult;
            _percent -= modifier.Percent;
            
            onVariableChanged?.Invoke(this);
        }
        
        public float GetValue()
        {
            float value = _baseValue;
            value += _add;
            value += value * _percent / 100;
            value *= _mult;
            return value;
        }
    }
}