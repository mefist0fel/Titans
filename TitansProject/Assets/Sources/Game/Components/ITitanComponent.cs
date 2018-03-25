using UnityEngine;

public interface ITitanComponent {
    void Attach(TitanView titan);
    IInterfaceController[] GetInterfaceControllers();
    void Detach();
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
