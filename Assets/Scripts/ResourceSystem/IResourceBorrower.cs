using System.Collections.Generic;

namespace ResourceSystem
{
    public interface IResourceBorrower
    {
        public Dictionary<ResourceSlider, float> loaners { get; }

        public string borrowerName { get; }
        
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
            if(slider.availableQuantity <= 0) return;

            quantityToBorrow = slider.LoanQuantity(quantityToBorrow, this);
            
            if (!loaners.TryAdd(slider, quantityToBorrow))
            {
                loaners[slider] += quantityToBorrow;    
            }
        }

        public void ReturnResourceAll()
        {
            List<ResourceSlider> sliders = new List<ResourceSlider>(loaners.Keys);
            foreach (var slider in sliders)
            {
                ReturnResourceAll(slider);
            }
        }
        
        public void ReturnResourceAll(ResourceType resource)
        {
            foreach (var slider in loaners.Keys)
            {
                if (slider.resource == resource)
                    ReturnResourceAll(slider);
            }
        }
        
        public void ReturnResourceAll(ResourceSlider slider)
        {
            if (!loaners.TryGetValue(slider, out float borrowedQuantity)) return;

            ReturnResource(borrowedQuantity, slider);
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

        public float ReturnResource(float quantityToReturn, ResourceSlider slider)
        {
            if (quantityToReturn == 0) return 0;
            
            if (!loaners.TryGetValue(slider, out float borrowedQuantity)) return 0;

            if (quantityToReturn > borrowedQuantity) quantityToReturn = borrowedQuantity;
            
            loaners[slider] -= quantityToReturn;
            
            slider.RefundQuantity(quantityToReturn, this);

            if (loaners[slider] == 0)
                loaners.Remove(slider);

            return quantityToReturn;
        }
    }
}