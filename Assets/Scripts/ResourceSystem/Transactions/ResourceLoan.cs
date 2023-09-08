using UnityEngine;

namespace ResourceSystem.Transactions
{
    public class ResourceLoan
    {
        public ResourceType resource { get; private set; }
        public ITransactor creditor { get; private set; }
        public ITransactor debtor { get; private set; }
        
        public float lentQuantity { get; private set; }

        #region CONSTRUCTORS

        public ResourceLoan(ResourceType resource, ITransactor creditor, ITransactor debtor, float lentQuantity)
        {
            this.resource = resource;
            this.creditor = creditor;
            this.debtor = debtor;
            this.lentQuantity = lentQuantity;
        }

        #endregion
        
        public float AddLentQuantity(float quantityToAdd)
        {
            quantityToAdd = Mathf.Clamp(quantityToAdd, -lentQuantity, quantityToAdd);
            lentQuantity += quantityToAdd;

            return quantityToAdd;
            //notify debtor that quantity has changed ?
        }
    }
}