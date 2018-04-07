using UnityEngine;

public interface ITitanModule {
    void OnAttach(TitanView titan);
    void OnDetach();
}

public interface IModificator {
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
