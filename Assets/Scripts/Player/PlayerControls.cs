// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""CharacterControls"",
            ""id"": ""12d8c98f-e88f-49fc-b027-f19e7dedd221"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""8aba3999-ec45-41a1-92dd-9e30260413e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""Value"",
                    ""id"": ""23b3096f-7c94-4d8b-86d4-ce4b0ac8445b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scrollwheel"",
                    ""type"": ""Value"",
                    ""id"": ""98146e36-3bb1-4398-a12b-8da590537bbf"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LaunchGrenadeOne"",
                    ""type"": ""Button"",
                    ""id"": ""bf1582de-2107-4064-a060-d47e0793d626"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LaunchGrenadeTwo"",
                    ""type"": ""Button"",
                    ""id"": ""78a9ff65-d155-4e3e-8f15-60688caa5b1f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LaunchGrenadeThree"",
                    ""type"": ""Button"",
                    ""id"": ""0203e9f5-6103-4ffd-858b-c1db1715f26c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""9dfe3566-2f7b-4eab-9c95-b1a753e20d75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""86ef3f8a-2bff-4d76-88b8-dd1f02a9382c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelfCast"",
                    ""type"": ""Button"",
                    ""id"": ""94f726b7-ebb0-432a-8998-f0af00c8b5ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""226e2cde-ecd1-4133-9053-99e30fa8103e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShootCharged"",
                    ""type"": ""Button"",
                    ""id"": ""b56e676e-bbf2-4bba-a8f1-5246fe5a64cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a9c5b84a-2f60-4eb7-aec1-94a910e2ce1c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""99947bd8-942b-4bdd-8f45-9d1401df9349"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e6236bff-40f5-4d35-a2a6-6b78d9743256"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f7d7364-6a9a-4e4b-9260-1449ec83895a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7a949ea2-8aac-4eeb-b02e-996b83b97e2d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e8f63c06-1f7f-40c4-bc01-c4ebe8ec3e73"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da2eb18a-b0bd-4794-a61e-ea94067fa4e8"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Scrollwheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9530eb38-092d-414d-889b-dcc52252ce30"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LaunchGrenadeOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2507d75-767f-4274-824d-e9402fa5a247"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LaunchGrenadeTwo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8cf477d-53eb-4106-98d6-5031b62c6a1a"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LaunchGrenadeThree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2207f383-74e8-40c8-8447-8e2ad704380c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""664f6a90-355d-49f5-a5a1-520530768c84"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c071abb2-c70a-4b2f-8442-394d2adb263d"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SelfCast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""076ed7c5-f534-47a7-ba05-a1e868f2119f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f15bbdaf-3b7e-4a97-a9f8-e04c940a2dd2"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ShootCharged"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": []
        }
    ]
}");
        // CharacterControls
        m_CharacterControls = asset.FindActionMap("CharacterControls", throwIfNotFound: true);
        m_CharacterControls_Movement = m_CharacterControls.FindAction("Movement", throwIfNotFound: true);
        m_CharacterControls_MousePos = m_CharacterControls.FindAction("MousePos", throwIfNotFound: true);
        m_CharacterControls_Scrollwheel = m_CharacterControls.FindAction("Scrollwheel", throwIfNotFound: true);
        m_CharacterControls_LaunchGrenadeOne = m_CharacterControls.FindAction("LaunchGrenadeOne", throwIfNotFound: true);
        m_CharacterControls_LaunchGrenadeTwo = m_CharacterControls.FindAction("LaunchGrenadeTwo", throwIfNotFound: true);
        m_CharacterControls_LaunchGrenadeThree = m_CharacterControls.FindAction("LaunchGrenadeThree", throwIfNotFound: true);
        m_CharacterControls_Dash = m_CharacterControls.FindAction("Dash", throwIfNotFound: true);
        m_CharacterControls_Cancel = m_CharacterControls.FindAction("Cancel", throwIfNotFound: true);
        m_CharacterControls_SelfCast = m_CharacterControls.FindAction("SelfCast", throwIfNotFound: true);
        m_CharacterControls_Shoot = m_CharacterControls.FindAction("Shoot", throwIfNotFound: true);
        m_CharacterControls_ShootCharged = m_CharacterControls.FindAction("ShootCharged", throwIfNotFound: true);
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

    // CharacterControls
    private readonly InputActionMap m_CharacterControls;
    private ICharacterControlsActions m_CharacterControlsActionsCallbackInterface;
    private readonly InputAction m_CharacterControls_Movement;
    private readonly InputAction m_CharacterControls_MousePos;
    private readonly InputAction m_CharacterControls_Scrollwheel;
    private readonly InputAction m_CharacterControls_LaunchGrenadeOne;
    private readonly InputAction m_CharacterControls_LaunchGrenadeTwo;
    private readonly InputAction m_CharacterControls_LaunchGrenadeThree;
    private readonly InputAction m_CharacterControls_Dash;
    private readonly InputAction m_CharacterControls_Cancel;
    private readonly InputAction m_CharacterControls_SelfCast;
    private readonly InputAction m_CharacterControls_Shoot;
    private readonly InputAction m_CharacterControls_ShootCharged;
    public struct CharacterControlsActions
    {
        private @PlayerControls m_Wrapper;
        public CharacterControlsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_CharacterControls_Movement;
        public InputAction @MousePos => m_Wrapper.m_CharacterControls_MousePos;
        public InputAction @Scrollwheel => m_Wrapper.m_CharacterControls_Scrollwheel;
        public InputAction @LaunchGrenadeOne => m_Wrapper.m_CharacterControls_LaunchGrenadeOne;
        public InputAction @LaunchGrenadeTwo => m_Wrapper.m_CharacterControls_LaunchGrenadeTwo;
        public InputAction @LaunchGrenadeThree => m_Wrapper.m_CharacterControls_LaunchGrenadeThree;
        public InputAction @Dash => m_Wrapper.m_CharacterControls_Dash;
        public InputAction @Cancel => m_Wrapper.m_CharacterControls_Cancel;
        public InputAction @SelfCast => m_Wrapper.m_CharacterControls_SelfCast;
        public InputAction @Shoot => m_Wrapper.m_CharacterControls_Shoot;
        public InputAction @ShootCharged => m_Wrapper.m_CharacterControls_ShootCharged;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControlsActions instance)
        {
            if (m_Wrapper.m_CharacterControlsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMovement;
                @MousePos.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMousePos;
                @MousePos.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMousePos;
                @MousePos.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnMousePos;
                @Scrollwheel.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnScrollwheel;
                @Scrollwheel.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnScrollwheel;
                @Scrollwheel.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnScrollwheel;
                @LaunchGrenadeOne.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeOne;
                @LaunchGrenadeOne.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeOne;
                @LaunchGrenadeOne.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeOne;
                @LaunchGrenadeTwo.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeTwo;
                @LaunchGrenadeTwo.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeTwo;
                @LaunchGrenadeTwo.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeTwo;
                @LaunchGrenadeThree.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeThree;
                @LaunchGrenadeThree.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeThree;
                @LaunchGrenadeThree.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnLaunchGrenadeThree;
                @Dash.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnDash;
                @Cancel.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnCancel;
                @SelfCast.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnSelfCast;
                @SelfCast.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnSelfCast;
                @SelfCast.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnSelfCast;
                @Shoot.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
                @ShootCharged.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShootCharged;
                @ShootCharged.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShootCharged;
                @ShootCharged.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShootCharged;
            }
            m_Wrapper.m_CharacterControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @MousePos.started += instance.OnMousePos;
                @MousePos.performed += instance.OnMousePos;
                @MousePos.canceled += instance.OnMousePos;
                @Scrollwheel.started += instance.OnScrollwheel;
                @Scrollwheel.performed += instance.OnScrollwheel;
                @Scrollwheel.canceled += instance.OnScrollwheel;
                @LaunchGrenadeOne.started += instance.OnLaunchGrenadeOne;
                @LaunchGrenadeOne.performed += instance.OnLaunchGrenadeOne;
                @LaunchGrenadeOne.canceled += instance.OnLaunchGrenadeOne;
                @LaunchGrenadeTwo.started += instance.OnLaunchGrenadeTwo;
                @LaunchGrenadeTwo.performed += instance.OnLaunchGrenadeTwo;
                @LaunchGrenadeTwo.canceled += instance.OnLaunchGrenadeTwo;
                @LaunchGrenadeThree.started += instance.OnLaunchGrenadeThree;
                @LaunchGrenadeThree.performed += instance.OnLaunchGrenadeThree;
                @LaunchGrenadeThree.canceled += instance.OnLaunchGrenadeThree;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @SelfCast.started += instance.OnSelfCast;
                @SelfCast.performed += instance.OnSelfCast;
                @SelfCast.canceled += instance.OnSelfCast;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @ShootCharged.started += instance.OnShootCharged;
                @ShootCharged.performed += instance.OnShootCharged;
                @ShootCharged.canceled += instance.OnShootCharged;
            }
        }
    }
    public CharacterControlsActions @CharacterControls => new CharacterControlsActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface ICharacterControlsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
        void OnScrollwheel(InputAction.CallbackContext context);
        void OnLaunchGrenadeOne(InputAction.CallbackContext context);
        void OnLaunchGrenadeTwo(InputAction.CallbackContext context);
        void OnLaunchGrenadeThree(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnSelfCast(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnShootCharged(InputAction.CallbackContext context);
    }
}
