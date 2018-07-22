using UnityEngine;

namespace Model {
    public sealed class MoveTask : Titan.Task {
        public override Vector3 Position { get { return position; } }
        private readonly Vector3 position;
        private readonly TitanMover titanMover;
        private bool isStarted = false;

        public override bool IsEnded {
            get { return isStarted && !titanMover.IsMoving; }
        }

        public MoveTask(Vector3 toPosition, TitanMover mover) {
            position = toPosition;
            titanMover = mover;
        }

        public override void MakeTask(float deltaTime) {
            if (!isStarted) {
                isStarted = true;
                titanMover.MoveTo(position);
            }
            titanMover.Update(deltaTime);
        }
    }
}
