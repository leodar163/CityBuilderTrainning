using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/Scenario", fileName = "NewScenario")]
    public class Scenario : ScriptableObject
    {
        [SerializeField] private EventTimeRange[] _events;

        public EventTimeRange[] Events => _events;
    }
}