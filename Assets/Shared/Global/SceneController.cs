using System.Collections;
using Assets.Shared.Global;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Shared
{ 
    public abstract class SceneController : MonoBehaviour
    {
        private void Awake()
        {
            if(GlobalScene.Instance != null)
            {
                Init();
            }
            else
            {
                StartCoroutine(LoadGlobalScene());
            }
        }

        private IEnumerator LoadGlobalScene()
        {
            SceneManager.LoadScene("GlobalScene", LoadSceneMode.Additive);

            while(GlobalScene.Instance == null)
            {
                yield return null;
            }

            Init();
        }

        public abstract void Init();
    }
}
 
