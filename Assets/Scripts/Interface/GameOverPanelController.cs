using System;
using QWOPCycle.Gameplay;
using QWOPCycle.Persistence;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;
using UnityEngine.UIElements;

namespace QWOPCycle.Interface {
    [RequireComponent(typeof(UIDocument))]
    public sealed class GameOverPanelController : MonoBehaviour {
        [SerializeField] private MainMenuScene _mainMenuScene;
        private UIDocument _doc;

        private Label _distanceLabel;
        private Label _scoreLabel;
        private Label _timeLabel;
        private Label _newBestLabel;

        private Button _restartButton;
        private Button _quitButton;

        private EventBinding<StartGameEvent> _gameStartBinding;
        private EventBinding<GameOverEvent> _gameOverBinding;
        private bool _hasNewBestScore;

        // label templates
        private const string T_Distance = "Distance Travelled: {0:N1}m";
        private const string T_Score = "Score: {0}";
        private const string T_Time = "Time: {0:mm\\:ss}";

        // toggleable style for new best scores
        private const string NewBestCssClass = "new-best";

        private void Awake() {
            _doc = GetComponent<UIDocument>();
            _doc.rootVisualElement.visible = false;
            _distanceLabel = _doc.rootVisualElement.Q<Label>("distance-travelled");
            _scoreLabel = _doc.rootVisualElement.Q<Label>("score");
            _timeLabel = _doc.rootVisualElement.Q<Label>("runtime");
            _newBestLabel = _doc.rootVisualElement.Q<Label>("new-best");
            _restartButton = _doc.rootVisualElement.Q<Button>("restart");
            _quitButton = _doc.rootVisualElement.Q<Button>("quit");
        }

        private void OnEnable() {
            _restartButton.clicked += OnRestartClicked;
            _quitButton.clicked += OnQuitClicked;

            _gameStartBinding = new EventBinding<StartGameEvent>(OnGameStart);
            _gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
            EventBus<StartGameEvent>.Register(_gameStartBinding);
            EventBus<GameOverEvent>.Register(_gameOverBinding);
        }

        private void OnDisable() {
            _restartButton.clicked -= OnRestartClicked;
            _quitButton.clicked -= OnQuitClicked;

            EventBus<StartGameEvent>.Deregister(_gameStartBinding);
            EventBus<GameOverEvent>.Deregister(_gameOverBinding);
        }

        private void OnGameStart(StartGameEvent e) {
            _doc.rootVisualElement.visible = false;
        }

        private void OnGameOver(GameOverEvent e) {
            SaveData save = SaveDataManager.Instance.Save;

            _hasNewBestScore = false;

            SetLabel(_distanceLabel, e.Distance, save.BestDistance, T_Distance);
            SetLabel(_scoreLabel,  Math.Floor(e.Score), Math.Floor(save.HighScore), T_Score);
            SetLabel(_timeLabel, e.RunTime, save.BestRunTime, T_Time);

            _newBestLabel.visible = _hasNewBestScore;
            _doc.rootVisualElement.visible = true;
        }

        private void OnRestartClicked() {
            Log.Debug("GameOver Interface : Restart clicked");
            EventBus<RestartGameEvent>.Raise(default);
            _doc.enabled = false;
        }

        private void OnQuitClicked() {
            Log.Debug("GameOver Interface : Quit clicked");
            EventBus<LoadRequest>.Raise(new LoadRequest(_mainMenuScene));
        }

        private void SetLabel<T>(Label label, T fromEvent, T fromSave, string template)
            where T : IComparable<T> {
            // if fromEvent >= fromSave, set new best to true
            bool newBest = fromEvent.CompareTo(fromSave) > 0;
            _hasNewBestScore |= newBest;

            label.text = string.Format(template, fromEvent);
            if (_hasNewBestScore) {
                label.EnableInClassList(NewBestCssClass, newBest);
                _newBestLabel.EnableInClassList(NewBestCssClass, _hasNewBestScore);
            }
            else { label.EnableInClassList(NewBestCssClass, false); }
        }
    }
}
