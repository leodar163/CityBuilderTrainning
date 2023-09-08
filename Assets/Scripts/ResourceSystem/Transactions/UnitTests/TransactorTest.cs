using System.Collections.Generic;

namespace ResourceSystem.Transactions.UnitTests
{
    public class TransactorTest : ITransactor
    {
        public List<ResourceContainer> registry { get; } = new();
        public ITransactor transactorSelf => this;
    }
}