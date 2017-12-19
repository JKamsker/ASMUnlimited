using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ASMUnlimited.Utilities;

namespace ASMUnlimited
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task _taskWorker;

        private HackStatusManager _unlimitedManager;
        private MemManager _mem;

        public MainWindow()
        {
            InitializeComponent();

            _activeManagers = new ConcurrentQueue<HackStatusManager>();
            _unlimitedManager = new HackStatusManager(cwUnlimited);

            _mem = new MemManager(Process.GetProcesses().FirstOrDefault(m => m.ProcessName.Contains("ASAMU")));

            _unlimitedManager.ActivationAction = () =>
            {
                var address = _mem.EvaluateOffsets(0x0270AB1C, 0x4, 0x28, 0x3d0);
                _mem.WriteInt32(address, 0);
            };

            _activeManagers.Enqueue(_unlimitedManager);

            _taskWorker = new Task(TaskWorker, new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning);
            _taskWorker.Start();
        }

        private void butToggleUnlimited_Click(object sender, RoutedEventArgs e)
        {
            _unlimitedManager.Active = !_unlimitedManager.Active;
        }

        private ConcurrentQueue<HackStatusManager> _activeManagers;

        private async void TaskWorker()
        {
            var tQueue = new Queue<HackStatusManager>();
            while (true)
            {
                while (_activeManagers.TryDequeue(out var cManager))
                {
                    cManager.ActivationAction?.Invoke();
                    tQueue.Enqueue(cManager);
                }

                while (tQueue.Count != 0)
                    _activeManagers.Enqueue(tQueue.Dequeue());

                await Task.Delay(TimeSpan.FromSeconds(0.25));
            }
        }
    }
}