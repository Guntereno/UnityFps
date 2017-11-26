using Assets.Shared;
using UnityEngine;

namespace Assets.Levels
{
    public class LevelController : SceneController
    {
        [SerializeField]
        LevelPrefabs m_prefabs;

        public override void Init()
        {
            var player = GameObject.Instantiate<FirstPersonController>(m_prefabs.Player);
            player.Init(m_prefabs);
        }
    }
}