namespace Model.AI {
    public sealed class WaitingState : AbstractAIState {
        private float timer;
        private readonly AbstractAIState nextState;

        public WaitingState(TitanAI ai, AbstractAIState next, float waitTime = 0) : base(ai) {
            timer = waitTime;
            nextState = next;
        }

        public override void Update(float deltaTime) {
            timer -= deltaTime;
            if (timer <= 0)
                titanAI.SetState(nextState);
        }
    }
}