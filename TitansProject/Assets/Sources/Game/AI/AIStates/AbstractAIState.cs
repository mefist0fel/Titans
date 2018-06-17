namespace Model.AI {
    public abstract class AbstractAIState {
        protected readonly TitanAI titanAI;
        public AbstractAIState(TitanAI ai) {
            titanAI = ai;
        }

        public abstract void Update(float deltaTime);
        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }
    }
}
