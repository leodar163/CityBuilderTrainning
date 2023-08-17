namespace Interactions
{
    public interface IInteractionMode
    {
        public bool isActive { get;}
        public void ActivateMode();
        public void DeactivateMode();
    }
}