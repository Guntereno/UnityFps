using InControl;
using UnityEngine;


namespace Shared
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField]
        CharacterController m_characterController;

        [SerializeField]
        Animator m_animator;

        [SerializeField]
        Camera m_camera;

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
        private float m_minPitch;

        [SerializeField]
        private float m_maxPitch;

        [SerializeField]
        private float m_sprintThresholdSq;


        static readonly int kParamIsCrouching = Animator.StringToHash("IsCrouching");
        static readonly int kParamIsMoving = Animator.StringToHash("IsMoving");
        static readonly int kParamIsSprinting = Animator.StringToHash("IsSprinting");
        static readonly int kParamIsJumping = Animator.StringToHash("IsJumping");


        private float m_cameraPitch;
        private float m_ySpeed;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            InputDevice device = InputManager.ActiveDevice;

            bool isCrouching = m_animator.GetBool(kParamIsCrouching);
            bool isSprinting = m_animator.GetBool(kParamIsSprinting);

            if (device.Action2.WasPressed)
            {
                isCrouching = !isCrouching;
                isSprinting = false;
            }

            // Translation
            var movementInput = device.LeftStick.Vector;
            float movementInputMagSq = movementInput.sqrMagnitude;
            bool isMoving = (movementInputMagSq > 0.0f);

            if (movementInputMagSq > m_sprintThresholdSq)
            {
                if (device.LeftStickButton.WasPressed)
                {
                    isSprinting = !isSprinting;
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

            Vector3 speed = transform.forward * movementInput.y * m_parallelSpeed;
            speed += transform.right * movementInput.x * m_strafeSpeed;

            if (isSprinting)
            {
                speed *= m_sprintSpeedFactor;
            }
            else if(isCrouching)
            {
                speed *= m_crouchSpeedFactor;
            }

            m_ySpeed += Physics.gravity.y * Time.deltaTime;
            if (m_characterController.isGrounded)
            {
                if (device.Action1)
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
            float yawAxis = device.RightStick.X;
            transform.Rotate(0.0f, yawAxis * m_mouseSensitivity, 0.0f);

            float pitchAxis = device.RightStick.Y;
            m_cameraPitch -= pitchAxis * m_mouseSensitivity;
            m_cameraPitch = Mathf.Clamp(m_cameraPitch, m_minPitch, m_maxPitch);
            m_camera.transform.localRotation = Quaternion.Euler(m_cameraPitch, 0.0f, 0.0f);
        }
    }
}
