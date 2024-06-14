using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface {
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuController : MonoBehaviour {
        [field: SerializeField] private GameplayScene _gameplayScene;

        private UIDocument _doc;
        private Button _startButton;
        private Button _optionsButton;
        private Button _exitButton;

        private void Awake() {
            _doc = GetComponent<UIDocument>();
        }

        private void OnEnable() {
            _startButton = _doc.rootVisualElement.Q<Button>("start-game");
            _optionsButton = _doc.rootVisualElement.Q<Button>("options");
            _exitButton = _doc.rootVisualElement.Q<Button>("exit");
            _startButton.clicked += StartButtonPressed;
            _optionsButton.clicked += OptionsButtonPressed;
            _exitButton.clicked += ExitButtonPressed;
        }

        private void StartButtonPressed() {
            Debug.Log("Loading main gameplay scene!");
            EventBus<LoadRequest>.Raise(new LoadRequest(_gameplayScene));
        }

        private void OptionsButtonPressed() {
            Debug.Log("Options Button Pressed!");
        }

        private void ExitButtonPressed() {
            Debug.Log("Exit Button Pressed!");
        }
    }
}
