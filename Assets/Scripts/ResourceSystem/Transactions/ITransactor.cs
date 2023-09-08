using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Transactions
{
    public interface ITransactor
    {
        public List<ResourceContainer> registry { get; }
        public ITransactor transactorSelf { get; }

        #region DIRECT_RESOURCE_MANAGEMENT

        /// <summary>
        /// Add a container of a certain resource to the transactor.
        /// If a container with this resource exists already, the quantity will be added to it instead of create a new container.
        /// In the same spirit, the max quantity will be the biggest between the one in argument and the one of the already existing container.
        /// The function returns the quantity that couldn't have been added to the potentially existing container, or the delta between argument max quantity and argument quantity if quantity is higher than max quantity 
        /// </summary>
        public float AddResource(ResourceType resource, float quantity, float maxQuantity = float.PositiveInfinity)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
                
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.SetNativeMaxQuantity(maxQuantity);
                return container.AddNativeQuantity(quantity);
            }

            float quantityDelta = Mathf.Clamp(quantity - maxQuantity, 0, maxQuantity);
            
            registry.Add(new ResourceContainer(this, resource, quantity - quantityDelta, maxQuantity));

            return quantityDelta;
        }

        #endregion
        
        public float LoanTo(ITransactor debtor, ResourceType resourceToLoan, float quantityToLoan, out ResourceLoan contractedLoan)
        {
            if (TryGetContainer(resourceToLoan, out ResourceContainer container))
            {
                return container.LoanTo(debtor, quantityToLoan, out contractedLoan);
            }

            contractedLoan = null;
            return 0;
        }

        public float LoanAllTo(ITransactor debtor, ResourceType resource, out ResourceLoan contractedLoan)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.LoanTo(debtor, container.availableQuantity, out contractedLoan);
            }

            contractedLoan = null;
            return 0;
        }
        
        public void BorrowTo(ITransactor creditor, ResourceType resourceToBorrow, float quantityToBorrow)
        {
            if (creditor == this || quantityToBorrow == 0) return;
            
            if (TryGetContainer(resourceToBorrow, out ResourceContainer container))
            {
                container.BorrowTo(creditor, quantityToBorrow);
            }
        }

        public void BorrowAllTo(ITransactor creditor, ResourceType resourceToBorrow)
        {
            if (creditor == this) return;
            
            if (TryGetContainer(resourceToBorrow, out ResourceContainer container))
            {
                container.BorrowAllTo(creditor);
            }
        }

        public float Refund(ITransactor creditor, ResourceType resource, float quantityRefunded)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.Refund(creditor, quantityRefunded);
            }

            return 0;
        }

        public float RefundAll(ITransactor creditor, ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.Refund(creditor, container.borrowedQuantity);
            }

            return 0;
        }

        public void BeRefundedBy(ITransactor debtor, ResourceType resource, float quantityRefunded)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.BeRefundedBy(debtor, quantityRefunded);
            }
        }
        
        public void NotifyDebtDevaluation(ITransactor creditor, ResourceType resource, float devaluation)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.NotifyDebtDevaluation(creditor, devaluation);
            }
        }

        #region REGISTRY_ASSESSION

        public bool TryGetContainer(ResourceType resourceType, out ResourceContainer containerToGet)
        {
            foreach (var container in registry)
            {
                if (container.resource == resourceType)
                {
                    containerToGet = container;
                    return true;
                }
            }

            containerToGet = null;
            return false;
        }

        #endregion
       
    }
}