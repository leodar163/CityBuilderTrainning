namespace GridSystem.Interaction
{
    public interface IGridInteractor
    {
        public GridInteractorType type { get; }

        public bool isActive { get; set; }
        public bool cancelable { get; }

        public void Activate()
        {
            if (isActive) return;
            isActive = true;
            OnActivated();
        }

        public void Deactivate()
        {
            if (!isActive) return;
            isActive = false;
            OnDeactivated();
        }
        
        public void OnHoveredCellChanged(CellData cellData);
        protected void OnActivated();
        protected void OnDeactivated();
    }
}