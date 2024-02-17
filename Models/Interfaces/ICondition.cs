namespace Models.Interfaces;

public interface ICondition
{
    StateMachine IfEquals(string variable, int value);
    StateMachine IfGreaterThan(string variable, int value);
    StateMachine IfLessThan(string variable, int value);
}