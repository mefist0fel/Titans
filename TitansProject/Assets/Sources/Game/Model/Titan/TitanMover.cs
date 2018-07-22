using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [System.Serializable]
    public sealed class TitanMover {
        private readonly Planet planet;

        private Vector3 moveAxe;
        private float moveTimer = 0;
        private float fullTime = 0;
        private float angle;

        private Quaternion endRotation;

        private Vector3 endPosition;
        private float maxSpeed;

        public Vector3 Position { get; private set; }
        public Quaternion UpRotation { get; private set; }
        public bool IsMoving {
            get {
                return moveTimer > 0;
            }
        }
        private BesieCurve curve;

        public TitanMover(Planet controlPlanet, Vector3 position, float moveSpeed) {
            planet = controlPlanet;
            Position = position;
            UpRotation = GetUpRotation(Position);
            endRotation = UpRotation;
            maxSpeed = moveSpeed;
        }

        public void MoveTo(Vector3 position) {
            var startPosition = Position;
            endPosition = position;
            var startId = planet.Graph.FindNearestId(startPosition);
            var endId = planet.Graph.FindNearestId(endPosition);
            var path = planet.Graph.FindPath(startId, endId);
            NormalizePath(path);
            path[0] = endPosition;
            path[path.Count - 1] = startPosition;
            curve = new BesieCurve(path.ToArray());

            float distance = curve.Lenght;
            fullTime = curve.Lenght / maxSpeed;
            moveTimer = fullTime;
            Debug.Assert(!float.IsNaN(endPosition.x));
            Debug.Assert(!float.IsNaN(endPosition.y));
            Debug.Assert(!float.IsNaN(endPosition.z));
        }

        private void NormalizePath(List<Vector3> path, float maxDistance = 0.3f) {
            for (int i = 0; i < path.Count - 1; i++) {
                var distance = Vector3.Distance(path[i], path[i + 1]);
                if (distance > 0.3f) {
                    var middlePoint = ((path[i] + path[i + 1]) * 0.5f).normalized * planet.Radius;
                    path.Insert(i + 1, middlePoint);
                    i += 2;
                }
            }
        }

        public void Update(float deltaTime) {
            moveTimer -= deltaTime;
            if (moveTimer < 0) {
                moveTimer = 0;
            }
            AnimateMove(moveTimer / fullTime);
        }

        private void AnimateMove(float normalizedTime) {
            float movedDistance = 0;
            if (curve.Lenght > 0)
                movedDistance = normalizedTime * curve.Lenght;
            Position = curve.GetPositionOnCurve(movedDistance);
            UpRotation = GetUpRotation(Position);
        }

        private Quaternion GetUpRotation(Vector3 position) {
            return Quaternion.LookRotation(position) * Quaternion.Euler(90, 0, 0);
        }

#if UNITY_EDITOR
        public void DrawGizmos() {
            if (curve == null)
                return;
            Gizmos.color = Color.red;
            for (int i = 0; i < curve.PathPoints.Length - 1; i++) {
                Gizmos.DrawLine(curve.PathPoints[i], curve.PathPoints[i + 1]);
            }
        }
#endif
    }
}
