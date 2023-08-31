using System.Collections.Generic;

namespace ResourceSystem
{
    public interface IResourceBorrower
    {
        public Dictionary<ResourceSlider, float> loaners { get; }
        public IResourceBorrower selfBorrower { get; }
        
        
        public float GetBorrowedQuantity(ResourceType resource)
        {
            float quantity = 0;

            foreach (var pair in loaners)
            {
                if (pair.Key.resource == resource)
                {
                    quantity += pair.Value;
                }
            }

            return quantity;
        }

        public void BorrowResource(float quantityToBorrow, ResourceSlider slider)
        {
            loaners.TryAdd(slider, 0);
            loaners[slider] = slider.LoanQuantity(quantityToBorrow, this);
        }

        public void ReturnResourceAll(ResourceType resource)
        {
            foreach (var slider in loaners.Keys)
            {
                if (slider.resource == resource)
                    ReturnResourceAll(slider);
            }
        }
        
        public void ReturnResource(float quantityToReturn, ResourceType resource)
        {
            if (quantityToReturn == 0) return;
            
            foreach (var slider in loaners.Keys)
            {
                if (slider.resource == resource)
                {
                    quantityToReturn -= ReturnResource(quantityToReturn, slider);
                    if(quantityToReturn == 0) return;
                }
            }
        }

        public void ReturnResourceAll(ResourceSlider slider)
        {
            if (!loaners.TryGetValue(slider, out float borrowedQuantity)) return;

            ReturnResource(borrowedQuantity, slider);
        }
        
        public float ReturnResource(float quantityToReturn, ResourceSlider slider)
        {
            if (quantityToReturn == 0) return 0;
            
            if (!loaners.TryGetValue(slider, out float borrowedQuantity)) return 0;

            quantityToReturn = quantityToReturn > borrowedQuantity ? borrowedQuantity : quantityToReturn;
            loaners[slider] -= quantityToReturn;
            
            slider.RefundQuantity(quantityToReturn, this);

            if (loaners[slider] == 0)
                loaners.Remove(slider);

            return quantityToReturn;
        }
    }
}