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
        private Button _tutorialButton;
        private Button _tutorialPanel;
        private bool _showTutorial;

        [SerializeField] private ScoreTracker _scoreTracker;

        private void Awake() {
            _doc = GetComponent<UIDocument>();
            _currentDistanceLabel = _doc.rootVisualElement.Q<Label>("current-distance");
            _bestDistanceLabel = _doc.rootVisualElement.Q<Label>("best-distance");
            _scoreLabel = _doc.rootVisualElement.Q<Label>("score");
            _runTimeLabel = _doc.rootVisualElement.Q<Label>("run-time");
        }

        private void Start() {
            if (_scoreTracker == null) {
                Log.Error("HUDController: ScoreTracker is null");
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
            _runTimeLabel.text = $@"Run Time: {_scoreTracker.RunTime:mm\:ss}";
        }

        private void ToggleTutorialPanel() {
            _showTutorial = !_showTutorial;
            _tutorialPanel.visible = _showTutorial;
            _tutorialButton.visible = !_showTutorial;
        }
    }
}
