using JetBrains.Annotations;

namespace ToolTipSystem
{
    //[CanBeNull]
    public interface IToolTipSpeaker
    {
        public ToolTipMessage ToToolTipMessage();
    }
}