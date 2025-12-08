using Xunit;
using TrafficLightControl;
using System;

namespace TrafficLightControl.Tests
{
    public class TrafficLightTests
    {
        [Fact]
        public void New_TrafficLight_Has_InitialState_Red()
        {
            var light = new TrafficLight();
            Assert.Equal("Red", light.State);
        }

        [Fact]
        public void NextState_Follows_Red_To_Green_To_Yellow_To_Red()
        {
            var light = new TrafficLight();
            Assert.Equal("Red", light.State);

            light.NextState();
            Assert.Equal("Green", light.State);

            light.NextState();
            Assert.Equal("Yellow", light.State);

            light.NextState();
            Assert.Equal("Red", light.State);
        }

        [Fact]
        public void SetState_Allows_ExplicitState_Change()
        {
            var light = new TrafficLight();
            light.SetState("Green");
            Assert.Equal("Green", light.State);

            light.SetState("Yellow");
            Assert.Equal("Yellow", light.State);
        }

        [Fact]
        public void Invalid_SetState_Throws()
        {
            var light = new TrafficLight();
            Assert.Throws<ArgumentException>(() => light.SetState("NotAState"));
        }
    }
}
