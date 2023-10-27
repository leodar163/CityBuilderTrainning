using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Utils.UI
{
    public interface IPanel : IPointerEnterHandler, IPointerExitHandler
    {
        public static List<IPanel> openPanels = new ();

        public static IPanel focusedPanel => openPanels.Count > 0 ? openPanels[^1] : null;
        
        public static void PutFocusOnPanel(IPanel panel)
        {
            if (!openPanels.Contains(panel))
            {
                openPanels.Add(panel);
                return;
            }

            openPanels.Remove(panel);
            openPanels.Add(panel);
        }

        public static void NotifyClosingPanel(IPanel panel)
        {
            openPanels.Remove(panel);
        }

        public void OpenPanel();
        public void ClosePanel();
    }
}