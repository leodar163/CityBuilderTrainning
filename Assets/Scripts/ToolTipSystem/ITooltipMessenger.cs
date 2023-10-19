
namespace ToolTipSystem
{
    public interface ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf { get; }
        
        public TooltipMessageUI message { get; }

        public void UpdateTooltipMessage(TooltipMessageUI messageUI);
    }
}