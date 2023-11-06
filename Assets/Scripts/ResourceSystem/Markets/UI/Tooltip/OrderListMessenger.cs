using System.Collections.Generic;
using System.Globalization;
using Localization;
using TMPro;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ResourceSystem.Markets.UI.Tooltip
{
    public class OrderListMessenger : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private OrderType _orderType;
        [SerializeField] private TextTooltipMessageUI _textTooltip; 
        [SerializeField] private TextMeshProUGUI _amount;
        private ResourceType _resource;
        private Market _market;

        public ResourceType Resource
        {
            get => _resource;
            set
            {
                _resource = value;
               UpdateText();
            }
        }

        public Market Market
        {
            get => _market;
            set
            {
                _market = value;
                UpdateText();
            }
        }
        
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _textTooltip;

        private void UpdateText()
        {
            if (_market != null && _resource != null)
                _amount.SetText(
                    (Mathf.RoundToInt(_market.GetResourceValueAmount(_resource, _orderType) * 10) / 10).ToString(
                        CultureInfo.CurrentCulture));
        }
        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
              textTooltipMessageUI.SetTexts(formatTitle(), FormatOrderList());   
            }
        }

        private string formatTitle()
        {
            string title = _orderType == OrderType.Demand ? VariableNameManager.DemandName : VariableNameManager.OfferName;

            if (_resource == null || _market == null)
                return title;
            return title + $" ({_market.GetResourceValueAmount(_resource, _orderType)})";
        }

        private string FormatOrderList()
        {
            string ordersFormat = "";
            
            if (_resource == null || _market == null)
                return ordersFormat;

            Dictionary<string, Vector2> orderCollapser = new();

            if (_market.TryToGetOrders(_resource, _orderType, out List<ResourceOrder> orders))
            {
                foreach (var order in orders)
                {
                    Vector2 orderOffset = new Vector2(1, order.quantity);

                    if (!orderCollapser.TryAdd(order.sender.EconomicActorName, orderOffset))
                        orderCollapser[order.sender.EconomicActorName] += orderOffset;
                }
            }

            foreach (var collapsedOrder in orderCollapser)
            {
                ordersFormat += $"{collapsedOrder.Key}({collapsedOrder.Value.x}) : {collapsedOrder.Value.y}\n";
            }

            return ordersFormat;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }
    }
}