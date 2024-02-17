namespace Models.Interfaces;

public interface IOperation
{
    StateMachine Set(string variable, int value);
    StateMachine Increment(string variable);
    StateMachine Decrement(string variable);
}