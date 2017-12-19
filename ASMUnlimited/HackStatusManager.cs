using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ASMUnlimited
{
    internal class HackStatusManager
    {
        private Canvas _target;
        private Rectangle _statusRect;

        private bool _active;
        private Action _activationAction;

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

        public Action ActivationAction
        {
            get => _active ? _activationAction : null;
            set => _activationAction = value;
        }

        public HackStatusManager(Canvas targetCw)
        {
            _target = targetCw;

            _statusRect = new Rectangle
            {
                Width = 22,
                Height = 22,
                Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            };
            _target.Children.Add(_statusRect);
        }

        /// <summary>
        /// Inverts the status and returns the new status value
        /// </summary>
        /// <returns></returns>
        public bool ToggleStatus()
        {
            return Active = !_active;
        }

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