using System.Collections.Generic;
using SideFX.Anchors;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

namespace QWOPCycle.Gameplay {
    public class Block : MonoBehaviour {
        public Obstacle obstaclePrefab;
        public GameManagerAnchor gameManagerAnchor;
        public List<Obstacle> obstacles;
        public double laneObstacleSpawnChance = 0.2f; // max value is 1
        public float obstacleCenterDistanceVariance = 0.2f; // max value is 0.5,
        public int emptyLanes = 1;

        private Random _random;

        private double random0to1 => _random.NextDouble();

        private void Awake() {
            _random = new Random();
        }

        private void Update() {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) obstacles = CreateRandomObstacles();
        }

        /// <summary>
        /// Spawns random obstacles within the given number of tracks,
        /// trackSpawnChance is a percentage out of 100 to spawn in a single track,
        /// there must always be at least one track without an obstacle
        /// </summary>
        public List<Obstacle> CreateRandomObstacles() {
            int lanesWithObstacles = 0;
            float leftOfBlock = 0f - gameManagerAnchor.Value.BlockWidth / 2;
            List<Obstacle> newObstacles = new List<Obstacle>();
            for (int i = 0; i < gameManagerAnchor.Value.blockLanes; i++) {
                if (lanesWithObstacles >= gameManagerAnchor.Value.blockLanes - emptyLanes)
                    break; // max filled lanes reached
                double laneObstacleSpawnValue = random0to1;
                if (laneObstacleSpawnValue > laneObstacleSpawnChance) continue;
                newObstacles.Add(
                    SpawnObstacle(
                        i,
                        gameManagerAnchor.Value.BlockLaneWidth,
                        leftOfBlock,
                        obstacleCenterDistanceVariance
                    )
                );
                lanesWithObstacles++;
            }

            return newObstacles;
        }

        private Obstacle SpawnObstacle(int laneToSpawnIn, float laneWidth, float xOrigin, float distanceVariance) {
            float xPos = xOrigin + laneWidth / 2 + laneWidth * laneToSpawnIn;
            float zPos = transform.position.z
                         + gameManagerAnchor.Value.BlockLength * ((float)random0to1 * distanceVariance);
            Obstacle newObstacle = Instantiate(obstaclePrefab);
            newObstacle.transform.position = new Vector3(
                xPos,
                newObstacle.transform.localScale.y,
                zPos
            );
            newObstacle.transform.parent = transform;
            return newObstacle;
        }
    }
}
