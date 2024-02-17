using MetaModels;

namespace Models.Extensions;

public static class StateMachineExtensions
{
    public static StateMachine State(this StateMachine stateMachine, string name)
    {
        var newState = new State
        {
            Name = name,
            Transitions = []
        };
        stateMachine.Machine.States.Add(newState);
        stateMachine.CurrentState = newState;

        return stateMachine;
    }
    
    public static StateMachine Initial(this StateMachine stateMachine)
    {
        stateMachine.Machine.InitialState = stateMachine.CurrentState;
        
        return stateMachine;
    }
    
    public static StateMachine When(this StateMachine stateMachine, string eventName)
    {
        var newTransition = new Transition
        {
            EventName = eventName,
        };
        stateMachine.CurrentState.Transitions.Add(newTransition);
        stateMachine.CurrentTransition = newTransition;
        
        return stateMachine;
    }
    
    public static StateMachine To(this StateMachine stateMachine, string stateName)
    {
        stateMachine.CurrentTransition.State = new State { Name = stateName };
        
        return stateMachine;
    }
    
    public static StateMachine Integer(this StateMachine stateMachine, string name)
    {
        stateMachine.Machine.Integers[name] = 0;
        
        return stateMachine;
    }
}