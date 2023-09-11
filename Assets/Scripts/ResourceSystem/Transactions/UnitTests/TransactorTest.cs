using System.Collections.Generic;

namespace ResourceSystem.Transactions.UnitTests
{
    public class TransactorTest : ITransactor
    {
        List<ResourceContainer> ITransactor.registry { get; } = new();

        public ITransactor transactorSelf => this;
    }
}