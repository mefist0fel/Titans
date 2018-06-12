namespace Model {
    public sealed class DelayedInteraction : AbstractInteraction {
        private readonly AbstractInteraction interaction;
        private float delayTimer;

        public DelayedInteraction(Titan parent, float delay, AbstractInteraction delayedInteraction) : base(parent) {
            delayTimer = delay;
            interaction = delayedInteraction;
        }

        public override bool IsEnded {
            get {
                return delayTimer <= 0;
            }
        }

        public override void Update(float deltaTime) {
            delayTimer -= deltaTime;
            if (delayTimer > 0) {
                return;
            }
            if (ParentTitan != null && ParentTitan.IsAlive) {
                ParentTitan.Context.AddInteraction(interaction);
            }
        }
    }
}
