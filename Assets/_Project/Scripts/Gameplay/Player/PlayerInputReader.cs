using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostDungeon.Gameplay.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        InputAction move;
        readonly Dictionary<PlayerButton, InputAction> actions = new Dictionary<PlayerButton, InputAction>();
        readonly HashSet<PlayerButton> virtualHeld = new HashSet<PlayerButton>();
        readonly Dictionary<PlayerButton, int> virtualPressedFrame = new Dictionary<PlayerButton, int>();
        float virtualMove;

        public bool TouchControlsPreferred { get; private set; }

        public float Move => Mathf.Clamp(move.ReadValue<Vector2>().x + virtualMove, -1f, 1f);
        public bool JumpHeld => Held(PlayerButton.Jump);
        public bool DownHeld => move.ReadValue<Vector2>().y < -0.5f || virtualHeld.Contains(PlayerButton.Down);

        void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            TouchControlsPreferred = true;
#else
            TouchControlsPreferred = false;
#endif
            move = new InputAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            move.AddBinding("<Gamepad>/leftStick");
            move.AddBinding("<Gamepad>/dpad");

            Register(PlayerButton.Jump, "<Keyboard>/space", "<Keyboard>/w", "<Keyboard>/upArrow", "<Gamepad>/buttonSouth");
            Register(PlayerButton.Attack, "<Keyboard>/j", "<Mouse>/leftButton", "<Gamepad>/buttonWest");
            Register(PlayerButton.Dash, "<Keyboard>/k", "<Keyboard>/leftShift", "<Gamepad>/rightShoulder");
            Register(PlayerButton.Fire, "<Keyboard>/l", "<Mouse>/rightButton", "<Gamepad>/buttonNorth");
            Register(PlayerButton.Interact, "<Keyboard>/e", "<Gamepad>/buttonEast");
            Register(PlayerButton.Potion, "<Keyboard>/q", "<Gamepad>/leftShoulder");
            Register(PlayerButton.Pause, "<Keyboard>/escape", "<Keyboard>/p", "<Gamepad>/start");
        }

        void Register(PlayerButton button, params string[] bindings)
        {
            var action = new InputAction(button.ToString(), InputActionType.Button);
            foreach (string binding in bindings) action.AddBinding(binding);
            actions[button] = action;
        }

        void OnEnable()
        {
            move.Enable();
            foreach (InputAction action in actions.Values) action.Enable();
        }

        void OnDisable()
        {
            move.Disable();
            foreach (InputAction action in actions.Values) action.Disable();
            virtualHeld.Clear();
            virtualMove = 0f;
        }

        void OnDestroy()
        {
            move.Dispose();
            foreach (InputAction action in actions.Values) action.Dispose();
        }

        public bool Pressed(PlayerButton button)
        {
            if (actions.TryGetValue(button, out InputAction action) && action.WasPressedThisFrame()) return true;
            return virtualPressedFrame.TryGetValue(button, out int frame) && frame == Time.frameCount;
        }

        public bool Held(PlayerButton button)
        {
            if (actions.TryGetValue(button, out InputAction action) && action.IsPressed()) return true;
            return virtualHeld.Contains(button);
        }

        public void SetVirtualMove(float value) => virtualMove = Mathf.Clamp(value, -1f, 1f);

        public void SetVirtualButton(PlayerButton button, bool isDown)
        {
            if (isDown)
            {
                if (virtualHeld.Add(button)) virtualPressedFrame[button] = Time.frameCount;
            }
            else
            {
                virtualHeld.Remove(button);
            }
        }
    }
}
