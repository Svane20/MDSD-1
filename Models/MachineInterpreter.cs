using MetaModels;
using MetaModels.Models;
using Models.Interfaces;

namespace Models;

public class MachineInterpreter : IMachineInterpreter
{
    private Machine _machine;

    public State CurrentState { get; set; } = null!;

    public void Run(Machine machine)
    {
        _machine = machine;
        CurrentState = machine.InitialState;
    }

    public void ProcessEvent(string eventName)
    {
        foreach (var transition in CurrentState.Transitions)
        {
            if (transition.EventName != eventName)
            {
                continue;
            }

            var isConditionMet = true;

            if (transition.IsConditional)
            {
                var value = GetInteger(transition.Condition!.VariableName);
                isConditionMet = transition.Condition.Type switch
                {
                    ConditionType.Equals => value == transition.Condition.Value,
                    ConditionType.GreaterThan => value > transition.Condition.Value,
                    ConditionType.LessThan => value < transition.Condition.Value,
                    _ => throw new ArgumentOutOfRangeException(nameof(transition.Condition.Type))
                };
            }

            if (!isConditionMet)
            {
                continue;
            }

            if (transition.HasOperation)
            {
                var currentValue = GetInteger(transition.Operation!.VariableName);
                _machine.Integers[transition.Operation.VariableName] = transition.Operation.Type switch
                {
                    OperationType.Set => transition.Operation.Value!.Value,
                    OperationType.Increment => currentValue + 1,
                    OperationType.Decrement => currentValue - 1,
                    _ => _machine.Integers[transition.Operation.VariableName]
                };
            }

            CurrentState = transition.State;
            break;
        }
    }

    public int GetInteger(string name)
    {
        return _machine.Integers.GetValueOrDefault(name, 0);
    }
}