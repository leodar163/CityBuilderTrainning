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

        public readonly List<ResourceLoan> credits = new();
        public readonly List<ResourceLoan> debts = new();

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
        
        #region BORROWING_METHODS
        
        /// <summary>
        /// Loans a certain quantity of resource to a debtor.
        /// Contracted Loan is ether a new loan, ether the one that already exists with this same debtor.
        /// Returns the actual quantity that could have been lent.
        /// </summary>
        /// <returns>Actual quantity that could have been lent</returns>
        public float LoanTo(ITransactor debtor, float quantityToLoan, out ResourceLoan contractedLoan)
        {
            quantityToLoan = Mathf.Clamp(quantityToLoan, 0, availableQuantity);
            contractedLoan = null;

            if (quantityToLoan != 0)
            {
                contractedLoan = AddCredit(debtor, quantityToLoan);
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
            quantityToBorrow = creditor.LoanTo(transactor, resource, quantityToBorrow, out ResourceLoan contractedLoan);

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
            float quantityToBorrow = creditor.LoanAllTo(transactor, resource, out ResourceLoan contractedLoan);
            
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
            if (TryGetLoan(creditor, false, out ResourceLoan loan))
            {
                quantity = Mathf.Clamp(quantity, 0, loan.lentQuantity);
                
                creditor.BeRefundedBy(transactor, resource, quantity);
                if (loan.lentQuantity == 0)
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
            if (TryGetLoan(debtor, true, out ResourceLoan loan))
            {
                quantity = Mathf.Clamp(quantity,0, loan.lentQuantity);
                
                loan.AddLentQuantity(-quantity);
                if (loan.lentQuantity == 0)
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
                remainingQuantity -= loan.debtor.Refund(transactor, resource, remainingQuantity);
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
                remainingQuantity -= Refund(loan.creditor, remainingQuantity);
                if (remainingQuantity == 0) break;
            }

            return quantityToRefund - remainingQuantity;
        }

        public void NotifyDebtDevaluation(ITransactor creditor, float devaluation)
        {
            if (TryGetLoan(creditor, false, out ResourceLoan loan))
            {
                devaluation = Mathf.Clamp(devaluation, 0, loan.lentQuantity);
                devaluation = Mathf.Abs(loan.AddLentQuantity(-devaluation));
                borrowedQuantity -= devaluation;
                if (loan.lentQuantity == 0)
                    debts.Remove(loan);
            }
        }
        
        private float DevalueCredits(float devaluation)
        {
            devaluation = Mathf.Clamp(devaluation, 0, lentQuantity);
            float remainingDevaluation = devaluation;

            List<ResourceLoan> creditsToRemove = new();

            foreach (var credit in credits)
            {
                float creditDevaluation = Mathf.Clamp(remainingDevaluation, 0, credit.lentQuantity);
                remainingDevaluation -= creditDevaluation;
                
                credit.debtor.NotifyDebtDevaluation(transactor,resource,creditDevaluation);
                
                if(credit.lentQuantity == 0) creditsToRemove.Add(credit);
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
        private ResourceLoan AddCredit(ITransactor debtor, float quantity)
        {
            if (TryGetLoan(debtor, true, out ResourceLoan loan))
            {
                loan.AddLentQuantity(quantity);
            }
            else
            {
                loan = new ResourceLoan(resource, transactor, debtor, quantity);

                credits.Add(loan);
            }

            return loan;
        }
        
        /// <summary>
        /// Check if a loan already exists in credits or debts.
        /// </summary>
        private bool TryGetLoan(ITransactor otherTransactor, bool inCredits, out ResourceLoan loanToGet)
        {
            List<ResourceLoan> loans = inCredits ? credits : debts;

            foreach (var loan in loans)
            {
                ITransactor loanTransactor = inCredits ? loan.debtor : loan.creditor;

                if (loanTransactor == otherTransactor)
                {
                    loanToGet = loan;
                    return true;
                }
            }

            loanToGet = null;
            return false;
        }

        #endregion
    }
}