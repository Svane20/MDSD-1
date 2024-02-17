using MetaModels;

namespace Models.Interfaces;

public interface IMachineInterpreter
{
    void Run(Machine machine);
    void ProcessEvent(string eventName);
    int GetInteger(string name);
}