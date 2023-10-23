
namespace ToolTipSystem
{
    public interface ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf { get; }
        
        public TooltipMessageUI tooltipMessage { get; }

        public void UpdateTooltipMessage(TooltipMessageUI messageUI);

        public void SubToTooltip()
        {
            Tooltip.Sub(this);
        }

        public void UnsubFromTooltip()
        {
            Tooltip.Unsub(this);
        }
    }
}