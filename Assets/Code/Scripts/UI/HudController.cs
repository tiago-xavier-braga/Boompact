using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XaviGames.Ui
{
    public class HudController : CanvasGroupController
    {
        [field: Header("Button References")]
        [field: SerializeField]
        public EventTrigger LeftButton { get; private set; }

        [field: SerializeField]
        public EventTrigger RightButton { get; private set; }

        [field: SerializeField]
        public EventTrigger AcceleratorButton { get; private set; }

        [field: SerializeField]
        public EventTrigger BrakeButton { get; private set; }
        
        [field: SerializeField]
        public EventTrigger HandbrakeButton { get; private set; }
    }
}
