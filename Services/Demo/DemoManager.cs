using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Services.Demo.Scenarios;

namespace BookingApp.Services.Demo
{
    public enum DemoState
    {
        Stopped,
        Running,
        Paused
    }

    public class DemoManager
    {
        private Timer demoTimer;
        private int currentScenarioIndex = 0;
        private int currentStep = 0;
        private List<IDemoScenario> scenarios;
        private OwnerDashboardViewModel dashboardViewModel;

        public event Action<string> OnDemoMessage;
        public bool IsRunning { get; private set; }

        public DemoManager(OwnerDashboardViewModel dashboardViewModel)
        {
            this.dashboardViewModel = dashboardViewModel;
            InitializeScenarios();
        }

        private void InitializeScenarios()
        {
            scenarios = new List<IDemoScenario>();

            var accommodationDemo = new AccommodationRegistrationDemo();
            accommodationDemo.SetDashboardViewModel(dashboardViewModel);

            var statisticsDemo = new StatisticsDemo();
            statisticsDemo.SetDashboardViewModel(dashboardViewModel);

            var requestsDemo = new RequestManagementDemo();
            requestsDemo.SetDashboardViewModel(dashboardViewModel);

            scenarios.Add(accommodationDemo);
            scenarios.Add(statisticsDemo);
            scenarios.Add(requestsDemo);
        }

        public void StartDemo()
        {
            IsRunning = true;
            currentScenarioIndex = 0;
            currentStep = 0;
            OnDemoMessage?.Invoke("🎬 DEMO STARTED - Press ESC to stop");

            demoTimer = new Timer(ExecuteNextStep, null, 1000, 3000);
            scenarios[0].Initialize();
        }

        private void ExecuteNextStep(object state)
        {
            if (!IsRunning) return;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var currentScenario = scenarios[currentScenarioIndex];
                    if (currentScenario.ExecuteStep(currentStep))
                    {
                        currentStep++;
                    }
                    else
                    {
                        
                        currentScenarioIndex = (currentScenarioIndex + 1) % scenarios.Count;
                        currentStep = 0;

                        demoTimer?.Change(2000, 3000); 
                        scenarios[currentScenarioIndex].Initialize();
                    }
                }
                catch (Exception ex)
                {
                    OnDemoMessage?.Invoke($"Demo error: {ex.Message}");
                    StopDemo();
                }
            });
        }

        public void StopDemo()
        {
            IsRunning = false;
            demoTimer?.Dispose();
            OnDemoMessage?.Invoke("Demo stopped");

            foreach (var scenario in scenarios)
            {
                scenario?.Cleanup();
            }

        }
    }
}