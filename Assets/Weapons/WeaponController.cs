using UnityEngine;

namespace Assets.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField]
        Animator m_animator;

        [SerializeField]
        ParticleSystem m_gunshotParticles;

        [SerializeField]
        Transform m_muzzleTransform;

        public delegate void FiredHandler(Transform muzzleLocation);
        public event FiredHandler Fired;


        static readonly int kParamTriggerAmount = Animator.StringToHash("TriggerAmount");

        public float TriggerAmount
        {
            set
            {
                value = Mathf.Clamp01(value);
                m_animator.SetFloat(kParamTriggerAmount, value);
            }
        }

        public void Fire()
        {
            m_gunshotParticles.Play();

            if (Fired != null)
            {
                Fired(m_muzzleTransform);
            }
        }
    }
}

