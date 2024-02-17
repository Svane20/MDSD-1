namespace MetaModels;

public class State
{
    public string Name { get; set; } = null!;
    public List<Transition> Transitions { get; set; } = null!;
    
    public Transition? GetTransitionByEvent(string eventName) => Transitions.FirstOrDefault(t => t.EventName == eventName);
}