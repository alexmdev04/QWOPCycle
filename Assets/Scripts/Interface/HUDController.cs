using QWOPCycle.Persistence;
using QWOPCycle.Scoring;
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
        private Button _tutorialButton;
        private Button _tutorialPanel;
        private bool _showTutorial;
        private float _warningLerpValue;
        private bool _warningLerpUp;

        [SerializeField] private ScoreTracker _scoreTracker;
        [SerializeField] private PedalTracker _pedalTracker;

        [SerializeField] private Color warningBaseColor = Color.cyan;
        [SerializeField] [Range(0f, 1f)] [Tooltip("Percentage of max pedal power you must be below to display the warning")]
        private float warningVisiblePercent = 0.4f;

        private void Awake() {
            _doc = GetComponent<UIDocument>();
            _currentDistanceLabel = _doc.rootVisualElement.Q<Label>("current-distance");
            _bestDistanceLabel = _doc.rootVisualElement.Q<Label>("best-distance");
            _scoreLabel = _doc.rootVisualElement.Q<Label>("score");
            _warningLabel = _doc.rootVisualElement.Q<Label>("warning");
            _runTimeLabel = _doc.rootVisualElement.Q<Label>("run-time");
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
            _tutorialButton = _doc.rootVisualElement.Q<Button>("tutorial-button");
            _tutorialPanel = _doc.rootVisualElement.Q<Button>("tutorial-panel");
            _tutorialButton.clicked += ToggleTutorialPanel;
            _tutorialPanel.clicked += ToggleTutorialPanel;
        }

        private void OnDisable() {
            _tutorialButton.clicked -= ToggleTutorialPanel;
            _tutorialPanel.clicked -= ToggleTutorialPanel;
        }

        private void LateUpdate() {
            _currentDistanceLabel.text = $"{_scoreTracker.DistanceTravelled:N1}m";
            _scoreLabel.text = $"{_scoreTracker.Score} points";
            float bestDistance = SaveDataManager.Instance.Save.BestDistance > _scoreTracker.DistanceTravelled
                                     ? SaveDataManager.Instance.Save.BestDistance
                                     : _scoreTracker.DistanceTravelled;
            _bestDistanceLabel.text = $"Best Distance: {bestDistance:N1}m";
            _runTimeLabel.text = $@"{_scoreTracker.RunTime:mm\:ss}";
            _warningLabel.visible =
                _pedalTracker.PedalPower < warningVisiblePercent * _pedalTracker.MaxPedalPower;
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

        private void ToggleTutorialPanel() {
            _showTutorial = !_showTutorial;
            _tutorialPanel.visible = _showTutorial;
            _tutorialButton.visible = !_showTutorial;
        }
    }
}
