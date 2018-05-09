namespace Model {
    public interface IModule {
        string Id { get; }
        void OnAttach(Titan titan);
        void OnDetach();
        void Update(float deltaTime);
    }
}
