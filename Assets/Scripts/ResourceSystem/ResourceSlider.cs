﻿using System;
using System.Collections.Generic;
using GridSystem.Localization;
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
        private readonly Dictionary<IResourceBorrower, float> _borrowers = new();

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

        public float GetMaxQuantityFromModifiers(out string message, bool collapse = true)
        {
            float modifiersMaxQuantity = 0;
            
            message = "";

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
                            
                        modifiersMaxQuantity += resourceDelta.maxQuantityDelta;
                        
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
            
            return modifiersMaxQuantity;
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

        public float GetQuantityFromModifiers(out string message, bool collapse = true)
        {
            float modifiersQuantity = 0;
            
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
                    if (resourceDelta.resource == resource && resourceDelta.quantityDelta != 0)
                    {
                        if (collapse)
                        {
                            modifs.TryAdd(modifier.modifierName, 0);
                            modifs[modifier.modifierName] += resourceDelta.quantityDelta;
                            modifsNbr.TryAdd(modifier.modifierName, 0);
                            modifsNbr[modifier.modifierName] ++;
                        }
                        
                        modifiersQuantity += resourceDelta.quantityDelta;
                        
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

            return modifiersQuantity;
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

        public ToolTipMessage ToToolTipMessage()
        {
            float monthDelta = GetNextMonthResourceDelta(out string monthDeltaMessage);
            string monthDeltaColor =
                ColorUtility.ToHtmlStringRGBA(monthDelta == 0 ? Color.white : monthDelta > 0 ? Color.green : Color.red);
            monthDeltaMessage = monthDeltaMessage == "" ? "" :
                $"<b>{TimeManager.previsionName} : <color=#{monthDeltaColor}>{(monthDelta > 0 ? "+" : "")}" +
                $"{monthDelta}</color>/{TimeManager.monthName}</b>\n{monthDeltaMessage}";

            GetQuantityFromModifiers(out string quantityMessage);
            quantityMessage =
                $"<b>{VariableManager.quantityName} : {totalQuantity}</b>\n   {VariableManager.baseName} : {_nativeQuantity}\n{quantityMessage}";

            string maxQuantityMessage = "";
            if (_maxQuantity != float.PositiveInfinity)
            {
                GetMaxQuantityFromModifiers(out maxQuantityMessage);
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

        #endregion

        #region LOANING

        public float LoanQuantity(float quantityToBorrow, IResourceBorrower borrower)
        {
            if (!resource.borrowable) return 0;
            
            float quantityBorrowable = availableQuantity < quantityToBorrow ? availableQuantity : quantityToBorrow;

            _borrowers.TryAdd(borrower, 0);

            _borrowers[borrower] += quantityBorrowable;
            _borrowedQuantity += quantityBorrowable;

            return quantityBorrowable;
        }

        public void RefundQuantity(float QuantityToReturn, IResourceBorrower borrower)
        {
            if (!resource.borrowable || !_borrowers.TryGetValue(borrower, out float borrowedQuantity)) return;

            QuantityToReturn = borrowedQuantity < QuantityToReturn ? borrowedQuantity : QuantityToReturn;

            _borrowedQuantity -= QuantityToReturn;
            _borrowers[borrower] -= QuantityToReturn;

            if (_borrowers[borrower] == 0)
            {
                _borrowers.Remove(borrower);
            }
        }

        private void CallBorrowersForRefund(float quantityToReturn)
        {
            foreach (var pair in _borrowers)
            {
                var quantityReturnable = pair.Value < quantityToReturn ? pair.Value : quantityToReturn;
                pair.Key.ReturnResource(quantityReturnable, this);
                quantityToReturn -= quantityReturnable;
            }
        }

        #endregion
    }
}