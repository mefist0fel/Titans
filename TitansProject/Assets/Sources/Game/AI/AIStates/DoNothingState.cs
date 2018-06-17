namespace Model.AI {
    public sealed class DoNothingState : AbstractAIState {
        public DoNothingState(TitanAI ai) : base(ai) {}

        public override void Update(float deltaTime) {}
    }
}