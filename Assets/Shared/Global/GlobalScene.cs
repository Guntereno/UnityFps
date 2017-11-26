using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Shared.Global
{
    class GlobalScene: MonoBehaviour
    {
        public static GlobalScene Instance
        {
            get;
            private set;
        }

        private void Awake()
        {
            Assert.IsTrue(Instance == null);

            GameObject.DontDestroyOnLoad(gameObject);

            Instance = this;
        }
    }
}
