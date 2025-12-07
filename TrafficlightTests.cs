using Xunit;
using TrafficLightControl;

public class TrafficLightTests
{
    [Fact]
    public void InitialState_ShouldBeRed()
    {
        var light = new TrafficLight();
        Assert.Equal("Red", light.State);
    }

    [Fact]
    public void ChangeState_ShouldGoToGreen()
    {
        var light = new TrafficLight();
        light.ChangeState("Green");
        Assert.Equal("Green", light.State);
    }
}
