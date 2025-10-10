using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficLightControl.Models;

namespace TrafficLightControl.Services
{
    public class TrafficLightService
    {
        private readonly TrafficLight _light = new();
        private readonly object _lock = new();
        private Timer? _timer;

        public TrafficLightService()
        {
            _timer = new Timer(ChangeStateAutomatically, null, 0, 5000);
        }

        private void ChangeStateAutomatically(object? state)
        {
            lock (_lock)
            {
                switch (_light.CurrentState)
                {
                    case TrafficLightState.Red:
                        _light.ChangeState(TrafficLightState.Green);
                        break;
                    case TrafficLightState.Green:
                        _light.ChangeState(TrafficLightState.Yellow);
                        break;
                    case TrafficLightState.Yellow:
                        _light.ChangeState(TrafficLightState.Red);
                        break;
                }
            }
        }

        public TrafficLight GetStatus() => _light;

        public void SetManual(TrafficLightState newState)
        {
            lock (_lock)
            {
                _light.ChangeState(newState);
            }
        }

        public async Task PedestrianCrossAsync()
        {
            lock (_lock)
            {
                _light.ChangeState(TrafficLightState.Red);
            }

            Console.WriteLine("?? Pedestrian crossing...");
            await Task.Delay(5000);
            lock (_lock)
            {
                _light.ChangeState(TrafficLightState.Green);
            }
            Console.WriteLine("?? Traffic resumed");
        }
    }
}
