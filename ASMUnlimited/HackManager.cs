using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ASMUnlimited
{
    /// <summary>
    /// Automatically changes the rectangle's color
    /// Enables hack-callback when its active
    /// </summary>
    public class HackManager
    {
        private static Task _taskWorker;
        private static ConcurrentQueue<HackManager> _activeManagers;

        static HackManager()
        {
            _activeManagers = new ConcurrentQueue<HackManager>();
            _taskWorker = new Task(TaskWorker, new System.Threading.CancellationToken(), TaskCreationOptions.LongRunning);
            _taskWorker.Start();
        }

        /// <summary>
        /// Central worker void which executes every long-running hack
        /// </summary>
        private static async void TaskWorker()
        {
            var tQueue = new Queue<HackManager>();
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

        private Canvas _target;
        private Rectangle _statusRect;

        private bool _active;
        private Action _activationAction;

        /// <summary>
        /// Changes the color when value changes
        /// </summary>
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    _statusRect.Fill = GetColor(value);
                }
            }
        }

        /// <summary>
        /// Is called by the taskworker when hack is active
        /// </summary>
        public Action ActivationAction
        {
            get => _active ? _activationAction : null;
            set => _activationAction = value;
        }

        public HackManager(Canvas targetCw)
        {
            _target = targetCw;

            _statusRect = new Rectangle
            {
                Width = 22,
                Height = 22,
                Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            };
            _target.Children.Add(_statusRect);

            _activeManagers.Enqueue(this);
        }

        /// <summary>
        /// Inverts the status and returns the new status value
        /// </summary>
        /// <returns></returns>
        public bool ToggleStatus()
        {
            return Active = !_active;
        }

        /// <summary>
        /// Active:     Green
        /// Inactive:   Red
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Brush GetColor(bool status)
        {
            if (status)
            {
                return new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            return new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
    }
}