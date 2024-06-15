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
        private Random _random;
        private double random0to1 => _random.NextDouble();

        private void Awake() {
            _random = new Random();
        }

        private void Start() { }

        private void Update() {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) obstacles = CreateRandomObstacles();
        }

        /// <summary>
        /// Spawns random obstacles within the given number of tracks,
        /// trackSpawnChance is a percentage out of 100 to spawn in a single track,
        /// there must always be at least one track without an obstacle
        /// </summary>
        /// <param name="lanes"></param>
        /// <param name="laneObstacleSpawnChance"></param>
        /// <param name="emptyLanes"></param>
        private List<Obstacle> CreateRandomObstacles(
            int lanes = 4,
            double laneObstacleSpawnChance = 0.2f, // max value is 1
            float obstacleDistanceVariance = 0.2f, // max value is 0.5,
            // the area of the block from the center, forwards and backwards, where the block could be randomly placed
            int emptyLanes = 1
        ) {
            var lanesWithObstacles = 0;
            float leftOfBlock = 0f - gameManagerAnchor.Value.BlockWidth / 2;
            float laneWidth = gameManagerAnchor.Value.BlockWidth / lanes;
            var newObstacles = new List<Obstacle>();
            for (var i = 0; i < lanes; i++) {
                if (lanesWithObstacles >= lanes - emptyLanes)
                    // max filled lanes reached
                    break;
                double laneObstacleSpawnValue = random0to1;
                if (laneObstacleSpawnValue <= laneObstacleSpawnChance) {
                    newObstacles.Add(SpawnObstacle(i, laneWidth, leftOfBlock, obstacleDistanceVariance));
                    lanesWithObstacles++;
                }
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
