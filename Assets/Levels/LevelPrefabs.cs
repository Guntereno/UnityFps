using Assets.Shared;
using Assets.Weapons;
using UnityEngine;

namespace Assets.Levels
{
    public class LevelPrefabs : ScriptableObject
    {
        [SerializeField]
        private WeaponController m_handCannon;

        [SerializeField]
        private FirstPersonController m_player;

        public WeaponController HandCannon
        {
            get
            {
                return m_handCannon;
            }
        }

        public FirstPersonController Player
        {
            get
            {
                return m_player;
            }
        }

    }
}
