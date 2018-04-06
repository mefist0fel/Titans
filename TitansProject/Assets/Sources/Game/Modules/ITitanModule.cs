using UnityEngine;

public interface ITitanModule {
    void OnAttach(TitanView titan);
    void OnDetach();
}

public enum InterfaceType {
    Module,
    Skill
}

public interface IInterfaceController {
    InterfaceType Type { get; }
    string Name { get; }
    void Execute();
}
