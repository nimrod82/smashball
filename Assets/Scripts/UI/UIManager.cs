using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smashball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ServeUI serveUI;
        [SerializeField] CanvasGroup joystickCanvasGroup;
        
        public ServeUI ServeUI => serveUI;
        
        private void Awake()
        {
            Services.Register<UIManager>(this);
        }

        public void ShowJoystick(bool show)
        {
            joystickCanvasGroup.alpha = show ? 1 : 0;
        }
    }
}
