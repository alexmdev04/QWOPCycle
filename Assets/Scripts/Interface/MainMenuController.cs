using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QWOPCycle.Interface {
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuController : MonoBehaviour {
        [field: SerializeField] private GameplayScene _gameplayScene;

        private UIDocument _doc;
        private Button _startButton;
        private Button _settingsButton;
        private Button _exitButton;

        private void Awake() {
            _doc = GetComponent<UIDocument>();
        }

        private void OnEnable() {
            _startButton = _doc.rootVisualElement.Q<Button>("start-game");
            _settingsButton = _doc.rootVisualElement.Q<Button>("options");
            _exitButton = _doc.rootVisualElement.Q<Button>("exit");
            _startButton.clicked += StartButtonPressed;
            _settingsButton.clicked += SettingsButtonPressed;
            _exitButton.clicked += ExitButtonPressed;

#if UNITY_WEBGL
            _optionsButton.style.display = DisplayStyle.None;
            _exitButton.style.display = DisplayStyle.None;
#endif
        }

        private void StartButtonPressed() {
            Debug.Log("MainMenuController : Loading main gameplay scene!");
            EventBus<LoadRequest>.Raise(new LoadRequest(_gameplayScene));
        }

        private void SettingsButtonPressed() {
            Debug.Log("MainMenuController : Opening settings menu!");
        }

        private void ExitButtonPressed() {
            Debug.Log("MainMenuController : Exiting QWOPCycle!");

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
