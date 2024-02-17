using MetaModels;
using MetaModels.Models;
using Models.Interfaces;

namespace Models;

public class StateMachine : IStateMachine
{
    public Machine Machine { get; } = new();
    public State CurrentState { get; set; }
    public Transition CurrentTransition { get; set; }
    
    public StateMachine()
    {
        Machine.States = [];
        Machine.Integers = new Dictionary<string, int>();
    }

    public Machine Build()
    {
        var stateLookup = Machine.States.ToDictionary(state => state.Name);

        foreach (var transition in Machine.States.SelectMany(state => state.Transitions))
        {
            if (stateLookup.TryGetValue(transition.State.Name, out var targetState))
            {
                transition.State = targetState;
            }
        }

        return Machine;
    }
    
    public StateMachine Set(string variable, int value)
    {
        CurrentTransition.Operation = new Operation(variable, OperationType.Set, value);
        return this;
    }
    
    public StateMachine Increment(string variable)
    {
        CurrentTransition.Operation = new Operation(variable, OperationType.Increment);
        return this;
    }
    
    public StateMachine Decrement(string variable)
    {
        CurrentTransition.Operation = new Operation(variable, OperationType.Decrement);
        return this;
    }
    
    public StateMachine IfEquals(string variable, int value)
    {
        CurrentTransition.Condition = new Condition(variable, value, ConditionType.Equals);
        return this;
    }
    
    public StateMachine IfGreaterThan(string variable, int value)
    {
        CurrentTransition.Condition = new Condition(variable, value, ConditionType.GreaterThan);
        return this;
    }
    
    public StateMachine IfLessThan(string variable, int value)
    {
        CurrentTransition.Condition = new Condition(variable, value, ConditionType.LessThan);
        return this;
    }
}