using System;
using ResourceSystem.UI;
using TimeSystem;
using UnityEngine;
using Utils.UI;

namespace ResourceSystem.Global.UI
{
    public class GlobalResourceDeckUI : PanelUI<GlobalResourceDeckUI>
    {
        private Action<InGameDate> monthlyUpdate;

        private void OnEnable()
        {
            TimeManager.onNewMonth += monthlyUpdate;
        }

        private void OnDisable()
        {
            TimeManager.onNewMonth -= monthlyUpdate;
        }

        protected override void Awake()
        {
            monthlyUpdate = _ => CalculateResources();
        }

        private void CalculateResources()
        {
            
        }
    }
}