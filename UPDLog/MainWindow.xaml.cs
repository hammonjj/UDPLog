using System;
using System.Timers;
using System.Windows;
using System.Threading;
using System.Collections.Concurrent;

using Microsoft.Win32;

namespace UPDLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Thread _udpListenerThread;
        private UdpListener _updListener;
        private System.Timers.Timer _messagePumpTimer;
        private static ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

        public MainWindow()
        {
            //Setup UI Components
            InitializeComponent();

            //Load Config
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog");
            if(key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog");
            }

            SetupUdpListener(key);
            SetupMessageQueueTimer(key);
            LvCustomTest.LoadConfig(key);
        }

        private void PumpMessageQueue(object sender, ElapsedEventArgs e)
        {
            if(_messageQueue.IsEmpty) { return; }
            
            //Disable timer so we don't bind up the main thread
            _messagePumpTimer.Enabled = false;

            string message;
            while (_messageQueue.TryDequeue(out message))
            {
                //Parse message and add it to the log
                var lm = new LogMessage(message);
                OnUiThread(() => LvCustomTest.Items.Add(lm));
            }

            _messagePumpTimer.Enabled = true;
        }

        private void OnUiThread(Action action)
        {
            if(Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.Invoke(action);
            }
        }

        private void ListenClicked(object sender, RoutedEventArgs e)
        {
            _udpListenerThread = new Thread(_updListener.BeginListening);
            _udpListenerThread.Start();
            _messagePumpTimer.Enabled = true;
            btnStart.IsEnabled = false;
        }

        private void StopListeningClicked(object sender, RoutedEventArgs e)
        {
            _updListener.StopListening();
            _messagePumpTimer.Enabled = false;
            btnStart.IsEnabled = true;
        }

        private void ClearLogClicked(object sender, RoutedEventArgs e)
        {
            LvCustomTest.Items.Clear();
        }

        private void ApplyFilterClicked(object sender, RoutedEventArgs e)
        {

        }

        private void AddMarkerClicked(object sender, RoutedEventArgs e)
        {
            LvCustomTest.Items.Add(new LogMessage());
        }

        private void PreferencesClicked(object sender, RoutedEventArgs e)
        {
#if false
            window2 win2= new window2();
            win2.Show();
#endif
        }

        private void FilterConfigClicked(object sender, RoutedEventArgs e)
        {
#if false
            window2 win2= new window2();
            win2.Show();
#endif
        }

        private void SaveLogClicked(object sender, RoutedEventArgs e) 
        {
#if false
            window2 win2= new window2();
            win2.Show();
#endif
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetupMessageQueueTimer(RegistryKey root)
        {
            var messageQueuePumpTimeKey = Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Aventura\UdpLog",
                "MessageQueueTimer",
                500);

            if(messageQueuePumpTimeKey == null)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog");
                key.SetValue("MessageQueueTimer", 500);
            }

            _messagePumpTimer = new System.Timers.Timer(messageQueuePumpTimeKey == null ? 
                500 : Convert.ToInt32(messageQueuePumpTimeKey));
            _messagePumpTimer.Elapsed += PumpMessageQueue;
        }

        private void SetupUdpListener(RegistryKey root)
        {
            var listenPortKey = Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Aventura\UdpLog",
                "ListenPort",
                514);

            if (listenPortKey == null)
            {
                var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog");
                key.SetValue("ListenPort", 514);
            }

            _updListener = new UdpListener(
                listenPortKey == null ? 514 : Convert.ToInt32(listenPortKey), 
                ref _messageQueue);
        }
    }
}
