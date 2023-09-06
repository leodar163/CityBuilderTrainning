using System;
using System.Collections.Generic;
using Format;
using GridSystem;
using Localization;
using TimeSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace ResourceSystem
{
    [Serializable]
    public class ProductionLine : IResourceModifier
    {
        [SerializeField] private LocalizedString _lineName;
        [Space]
        [SerializeField] private ResourceDelta[] _resourceIn;
        [SerializeField] private ResourceDelta[] _resourceOut;
        public CellData cell { get; private set; }
        public IResourceBorrower borrower { get; private set; }
        public string modifierName => _lineName.GetLocalizedString();
        public ResourceType[] missingResources { get; private set; }
        public ResourceDelta[] resourceIn => _resourceIn;
        public ResourceDelta[] resourceOut => _resourceOut;
        public float efficiency { get; private set; }
        
        protected void EvaluateProductionEfficiency()
        {
            if (cell == null || borrower == null) return;

            efficiency = 1;
            
            List<ResourceType> missingResourcesList = new();

            foreach (var delta in _resourceIn)
            {
                ResourceSlider slider = cell.resourceDeck.GetSlider(delta.resource);

                float askedQuantity = delta.resource.borrowable ? delta.quantityDelta : delta.monthDelta;

                float availableQuantity = delta.resource.borrowable
                    ? borrower.GetBorrowedQuantity(delta.resource)
                    : Mathf.Clamp(slider.availableQuantity, 0, askedQuantity);

                if (availableQuantity < askedQuantity) missingResourcesList.Add(delta.resource);

                float availability =  availableQuantity / askedQuantity;

                if (availability < efficiency) efficiency = availability;
                
                if (delta.resource.borrowable) Debug.Log($"{borrower.borrowerName} borrowed " +
                                                         $"{borrower.GetBorrowedQuantity(delta.resource)} " +
                                                         $"{slider.resource.resourceName} and efficiency is {efficiency}, so " +
                                                         $"available quantity is {availableQuantity}");
            }

            missingResources = missingResourcesList.ToArray();
        }
        
        public ResourceDelta[] GetResourceDelta()
        {
            EvaluateProductionEfficiency();

            ResourceDelta[] deltaIn = GetResourceDeltaIn(true);
            ResourceDelta[] deltaOut = GetResourceDeltaOut();
            ResourceDelta[] deltas = new ResourceDelta[deltaIn.Length + deltaOut.Length];
            deltaIn.CopyTo(deltas,0);
            deltaOut.CopyTo(deltas, deltaIn.Length);

            return deltas;
        }

        public ResourceDelta[] GetResourceDeltaIn(bool negate = false)
        {
            List<ResourceDelta> deltas = new();
            
            foreach (var delta in _resourceIn)
            {
                deltas.Add(delta * efficiency * (negate ? -1 : 1));
            }

            return deltas.ToArray();
        }
        
        public ResourceDelta[] GetResourceDeltaOut()
        {
            List<ResourceDelta> deltas = new();
            
            foreach (var delta in _resourceOut)
            {
                deltas.Add(delta * efficiency);
            }

            return deltas.ToArray();
        }
        
        public void Init(CellData cellData, IResourceBorrower resourceBorrower)
        {
            borrower = resourceBorrower;
            cell = cellData;
        }

        #region FORMATTING

        public string GetProductionFormat(bool showEfficiency = false)
        {
            return GetProductionFormat(this, _resourceIn,_resourceOut, showEfficiency);
        }

        public static string GetProductionFormat(ProductionLine line, ResourceDelta[] In, ResourceDelta[] Out, bool showEfficiency = false)
        {
            string inFormat = FormatResourceDelta(true, In);
            inFormat = inFormat == "" ? "" : "\n" + inFormat;
            string outFormat = FormatResourceDelta(false, Out);
            outFormat = outFormat == "" ? "" : "\n" + outFormat;
            
            return $"<b><smallcaps>{line.modifierName}</smallcaps></b>{(showEfficiency ? $"\n{GetEfficiencyFormat(line)}" : "")}" +
                   $"<indent=10%>{inFormat}{outFormat}</indent>";
        }
        
        public static string GetEfficiencyFormat(ProductionLine line)
        {
            return $"{(line.efficiency < 1 ? $"<color=#{FormatManager.negativeColor}>" : "")}" +
                   $"{VariableNameManager.EfficiencyName} : {line.efficiency * 100}%" +
                   $"{(line.efficiency < 1 ? "</color>" : "")}";
        }
        
        public static string FormatResourceDelta( bool asIn, params ResourceDelta[] deltas)
        {
            string format = $"<b>{(asIn ? VariableNameManager.InName : VariableNameManager.OutName)} : </b><indent=20%>";

            string prodFormat = "";
            
            foreach (var delta in deltas)
            {
                float deltaValue = delta.resource.borrowable ? delta.quantityDelta : delta.monthDelta;
                if (deltaValue == 0) continue;

                prodFormat +=
                    $"\n{delta.resource.resourceName} : " +
                    $"{(asIn ? $"<color=#{FormatManager.negativeColor}>-{deltaValue}" : $"<color=#{FormatManager.positiveColor}>+{deltaValue}")}</color>" +
                    $"{(!delta.resource.borrowable ? $"/{TimeManager.monthName}" : "")}";
            }

            if (prodFormat == "")
                return "";
            
            format += prodFormat + "</indent>";

            return format;
        }
        
        #endregion
    }
}