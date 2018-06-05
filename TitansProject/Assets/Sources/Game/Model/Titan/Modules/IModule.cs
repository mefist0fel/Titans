namespace Model {
    public interface IModule {
        string Id { get; }
        void OnAttach(Titan titan);
        void OnDetach(Titan titan);
        void Update(float deltaTime);
    }
}
