using Models;
using Models.Extensions;

namespace Tests;

[TestFixture]
public class CdPlayerTest
{
    private MachineInterpreter _interpreter;
    
    [SetUp]
    public void Init()
    {
        var stateMachine = new StateMachine();
        const int numberTracks = 10;

        var machine = stateMachine
            .Integer("track")
            .State("STOP").Initial()
                .When("PLAY").To("PLAYING").Set("track", 1).IfEquals("track", 0)
                .When("PLAY").To("PLAYING")
            .State("PLAYING")
                .When("STOP").To("STOP")
                .When("PAUSE").To("PAUSED")
                .When("TRACK_END").To("STOP").IfEquals("track", numberTracks)
                .When("TRACK_END").To("PLAYING").Increment("track").
            State("PAUSED")
                .When("STOP").To("STOP")
                .When("PLAY").To("PLAYING")
                .When("FORWARD").To("PAUSED").Increment("track").IfLessThan("track", numberTracks + 1)
                .When("BACK").To("PAUSED").Decrement("track").IfGreaterThan("track", 1)
            .Build();
        
        _interpreter = new MachineInterpreter();
        _interpreter.Run(machine);
    }

    [Test]
    public void PlayMusic()
    {
        _interpreter.ProcessEvent("PLAY");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(1));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PLAYING"));
        });
        
        _interpreter.ProcessEvent("TRACK_END");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(2));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PLAYING"));
        });
        
        _interpreter.ProcessEvent("STOP");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(2));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("STOP"));
        });
        
        _interpreter.ProcessEvent("PLAY");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(2));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PLAYING"));
        });
        
        _interpreter.ProcessEvent("PAUSE");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(2));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PAUSED"));
        });
        
        _interpreter.ProcessEvent("BACK");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(1));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PAUSED"));
        });
        
        _interpreter.ProcessEvent("FORWARD");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(2));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PAUSED"));
        });
        
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        _interpreter.ProcessEvent("FORWARD");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(10));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PAUSED"));
        });
        
        _interpreter.ProcessEvent("PLAY");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(10));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("PLAYING"));
        });
        
        _interpreter.ProcessEvent("TRACK_END");
        Assert.Multiple(() =>
        {
            Assert.That(_interpreter.GetInteger("track"), Is.EqualTo(10));
            Assert.That(_interpreter.CurrentState.Name, Is.EqualTo("STOP"));
        });
    }
}