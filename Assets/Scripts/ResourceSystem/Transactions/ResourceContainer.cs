using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Transactions
{
    public class ResourceContainer
    {
        public ResourceType resource { get; private set; }
        public ITransactor transactor { get; private set; }
        
        #region QUANTITY_PROPERTIES

        private float _nativeQuantity;
        private float _borrowedQuantity;
        private float _lentQuantity;
        private float _nativeMaxQuantity;
        private float _borrowedMaxQuantity;

        public float nativeQuantity => _nativeQuantity;
        public float borrowedQuantity => _borrowedQuantity;
        public float lentQuantity => _lentQuantity;
        public float totalQuantity => _nativeQuantity + _borrowedQuantity;
        public float availableQuantity => totalQuantity - _lentQuantity;
        public float nativeMaxQuantity => _nativeMaxQuantity;
        public float borrowedMaxQuantity => _borrowedMaxQuantity;
        public float totalMaxQuantity => _borrowedMaxQuantity + _nativeMaxQuantity;
        public float remainingCapacity => totalMaxQuantity - totalQuantity;

        #endregion

        #region TRANSACTION_PROPERTIES

        public readonly List<ResourceTransaction> credits = new();
        public readonly List<ResourceTransaction> debts = new();

        public readonly List<ResourceTransaction> inputs = new();
        public readonly List<ResourceTransaction> outputs = new();

        private float _deltaQuantity;
        public float deltaQuantity => _deltaQuantity;
        public float remainingDeltaCapacity => remainingCapacity - _deltaQuantity;

        #endregion

        #region CONSTRUCTORS

        public ResourceContainer(ITransactor owner, ResourceType resourceType, float quantity = 0, float maxQuantity = Mathf.Infinity)
        {
            transactor = owner;
            resource = resourceType;
            _nativeQuantity = quantity;
            _nativeMaxQuantity = maxQuantity;
        }

        #endregion

        #region DIRECT_QUANTITY_MANAGEMENT_METHODS

        /// <summary>
        /// Set native quantity. Native quantity has priority over borrowed quantity,
        /// which means that if adding native quantity exceeds max quantity, container will refund its debts.
        /// Returns the quantity that could have been set.
        /// </summary>
        /// <returns>Quantity that could have been set.</returns>
        public float SetNativeQuantity(float quantity)
        {
            quantity = Mathf.Clamp(quantity, 0, totalMaxQuantity);

            _nativeQuantity = quantity;
            
            float overQuantity = totalQuantity - totalMaxQuantity; 
            
            if (overQuantity > 0)
            {
                RefundMultiple(overQuantity);
            }
            
            if (lentQuantity > totalQuantity)
            {
                DevalueCredits(lentQuantity - totalQuantity);
            }

            RemoveInputsInExcess();
            
            return quantity;
        }

        /// <summary>
        /// Add quantity to native quantity. Native quantity has priority over borrowed quantity,
        /// which means that if adding native quantity exceeds max quantity, container will refund its debts.
        /// Returns the quantity that could have been added.
        /// </summary>
        /// <returns>Quantity that could have been added.</returns>
        public float AddNativeQuantity(float quantity)
        {
            quantity = Mathf.Clamp(quantity, -nativeQuantity, totalQuantity + remainingCapacity - nativeQuantity);

            SetNativeQuantity(nativeQuantity + quantity);

            return quantity;
        }
        
        public void SetNativeMaxQuantity(float newMaxQuantity)
        {
            _nativeMaxQuantity = Mathf.Abs(newMaxQuantity);
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
                _nativeQuantity -= overQuantity;
            }

            if (lentQuantity > totalQuantity)
            {
                DevalueCredits(lentQuantity - totalQuantity);
            }
        }
        
        #endregion

        #region TRANSACTION_METHODS

            #region OUTPUT_METHODS

            /// <summary>
            /// Set output amount to target at quantity value.
            /// If there's not yet any transaction to target, creates such transaction.
            /// Returns quantity that could be set to the transaction.
            /// </summary>
            /// <returns>Quantity that could be set to the transaction.</returns>
            public float SetOutputTransaction(ITransactor target, float quantity)
            {
                quantity = Mathf.Clamp(Mathf.Abs(quantity), 0, nativeQuantity);
                quantity = Mathf.Clamp(quantity, 0, target.GetRemainingCapacity(resource));

                if (TryGetTransaction(target, true, out ResourceTransaction transaction))
                {
                    _deltaQuantity += transaction.quantity;
                    
                    if (quantity == 0)
                    {
                        outputs.Remove(transaction);
                    }
                    else
                    {
                        transaction.SetQuantity(quantity);   
                    }
                }
                else if (quantity != 0)
                {
                    transaction = new ResourceTransaction(resource, transactor, target, quantity);   
                    outputs.Add(transaction);
                }

                _deltaQuantity -= quantity;

                if (target.TryGetContainer(resource, out ResourceContainer container))
                {
                    container.NotifyInputChange(transaction);
                }

                return quantity;
            }
            
            public void GiveOutputs()
            {
                foreach (var output in outputs.ToArray())
                {
                    GiveOutput(output);
                }
            }

            private void NotifyOutputChange(ResourceTransaction output)
            {
                if (output.resource != resource || output.origin != transactor) return;

                if (!outputs.Contains(output))
                {
                    outputs.Add(output);
                }
                
                if (output.quantity == 0)
                {
                    outputs.Remove(output);
                }
                
                _deltaQuantity = CalculateTransactionDelta();
            }

            private void GiveOutput(ResourceTransaction output)
            {
                if (output.origin == transactor && outputs.Contains(output))
                {
                    RemoveOutputInExcess(output);

                    if (output.quantity <= 0)
                    {
                        outputs.Remove(output);
                        return;
                    }
                    
                    _nativeQuantity -= Mathf.Clamp(output.quantity, 0, _nativeQuantity);

                    if (output.target.TryGetContainer(resource, out ResourceContainer target))
                    {
                        target.NotifyInputReceiving(output);
                    }

                    if (totalQuantity < lentQuantity)
                    {
                        AskForRefund(lentQuantity - totalQuantity);
                    }
                }
            }
            
            #endregion

            #region INPUT_METHODS

            /// <summary>
            /// Set input amount from origin at quantity value.
            /// If there's not yet any transaction from origin, creates such transaction.
            /// Returns quantity that could be set to the transaction.
            /// </summary>
            /// <returns>Quantity that could be set to the transaction.</returns>
            public float SetInputTransaction(ITransactor origin, float quantity)
            {
                return origin.SetOutputTransaction(transactor, resource, quantity);
            }
        
            private void NotifyInputChange(ResourceTransaction transaction)
            {
                if (transaction.target != transactor || transaction.resource != resource) return;

                if (!inputs.Contains(transaction))
                {
                    inputs.Add(transaction);    
                }

                if (transaction.quantity <= 0)
                {
                    inputs.Remove(transaction);
                }

                _deltaQuantity = CalculateTransactionDelta();
            }

            public void AskInputs()
            {
                foreach (var input in inputs)
                {
                    if (input.origin.TryGetContainer(resource, out ResourceContainer container))
                    {
                        container.GiveOutput(input);
                    }
                }
            }

            private void NotifyInputReceiving(ResourceTransaction transaction)
            {
                if (inputs.Contains(transaction))
                {
                    _nativeQuantity += Mathf.Clamp(transaction.quantity, 0, remainingCapacity);
                }
            }

            #endregion

        #endregion
        
        #region LAONING_METHODS

            #region CREDITOR_METHODS

        /// <summary>
        /// Loans a certain quantity of resource to a debtor.
        /// If a loan already exists, it replaces it. 
        /// Returns the actual quantity that could have been lent.
        /// </summary>
        /// <returns>Actual quantity that could be lent</returns>
        public float LoanTo(ITransactor debtor, float quantityToLoan)
        {
            if (TryGetLoan(debtor, true, out ResourceTransaction loan))
            {
                _lentQuantity -= loan.quantity;

                if (quantityToLoan == 0)
                {
                    //to do: remove loan
                    return 0;
                }
            }
            else
            {
                loan = new ResourceTransaction(resource, transactor, debtor, 0);

                credits.Add(loan);
            }

            quantityToLoan = Mathf.Clamp(quantityToLoan, 0, availableQuantity);
            quantityToLoan = Mathf.Clamp(quantityToLoan, 0, debtor.GetRemainingCapacity(resource));
            loan.SetQuantity(quantityToLoan);

            _lentQuantity += quantityToLoan;

            if (debtor.TryGetContainer(resource, out ResourceContainer container))
            {
                container.NotifyBorrowing(loan);
            }

            return quantityToLoan;
        }

        public void RemoveLoan(ITransactor debtor)
        {
            if (TryGetLoan(debtor, true, out ResourceTransaction loan))
            {
                credits.Remove(loan);
                _lentQuantity -= loan.quantity;
                debtor.RemoveBorrow(resource, transactor);
            }
        }
            #endregion

            #region DEBTOR_METHODS

        /// <summary>
        /// Borrow an amount of resource to a creditor which cannot be the same as this container's.
        /// Returns the quantity that could have been borrowed.
        /// </summary>
        /// <returns>Quantity that could be borrowed.</returns>
        public float BorrowTo(ITransactor creditor, float quantityToBorrow)
        {
            quantityToBorrow = creditor.LoanTo(transactor, resource, quantityToBorrow);

            RemoveInputsInExcess();
            
            return quantityToBorrow;
        }

        /// <summary>
        /// Borrow all the available quantity of resource a transactor.
        /// Returns the quantity that could have been borrowed.
        /// </summary>
        /// <returns>Quantity that could be borrowed.</returns>
        public float BorrowAllTo(ITransactor creditor)
        {
            float quantityToBorrow = creditor.LoanAllTo(transactor, resource);

            RemoveInputsInExcess();
            
            return quantityToBorrow;
        }

        private void NotifyBorrowing(ResourceTransaction transaction)
        {
            if (transaction.resource != resource || transaction.target != transactor) return;

            if (debts.Contains(transaction))
            {
                if (transaction.quantity == 0) debts.Remove(transaction);
                CalculateBorrowedQuantity(out _borrowedQuantity);
            }
            else if (transaction.quantity != 0)
            {
                debts.Add(transaction);
                _borrowedQuantity += transaction.quantity;
            }
            
            RemoveInputsInExcess();
        }

        public void RemoveBorrow(ITransactor creditor)
        {
            if (TryGetLoan(creditor, false, out ResourceTransaction debt))
            {
                credits.Remove(debt);
                _borrowedQuantity -= debt.quantity;
                creditor.RemoveLoan(resource, transactor);
            }
        }

            #endregion

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

                if (creditor.TryGetContainer(resource, out ResourceContainer container))
                {
                    container.BeRefundedBy(transactor, quantity);
                }
                
                if (loan.quantity == 0)
                {
                    debts.Remove(loan);
                }

                _borrowedQuantity -= quantity;

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
        private float BeRefundedBy(ITransactor debtor, float quantity)
        {
            if (TryGetLoan(debtor, true, out ResourceTransaction loan))
            {
                quantity = Mathf.Clamp(quantity,0, loan.quantity);
                
                loan.AddQuantity(-quantity);
                if (loan.quantity == 0)
                    credits.Remove(loan);

                _lentQuantity -= quantity;
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

        private void NotifyDebtDevaluation(ITransactor creditor, float devaluation)
        {
            if (TryGetLoan(creditor, false, out ResourceTransaction loan))
            {
                devaluation = Mathf.Clamp(devaluation, 0, loan.quantity);
                devaluation = Mathf.Abs(loan.AddQuantity(-devaluation));
                _borrowedQuantity -= devaluation;
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

            foreach (var credit in credits.ToArray())
            {
                float creditDevaluation = Mathf.Clamp(remainingDevaluation, 0, credit.quantity);
                remainingDevaluation -= creditDevaluation;

                if (credit.target.TryGetContainer(resource, out ResourceContainer container))
                {
                    container.NotifyDebtDevaluation(transactor, creditDevaluation);
                }

                if(credit.quantity == 0) credits.Remove(credit);
                if (remainingDevaluation == 0) break;
            }

            _lentQuantity -= devaluation - remainingDevaluation;
            
            return devaluation - remainingDevaluation;
        }
        
        #endregion
        
        #region REGISTRY MANAGEMENT

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

        private void CalculateLentQuantity(out float quantity)
        {
            quantity = 0;

            foreach (var credit in credits)
            {
                quantity += credit.quantity;
            }
        }
        
        private void CalculateBorrowedQuantity(out float quantity)
        {
            quantity = 0;
            foreach (var debt in debts)
            {
                quantity += debt.quantity;
            }
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

        private void RemoveInputsInExcess()
        {
            float transactionDevaluation = remainingDeltaCapacity;
            
            if (transactionDevaluation < 0)
            {
                foreach (var input in inputs)
                {
                    transactionDevaluation += input.AddQuantity(transactionDevaluation);

                    if (input.origin.TryGetContainer(resource, out ResourceContainer container))
                    {
                        container.NotifyOutputChange(input);
                    }
                    
                    if (transactionDevaluation == 0) break;
                }
            }

            _deltaQuantity = CalculateTransactionDelta();
        }
        
        private void RemoveOutputInExcess(ResourceTransaction output)
        {
            float transactionDevaluation = output.target.GetRemainingDeltaCapacity(resource);
                    
            if (transactionDevaluation < 0)
            {
                output.AddQuantity(transactionDevaluation);
                _deltaQuantity += -transactionDevaluation;

                if (output.target.TryGetContainer(resource, out ResourceContainer target))
                {
                    target.NotifyInputChange(output);
                }
            }
        }
        
        #endregion
    }
}