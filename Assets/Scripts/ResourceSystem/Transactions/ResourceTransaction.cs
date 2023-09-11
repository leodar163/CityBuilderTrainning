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
        
        /// <summary>
        /// Add quantityToAdd to quantity.
        /// Returns quantity that could be added.
        /// </summary>
        /// <returns>Quantity that could be added.</returns>
        public float AddQuantity(float quantityToAdd)
        {
            quantityToAdd = Mathf.Clamp(quantityToAdd, -quantity, quantityToAdd);
            
            quantity += quantityToAdd;

            return quantityToAdd;
        }

        public float SetQuantity(float quantityToSet)
        {
            quantityToSet = Mathf.Abs(quantityToSet);

            quantity = quantityToSet;

            return quantity;
        }
    }
}