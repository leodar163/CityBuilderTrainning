using System.Collections.Generic;
using ResourceSystem.Transactions;
using UnityEngine;

namespace ResourceSystem.Productions
{
    public interface IProducer
    {
        public ITransactor transactor { get; }
        public IProducer producerSelf { get; }
        protected List<ProductionLine> productionLines { get; }

        protected List<ITransactor> creditors { get; }

        protected List<ITransactor> debtors { get; }

        public void AddCreditor(ITransactor creditor)
        {
            if (!creditors.Contains(creditor))
            {
                creditors.Add(creditor);
            }
        }

        public void RemoveCreditor(ITransactor creditor)
        {
            if (creditors.Contains(creditor))
            {
                creditors.Remove(creditor);
            }
        }
        
        public void AddDebtor(ITransactor debtor)
        {
            if (!debtors.Contains(debtor))
            {
                debtors.Add(debtor);
            }
        }

        public void RemoveDebtor(ITransactor debtor)
        {
            if (debtors.Contains(debtor))
            {
                debtors.Remove(debtor);
            }
        }
        
        public void InitTransactor()
        {
            foreach (var line in productionLines)
            {
                foreach (var input in line.inputs)
                {
                    transactor.AddResource(input.resource, 0);
                }
                foreach (var output in line.outputs)
                {
                    transactor.AddResource(output.resource, 0);
                }
            }
        }

        public void AddProductionLine(ProductionLine lineToAdd)
        {
            if (!productionLines.Contains(lineToAdd))
            {
                productionLines.Add(lineToAdd);
            }

            foreach (var input in lineToAdd.inputs)
            {
                transactor.AddResource(input.resource, 0);
            }
            foreach (var output in lineToAdd.outputs)
            {
                transactor.AddResource(output.resource, 0);
            }
        }
        
        public void Produce()
        {
            FetchInputResources();
            ProduceResources();
            ConsumeInputs();
            DeliverProducedResources();
        }
        
        private void FetchInputResources()
        {
            transactor.RemoveInputsAll();
            
            foreach (var line in productionLines)
            {
                line.efficiency = EvaluateEfficiency(line);

                foreach (var input in line.inputs)
                {
                    float expectedInput = input.amount * line.efficiency;
                    
                    foreach (var creditor in creditors)
                    {
                        if (input.isLoan)
                        {
                            expectedInput -= creditor.LoanTo(transactor, input.resource, expectedInput);
                        }
                        else
                        {
                            expectedInput -= creditor.SetOutputTransaction(transactor, input.resource, expectedInput);
                        }
                        
                        if (expectedInput == 0) break;    
                    }
                }
            }    
            
            transactor.AskInputs();
        }

        private void ProduceResources()
        {
            foreach (var line in productionLines)
            {
                foreach (var output in line.outputs)
                {
                    if (output.isLoan)
                    {
                        transactor.SetResource(output.resource, output.amount * line.efficiency);
                    }
                    else
                    {
                        transactor.AddResource(output.resource, output.amount * line.efficiency);
                    }
                }
            }
        }
        
        private void DeliverProducedResources()
        {
            transactor.RemoveOutputsAll();
            
            foreach (var line in productionLines)
            {
                foreach (var output in line.outputs)
                {
                    float expectedOutput = output.amount * line.efficiency;
                    
                    foreach (var debtor in debtors)
                    {
                        if (output.isLoan)
                        {
                            expectedOutput -= debtor.BorrowTo(transactor, output.resource, expectedOutput);
                        }
                        else
                        {
                            expectedOutput -= debtor.SetInputTransaction(transactor, output.resource, expectedOutput);
                        }
                        if (expectedOutput == 0) break;
                    }

                    if (expectedOutput > 0)
                    {
                        transactor.AddResource(output.resource, -expectedOutput);
                    }
                }
            }
            
            transactor.GiveOutputs();
        }

        private void ConsumeInputs()
        {
            foreach (var line in productionLines)
            {
                foreach (var input in line.inputs)
                {
                    if (!input.isLoan)
                    {
                        transactor.SetResource(input.resource, 0);
                    }
                }
            }
        }

        private float EvaluateEfficiency(ProductionLine line)
        {
            if (creditors.Count == 0 || debtors.Count == 0)
                return 0;
            
            float efficiency = 1;
            
            float localEfficiency;
            
            foreach (var input in line.inputs)
            {
                float expectedQuantity = input.amount;
                
                foreach (var creditor in creditors)
                {
                    if (input.isLoan)
                    {
                        float availableQuantity = creditor.GetAvailableQuantity(input.resource);

                        if (availableQuantity < 0) return 0;

                        expectedQuantity -= Mathf.Clamp(availableQuantity, 0,expectedQuantity);
                    }
                    else
                    {
                        float totalQuantity = creditor.GetTotalQuantity(input.resource);

                        if (totalQuantity < 0) return 0;

                        expectedQuantity -= Mathf.Clamp(totalQuantity, 0, expectedQuantity);
                    }
                    
                    if (expectedQuantity == 0) break;
                }

                localEfficiency = (input.amount - expectedQuantity) / input.amount;

                if (localEfficiency < efficiency) efficiency = localEfficiency;
            }

            foreach (var output in line.outputs)
            {
                float expectedQuantity = output.amount;
                
                foreach (var debtor in debtors)
                {
                    if (output.isLoan)
                    {
                        float remainingCapacity = debtor.GetRemainingCapacity(output.resource);

                        if (remainingCapacity < 0) return 0;

                            expectedQuantity -= Mathf.Clamp(remainingCapacity, 0,expectedQuantity);
                    }
                    else
                    {
                        float remainingDeltaCapacity = debtor.GetRemainingDeltaCapacity(output.resource);

                        if (remainingDeltaCapacity < 0) return 0;
                        
                        expectedQuantity -= Mathf.Clamp(remainingDeltaCapacity, 0, expectedQuantity);
                    }
                    
                    if (expectedQuantity == 0) break;
                }

                localEfficiency = (output.amount - expectedQuantity) / output.amount;

                if (localEfficiency < efficiency) efficiency = localEfficiency;
            }

            return efficiency;
        }
    }
}