using System.Collections.Generic;
using UnityEngine;

namespace Utils.UI
{
    public class PanelManager : Singleton<PanelManager>
    {
        private static List<IPanel> s_panel = new();
    }
}