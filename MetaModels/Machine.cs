namespace MetaModels;

public class Machine
{
    public List<State> States { get; set; } = null!;
    public State InitialState { get; set; } = null!;
    public Dictionary<string, int> Integers { get; set; } = null!;
    
    public State? GetState(string stateName) => States.FirstOrDefault(s => s.Name == stateName);
    
    public int IntCount => Integers.Count;
    
    public bool HasInteger(string name) => Integers.ContainsKey(name);
}