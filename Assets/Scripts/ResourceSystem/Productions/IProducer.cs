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
                RegisterIOs();
            }
        }

        public void RemoveCreditor(ITransactor creditor)
        {
            if (creditors.Contains(creditor))
            {
                creditors.Remove(creditor);
                RegisterIOs();
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
                    transactor.AddResource(input.resource, float.PositiveInfinity);
                }

                foreach (var output in line.outputs)
                {
                    transactor.AddResource(output.resource, float.PositiveInfinity);
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
                transactor.AddResource(input.resource, float.PositiveInfinity);
            }

            foreach (var output in lineToAdd.outputs)
            {
                transactor.AddResource(output.resource, float.PositiveInfinity);
            }
        }

        public void RegisterIOs()
        {
            foreach (var line in productionLines)
            {
                foreach (var input in line.inputs)
                {
                    input.expectedAmount = input.amount;

                    foreach (var creditor in creditors)
                    {
                        if (input.isLoan)
                        {
                            input.expectedAmount -= creditor.LoanTo(transactor, input.resource, input.expectedAmount);
                        }
                        else
                        {
                            input.expectedAmount -=
                                creditor.SetOutputTransaction(transactor, input.resource, input.expectedAmount);
                        }
                    }

                    if (input.expectedAmount == 0) break;
                }

                line.CalculateEfficiency();
            }

            foreach (var line in productionLines)
            {
                foreach (var output in line.outputs)
                {
                    output.expectedAmount = output.amount * line.efficiency;

                    foreach (var debtor in debtors)
                    {
                        if (output.isLoan)
                        {
                            output.expectedAmount -=
                                debtor.LoanTo(transactor, output.resource, output.expectedAmount);
                        }
                        else
                        {
                            output.expectedAmount -=
                                debtor.SetInputTransaction(transactor, output.resource, output.expectedAmount);
                        }

                        if (output.expectedAmount == 0) return;
                    }
                }
            }
        }

        public void ProduceOutputs()
        {
            transactor.AskInputs();

            foreach (var line in productionLines)
            {
                foreach (var output in line.outputs)
                {
                    transactor.AddResource(output.resource, output.amount * line.efficiency);
                }
            }
        }
        
        public void DeliverOutputs()
        {
            transactor.GiveOutputs();
        }

        public void Produce()
        {
            RegisterIOs();
            ProduceOutputs();
            DeliverOutputs();
        }
    }
}