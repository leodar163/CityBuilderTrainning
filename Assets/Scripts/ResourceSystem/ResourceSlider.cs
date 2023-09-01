using System;
using System.Collections.Generic;
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
        private float _borrowedQuantity = 0;
        private List<IResourceModifier> _modifiers = new ();
        private readonly Dictionary<IResourceBorrower, float> _loans = new();

        private float _modifierQuantity;
        private float _modifierMaxQuantity;
        
        public float availableQuantity => _nativeQuantity - _borrowedQuantity;

        public float maxQuantity
        {
            get => _maxQuantity + _modifierMaxQuantity;
            set => _maxQuantity = Mathf.Round(value * 100) / 100;
        }

        public float totalQuantity => _nativeQuantity + Mathf.Clamp(_modifierQuantity, 0, maxQuantity - nativeQuantity);
        
        public float nativeQuantity
        {
            get => _nativeQuantity;
            set
            {
                _nativeQuantity = Mathf.Round(Mathf.Clamp(value, 0, maxQuantity) * 100) / 100;
                if (resource.borrowable && _borrowedQuantity > _nativeQuantity)
                    CallBorrowersForRefund(_borrowedQuantity - _nativeQuantity);
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
            if (_maxQuantity != float.PositiveInfinity) _modifierMaxQuantity = GetMaxQuantityFromModifiers();
            _nativeQuantity += GetNextMonthResourceDelta();
            _modifierQuantity = GetQuantityFromModifiers();
        }

        #endregion

        #region MODIFIER_SUBSCRIPTION

        public void Sub(IResourceModifier modifier)
        {
            if(!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        public void Unsub(IResourceModifier modifier)
        {
            if (_modifiers.Contains(modifier))
                _modifiers.Remove(modifier);
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
                                $"   {modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.monthDelta}</color>" +
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
                    message += $"   {modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color>" +
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
                                $"   {modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.quantityDelta}</color>\n";
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
                    message += $"   {modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color>\n";
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
                                $"   {modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.maxQuantityDelta}</color> " +
                                $"{VariableManager.maxQuantityName}";
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
                    message += $"   {modif}({modifsNbr[modif]}) : <color=#{deltaColor}>{modifs[modif]}</color> " +
                               $"{VariableManager.maxQuantityName}";
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
                if (collapse)
                {
                    borrowers.TryAdd(borrower.borrowerName, 0);
                    borrowers[borrower.borrowerName] += _loans[borrower];
                    borrowersNbr.TryAdd(borrower.borrowerName, 0);
                    borrowersNbr[borrower.borrowerName]++;
                }
                else
                {
                    message += $"   {borrower.borrowerName} : <color=#{colorRed}>-{_loans[borrower]}</color>\n";
                }
            }

            if (collapse)
            {
                foreach (var borrowerName in borrowers.Keys)
                {
                    message +=
                        $"   {borrowerName}({borrowersNbr[borrowerName]}) : <color=#{colorRed}>-{borrowers[borrowerName]}</color>\n";
                }
            }
            
            return message;
        }
        
        public ToolTipMessage ToToolTipMessage()
        {
            string quantityMessage = GetQuantityModifierFormat();
            quantityMessage =
                $"<b>{VariableManager.quantityName} : {totalQuantity}</b>\n   {VariableManager.baseName} : {_nativeQuantity}\n{quantityMessage}";

            string loanMessage = GetLoanFormat();
            loanMessage =
                $"<b>{VariableManager.availableQuantityName} : {availableQuantity}</b>\n{loanMessage}";
                
            if (resource.borrowable)
            {
                
                return new ToolTipMessage
                {
                    title = resource.resourceName,
                    message = $"{quantityMessage}\n========\n{loanMessage}"
                };
            }
            else
            {
                float monthDelta = GetNextMonthResourceDelta(out string monthDeltaMessage);
                string monthDeltaColor =
                    ColorUtility.ToHtmlStringRGBA(monthDelta == 0 ? Color.white : monthDelta > 0 ? Color.green : Color.red);
                monthDeltaMessage = monthDeltaMessage == "" ? "" :
                    $"<b>{TimeManager.previsionName} : <color=#{monthDeltaColor}>{(monthDelta > 0 ? "+" : "")}" +
                    $"{monthDelta}</color>/{TimeManager.monthName}</b>\n{monthDeltaMessage}";

                string maxQuantityMessage = "";
                
                if (_maxQuantity != float.PositiveInfinity)
                {
                    maxQuantityMessage = GetMaxQuantityModifierFormat();
                    maxQuantityMessage =
                        $"<b>{VariableManager.maxQuantityName} : {maxQuantity}</b>\n   {VariableManager.baseName} : {_maxQuantity}\n{maxQuantityMessage}";
                }
            
                return new ToolTipMessage
                {
                    title = resource.resourceName,
                    message = $"{quantityMessage}" +
                              $"{(monthDeltaMessage != "" ? $"\n========\n{monthDeltaMessage}" :"")}" +
                              $"{(maxQuantityMessage != "" ? $"\n========\n{maxQuantityMessage}" : "")}" 
                };
            }
        }

        #endregion

        #region LOANING

        public float LoanQuantity(float quantityToBorrow, IResourceBorrower borrower)
        {
            if (!resource.borrowable) return 0;
            
            float quantityBorrowable = availableQuantity < quantityToBorrow ? availableQuantity : quantityToBorrow;

            _loans.TryAdd(borrower, 0);

            _loans[borrower] += quantityBorrowable;
            _borrowedQuantity += quantityBorrowable;

            return quantityBorrowable;
        }

        public void RefundQuantity(float QuantityToReturn, IResourceBorrower borrower)
        {
            if (!resource.borrowable || !_loans.TryGetValue(borrower, out float borrowedQuantity)) return;

            QuantityToReturn = borrowedQuantity < QuantityToReturn ? borrowedQuantity : QuantityToReturn;

            _borrowedQuantity -= QuantityToReturn;
            _loans[borrower] -= QuantityToReturn;

            if (_loans[borrower] == 0)
            {
                _loans.Remove(borrower);
            }
        }

        private void CallBorrowersForRefund(float quantityToReturn)
        {
            foreach (var pair in _loans)
            {
                var quantityReturnable = pair.Value < quantityToReturn ? pair.Value : quantityToReturn;
                pair.Key.ReturnResource(quantityReturnable, this);
                quantityToReturn -= quantityReturnable;
            }
        }

        #endregion
    }
}