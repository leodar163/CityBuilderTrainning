using System;
using System.Collections.Generic;
using ResourceSystem.Transactions;
using ResourceSystem.Transactions.UnitTests;
using UnityEngine;
using Utils;

namespace ResourceSystem.Productions.UnitTests
{
    public class ProductionTests : Singleton<ProductionTests>
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                //RunTestAlpha();    
                RunTestBeta();
            }
        }

        private void RunTestAlpha()
        {
            ITransactor transactor = new TransactorTest();
            IProducer producer = new ProducerTest();

            ResourceType wood = ResourceSet.Default.GetResource("resource_wood");
            ResourceType workforce = ResourceSet.Default.GetResource("resource_workforce");
            ResourceType minerals = ResourceSet.Default.GetResource("resource_minerals");
            ResourceType population = ResourceSet.Default.GetResource("resource_population");

            transactor.AddResource(wood, 100);
            transactor.AddResource(workforce, 100);
            transactor.AddResource(minerals, 0);
            transactor.AddResource(population, 0);
            
            producer.AddCreditor(transactor);
            producer.AddDebtor(transactor);
            
            producer.AddProductionLine(new ProductionLine(
                new List<ResourceProduction>
            {
                new (wood, 3),
                new (workforce, 2, true)
            }, 
                new List<ResourceProduction>
            {
                new (minerals, 4),
                new (population, 3, true)
            }));
            
            producer.Produce();
            producer.Produce();

            string message = "<b>Test Alpha</b>\n";
            message += "\n<b>transactor source</b>";
            if (transactor.TryGetContainer(wood, out ResourceContainer woodContainerSource))
            {
                message += $"\nwood container\nquantity : 94|{woodContainerSource.totalQuantity}";
            }
            if (transactor.TryGetContainer(minerals, out ResourceContainer mineralsContainerSource))
            {
                message += $"\nminerals container\nquantity : 8|{mineralsContainerSource.totalQuantity}";
            }
            if (transactor.TryGetContainer(workforce, out ResourceContainer wfContainerSource))
            {
                message += $"\nworkforce container\nquantity : 98|{wfContainerSource.availableQuantity}" +
                           $"\nlent : 2|{wfContainerSource.lentQuantity}";
            }
            if (transactor.TryGetContainer(population, out ResourceContainer popContainerSource))
            {
                message += $"\npopulation container\nquantity : 3|{popContainerSource.availableQuantity}" +
                           $"\nborrowed : 3|{popContainerSource.borrowedQuantity}";
            }

            message += "\n\n<b>producer</b>";
            if (producer.transactor.TryGetContainer(wood, out ResourceContainer woodContainerProducer))
            {
                message += $"\nwood container\nquantity : 0|{woodContainerProducer.totalQuantity}";
            }
            if (producer.transactor.TryGetContainer(minerals, out ResourceContainer mineralsContainerProducer))
            {
                message += $"\nminerals container\nquantity : 0|{mineralsContainerProducer.totalQuantity}";
            }
            if (producer.transactor.TryGetContainer(workforce, out ResourceContainer wfContainerProducer))
            {
                message += $"\nworkforce container\nquantity : 2|{wfContainerProducer.availableQuantity}" +
                           $"\nborrowed : 2|{wfContainerProducer.borrowedQuantity}";
            }
            if (producer.transactor.TryGetContainer(population, out ResourceContainer popContainerProducer))
            {
                message += $"\npopulation container\nquantity : 0|{popContainerProducer.availableQuantity}" +
                           $"\nlent : 3|{popContainerProducer.lentQuantity}";
            }
            
            print(message);
        }
        
        private void RunTestBeta()
        {
            ITransactor transactor = new TransactorTest();
            IProducer producer = new ProducerTest();

            ResourceType wood = ResourceSet.Default.GetResource("resource_wood");
            ResourceType workforce = ResourceSet.Default.GetResource("resource_workforce");
            ResourceType minerals = ResourceSet.Default.GetResource("resource_minerals");
            ResourceType population = ResourceSet.Default.GetResource("resource_population");

            transactor.AddResource(wood, 6);
            transactor.AddResource(workforce, 100);
            transactor.AddResource(minerals, 0);
            transactor.AddResource(population, 0);
            
            producer.AddCreditor(transactor);
            producer.AddDebtor(transactor);
            
            producer.AddProductionLine(new ProductionLine(
                new List<ResourceProduction>
            {
                new (wood, 4),
                new (workforce, 2, true)
            }, 
                new List<ResourceProduction>
            {
                new (minerals, 4),
                new (population, 3, true)
            }));
            
            producer.Produce();
            producer.Produce();

            string message = "<b>Test Bêta</b>\n";
            message += "\n<b>transactor source</b>";
            if (transactor.TryGetContainer(wood, out ResourceContainer woodContainerSource))
            {
                message += $"\nwood container\nquantity : 0|{woodContainerSource.totalQuantity}";
            }
            if (transactor.TryGetContainer(minerals, out ResourceContainer mineralsContainerSource))
            {
                message += $"\nminerals container\nquantity : 6|{mineralsContainerSource.totalQuantity}";
            }
            if (transactor.TryGetContainer(workforce, out ResourceContainer wfContainerSource))
            {
                message += $"\nworkforce container\nquantity : 99|{wfContainerSource.availableQuantity}" +
                           $"\nlent : 1|{wfContainerSource.lentQuantity}";
            }
            if (transactor.TryGetContainer(population, out ResourceContainer popContainerSource))
            {
                message += $"\npopulation container\nquantity : 1.5|{popContainerSource.availableQuantity}" +
                           $"\nborrowed : 1.5|{popContainerSource.borrowedQuantity}";
            }

            message += "\n\n<b>producer</b>";
            if (producer.transactor.TryGetContainer(wood, out ResourceContainer woodContainerProducer))
            {
                message += $"\nwood container\nquantity : 0|{woodContainerProducer.totalQuantity}";
            }
            if (producer.transactor.TryGetContainer(minerals, out ResourceContainer mineralsContainerProducer))
            {
                message += $"\nminerals container\nquantity : 0|{mineralsContainerProducer.totalQuantity}";
            }
            if (producer.transactor.TryGetContainer(workforce, out ResourceContainer wfContainerProducer))
            {
                message += $"\nworkforce container\nquantity : 1|{wfContainerProducer.availableQuantity}" +
                           $"\nborrowed : 1|{wfContainerProducer.borrowedQuantity}";
            }
            if (producer.transactor.TryGetContainer(population, out ResourceContainer popContainerProducer))
            {
                message += $"\npopulation container\nquantity : 0|{popContainerProducer.availableQuantity}" +
                           $"\nlent : 1.5|{popContainerProducer.lentQuantity}";
            }
            
            print(message);
        }
    }
}