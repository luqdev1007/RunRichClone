using UnityEngine;
using UnityEngine.InputSystem;

namespace RunRich.Input
{
    public sealed class PointerSwipeInput : IInputService
    {
        public float HorizontalDelta
        {
            get
            {
                var pointer = Pointer.current;
                if (pointer == null || !pointer.press.isPressed)
                    return 0f;

                return pointer.delta.ReadValue().x / Screen.width;
            }
        }

        public bool PressedThisFrame
        {
            get
            {
                var pointer = Pointer.current;
                return pointer != null && pointer.press.wasPressedThisFrame;
            }
        }
    }
}
