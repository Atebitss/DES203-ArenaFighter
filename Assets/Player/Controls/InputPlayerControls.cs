// GENERATED AUTOMATICALLY FROM 'Assets/Player/Controls/InputPlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputPlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputPlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputPlayerControls"",
    ""maps"": [
        {
            ""name"": ""PlayerInput"",
            ""id"": ""24f42940-03f1-4c14-8cf4-484e5b8dfdef"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""2f98b0d8-6030-490b-8257-0e76e38fcef7"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""0b0280ea-f431-4068-9f65-21ac31d61dcf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""X"",
                    ""type"": ""Button"",
                    ""id"": ""6e0ef3b3-5a3f-401c-9e96-a40cb94dd137"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Y"",
                    ""type"": ""Button"",
                    ""id"": ""c35e1895-b869-417a-8cc2-5c6f893801e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""B"",
                    ""type"": ""Button"",
                    ""id"": ""207c7b3e-d0ea-47b0-8ce5-0584692769f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""A"",
                    ""type"": ""Button"",
                    ""id"": ""378f591d-3e43-4556-8405-166d8100ded7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""D"",
                    ""type"": ""Button"",
                    ""id"": ""4bd90dcd-27a2-4dac-883a-c527c975b688"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Space"",
                    ""type"": ""Button"",
                    ""id"": ""e0dcaded-8d96-4e32-908c-fe1b8dd7ec08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c870972c-a78e-4d63-b4d6-d8ef8c42f32b"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5a9d3c4-e53e-4607-852d-884f60cee043"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9cc036e5-b336-4338-a7b4-2ab7e72cb7f6"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""446422ea-2b14-45d3-aff5-5dc30f630462"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Y"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc8f4561-2966-467c-bd3a-a1e683998d2a"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e15b7311-a5cf-4d67-bbc4-8c47d05557f0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8e19085-b787-498b-a108-00acb8e4178b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""D"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e8e5e14-d0d0-487f-9365-d272c04a7fb7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Space"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerInput
        m_PlayerInput = asset.FindActionMap("PlayerInput", throwIfNotFound: true);
        m_PlayerInput_Move = m_PlayerInput.FindAction("Move", throwIfNotFound: true);
        m_PlayerInput_Jump = m_PlayerInput.FindAction("Jump", throwIfNotFound: true);
        m_PlayerInput_X = m_PlayerInput.FindAction("X", throwIfNotFound: true);
        m_PlayerInput_Y = m_PlayerInput.FindAction("Y", throwIfNotFound: true);
        m_PlayerInput_B = m_PlayerInput.FindAction("B", throwIfNotFound: true);
        m_PlayerInput_A = m_PlayerInput.FindAction("A", throwIfNotFound: true);
        m_PlayerInput_D = m_PlayerInput.FindAction("D", throwIfNotFound: true);
        m_PlayerInput_Space = m_PlayerInput.FindAction("Space", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerInput
    private readonly InputActionMap m_PlayerInput;
    private IPlayerInputActions m_PlayerInputActionsCallbackInterface;
    private readonly InputAction m_PlayerInput_Move;
    private readonly InputAction m_PlayerInput_Jump;
    private readonly InputAction m_PlayerInput_X;
    private readonly InputAction m_PlayerInput_Y;
    private readonly InputAction m_PlayerInput_B;
    private readonly InputAction m_PlayerInput_A;
    private readonly InputAction m_PlayerInput_D;
    private readonly InputAction m_PlayerInput_Space;
    public struct PlayerInputActions
    {
        private @InputPlayerControls m_Wrapper;
        public PlayerInputActions(@InputPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerInput_Move;
        public InputAction @Jump => m_Wrapper.m_PlayerInput_Jump;
        public InputAction @X => m_Wrapper.m_PlayerInput_X;
        public InputAction @Y => m_Wrapper.m_PlayerInput_Y;
        public InputAction @B => m_Wrapper.m_PlayerInput_B;
        public InputAction @A => m_Wrapper.m_PlayerInput_A;
        public InputAction @D => m_Wrapper.m_PlayerInput_D;
        public InputAction @Space => m_Wrapper.m_PlayerInput_Space;
        public InputActionMap Get() { return m_Wrapper.m_PlayerInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerInputActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerInputActions instance)
        {
            if (m_Wrapper.m_PlayerInputActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @X.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnX;
                @X.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnX;
                @X.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnX;
                @Y.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnY;
                @Y.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnY;
                @Y.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnY;
                @B.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnB;
                @B.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnB;
                @B.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnB;
                @A.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnA;
                @A.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnA;
                @A.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnA;
                @D.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnD;
                @D.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnD;
                @D.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnD;
                @Space.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSpace;
                @Space.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSpace;
                @Space.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSpace;
            }
            m_Wrapper.m_PlayerInputActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @X.started += instance.OnX;
                @X.performed += instance.OnX;
                @X.canceled += instance.OnX;
                @Y.started += instance.OnY;
                @Y.performed += instance.OnY;
                @Y.canceled += instance.OnY;
                @B.started += instance.OnB;
                @B.performed += instance.OnB;
                @B.canceled += instance.OnB;
                @A.started += instance.OnA;
                @A.performed += instance.OnA;
                @A.canceled += instance.OnA;
                @D.started += instance.OnD;
                @D.performed += instance.OnD;
                @D.canceled += instance.OnD;
                @Space.started += instance.OnSpace;
                @Space.performed += instance.OnSpace;
                @Space.canceled += instance.OnSpace;
            }
        }
    }
    public PlayerInputActions @PlayerInput => new PlayerInputActions(this);
    public interface IPlayerInputActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnX(InputAction.CallbackContext context);
        void OnY(InputAction.CallbackContext context);
        void OnB(InputAction.CallbackContext context);
        void OnA(InputAction.CallbackContext context);
        void OnD(InputAction.CallbackContext context);
        void OnSpace(InputAction.CallbackContext context);
    }
}
