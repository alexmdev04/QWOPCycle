using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public class Obstacle : MonoBehaviour {
        public enum obstacleType { cube }

        private void Start() { }

        private void Update() { }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent(out QWOPCharacter player))
                Log.Debug(gameObject.name + " ran into " + other.gameObject.name);
        }
    }
}
