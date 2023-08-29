using System.Collections.Generic;

namespace ResourceSystem
{
    public interface IResourceBorrower
    {
        public Dictionary<ResourceSlider, float> borrowedResources { get; }

        public void BorrowResource(float quantity, ResourceType resource);


        public void ReleaseResource(float quantityToRelease, ResourceType resource);

        public virtual void BorrowResource(float quantityToBorrow, ResourceSlider slider)
        {
            borrowedResources.TryAdd(slider, 0);
            borrowedResources[slider] = slider.BorrowQuantity(quantityToBorrow, this);
        }
        
        public virtual void ReleaseResource(float quantityToRelease, ResourceSlider slider)
        {
            if (!borrowedResources.TryGetValue(slider, out float borrowedQuantity)) return;

            quantityToRelease = quantityToRelease > borrowedQuantity ? borrowedQuantity : quantityToRelease;
            borrowedResources[slider] -= quantityToRelease;
            
            slider.ReturnQuantity(quantityToRelease, this);

            if (borrowedResources[slider] == 0)
                borrowedResources.Remove(slider);
        }
    }
}