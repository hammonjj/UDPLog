using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Collections.Concurrent;

using Microsoft.Win32;

using UPDLog.DataStructures;
using UPDLog.Messaging;
using UPDLog.Windows;

namespace UPDLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //Temporary until the button styles are changed
        private readonly Brush DefaultButtonBackground;

        private readonly RegistryKey _root;
        private Thread _udpListenerThread;
        private UdpListener _updListener;
        private System.Timers.Timer _messagePumpTimer;
        private static ConcurrentQueue<RawMessage> _messageQueue = new ConcurrentQueue<RawMessage>();

        public MainWindow()
        {
            //Setup UI Components
            InitializeComponent();

            //Load Config
            _root = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog", true) ??
                Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Aventura\UdpLog");

            SetupUdpListener();
            SetupMessageQueueTimer();

            LoadWindowConfig();
            LvLogMessages.LoadConfig(_root);

            //Temporary
            DefaultButtonBackground = btnFilter.Background;
        }

        private void PumpMessageQueue(object sender, ElapsedEventArgs e)
        {
            if(_messageQueue.IsEmpty) { return; }
            
            //Disable timer so we don't bind up the main thread
            _messagePumpTimer.Enabled = false;

            RawMessage message;
            while (_messageQueue.TryDequeue(out message))
            {
                //Parse message and add it to the log
                var lm = new LogMessage(message);
                OnUiThread(() => LvLogMessages.AddLogMessage(lm));
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
            LvLogMessages.Items.Clear();
        }

        private void ApplyFilterClicked(object sender, RoutedEventArgs e)
        {
            if (LvLogMessages.FilterEnabled())
            {
                btnFilter.Background = DefaultButtonBackground;
                LvLogMessages.ApplyFilter(false);
            }
            else
            {
                btnFilter.Background = Brushes.Blue;
                LvLogMessages.ApplyFilter(true);
            }
        }

        private void AddMarkerClicked(object sender, RoutedEventArgs e)
        {
            LvLogMessages.AddLogMessage(new LogMessage());
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
            var filterConfigKey = _root.OpenSubKey("Filters", true);
            var filterConfig = new FilterConfig(filterConfigKey);
            filterConfig.Show();
        }

        private void SaveLogClicked(object sender, RoutedEventArgs e)
        {
            var logMessages = new StringBuilder();
            foreach(var item in LvLogMessages.Items)
            {
                logMessages.AppendLine(item.ToString());
            }

            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text File (*.txt)|*.txt"
            };

            var showDialog = saveFileDialog.ShowDialog();
            if(showDialog != null && (bool)showDialog)
            {
                File.WriteAllText(saveFileDialog.FileName, logMessages.ToString());
            }
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetupMessageQueueTimer()
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

        private void SetupUdpListener()
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

        private void LoadWindowConfig()
        {
            var windowWidthKey = _root.GetValue("WindowWidth");
            var windowHeightKey = _root.GetValue("WindowHeight");

            if (windowWidthKey == null || windowHeightKey == null) { return; }
            Width = Convert.ToDouble(windowWidthKey);
            Height = Convert.ToDouble(windowHeightKey);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Save Config
            LvLogMessages.SaveConfig();
            _root.SetValue("WindowWidth", Width);
            _root.SetValue("WindowHeight", Height);

            //Close Registry Handle
            _root.Close();
        }
    }
}
