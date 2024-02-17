using Models;
using Models.Extensions;

namespace Tests;

[TestFixture]
public class MachineInterpreterTest
{
    private StateMachine _stateMachine;
    private MachineInterpreter _interpreter;
    
    [SetUp]
    public void Init()
    {
        _stateMachine = new StateMachine();
        _interpreter = new MachineInterpreter();
    }

    [Test]
    public void StartInitState()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 1"));
    }

    [Test]
    public void EventNoTransition()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
                .When("FIRE").To("State 2")
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("to 2");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 1"));
    }

    [Test]
    public void EventTransition()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
                .When("FIRE").To("State 2")
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("FIRE");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 2"));
    }

    [Test]
    public void ListOfEvents()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
                .When("ON").To("State 2")
            .State("State 2")
                .When("GO").To("State 3")
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("ON");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 3"));
    }

    [Test]
    public void ChooseTransition()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
                .When("FIRE2").To("State 2")
                .When("FIRE3").To("State 3")
                .When("FIRE4").To("State 4")
            .State("State 2")
            .State("State 3")
            .State("State 4")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("FIRE3");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 3"));
    }
    
    [Test]
    public void InitVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
            .Build();
        
        _interpreter.Run(machine);
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(0));
    }

    [Test]
    public void TransitionSetVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("SET").To("State 2").Set("var", 42)
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("SET");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(42));
    }

    [Test]
    public void TransitionIncrementVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("SET").To("State 2").Increment("var")
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("SET");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(1));
    }
    
    [Test]
    public void TransitionDecrementVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("SET").To("State 2").Decrement("var")
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("SET");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(-1));
    }
    
    [Test]
    public void TransitionIfVariableEqual()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfEquals("var", 42)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 3"));
    }

    [Test]
    public void TransitionIfVariableNotEqual()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfEquals("var", 40)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 2"));
    }
    
    [Test]
    public void TransitionIfVariableGreaterThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfGreaterThan("var", 40)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 3"));
    }
    
    [Test]
    public void TransitionIfVariableNotGreaterThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfGreaterThan("var", 42)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 2"));
    }
    
    [Test]
    public void TransitionIfVariableLessThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfLessThan("var", 45)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 3"));
    }
    
    [Test]
    public void TransitionIfVariableNotLessThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42)
            .State("State 2")
                .When("GO").To("State 3").IfLessThan("var", 42)
            .State("State 3")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("State 2"));
    }
    
    [Test]
    public void TransitionIfVariableEqualsAndSet()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Set("var", 42).IfEquals("var", 0)
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(42));
    }
    
    [Test]
    public void TransitionIfVariableGreaterAndIncrement()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Increment("var").IfGreaterThan("var", -1)
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(1));
    }
    
    [Test]
    public void TransitionIfVariableLessAndDecrement()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Decrement("var").IfLessThan("var", 1)
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(-1));
    }
    
    [Test]
    public void TransitionOrder()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1").Initial()
                .When("GO").To("State 2").Increment("var")
                .When("GO").To("State 2").Decrement("var")
            .State("State 2")
            .Build();
        
        _interpreter.Run(machine);
        _interpreter.ProcessEvent("GO");
        
        Assert.That(_interpreter.GetInteger("var"), Is.EqualTo(1));
    }
}