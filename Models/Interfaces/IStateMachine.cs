using MetaModels;

namespace Models.Interfaces;

public interface IStateMachine : IOperation, ICondition
{
    Machine Build();
}