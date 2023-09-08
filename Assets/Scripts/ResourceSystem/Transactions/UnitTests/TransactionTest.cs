using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Transactions.UnitTests
{
    public class TransactionTest : MonoBehaviour
    {
        private void Start()
        {
            //RunTestBeta();
             //RunTestGamma();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                RunTestGamma();
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
        
    }
}