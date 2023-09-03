using System;
using System.Collections.Generic;
using Format;
using Localization;
using TimeSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace ResourceSystem
{
    [Serializable]
    public class ResourceSlider : IToolTipSpeaker
    {
        public ResourceType resource;
        [SerializeField] private float _maxQuantity;
        [FormerlySerializedAs("_quantity")] [SerializeField] private float _nativeQuantity;
        private float _borrowedQuantity;
        private List<IResourceModifier> _modifiers = new ();
        private Dictionary<IResourceBorrower, float> _loans = new();

        private float _modifierQuantity;
        private float _modifierMaxQuantity;
        
        public float availableQuantity => totalQuantity - _borrowedQuantity;

        public event Action onValuesUpdate;

        public float maxQuantity
        {
            get => _maxQuantity + _modifierMaxQuantity;
            set
            {
                _maxQuantity = Mathf.Round(value * 100) / 100;
                onValuesUpdate?.Invoke();
            }
        }

        public float totalQuantity => _nativeQuantity + Mathf.Clamp(_modifierQuantity, 0, maxQuantity - nativeQuantity);
        
        public float nativeQuantity
        {
            get => _nativeQuantity;
            set
            {
                _nativeQuantity = Mathf.Round(Mathf.Clamp(value, 0, maxQuantity) * 100) / 100;
                if (resource.borrowable && _borrowedQuantity > _nativeQuantity)
                    AskRefundAll(_borrowedQuantity - _nativeQuantity);
                onValuesUpdate?.Invoke();
            }
        }

        #region CONSTRUCTORS

        public ResourceSlider(ResourceSlider template)
        {
            resource = template.resource;
            maxQuantity = template.maxQuantity;
            nativeQuantity = template.nativeQuantity;
        }
        
        public ResourceSlider(ResourceType resource,  float maxQuantity)
        {
            this.resource = resource;
            this.maxQuantity = maxQuantity;
        }

        public ResourceSlider(string resourceId, float maxQuantity)
        {
            resource = ResourceSet.Default.GetResource(resourceId);
            this.maxQuantity = maxQuantity;
        }

        #endregion

        #region RESOURCE_DELTA_MANAGEMENT

        public float GetNextMonthResourceDelta()
        {
            float delta = 0;

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    continue;
                
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource)
                    {
                        delta += resourceDelta.monthDelta;
                    }
                }
            }

            return Mathf.Round(delta * 100) / 100;
        }

        public float GetMaxQuantityFromModifiers()
        {
            float modifiersMaxQuantity = 0;

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    continue;
                
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource)
                    {
                        modifiersMaxQuantity += resourceDelta.maxQuantityDelta;
                    }
                }
            }

            return modifiersMaxQuantity;
        }

        

        public float GetQuantityFromModifiers()
        {
            float modifiersQuantity = 0;

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    continue;
                
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource)
                    {
                        modifiersQuantity += resourceDelta.quantityDelta;
                    }
                }
            }

            return modifiersQuantity;
        }
        
        public void ApplyMonthDelta()
        {
            nativeQuantity += GetNextMonthResourceDelta();
        }

        private void UpdateModifiersQuantities()
        {
            if (_maxQuantity != float.PositiveInfinity) 
                _modifierMaxQuantity = GetMaxQuantityFromModifiers();
            _modifierQuantity = GetQuantityFromModifiers();
            onValuesUpdate?.Invoke();
        }
        
        #endregion

        #region MODIFIER_SUBSCRIPTION

        public void Sub(IResourceModifier modifier)
        {
            if (!_modifiers.Contains(modifier))
            {
                _modifiers.Add(modifier);
                UpdateModifiersQuantities();
            }
        }

        public void Unsub(IResourceModifier modifier)
        {
            if (_modifiers.Contains(modifier))
            {
                _modifiers.Remove(modifier);
                UpdateModifiersQuantities();
            }
        }

        #endregion

        #region TOOLTIP
        
        public float GetNextMonthResourceDelta(out string message, bool collapse = true)
        {
            float delta = 0;

            message = "";

            Dictionary<string, float> modifs = new();
            Dictionary<string, float> modifsNbr = new();

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if (resourceDeltas == null)
                    continue;
                
            
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource && resourceDelta.monthDelta != 0)
                    {
                        if (collapse)
                        {
                            modifs.TryAdd(modifier.modifierName, 0);
                            modifsNbr.TryAdd(modifier.modifierName, 0);
                            modifs[modifier.modifierName] += resourceDelta.monthDelta;
                            modifsNbr[modifier.modifierName] ++;
                        }
                        
                        delta += resourceDelta.monthDelta;
                        if (!collapse)
                        {
                            string deltaColor = ColorUtility.ToHtmlStringRGBA(resourceDelta.monthDelta == 0 
                                ? Color.white
                                : resourceDelta.monthDelta > 0
                                    ? Color.green
                                    : Color.red);
                            message +=
                                $"{modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.monthDelta}</color>" +
                                $"/{TimeManager.monthName}\n";
                        }
                    }
                }
            }

            if(collapse)
            {
                foreach (var modif in modifs.Keys)
                {
                    string deltaColor = ColorUtility.ToHtmlStringRGBA(modifs[modif] == 0 ? Color.white :
                        modifs[modif] > 0 ? Color.green : Color.red);
                    message += $"{modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color>" +
                               $"/{TimeManager.monthName}\n";
                }
            }
            return Mathf.Round(delta * 100) / 100;
        }
        
        public string GetQuantityModifierFormat(bool collapse = true)
        {
            string message = "";

            Dictionary<string, float> modifs = new();
            Dictionary<string, float> modifsNbr = new();

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if (resourceDeltas == null)
                    continue;

                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource && resourceDelta.quantityDelta != 0)
                    {
                        if (collapse)
                        {
                            modifs.TryAdd(modifier.modifierName, 0);
                            modifs[modifier.modifierName] += resourceDelta.quantityDelta;
                            modifsNbr.TryAdd(modifier.modifierName, 0);
                            modifsNbr[modifier.modifierName] ++;
                        }

                        if (!collapse)
                        {
                            string deltaColor = ColorUtility.ToHtmlStringRGBA(resourceDelta.quantityDelta == 0 
                                ? Color.white
                                : resourceDelta.quantityDelta > 0
                                    ? Color.green
                                    : Color.red);
                            message +=
                                $"{modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.quantityDelta}</color>\n";
                        }
                    }
                }
            }
            if(collapse)
            {
                foreach (var modif in modifs.Keys)
                {
                    string deltaColor = ColorUtility.ToHtmlStringRGBA(modifs[modif] == 0 ? Color.white :
                        modifs[modif] > 0 ? Color.green : Color.red);
                    message += $"{modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color>\n";
                }
            }

            return message;
        }

        public string GetMaxQuantityModifierFormat(bool collapse = true)
        {
            string message = "";

            Dictionary<string, float> modifs = new();
            Dictionary<string, float> modifsNbr = new();

            foreach (var modifier in _modifiers)
            {
                ResourceDelta[] resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    continue;
                
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource && resourceDelta.maxQuantityDelta != 0)
                    {
                        if (collapse)
                        {
                            modifs.TryAdd(modifier.modifierName, 0);
                            modifs[modifier.modifierName] += resourceDelta.maxQuantityDelta;
                            modifsNbr.TryAdd(modifier.modifierName, 0);
                            modifsNbr[modifier.modifierName] ++;
                        }

                        if (!collapse)
                        {
                            string deltaColor = ColorUtility.ToHtmlStringRGBA(resourceDelta.maxQuantityDelta == 0 
                                ? Color.white
                                : resourceDelta.maxQuantityDelta > 0
                                    ? Color.green
                                    : Color.red);
                            message +=
                                $"{modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.maxQuantityDelta}</color> " +
                                $"{VariableNameManager.maxQuantityName}";
                        }
                    }
                }
            }

            if(collapse)
            {
                foreach (var modif in modifs.Keys)
                {
                    string deltaColor = ColorUtility.ToHtmlStringRGBA(modifs[modif] == 0 ? Color.white :
                        modifs[modif] > 0 ? Color.green : Color.red);
                    message += $"{modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color> " +
                               $"{VariableNameManager.maxQuantityName}";
                }
            }
            
            return message;
        }

        public string GetLoanFormat(bool collapse = true)
        {
            string message = "";

            Dictionary<string, float> borrowers = new();
            Dictionary<string, float> borrowersNbr = new();
            string colorRed = ColorUtility.ToHtmlStringRGBA(Color.red);
            
            foreach (var borrower in _loans.Keys)
            {
                if(_loans[borrower] == 0)
                    continue;
                
                if (collapse)
                {
                    borrowers.TryAdd(borrower.borrowerName, 0);
                    borrowers[borrower.borrowerName] += _loans[borrower];
                    borrowersNbr.TryAdd(borrower.borrowerName, 0);
                    borrowersNbr[borrower.borrowerName]++;
                }
                else
                {
                    message += $"{borrower.borrowerName} : <color=#{colorRed}>-{_loans[borrower]}</color>\n";
                }
            }

            if (collapse)
            {
                foreach (var borrowerName in borrowers.Keys)
                {
                    message +=
                        $"{borrowerName}({borrowersNbr[borrowerName]}) : <color=#{colorRed}>-{borrowers[borrowerName]}</color>\n";
                }
            }
            
            return message;
        }
        
        public ToolTipMessage ToToolTipMessage()
        {
            string quantityMessage = GetQuantityModifierFormat(false);
            quantityMessage =
                $"<b>{VariableNameManager.quantityName} : {totalQuantity}</b>\n" +
                $"<indent=10%>{VariableNameManager.baseName} : {_nativeQuantity}\n{quantityMessage}</indent>";

            if (resource.borrowable)
            {
                string loanMessage = GetLoanFormat(false);
                loanMessage =
                    $"<b>{VariableNameManager.availableQuantityName} : {availableQuantity}</b>\n<indent=10%>{loanMessage}</indent>";
                
                return new ToolTipMessage
                {
                    title = resource.resourceName,
                    message = $"{quantityMessage}{FormatManager.separator}\n{loanMessage}"
                };
            }
            else
            {
                float monthDelta = GetNextMonthResourceDelta(out string monthDeltaMessage);
                string monthDeltaColor =
                    ColorUtility.ToHtmlStringRGBA(monthDelta == 0 ? Color.white : monthDelta > 0 ? Color.green : Color.red);
                monthDeltaMessage = monthDeltaMessage == "" ? "" :
                    $"<b>{TimeManager.previsionName} : <color=#{monthDeltaColor}>{(monthDelta > 0 ? "+" : "")}" +
                    $"{monthDelta}</color>/{TimeManager.monthName}</b>\n<indent=10%>{monthDeltaMessage}</indent>";

                string maxQuantityMessage = "";
                
                if (_maxQuantity != float.PositiveInfinity)
                {
                    maxQuantityMessage = GetMaxQuantityModifierFormat();
                    maxQuantityMessage =
                        $"<b>{VariableNameManager.maxQuantityName} : {maxQuantity}</b>\n" +
                        $"<indent=10%>{VariableNameManager.baseName} : {_maxQuantity}\n{maxQuantityMessage}</indent>";
                }
            
                return new ToolTipMessage
                {
                    title = resource.resourceName,
                    message = $"{quantityMessage}" +
                              $"{(monthDeltaMessage != "" ? $"{FormatManager.separator}\n{monthDeltaMessage}" : "")}" +
                              $"{(maxQuantityMessage != "" ? $"{FormatManager.separator}\n{maxQuantityMessage}" : "")}" 
                };
            }
        }

        #endregion

        #region LOANING

        public float LoanQuantity(float quantityToBorrow, IResourceBorrower borrower)
        {
            if (!resource.borrowable) return 0;

            if (quantityToBorrow > availableQuantity) quantityToBorrow = availableQuantity;

            if (!_loans.TryAdd(borrower, quantityToBorrow))
            {
                _loans[borrower] += quantityToBorrow;
            }

            //Debug.Log($"{borrower.borrowerName} borrow {quantityToBorrow} {resource.resourceName}" );
            _borrowedQuantity += quantityToBorrow;

            UpdateModifiersQuantities();
            
            return quantityToBorrow;
        }

        public void RefundQuantity(float quantityToRefund, IResourceBorrower borrower)
        {
            if (!resource.borrowable || !_loans.TryGetValue(borrower, out float borrowedQuantity)) return;

            if (quantityToRefund > borrowedQuantity) quantityToRefund = borrowedQuantity;

            _borrowedQuantity -= quantityToRefund;
            _loans[borrower] -= quantityToRefund;

            if (_loans[borrower] <= 0)
            {
                _loans.Remove(borrower);
                //Debug.Log(borrower.borrowerName + " is removed from loans");
            }
            
            UpdateModifiersQuantities();
        }

        private void AskRefundAll(float quantityToRefund)
        {
            List<IResourceBorrower> borrowers = new List<IResourceBorrower>(_loans.Keys);
                
            foreach (var borrower in borrowers)
            {
                float quantityReturnable = quantityToRefund;
                if (quantityReturnable > _loans[borrower]) quantityReturnable = _loans[borrower];
                
                borrower.ReturnResource(quantityReturnable, this);
                quantityToRefund -= quantityReturnable;

                if (_loans[borrower] <= 0) _loans.Remove(borrower);
            }
            
            UpdateModifiersQuantities();
        }

        #endregion
    }
}