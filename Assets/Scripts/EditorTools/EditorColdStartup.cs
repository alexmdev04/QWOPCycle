using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace QWOPCycle.EditorTools {
    public sealed class EditorColdStartup : MonoBehaviour {
#if UNITY_EDITOR
        [SerializeField] private GameplayScene _thisScene;
        [SerializeField] private PersistentManagersScene _persistentManagersScene;

        private bool _isColdStart;

        private void Awake() {
            if (!SceneManager.GetSceneByName(_persistentManagersScene.SceneReference.editorAsset.name).isLoaded)
                _isColdStart = true;
        }

        private void Start() {
            if (_isColdStart)
                _persistentManagersScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive).Completed +=
                    LoadEventChannel;
        }

        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj) {
            if (_thisScene != null)
                EventBus<LoadRequest>.Raise(new LoadRequest(_thisScene));
            else
                EventBus<SceneReady>.Raise(default);
        }
#endif
    }
}
