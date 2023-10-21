
namespace ToolTipSystem
{
    public interface ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf { get; }
        
        public TooltipMessageUI tooltipMessage { get; }

        public void UpdateTooltipMessage(TooltipMessageUI messageUI);
    }
}