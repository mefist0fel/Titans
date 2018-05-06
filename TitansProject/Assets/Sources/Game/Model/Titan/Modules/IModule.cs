namespace Model {
    public interface IModule {
        void OnAttach(Titan titan);
        void OnDetach();
        void Update(float deltaTime);
    }
}
