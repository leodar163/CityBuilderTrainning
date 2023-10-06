using System;

namespace Interactions
{
    public interface IInteractor
    {
        public static Action<IInteractor> onEnable;

        public bool isActive { get;}
        public void ActivateMode();
        public void DeactivateMode();

        public InteractionMode interactionMode { get; }
    }
}