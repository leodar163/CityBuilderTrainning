﻿using System;
using System.Collections.Generic;
using ResourceSystem.Scriptables;
using UnityEngine;

namespace ResourceSystem.Transactions
{
    public interface ITransactor
    {
        protected List<ResourceContainer> registry { get; }
        public ITransactor transactorSelf { get; }

        #region DIRECT_RESOURCE_MANAGEMENT

        /// <summary>
        /// Add a container of a certain resource to the transactor.
        /// If a container with this resource exists already, the quantity will be added to it instead of create a new container.
        /// In the same spirit, the max quantity will be the biggest between the one in argument and the one of the already existing container.
        /// <returns>Returns the quantity that could be added to the container.
        /// or the delta between argument max quantity and argument quantity if quantity is higher than max quantity </returns>
        /// </summary>
        public float AddResource(ResourceType resource, float quantity, float maxQuantity)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.SetNativeMaxQuantity(maxQuantity);
                return container.AddNativeQuantity(quantity);
            }
            
            quantity = Mathf.Clamp(Mathf.Abs(quantity), quantity, Mathf.Abs(maxQuantity));
            
            registry.Add(new ResourceContainer(this, resource, quantity, maxQuantity));

            return quantity;
        }
        
        /// <summary>
        /// Add a container of a certain resource to the transactor.
        /// If a container with this resource exists already, the quantity will be added to it instead of create a new container.
        /// <returns>Returns the quantity that could be added to the container.</returns>
        /// </summary>
        public float AddResource(ResourceType resource, float quantity)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
                
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.AddNativeQuantity(quantity);
            }

            registry.Add(new ResourceContainer(this, resource, quantity));

            return quantity;
        }

        public float SetResource(ResourceType resource, float quantity, float maxQuantity)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.SetNativeMaxQuantity(maxQuantity);
                return container.SetNativeQuantity(quantity);
            }
            
            float quantityDelta = Mathf.Clamp(quantity - maxQuantity, 0, maxQuantity);
            
            registry.Add(new ResourceContainer(this, resource, quantity - quantityDelta, maxQuantity));

            return quantity;
        }
        
        public float SetResource(ResourceType resource, float quantity)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.SetNativeQuantity(quantity);
            }
            
            registry.Add(new ResourceContainer(this, resource, quantity));

            return quantity;
        }

        #region ASSESSORS

            public float GetAvailableQuantity(ResourceType resource)
            {
                if (TryGetContainer(resource, out ResourceContainer container))
                {
                    return container.availableQuantity;
                }

                return -1;
            }

            public float GetTotalQuantity(ResourceType resource)
            {
                if (TryGetContainer(resource, out ResourceContainer container))
                {
                    return container.totalQuantity;
                }

                return -1;
            }
            
            public float GetRemainingCapacity(ResourceType resource)
            {
                if (TryGetContainer(resource, out ResourceContainer container))
                {
                    return container.remainingCapacity;
                }

                return -1;
            }

            public float GetRemainingDeltaCapacity(ResourceType resource)
            {
                if (TryGetContainer(resource, out ResourceContainer container))
                {
                    return container.remainingDeltaCapacity;
                }

                return -1;
            }
            
            #endregion

        #endregion

        #region TRANSACTION_METHODS

        /// <summary>
        /// Set output amount to target at quantity value for this resource.
        /// Returns quantity that could be set to the transaction. -1 if there is no container of this resource.
        /// </summary>
        /// <returns>Quantity that could be set to the transaction. -1 if there is no container of this resource.</returns>
        public float SetOutputTransaction(ITransactor target, ResourceType resource, float quantity)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.SetOutputTransaction(target, quantity);
            }

            return -1;
        }

        /// <summary>
        /// Remove transaction of resource from this transactor to target, if exists.
        /// </summary>
        public void RemoveOutputTransaction(ITransactor target, ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.SetOutputTransaction(target, 0);
            }
        }

        public void RemoveOutputsAll()
        {
            foreach (var container in registry)
            {
                foreach (var output in container.outputs.ToArray())
                {
                    container.SetOutputTransaction(output.target, 0);
                }
            }
        }
        
        /// <summary>
        /// Set input amount from origin at quantity value for this resource.
        /// Returns quantity that could be set to the transaction. -1 if there is no container of this resource.
        /// </summary>
        /// <returns>Quantity that could be set to the transaction. -1 if there is no container of this resource.</returns>
        public float SetInputTransaction(ITransactor origin, ResourceType resource, float quantity)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.SetInputTransaction(origin, quantity);
            }

            return -1;
        }

        /// <summary>
        /// Remove transaction of resource from origin to this transactor, if exists.
        /// </summary>
        public void RemoveInputTransaction(ITransactor origin, ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.SetInputTransaction(origin, 0);
            }
        }
        
        public void RemoveInputsAll()
        {
            foreach (var container in registry)
            {
                foreach (var input in container.inputs.ToArray())
                {
                    container.SetInputTransaction(input.origin, 0);
                }
            }
        }
        
        public void AskInputs()
        {
            foreach (var container in registry)
            {
                container.AskInputs();
            }
        }

        public void AskInputs(ResourceType resource)
        {
            foreach (var container in registry)
            {
                container.AskInputs();
            }
        }
        
        public void GiveOutputs()
        {
            foreach (var container in registry)
            {
                container.GiveOutputs();
            }
        }

        public void GiveOutputs(ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.GiveOutputs();
            }
        }
        
        #endregion

        #region LOANING_METHODS

        #region CREDITOR_METHODS

        /// <summary>
        /// Loans a certain quantity of resource to a debtor.
        /// If a loan already exists, it replaces it. 
        /// Returns the actual quantity that could have been lent.
        /// </summary>
        /// <returns>Actual quantity that could be lent</returns>
        public float LoanTo(ITransactor debtor, ResourceType resourceToLoan, float quantityToLoan)
        {
            if (TryGetContainer(resourceToLoan, out ResourceContainer container))
            {
                return container.LoanTo(debtor, quantityToLoan);
            }

            return 0;
        }

        public float LoanAllTo(ITransactor debtor, ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.LoanTo(debtor, container.availableQuantity);
            }
            
            return 0;
        }

        public void RemoveLoan(ResourceType resource, ITransactor debtor)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.RemoveCredit(debtor);
            }
        }
        
        #endregion
        
        
        #region DEBTOR_METHODS
        
        public float BorrowTo(ITransactor creditor, ResourceType resourceToBorrow, float quantityToBorrow)
        {
            if (creditor == this || quantityToBorrow == 0) return -1;
            
            if (TryGetContainer(resourceToBorrow, out ResourceContainer container))
            {
                return container.BorrowTo(creditor, quantityToBorrow);
            }

            return -1;
        }

        public void BorrowAllTo(ITransactor creditor, ResourceType resourceToBorrow)
        {
            if (creditor == this) return;
            
            if (TryGetContainer(resourceToBorrow, out ResourceContainer container))
            {
                container.BorrowAllTo(creditor);
            }
        }

        public void RemoveBorrow(ResourceType resource, ITransactor creditor)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                container.RemoveDebt(creditor);
            }
        }
        
        #endregion
        

        #region REFUND_METHODS

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

        public void ReleaseAll()
        {
            foreach (var container in registry)
            {
                container.AskForRefund(container.lentQuantity);
                container.RefundMultiple(container.borrowedQuantity);
                RemoveInputsAll();
                RemoveOutputsAll();
            }
        }
        
        #endregion

        #endregion
        
        #region PROMISES_METHODS

        public bool TryPromiseTo(ITransactor target, ResourceType resource, float quantity)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                return container.TryPromiseTo(target, quantity);
            }

            return false;
        }

        public void AskAllPromises(bool consumePromises = false)
        {
            foreach (var container in registry)
            {
                container.AskForAllPromises(consumePromises);
            }
        }

        public void AskAllPromisesFrom(ITransactor origin, bool consumePromises = false)
        {
            foreach (var container in registry)
            {
                container.AskPromiseFrom(origin, consumePromises);
            }
        }
        
        public void GiveAllPromises(bool consumePromises = false)
        {
            foreach (var container in registry)
            {
                container.GiveAllPromises(consumePromises);
            }
        }

        public void GiveAllPromisesTo(ITransactor target, bool consumePromises = false)
        {
            foreach (var container in registry)
            {
                container.GivePromiseTo(target);
            }
        }
        
        #endregion
        
        #region REGISTRY_MANAGEMENT

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
        
        public void InitContainers(ResourceDeck template)
        {
            foreach (var containerTemplate in template.containers)
            {
                SetResource(containerTemplate.resource, containerTemplate.nativeQuantity,
                    containerTemplate.nativeMaxQuantity);
            }
        }

        public void RemoveContainer(ResourceType resource)
        {
            if (TryGetContainer(resource, out ResourceContainer container))
            {
                registry.Remove(container);
            }
        }

        public void RemoveContainersAll()
        {
            foreach (var container in registry.ToArray())
            {
                registry.Remove(container);
            }
        }
        
        #endregion
       
    }
}