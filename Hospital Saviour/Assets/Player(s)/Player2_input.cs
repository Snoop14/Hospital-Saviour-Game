//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Player(s)/Player2_input.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Player2_input : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player2_input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player2_input"",
    ""maps"": [
        {
            ""name"": ""Player_Main"",
            ""id"": ""b2693b43-581b-4dec-9cc4-f931b86ed0bc"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""ffa1d1a9-06ce-4cbf-803d-95810af3e201"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""fe030467-949c-4565-84c1-a9a58f68b990"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""96f2adc3-0382-45b5-b439-f772c7d61cab"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a384c0d7-3581-451d-a890-0d7c3ffae2ce"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""70462dff-e4e9-412b-bee3-6744f9e7b056"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""77ea28eb-662c-4784-beeb-ba9420303dfa"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""55d27b7b-ef6f-4280-9b04-73a06b1293a3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""db7f0612-e23e-4d75-b8be-0989e1c6c568"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player_Main
        m_Player_Main = asset.FindActionMap("Player_Main", throwIfNotFound: true);
        m_Player_Main_Move = m_Player_Main.FindAction("Move", throwIfNotFound: true);
        m_Player_Main_Interact = m_Player_Main.FindAction("Interact", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player_Main
    private readonly InputActionMap m_Player_Main;
    private IPlayer_MainActions m_Player_MainActionsCallbackInterface;
    private readonly InputAction m_Player_Main_Move;
    private readonly InputAction m_Player_Main_Interact;
    public struct Player_MainActions
    {
        private @Player2_input m_Wrapper;
        public Player_MainActions(@Player2_input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Main_Move;
        public InputAction @Interact => m_Wrapper.m_Player_Main_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Player_Main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Player_MainActions set) { return set.Get(); }
        public void SetCallbacks(IPlayer_MainActions instance)
        {
            if (m_Wrapper.m_Player_MainActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnMove;
                @Interact.started -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_Player_MainActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_Player_MainActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public Player_MainActions @Player_Main => new Player_MainActions(this);
    public interface IPlayer_MainActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
}