using InControl;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    Animator m_animator;

    [SerializeField]
    ParticleSystem m_gunshotParticles;

    static readonly int kParamTriggerAmount = Animator.StringToHash("TriggerAmount");

	void Update ()
    {
        InputDevice device = InputManager.ActiveDevice;
        float triggerAmount = device.RightTrigger.Value;
        m_animator.SetFloat(kParamTriggerAmount, triggerAmount);
    }

    public void Fire()
    {
        m_gunshotParticles.Play();
    }
}
