using System.Collections.Generic;
using ResourceSystem.Transactions;

namespace ResourceSystem.Productions.UnitTests
{
    public class ProducerTest : IProducer, ITransactor
    {
        public ITransactor transactor => this;
        public IProducer producerSelf => this;
        public ITransactor transactorSelf => this;
        List<ProductionLine> IProducer.productionLines { get; } = new();

        List<ITransactor> IProducer.creditors { get; } = new();

        List<ITransactor> IProducer.debtors { get; } = new();

        List<ResourceContainer> ITransactor.registry { get; } = new();
        
    }
}