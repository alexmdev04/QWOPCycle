using QWOPCycle.Scoring;
using Unity.Logging;
using UnityEngine;
using UnityEngine.UIElements;

namespace QWOPCycle.Interface {
    [RequireComponent(typeof(UIDocument))]
    public sealed class HUDController : MonoBehaviour {
        private UIDocument _doc;
        private Label _currentDistanceLabel;
        private Label _bestDistanceLabel;
        private Label _scoreLabel;

        [SerializeField] private ScoreTracker _scoreTracker;

        private string CurrentScore => _scoreTracker.Score.ToString("#.#");

        private void Awake() {
            _doc = GetComponent<UIDocument>();
            _currentDistanceLabel = _doc.rootVisualElement.Q<Label>("current-distance");
            _bestDistanceLabel = _doc.rootVisualElement.Q<Label>("best-distance");
            _scoreLabel = _doc.rootVisualElement.Q<Label>("score");
        }

        private void Start() {
            if (_scoreTracker == null) {
                Log.Error("HUDController: ScoreTracker is null");
                enabled = false;
            }
        }

        private void LateUpdate() {
            _currentDistanceLabel.text = $"{_scoreTracker.DistanceTravelled:N1}m";
            // _bestDistanceLabel.text = _scoreTracker.BestDistance.ToString();
            _scoreLabel.text = _scoreTracker.Score.ToString();
            // _doc.rootVisualElement.MarkDirtyRepaint();
        }
    }
}
