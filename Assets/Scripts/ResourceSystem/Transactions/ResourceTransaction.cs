using UnityEngine;

namespace ResourceSystem.Transactions
{
    public class ResourceTransaction
    {
        public ResourceType resource { get; private set; }
        public ITransactor origin { get; private set; }
        public ITransactor target { get; private set; }
        
        public float quantity { get; private set; }

        #region CONSTRUCTORS

        public ResourceTransaction(ResourceType resource, ITransactor origin, ITransactor target, float quantity)
        {
            this.resource = resource;
            this.origin = origin;
            this.target = target;
            this.quantity = quantity;
        }

        #endregion
        
        public float AddQuantity(float quantityToAdd)
        {
            quantityToAdd = Mathf.Clamp(quantityToAdd, -quantity, quantityToAdd);
            quantity += quantityToAdd;

            return quantityToAdd;
            //notify debtor that quantity has changed ?
        }
    }
}