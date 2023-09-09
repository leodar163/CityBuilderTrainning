using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Transactions
{
    public class ResourceContainer
    {
        public ResourceType resource { get; private set; }
        public ITransactor transactor { get; private set; }
        
        #region QUANTITY_PROPERTIES

        public float nativeQuantity { get; private set; }
        public float borrowedQuantity { get; private set; }
        public float lentQuantity { get; private set; }
        public float totalQuantity => nativeQuantity + borrowedQuantity;
        public float availableQuantity => totalQuantity - lentQuantity;
        public float borrowedMaxQuantity { get; private set; }
        public float nativeMaxQuantity { get; private set; }
        public float totalMaxQuantity => borrowedMaxQuantity + nativeMaxQuantity;
        public float remainingCapacity => totalMaxQuantity - totalQuantity;

        #endregion

        #region TRANSACTION_PROPERTIES

        public readonly List<ResourceTransaction> credits = new();
        public readonly List<ResourceTransaction> debts = new();

        public readonly List<ResourceTransaction> inputs = new();
        public readonly List<ResourceTransaction> outputs = new();
        
        public float deltaQuantity { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public ResourceContainer(ITransactor owner, ResourceType resourceType, float quantity = 0, float maxQuantity = Mathf.Infinity)
        {
            transactor = owner;
            resource = resourceType;
            nativeQuantity = quantity;
            nativeMaxQuantity = maxQuantity;
        }

        #endregion

        #region DIRECT_QUANTITY_MANAGEMENT_METHODS

        /// <summary>
        /// Add quantity to native quantity. Native quantity has priority over borrowed quantity,
        /// which means that if adding native quantity exceeds max quantity, container will refund its debts.
        /// Returns the quantity that could have been added.
        /// </summary>
        /// <returns>Quantity that could have been added.</returns>
        public float AddNativeQuantity(float quantity)
        {
            quantity = Mathf.Clamp(quantity, -nativeQuantity, totalMaxQuantity + remainingCapacity - nativeQuantity);

            nativeQuantity += quantity;
            
            float overQuantity = totalQuantity - totalMaxQuantity; 
            
            if (overQuantity > 0)
            {
                RefundMultiple(overQuantity);
            }
            
            if (lentQuantity > totalQuantity)
            {
                DevalueCredits(lentQuantity - totalQuantity);
            }
            
            return quantity;
        }
        
        public void SetNativeMaxQuantity(float newMaxQuantity)
        {
            nativeMaxQuantity = Mathf.Abs(newMaxQuantity);
            if (newMaxQuantity == float.PositiveInfinity) return;
            
            //The new max quantity could be lesser that the old one and total quantity might be higher than max quantity.
            //So we need to rebalance accounts so total quantity will be equal to max quantity.

            float overQuantity = totalQuantity - totalMaxQuantity;

            if (overQuantity > 0)
            {
                overQuantity -= RefundMultiple(overQuantity);
            }

            if (overQuantity > 0)
            {
                nativeQuantity -= overQuantity;
            }

            if (lentQuantity > totalQuantity)
            {
                DevalueCredits(lentQuantity - totalQuantity);
            }
        }
        
        #endregion

        #region TRANSACTION_METHODS

        public void AddOutputTransaction(ITransactor target, float quantity)
        {
            quantity = Mathf.Abs(quantity);
            if (TryGetTransaction(target, true, out ResourceTransaction transaction))
            {
                transaction.AddQuantity(quantity);
            }
            else
            {
               transaction = new ResourceTransaction(resource, transactor, target, quantity);   
               outputs.Add(transaction);
            }

            deltaQuantity -= quantity;
            
            target.NotifyInputAdding(transaction);
        }

        public void AddInputTransaction(ITransactor origin, float quantity)
        {
            quantity = Mathf.Abs(quantity);
            if (TryGetTransaction(origin, false, out ResourceTransaction transaction))
            {
                transaction.AddQuantity(quantity);
            }
            else
            {
                transaction = new ResourceTransaction(resource, origin, transactor, quantity);   
                inputs.Add(transaction);
            }

            deltaQuantity += quantity;
            
            origin.NotifyOutputAdding(transaction);
        }
        
        public void NotifyInputAdding(ResourceTransaction transaction)
        {
            if (transaction.target != transactor || transaction.resource != resource) return;

            if (!inputs.Contains(transaction))
            {
                inputs.Add(transaction);    
            }

            deltaQuantity = CalculateTransactionDelta();
        }

        public void NotifyOutputAdding(ResourceTransaction transaction)
        {
            if (transaction.origin != transactor || transaction.resource != resource) return;

            if (!outputs.Contains(transaction))
            {
                outputs.Add(transaction);
            }

            deltaQuantity = CalculateTransactionDelta();
        }

        public void AskInputs()
        {
            foreach (var input in inputs)
            {
                NotifyInputReceiving(input);
                input.origin.NotifyOutputGiving(input);
            }
        }

        public void NotifyInputReceiving(ResourceTransaction transaction)
        {
            if (inputs.Contains(transaction))
            {
                nativeQuantity += Mathf.Clamp(ProjectInputReceiving(transaction), 
                    0, transaction.origin.ProjectOutputGiving(transaction));
            }
        }

        public float ProjectInputReceiving(ResourceTransaction transaction)
        {
            if (inputs.Contains(transaction))
            {
                return Mathf.Clamp(transaction.quantity, 0, totalMaxQuantity - totalQuantity);
            }

            return 0;
        }

        public void GiveOutputs()
        {
            foreach (var output in outputs)
            {
                NotifyOutputGiving(output);
                output.target.NotifyInputReceiving(output);
            }
        }
        
        public void NotifyOutputGiving(ResourceTransaction transaction)
        {
            if (outputs.Contains(transaction))
            {
                nativeQuantity -= Mathf.Clamp(ProjectOutputGiving(transaction), 
                    0, transaction.target.ProjectInputReceiving(transaction));

                if (totalQuantity < lentQuantity)
                {
                    AskForRefund(lentQuantity - totalQuantity);
                }
            }
        }
        
        public float ProjectOutputGiving(ResourceTransaction transaction)
        {
            if (outputs.Contains(transaction))
            {
                return Mathf.Clamp(transaction.quantity, 0, nativeQuantity);
            }

            return 0;
        }
        
        #endregion
        
        #region BORROWING_METHODS
        
        /// <summary>
        /// Loans a certain quantity of resource to a debtor.
        /// Contracted Loan is ether a new loan, ether the one that already exists with this same debtor.
        /// Returns the actual quantity that could have been lent.
        /// </summary>
        /// <returns>Actual quantity that could have been lent</returns>
        public float LoanTo(ITransactor debtor, float quantityToLoan, out ResourceTransaction contractedTransaction)
        {
            quantityToLoan = Mathf.Clamp(quantityToLoan, 0, availableQuantity);
            contractedTransaction = null;

            if (quantityToLoan != 0)
            {
                contractedTransaction = AddCredit(debtor, quantityToLoan);
                lentQuantity += quantityToLoan;
            }

            return quantityToLoan;
        }

        /// <summary>
        /// Borrow an amount of resource to a creditor which cannot be the same as this container's.
        /// Returns the quantity that could have been borrowed.
        /// </summary>
        /// <returns>Quantity that could have been borrowed.</returns>
        public float BorrowTo(ITransactor creditor, float quantityToBorrow)
        {
            quantityToBorrow = creditor.LoanTo(transactor, resource, quantityToBorrow, out ResourceTransaction contractedLoan);

            if (quantityToBorrow != 0 && contractedLoan != null)
            {
                borrowedQuantity += quantityToBorrow;
                
                if(!debts.Contains(contractedLoan))
                    debts.Add(contractedLoan);
            }
            
            return quantityToBorrow;
        }

        /// <summary>
        /// Borrow all the available quantity of resource a transactor.
        /// Returns the quantity that could have been borrowed.
        /// </summary>
        /// <returns>Quantity that could have been borrowed.</returns>
        public float BorrowAllTo(ITransactor creditor)
        {
            float quantityToBorrow = creditor.LoanAllTo(transactor, resource, out ResourceTransaction contractedLoan);
            
            if (quantityToBorrow != 0 && contractedLoan != null)
            {
                borrowedQuantity += quantityToBorrow;
                
                if(!debts.Contains(contractedLoan))
                    debts.Add(contractedLoan);
            }
            
            return quantityToBorrow;
        }
        #endregion

        #region REFUNDING_METHODS

        /// <summary>
        /// Refund a certain amount of resource to a creditor.
        /// If the amount is bigger than what this container borrowed, it will be clamped.
        /// </summary>
        /// <returns>Returns quantity that have been refunded.</returns>
        public float Refund(ITransactor creditor, float quantity)
        {
            if (TryGetLoan(creditor, false, out ResourceTransaction loan))
            {
                quantity = Mathf.Clamp(quantity, 0, loan.quantity);
                
                creditor.BeRefundedBy(transactor, resource, quantity);
                if (loan.quantity == 0)
                {
                    debts.Remove(loan);
                }

                borrowedQuantity -= quantity;

                if (availableQuantity < 0)
                {
                    AskForRefund(Mathf.Abs(availableQuantity));
                }
            }

            return quantity;
        }
        
        /// <summary>
        /// Function that triggers the behavior of being refund by one of his debtor.
        /// If the debtor isn't in contact with the transactor of the container, returns 0.
        /// Else, returns the quantity that could have been refunded.
        /// </summary>
        /// <returns>Quantity that could have been refunded.</returns>
        public float BeRefundedBy(ITransactor debtor, float quantity)
        {
            if (TryGetLoan(debtor, true, out ResourceTransaction loan))
            {
                quantity = Mathf.Clamp(quantity,0, loan.quantity);
                
                loan.AddQuantity(-quantity);
                if (loan.quantity == 0)
                    credits.Remove(loan);

                lentQuantity -= quantity;
                return quantity;
            }

            return 0;
        }
        
        /// <summary>
        /// Parse all debtors to demand to refund a certain amount.
        /// Returns the quantity that could have been refunded.
        /// </summary>
        /// <returns>Quantity that could have been refunded.</returns>
        public float AskForRefund(float quantityToRefund)
        {
            quantityToRefund = Mathf.Abs(quantityToRefund);
            float remainingQuantity = quantityToRefund;
            
            foreach (var loan in credits.ToArray())
            {
                remainingQuantity -= loan.target.Refund(transactor, resource, remainingQuantity);
                if (remainingQuantity == 0) break;
            }

            return quantityToRefund - remainingQuantity;
        }

        /// <summary>
        /// Parse all creditors to refund a certain amount.
        /// Returns the quantity that could have been refunded.
        /// </summary>
        /// <returns>Quantity that could have been refunded.</returns>
        
        public float RefundMultiple(float quantityToRefund)
        {
            quantityToRefund = Mathf.Abs(quantityToRefund);
            float remainingQuantity = quantityToRefund;
            
            foreach (var loan in debts.ToArray())
            {
                remainingQuantity -= Refund(loan.origin, remainingQuantity);
                if (remainingQuantity == 0) break;
            }

            return quantityToRefund - remainingQuantity;
        }

        public void NotifyDebtDevaluation(ITransactor creditor, float devaluation)
        {
            if (TryGetLoan(creditor, false, out ResourceTransaction loan))
            {
                devaluation = Mathf.Clamp(devaluation, 0, loan.quantity);
                devaluation = Mathf.Abs(loan.AddQuantity(-devaluation));
                borrowedQuantity -= devaluation;
                if (loan.quantity == 0)
                    debts.Remove(loan);
                
                if (lentQuantity > totalQuantity)
                {
                    DevalueCredits(lentQuantity - totalQuantity);
                }
            }
        }
        
        private float DevalueCredits(float devaluation)
        {
            devaluation = Mathf.Clamp(devaluation, 0, lentQuantity);
            float remainingDevaluation = devaluation;

            List<ResourceTransaction> creditsToRemove = new();

            foreach (var credit in credits)
            {
                float creditDevaluation = Mathf.Clamp(remainingDevaluation, 0, credit.quantity);
                remainingDevaluation -= creditDevaluation;
                
                credit.target.NotifyDebtDevaluation(transactor,resource,creditDevaluation);
                
                if(credit.quantity == 0) creditsToRemove.Add(credit);
                if (remainingDevaluation == 0) break;
            }

            foreach (var credit in creditsToRemove)
            {
                credits.Remove(credit);
            }
            
            lentQuantity -= devaluation - remainingDevaluation;
            
            return devaluation - remainingDevaluation;
        }
        
        #endregion
        
        #region REGISTRY MANAGEMENT

        /// <summary>
        /// Try do add a new loan to debts or credits. If loan already exists, add quantity to it.
        /// </summary>
        /// <returns>the loan add to debts or credits, or the loan that already exists in them</returns>
        private ResourceTransaction AddCredit(ITransactor debtor, float quantity)
        {
            if (TryGetLoan(debtor, true, out ResourceTransaction loan))
            {
                loan.AddQuantity(quantity);
            }
            else
            {
                loan = new ResourceTransaction(resource, transactor, debtor, quantity);

                credits.Add(loan);
            }

            return loan;
        }
        
        /// <summary>
        /// Check if a loan already exists in credits or debts.
        /// </summary>
        private bool TryGetLoan(ITransactor otherTransactor, bool inCredits, out ResourceTransaction transactionToGet)
        {
            List<ResourceTransaction> loans = inCredits ? credits : debts;

            foreach (var loan in loans)
            {
                ITransactor loanTransactor = inCredits ? loan.target : loan.origin;

                if (loanTransactor == otherTransactor)
                {
                    transactionToGet = loan;
                    return true;
                }
            }

            transactionToGet = null;
            return false;
        }

        private bool TryGetTransaction(ITransactor otherTransactor, bool inOutputs,
            out ResourceTransaction transactionToGet)
        {
            List<ResourceTransaction> transactions = inOutputs ? outputs : inputs;

            foreach (var transaction in transactions)
            {
                ITransactor other = inOutputs ? transaction.target : transaction.origin;

                if (other == otherTransactor)
                {
                    transactionToGet = transaction;
                    return true;
                }
            }

            transactionToGet = null;
            return false;
        }

        private float CalculateTransactionDelta()
        {
            float delta = 0;
            
            foreach (var input in inputs)
            {
                delta += input.quantity;
            }

            foreach (var output in outputs)
            {
                delta -= output.quantity;
            }

            return delta;
        }
        
        #endregion
    }
}