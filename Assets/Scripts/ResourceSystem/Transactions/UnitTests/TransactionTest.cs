using Format;
using UnityEngine;

namespace ResourceSystem.Transactions.UnitTests
{
    public class TransactionTest : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                //RunTestAlpha();
                //RunTestBeta();
                //RunTestGamma();
                //RunTestEpsilon();
                //RunTestZeta();
                //RunTestEta();
                RunTestTheta();
            }
        }

        private void RunTestAlpha()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();

            ResourceType populationResource = ResourceSet.Default.GetResource("resource_population");

            a.AddResource(populationResource, 10);
            b.AddResource(populationResource, 0);
            
            b.BorrowTo(a,populationResource,5);
            a.AddResource(populationResource, -6);

            print("Test Alpha");
            if (a.TryGetContainer(populationResource, out ResourceContainer containerA))
            {
                print($"<b>ContainerA</b>\nTotalQuantity : 4|{containerA.totalQuantity}\nNativeQuantity : 4|{containerA.nativeQuantity}\nLentQuantity : 4|{containerA.lentQuantity}\n" +
                      $"BorrowedQuantity : 0|{containerA.borrowedQuantity}\nCredits : 1|{containerA.credits.Count}");
            }
            if (b.TryGetContainer(populationResource, out ResourceContainer containerB))
            {
                print($"<b>ContainerB</b>\nTotalQuantity : 4|{containerB.totalQuantity}\nNativeQuantity : 0|{containerB.nativeQuantity}\nLentQuantity : 0|{containerB.lentQuantity}\n" +
                      $"BorrowedQuantity : 4|{containerB.borrowedQuantity}\nDebts : 1|{containerB.debts.Count}");
            }
        }
        
        private void RunTestBeta()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();

            ResourceType populationResource = ResourceSet.Default.GetResource("resource_population");

            a.AddResource(populationResource, 10, 10);
            b.AddResource(populationResource, 0);
            
            b.BorrowTo(a,populationResource,5);

            print("Test Beta");
            
            if (a.TryGetContainer(populationResource, out ResourceContainer containerA))
            {
                containerA.SetNativeMaxQuantity(4);
                print($"<b>ContainerA</b>\nTotalQuantity : 4|{containerA.totalQuantity}\nNativeQuantity : 4|{containerA.nativeQuantity}\nLentQuantity : 4|{containerA.lentQuantity}\n" +
                      $"BorrowedQuantity : 0|{containerA.borrowedQuantity}\nCredits : 1|{containerA.credits.Count}");
            }
            if (b.TryGetContainer(populationResource, out ResourceContainer containerB))
            {
                print($"<b>ContainerB</b>\nTotalQuantity : 4|{containerB.totalQuantity}\nNativeQuantity : 0|{containerB.nativeQuantity}\nLentQuantity : 0|{containerB.lentQuantity}\n" +
                      $"BorrowedQuantity : 4|{containerB.borrowedQuantity}\nDebts : 1|{containerB.debts.Count}");
            }
        }
        
        private void RunTestGamma()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();
            ITransactor c = new TransactorTest();
            ITransactor d = new TransactorTest();

            ResourceType populationResource = ResourceSet.Default.GetResource("resource_population");

            a.AddResource(populationResource, 20);
            b.AddResource(populationResource, 0);
            c.AddResource(populationResource, 0);
            d.AddResource(populationResource, 0);
            
            b.BorrowTo(a,populationResource,5);
            c.BorrowTo(a,populationResource,5);
            d.BorrowTo(a,populationResource,5);

            print("Test Gamma");
            
            if (a.TryGetContainer(populationResource, out ResourceContainer containerA))
            {
                containerA.SetNativeMaxQuantity(9);
                print($"<b>ContainerA</b>\nTotalQuantity : 9|{containerA.totalQuantity}\nNativeQuantity : 9|{containerA.nativeQuantity}\nLentQuantity : 9|{containerA.lentQuantity}\n" +
                      $"BorrowedQuantity : 0|{containerA.borrowedQuantity}\nCredits : 2|{containerA.credits.Count}");
            }
            if (b.TryGetContainer(populationResource, out ResourceContainer containerB))
            {
                print($"<b>ContainerB</b>\nTotalQuantity : 0|{containerB.totalQuantity}\nNativeQuantity : 0|{containerB.nativeQuantity}\nLentQuantity : 0|{containerB.lentQuantity}\n" +
                      $"BorrowedQuantity : 0|{containerB.borrowedQuantity}\nDebts : 0|{containerB.debts.Count}");
            }
            if (c.TryGetContainer(populationResource, out ResourceContainer containerC))
            {
                print($"<b>ContainerC</b>\nTotalQuantity : 4|{containerC.totalQuantity}\nNativeQuantity : 0|{containerC.nativeQuantity}\nLentQuantity : 0|{containerC.lentQuantity}\n" +
                      $"BorrowedQuantity : 4|{containerC.borrowedQuantity}\nDebts : 1|{containerC.debts.Count}");
            }
            if (d.TryGetContainer(populationResource, out ResourceContainer containerD))
            {
                print($"<b>ContainerD</b>\nTotalQuantity : 5|{containerD.totalQuantity}\nNativeQuantity : 0|{containerD.nativeQuantity}\nLentQuantity : 0|{containerD.lentQuantity}\n" +
                      $"BorrowedQuantity : 5|{containerD.borrowedQuantity}\nDebts : 1|{containerD.debts.Count}");
            }
        }
        
        private void RunTestEpsilon()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();
            ITransactor c = new TransactorTest();
            ITransactor d = new TransactorTest();

            ResourceType populationResource = ResourceSet.Default.GetResource("resource_population");

            a.AddResource(populationResource, 20);
            b.AddResource(populationResource, 0);
            c.AddResource(populationResource, 0);
            d.AddResource(populationResource, 0);
            
            b.BorrowTo(a,populationResource,10);
            c.BorrowTo(b,populationResource,5);
            d.BorrowTo(c,populationResource,5);

            print("Test Epsilon");
            
            if (a.TryGetContainer(populationResource, out ResourceContainer containerA))
            {
                containerA.SetNativeMaxQuantity(3);
                print($"<b>ContainerA</b>\nTotalQuantity : 3|{containerA.totalQuantity}\nNativeQuantity : 3|{containerA.nativeQuantity}\nLentQuantity : 3|{containerA.lentQuantity}\n" +
                      $"BorrowedQuantity : 0|{containerA.borrowedQuantity}\nCredits : 1|{containerA.credits.Count}");
            }
            if (b.TryGetContainer(populationResource, out ResourceContainer containerB))
            {
                print($"<b>ContainerB</b>\nTotalQuantity : 3|{containerB.totalQuantity}\nNativeQuantity : 0|{containerB.nativeQuantity}\nLentQuantity : 3|{containerB.lentQuantity}\n" +
                      $"BorrowedQuantity : 3|{containerB.borrowedQuantity}\nDebts : 1|{containerB.debts.Count}");
            }
            if (c.TryGetContainer(populationResource, out ResourceContainer containerC))
            {
                print($"<b>ContainerC</b>\nTotalQuantity : 3|{containerC.totalQuantity}\nNativeQuantity : 0|{containerC.nativeQuantity}\nLentQuantity : 3|{containerC.lentQuantity}\n" +
                      $"BorrowedQuantity : 3|{containerC.borrowedQuantity}\nDebts : 1|{containerC.debts.Count}");
            }
            if (d.TryGetContainer(populationResource, out ResourceContainer containerD))
            {
                print($"<b>ContainerD</b>\nTotalQuantity : 3|{containerD.totalQuantity}\nNativeQuantity : 0|{containerD.nativeQuantity}\nLentQuantity : 0|{containerD.lentQuantity}\n" +
                      $"BorrowedQuantity : 3|{containerD.borrowedQuantity}\nDebts : 1|{containerD.debts.Count}");
            }
        }

        private void RunTestZeta()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();

            ResourceType woodResource = ResourceSet.Default.GetResource("resource_wood");

            a.AddResource(woodResource, 20);
            b.AddResource(woodResource, 0);
            a.SetOutputTransaction(b, woodResource, 1);
            
            b.AskInputs();
            b.AskInputs();
            b.AskInputs();

            string message = "<b>Test Zeta</b>";

            if (a.TryGetContainer(woodResource, out ResourceContainer containerA))
            {
                message += $"\nContainerA\ntotal: 17|{containerA.totalQuantity}\nnative: 17|{containerA.nativeQuantity}\noutputs: 1|{containerA.outputs.Count}";
            }
            if (b.TryGetContainer(woodResource, out ResourceContainer containerB))
            {
                message += $"\nContainerB\ntotal: 3|{containerB.totalQuantity}\nnative: 3|{containerB.nativeQuantity}\ninputs: 1|{containerB.inputs.Count}";
            }
            
            
            print(message);
        }

        private void RunTestEta()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();
            
            ResourceType woodResource = ResourceSet.Default.GetResource("resource_wood");

            a.AddResource(woodResource, 20);
            b.AddResource(woodResource, 0);
            
            b.BorrowTo(a, woodResource, 10);
            b.SetInputTransaction(a, woodResource, 11);
            
            b.AskInputs(woodResource);
            
            string message = "<b>Test Eta</b>";

            if (a.TryGetContainer(woodResource, out ResourceContainer containerA))
            {
                message += $"\n\n<b>ContainerA</b>\navailable : 0|{containerA.availableQuantity}\nnative : 9|{containerA.nativeQuantity}" +
                           $"\nlent : 9|{containerA.lentQuantity}\ncredits : 1|{containerA.credits.Count}\noutputs : 1|{containerA.outputs.Count}";
            }
            if (b.TryGetContainer(woodResource, out ResourceContainer containerB))
            {
                message += $"\n\n<b>ContainerB</b>\ntotal : 20|{containerB.totalQuantity}\nnative : 11|{containerB.nativeQuantity}" +
                           $"\nborrowed : 9|{containerB.borrowedQuantity}\ndebts : 1|{containerB.debts.Count}\ninputs : 1|{containerB.inputs.Count}";
            }
            
            print(message);
        }

        private void RunTestTheta()
        {
            ITransactor a = new TransactorTest();
            ITransactor b = new TransactorTest();
            
            ResourceType woodResource = ResourceSet.Default.GetResource("resource_wood");

            string message = "<b>Test Theta</b>\n\n";
            
            a.AddResource(woodResource, 5, 10);
            b.AddResource(woodResource, 10,10);

            b.SetOutputTransaction(a, woodResource,3);

            if (a.TryGetContainer(woodResource, out ResourceContainer containerA))
            {
                message += $"<b>ContainerA</b>\nTime 1 : Remaining Delta capacity : 2|{containerA.remainingDeltaCapacity}";
                b.LoanTo(a, woodResource, 3);
                message += $"\nTime 2 : Remaining Delta capacity : 0|{containerA.remainingDeltaCapacity}";
            }

            if (b.TryGetContainer(woodResource, out ResourceContainer containerB))
            {
                message += $"\n===========\n<b>ContainerB</b>\ntransaction to A : 2|{containerB.outputs[0].quantity}";
            }

            print(message);
        }
    }
}