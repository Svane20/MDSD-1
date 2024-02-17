using MetaModels;
using Models;
using Models.Extensions;

namespace Tests;

[TestFixture]
public class MachineStructureTest
{
    private StateMachine _stateMachine;
    
    [SetUp]
    public void Init()
    {
        _stateMachine = new StateMachine();
    }

    [Test]
    public void EmptyMachine()
    {
        var machine = _stateMachine.Build();
        
        Assert.That(machine.States.Count, Is.EqualTo(0));
    }

    [Test]
    public void States()
    {
        var machine = _stateMachine
            .State("State 1")
            .State("State 2")
            .State("State 3")
            .Build();
        var states = machine.States;
        
        Assert.Multiple(() =>
        {
            Assert.That(machine.States, Has.Count.EqualTo(3));
            Assert.That(states, Has.One.Matches<State>(state => state.Name == "State 1"));
            Assert.That(states, Has.One.Matches<State>(state => state.Name == "State 2"));
            Assert.That(states, Has.One.Matches<State>(state => state.Name == "State 3"));
        });
    }

    [Test]
    public void InitialFirstState()
    {
        var machine = _stateMachine
            .State("State 1").Initial()
            .State("State 2")
            .State("State 3")
            .Build();
        
        Assert.That(machine.InitialState.Name, Is.EqualTo("State 1"));
    }
    
    [Test]
    public void InitialState()
    {
        var machine = _stateMachine
            .State("State 1")
            .State("State 2").Initial()
            .State("State 3")
            .Build();
        
        Assert.That(machine.InitialState.Name, Is.EqualTo("State 2"));
    }

    [Test]
    public void GetState()
    {
        var machine = _stateMachine
            .State("State 1")
            .State("State 2").Initial()
            .State("State 3")
            .Build();
        
        Assert.That(machine.GetState("State 2")!.Name, Is.EqualTo("State 2"));
    }

    [Test]
    public void NoTransitions()
    {
        var machine = _stateMachine.State("State 1").Build();
        var state = machine.GetState("State 1")!;
        var transitions = state.Transitions;
        
        Assert.That(transitions, Is.Empty);
    }

    [Test]
    public void Transitions()
    {
        var machine = _stateMachine
            .State("State 1")
                .When("Change to 2").To("State 2")
                .When("Change to 3").To("State 3")
            .State("State 2")
                .When("Change to 3").To("State 3")
            .State("State 3")
            .Build();
        
        var state = machine.GetState("State 1")!;
        var transitions = state.Transitions;
        
        Assert.Multiple(() =>
        {
            Assert.That(transitions, Has.Count.EqualTo(2));
            Assert.That(transitions, Has.One.Matches<Transition>(transition => transition.EventName == "Change to 2"));
            Assert.That(state.GetTransitionByEvent("Change to 2")!.State.Name, Is.EqualTo("State 2"));
            Assert.That(transitions, Has.One.Matches<Transition>(transition => transition.EventName == "Change to 3"));
            Assert.That(state.GetTransitionByEvent("Change to 3")!.State.Name, Is.EqualTo("State 3"));
        });
    }

    [Test]
    public void NoVariables()
    {
        var machine = _stateMachine.Build();
        
        Assert.That(machine.IntCount, Is.EqualTo(0));
    }

    [Test]
    public void AddVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .Build();
        
        Assert.Multiple(() =>
        {
            Assert.That(machine.IntCount, Is.EqualTo(1));
            Assert.That(machine.HasInteger("var"), Is.True);
            Assert.That(machine.HasInteger("var 2"), Is.False);
        });
    }

    [Test]
    public void TransitionSetVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("SET").To("State 2").Set("var", 42)
            .State("State 2")
            .Build();
        
        var transition = machine.GetState("State 1")!.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.HasSetOperation, Is.True);
            Assert.That(transition.HasIncrementOperation, Is.False);
            Assert.That(transition.HasDecrementOperation, Is.False);
            Assert.That(transition.Operation!.VariableName, Is.EqualTo("var"));
        });
    }

    [Test]
    public void TransitionIncrementVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("SET").To("State 2").Increment("var")
            .State("State 2")
            .Build();
        
        var transition = machine.GetState("State 1")!.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.HasSetOperation, Is.False);
            Assert.That(transition.HasIncrementOperation, Is.True);
            Assert.That(transition.HasDecrementOperation, Is.False);
            Assert.That(transition.Operation!.VariableName, Is.EqualTo("var"));
        });
    }

    [Test]
    public void TransitionDecrementVariable()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("SET").To("State 2").Decrement("var")
            .State("State 2")
            .Build();
        
        var transition = machine.GetState("State 1")!.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.HasSetOperation, Is.False);
            Assert.That(transition.HasIncrementOperation, Is.False);
            Assert.That(transition.HasDecrementOperation, Is.True);
            Assert.That(transition.Operation!.VariableName, Is.EqualTo("var"));
        });
    }

    [Test]
    public void TransitionIfVariableEqual()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").IfEquals("var", 42)
            .State("State 2")
            .Build();

        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.Condition!.VariableName, Is.EqualTo("var"));
            Assert.That(transition.Condition.Value, Is.EqualTo(42));
            Assert.That(transition.IsEqualsCondition, Is.True);
            Assert.That(transition.IsGreaterThanCondition, Is.False);
            Assert.That(transition.IsLessThanCondition, Is.False);
        });
    }

    [Test]
    public void TransitionIfVariableGreaterThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").IfGreaterThan("var", 42)
            .State("State 2")
            .Build();

        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.Condition!.VariableName, Is.EqualTo("var"));
            Assert.That(transition.Condition.Value, Is.EqualTo(42));
            Assert.That(transition.IsEqualsCondition, Is.False);
            Assert.That(transition.IsGreaterThanCondition, Is.True);
            Assert.That(transition.IsLessThanCondition, Is.False);
        });
    }

    [Test]
    public void TransitionIfVariableLessThan()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").IfLessThan("var", 42)
            .State("State 2")
            .Build();

        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.Condition!.VariableName, Is.EqualTo("var"));
            Assert.That(transition.Condition.Value, Is.EqualTo(42));
            Assert.That(transition.IsEqualsCondition, Is.False);
            Assert.That(transition.IsGreaterThanCondition, Is.False);
            Assert.That(transition.IsLessThanCondition, Is.True);
        });
    }

    [Test]
    public void TransitionIfVariableEqualsAndSet()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").Set("var", 42).IfEquals("var", 42)
            .State("State 2")
            .Build();
        
        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.HasSetOperation, Is.True);
        });
    }

    [Test]
    public void TransitionIfVariableGreaterAndIncrement()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").Increment("var").IfGreaterThan("var", 42)
            .State("State 2")
            .Build();
        
        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.HasOperation, Is.True);
        });
    }

    [Test]
    public void TransitionIfVariableLessAndDecrement()
    {
        var machine = _stateMachine
            .Integer("var")
            .State("State 1")
                .When("GO").To("State 2").Decrement("var").IfLessThan("var", 42)
            .State("State 2")
            .Build();
        
        var state = machine.GetState("State 1")!;
        var transition = state.Transitions.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(transition.IsConditional, Is.True);
            Assert.That(transition.HasOperation, Is.True);
        });
    }
}