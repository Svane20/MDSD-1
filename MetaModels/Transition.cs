using MetaModels.Models;

namespace MetaModels;

public class Transition
{
    public string EventName { get; set; } = null!;
    public State State { get; set; } = null!;
    public Operation? Operation { get; set; }
    public Condition? Condition { get; set; }

    public bool HasOperation => Operation is not null;
    public bool HasSetOperation => HasOperation && Operation?.Type == OperationType.Set;
    public bool HasIncrementOperation => HasOperation && Operation?.Type == OperationType.Increment;
    public bool HasDecrementOperation => HasOperation && Operation?.Type == OperationType.Decrement;

    public bool IsConditional => Condition is not null;
    public bool IsEqualsCondition => IsConditional && Condition?.Type == ConditionType.Equals;
    public bool IsLessThanCondition => IsConditional && Condition?.Type == ConditionType.LessThan;
    public bool IsGreaterThanCondition => IsConditional && Condition?.Type == ConditionType.GreaterThan;
}