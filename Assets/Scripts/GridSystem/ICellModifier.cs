namespace GridSystem
{
    public interface ICellModifier
    {
        public void OnAddedToCell(CellData cell);
        public void OnRemovedFromCell(CellData cell);
    }
}