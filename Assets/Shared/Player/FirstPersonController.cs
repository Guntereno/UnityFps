using Assets.Levels;
using Assets.Weapons;
using Momo.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Shared
{
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Components")]

        [SerializeField]
        CharacterController m_characterController;

        [SerializeField]
        Animator m_animator;

        [SerializeField]
        Camera m_camera;

        [SerializeField]
        Transform m_handTransform;


        [Header("Parameters")]

        [SerializeField]
        private float m_strafeSpeed;

        [SerializeField]
        private float m_parallelSpeed;

        [SerializeField]
        private float m_mouseSensitivity;

        [SerializeField]
        private float m_jumpSpeed;

        [SerializeField]
        private float m_sprintSpeedFactor;

        [SerializeField]
        private float m_crouchSpeedFactor;

        [SerializeField]
        private float m_aimingSpeedFactor;

        [SerializeField]
        private float m_minPitch;

        [SerializeField]
        private float m_maxPitch;

        [SerializeField]
        private float m_sprintThresholdSq;


        static readonly int kParamIsCrouching = Animator.StringToHash("IsCrouching");
        static readonly int kParamIsMoving = Animator.StringToHash("IsMoving");
        static readonly int kParamIsSprinting = Animator.StringToHash("IsSprinting");
        static readonly int kParamIsJumping = Animator.StringToHash("IsJumping");
        static readonly int kParamIsAiming = Animator.StringToHash("IsAiming");

        private bool m_initialised = false;

        private float m_cameraPitch;
        private float m_ySpeed;

        private WeaponController m_weapon = null;


		#region Unity Events

		private void Start()
        {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
        }

        public void Init(LevelPrefabs prefabs)
        {
            m_weapon = GameObject.Instantiate(prefabs.HandCannon);
            m_weapon.transform.SetParent(m_handTransform, false);

            m_initialised = true;
        }

        void Update()
        {
			if (!m_initialised)
			{
				return;
			}

			bool isCrouching = m_animator.GetBool(kParamIsCrouching);
			bool isSprinting = m_animator.GetBool(kParamIsSprinting);

			bool isAiming = InputAimDownSights;

			isCrouching = InputCrouch;
			if(isCrouching)
			{
				isSprinting = false;
			}

			// Translation
			var movementInput = _inputMoveVector;
			float movementInputMagSq = movementInput.sqrMagnitude;
			bool isMoving = (movementInputMagSq > 0.0f);

			if ((movementInputMagSq > m_sprintThresholdSq) & !isAiming)
			{
				isSprinting = InputSprint;
				if (isSprinting)
				{
					isCrouching = false;
				}
			}
			else
			{
				isSprinting = false;
			}

			m_animator.SetBool(kParamIsSprinting, isSprinting);
			m_animator.SetBool(kParamIsMoving, isMoving);
			m_animator.SetBool(kParamIsCrouching, isCrouching);
			m_animator.SetBool(kParamIsAiming, isAiming);

			m_weapon.TriggerAmount = (!isSprinting) ? _inputFireAmount : 0.0f;

			Vector3 speed = transform.forward * movementInput.y * m_parallelSpeed;
			speed += transform.right * movementInput.x * m_strafeSpeed;

			if (isSprinting)
			{
				speed *= m_sprintSpeedFactor;
			}
			else if (isCrouching)
			{
				speed *= m_crouchSpeedFactor;
			}
			else if (isAiming)
			{
				speed *= m_aimingSpeedFactor;
			}

			m_ySpeed += Physics.gravity.y * Time.deltaTime;
			if (m_characterController.isGrounded)
			{
				if (InputJump)
				{
					m_ySpeed = m_jumpSpeed;
					m_animator.SetBool(kParamIsJumping, true);
				}
				else
				{
					m_animator.SetBool(kParamIsJumping, false);
				}
			}

			speed += new Vector3(0.0f, m_ySpeed, 0.0f);

			m_characterController.Move(speed * Time.deltaTime);

			// Orientation
			float yawAxis = _inputLookVector.x;
			transform.Rotate(0.0f, yawAxis * m_mouseSensitivity, 0.0f);

			float pitchAxis = _inputLookVector.y;
			m_cameraPitch -= pitchAxis * m_mouseSensitivity;
			m_cameraPitch = Mathf.Clamp(m_cameraPitch, m_minPitch, m_maxPitch);
			m_camera.transform.localRotation = Quaternion.Euler(m_cameraPitch, 0.0f, 0.0f);

			_inputSignals = InputSignals.None;
		}

		void OnGUI()
		{
#if UNITY_EDITOR
			GUI.Label(new Rect(25, 25, 180, 20), $"Move Vector: {_inputMoveVector}");
			GUI.Label(new Rect(25, 45, 180, 20), $"Look Vector: {_inputLookVector}");
			GUI.Label(new Rect(25, 65, 180, 20), $"Fire Amount: {_inputFireAmount}");
			GUI.Label(new Rect(25, 85, 180, 20), $"Input Signals: {_inputSignals}");
			GUI.Label(new Rect(25, 105, 180, 20), $"Input Flags: {_inputFlags}");
#endif
		}

		#endregion


		#region Input Events

		[System.Flags]
		private enum InputSignals
		{
			None = 0,

			Jump = 1 << 0
		}

		[System.Flags]
		private enum InputFlags
		{
			None = 0,

			Crouch = 1 << 0,
			AimDownSights = 1 << 1,
			Sprint = 1 << 2
		}

		private Vector2 _inputMoveVector;
		private Vector2 _inputLookVector;
		private float _inputFireAmount;
		private Flags32<InputSignals> _inputSignals;
		private Flags32<InputFlags> _inputFlags;

		private bool InputCrouch
		{
			get => _inputFlags.TestAll(InputFlags.Crouch);
			set => _inputFlags.Assign(InputFlags.Crouch, value);
		}

		private bool InputAimDownSights
		{
			get => _inputFlags.TestAll(InputFlags.AimDownSights);
			set => _inputFlags.Assign(InputFlags.AimDownSights, value);
		}

		private bool InputSprint
		{
			get => _inputFlags.TestAll(InputFlags.Sprint);
			set => _inputFlags.Assign(InputFlags.Sprint, value);
		}

		private bool InputJump
		{
			get => _inputSignals.TestAll(InputSignals.Jump);
			set => _inputSignals.Assign(InputSignals.Jump, value);
		}

		public void OnMove(InputValue value)
		{
			_inputMoveVector = value.Get<Vector2>();
		}

		public void OnLook(InputValue value)
		{
			_inputLookVector = value.Get<Vector2>();
		}

		public void OnFire(InputValue value)
		{
			_inputFireAmount = value.Get<float>();
		}

		public void OnCrouch(InputValue value)
		{
			InputCrouch = value.Get<float>() > 0.0f;
			Debug.Log($"Crouch {value.Get<float>()}");
		}

		public void OnAimDownSights(InputValue value)
		{
			InputAimDownSights = value.Get<float>() > 0.0f;
		}

		public void OnSprint(InputValue value)
		{
			InputSprint = value.Get<float>() > 0.0f;
		}

		public void OnJump(InputValue value)
		{
			InputJump = value.isPressed;
		}

		#endregion
	}
}
