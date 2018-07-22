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
        private float speed;

        public Vector3 Position { get; private set; }
        public Quaternion UpRotation { get; private set; }
        public bool IsMoving {
            get {
                return moveTimer > 0;
            }
        }

        public TitanMover(Planet controlPlanet, Vector3 position, float moveSpeed) {
            planet = controlPlanet;
            Position = position;
            UpRotation = GetUpRotation(Position);
            endRotation = UpRotation;
            speed = moveSpeed;
        }

        public void MoveTo(Vector3 position) {
            var startPosition = Position;
            endPosition = position;
            moveAxe = -Utils.GetNormal(startPosition, endPosition, Vector3.zero);
            angle = Vector3.Angle(startPosition, endPosition);
            endRotation = Quaternion.AngleAxis(-angle, moveAxe) * UpRotation;
            Debug.Assert(!float.IsNaN(endPosition.x));
            Debug.Assert(!float.IsNaN(endPosition.y));
            Debug.Assert(!float.IsNaN(endPosition.z));
            float distance = angle / 180f * Mathf.PI * 2f * planet.Radius;
            fullTime = distance / speed;
            moveTimer = fullTime;
        }

        public void Update(float deltaTime) {
            moveTimer -= deltaTime;
            if (moveTimer < 0) {
                moveTimer = 0;
            }
            AnimateMove(moveTimer / fullTime);
        }

        private void AnimateMove(float normalizedTime) {
            var rotation = Quaternion.AngleAxis(angle * normalizedTime, moveAxe);
            Position = rotation * endPosition;
            UpRotation = rotation * endRotation;
        }

        private Quaternion GetUpRotation(Vector3 position) {
            return Quaternion.LookRotation(position) * Quaternion.Euler(90, 0, 0);
        }
    }
}
