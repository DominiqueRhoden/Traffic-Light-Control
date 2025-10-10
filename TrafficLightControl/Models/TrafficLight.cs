using System;

namespace TrafficLightControl.Models
{
    public class TrafficLight
    {
        public TrafficLightState CurrentState { get; private set; } = TrafficLightState.Red;
        public DateTime LastChanged { get; private set; } = DateTime.Now;

        public void ChangeState(TrafficLightState newState)
        {
            CurrentState = newState;
            LastChanged = DateTime.Now;
            Console.WriteLine($"[{LastChanged}] Light changed to {CurrentState}");
        }
    }
}
