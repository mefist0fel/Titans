using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public sealed class MoveTask : Titan.Task {
        public readonly Vector3 Position;
        private readonly TitanMover titanMover;
        private bool isStarted = false;

        public override bool IsEnded {
            get { return isStarted && !titanMover.IsMoving; }
        }

        public MoveTask(Vector3 toPosition, TitanMover mover) {
            Position = toPosition;
            titanMover = mover;
        }

        public override void MakeTask(float deltaTime) {
            if (!isStarted) {
                isStarted = true;
                titanMover.MoveTo(Position);
            }
            titanMover.Update(deltaTime);
        }
    }
}
