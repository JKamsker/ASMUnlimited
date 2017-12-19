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
        private HackManager _unlimitedManager;
        private MemManager _mem;

        public MainWindow()
        {
            InitializeComponent();

            _unlimitedManager = new HackManager(cwUnlimited);
            _mem = new MemManager(Process.GetProcesses().FirstOrDefault(m => m.ProcessName.Contains("ASAMU")));

            _unlimitedManager.ActivationAction = () =>
            {
                var address = _mem.EvaluateOffsets(0x0270AB1C, 0x4, 0x28, 0x3d0);
                _mem.WriteInt32(address, 0);
            };
        }

        private void butToggleUnlimited_Click(object sender, RoutedEventArgs e)
        {
            _unlimitedManager.Active = !_unlimitedManager.Active;
        }
    }
}