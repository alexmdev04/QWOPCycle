using System;
using QWOPCycle.Gameplay;
using QWOPCycle.Persistence;
using QWOPCycle.Scoring;
using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;
using UnityEngine.UIElements;

namespace QWOPCycle.Interface {
    [RequireComponent(typeof(UIDocument))]
    public sealed class HUDController : MonoBehaviour {
        private UIDocument _doc;
        private Label _runTimeLabel;
        private Label _currentDistanceLabel;
        private Label _bestDistanceLabel;
        private Label _scoreLabel;
        private Label _warningLabel;
        private Label _levelIncreaseLabel;
        private Button _tutorialButton;
        private Button _tutorialPanel;
        private bool _showTutorial;
        private float _warningLerpValue;
        private bool _warningLerpUp;
        private float _levelIncreaseLabelTimer;
        private ProgressBar _pedalPowerBar;
        private bool _gameOver;

        private EventBinding<LevelIncreaseEvent> _levelIncreaseBinding;
        private EventBinding<GameOver> _gameOverBinding;
        private EventBinding<GameReset> _gameResetBinding;

        [SerializeField] private ScoreTracker _scoreTracker;
        [SerializeField] private PedalTracker _pedalTracker;

        [SerializeField] private Color warningBaseColor = Color.cyan;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Percentage of max pedal power you must be below to display the warning")]
        private float warningVisiblePercent = 0.4f;

        private void Awake() {
            _doc = GetComponent<UIDocument>();
            _currentDistanceLabel = _doc.rootVisualElement.Q<Label>("current-distance");
            _bestDistanceLabel = _doc.rootVisualElement.Q<Label>("best-distance");
            _scoreLabel = _doc.rootVisualElement.Q<Label>("score");
            _warningLabel = _doc.rootVisualElement.Q<Label>("warning");
            _runTimeLabel = _doc.rootVisualElement.Q<Label>("run-time");
            _levelIncreaseLabel = _doc.rootVisualElement.Q<Label>("level-increase");
            _pedalPowerBar = _doc.rootVisualElement.Q<ProgressBar>("pedal-power");
        }

        private void Start() {
            if (_scoreTracker == null) {
                Log.Error("HUDController: ScoreTracker is null");
                enabled = false;
            }

            if (_pedalTracker == null) {
                Log.Error("HUDController: PedalTracker is null");
                enabled = false;
            }
        }

        private void OnEnable() {
            _levelIncreaseBinding = new EventBinding<LevelIncreaseEvent>(OnLevelIncrease);
            EventBus<LevelIncreaseEvent>.Register(_levelIncreaseBinding);

            _gameOverBinding = new EventBinding<GameOver>(OnGameOver);
            EventBus<GameOver>.Register(_gameOverBinding);

            _gameResetBinding = new EventBinding<GameReset>(OnGameReset);
            EventBus<GameReset>.Register(_gameResetBinding);

            _tutorialButton = _doc.rootVisualElement.Q<Button>("tutorial-button");
            _tutorialPanel = _doc.rootVisualElement.Q<Button>("tutorial-panel");
            _tutorialButton.clicked += ToggleTutorialPanel;
            _tutorialPanel.clicked += ToggleTutorialPanel;
        }

        private void OnDisable() {
            EventBus<LevelIncreaseEvent>.Deregister(_levelIncreaseBinding);
            EventBus<GameOver>.Deregister(_gameOverBinding);
            EventBus<GameReset>.Deregister(_gameResetBinding);
            _tutorialButton.clicked -= ToggleTutorialPanel;
            _tutorialPanel.clicked -= ToggleTutorialPanel;
        }

        private void LateUpdate() {
            _currentDistanceLabel.text = $"{_scoreTracker.DistanceTravelled:N1}m";
            _scoreLabel.text = $"{System.Math.Round(_scoreTracker.Score)} points";
            float bestDistance = Mathf.Max(SaveDataManager.Instance.Save.BestDistance, _scoreTracker.DistanceTravelled);
            _bestDistanceLabel.text = $"Best Distance: {bestDistance:N1}m";
            _runTimeLabel.text = $@"{_scoreTracker.RunTime:mm\:ss}";
            _pedalPowerBar.value = _pedalTracker.PedalPowerRatio;

            WarningLabelTick();
            LevelIncreaseLabelTick();
        }

        private void ToggleTutorialPanel() {
            _showTutorial = !_showTutorial;
            _tutorialPanel.visible = _showTutorial;
            _tutorialButton.visible = !_showTutorial;
        }

        private void WarningLabelTick() {
            // check if warning label should be visible
            _warningLabel.visible =
                _pedalTracker.PedalPower < warningVisiblePercent * _pedalTracker.MaxPedalPower && !_gameOver;
            if (!_warningLabel.visible) return;

            // animate color
            float animationDelta = Time.deltaTime * 3f;
            if (_warningLerpUp) {
                if (_warningLerpValue >= 1f) _warningLerpUp = false;
                else _warningLerpValue += animationDelta;
            }
            else {
                if (_warningLerpValue <= 0f) _warningLerpUp = true;
                else _warningLerpValue -= animationDelta;
            }

            _warningLabel.style.color = new StyleColor(
                new Color(warningBaseColor.r, warningBaseColor.g, warningBaseColor.b, _warningLerpValue)
            );
        }

        private void LevelIncreaseLabelTick() {
            // toggles level increase label if the timer is greater than 0
            bool levelIncreaseLabelEnabled = _levelIncreaseLabelTimer > 0f;
            if (levelIncreaseLabelEnabled) _levelIncreaseLabelTimer -= Time.deltaTime;
            _levelIncreaseLabel.visible = levelIncreaseLabelEnabled;
        }

        private void OnLevelIncrease(LevelIncreaseEvent e) {
            _levelIncreaseLabel.text = $"Level {e.Level.ToString()}";
            _levelIncreaseLabelTimer = 3f;
        }

        private void OnGameOver() {
            _gameOver = true;
        }

        private void OnGameReset() {
            _gameOver = false;
        }
    }
}
