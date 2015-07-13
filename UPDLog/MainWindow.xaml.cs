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
using UPDLog.Utilities;
using UPDLog.Windows;

namespace UPDLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
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
            _root = Registry.CurrentUser.GetOrCreateRegistryKey(@"Software\Aventura\UdpLog", true);

            SetupUdpListener();
            SetupMessageQueueTimer();

            LoadWindowConfig();
            LvLogMessages.LoadConfig(_root);
        }

        private void PumpMessageQueue(object sender, ElapsedEventArgs e)
        {
            if(_messageQueue.IsEmpty) { return; }
            
            //Disable timer so we don't bind up the gui thread
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
            BtnStart.IsEnabled = false;
            LvLogMessages.Focus();
        }

        private void StopListeningClicked(object sender, RoutedEventArgs e)
        {
            _updListener.StopListening();
            _messagePumpTimer.Enabled = false;
            BtnStart.IsEnabled = true;
            LvLogMessages.Focus();
        }

        private void ClearLogClicked(object sender, RoutedEventArgs e)
        {
            LvLogMessages.Items.Clear();
            LvLogMessages.Focus();
        }

        private void ApplyFilterClicked(object sender, RoutedEventArgs e)
        {
            //This is not the right way to do this, but I'll leave it for now as I have more pressing things to
            //finish
            LvLogMessages.ApplyFilter(!LvLogMessages.FilterEnabled());
            BtnFilter.IsChecked = LvLogMessages.FilterEnabled();
            LvLogMessages.Focus();
        }

        private void AddMarkerClicked(object sender, RoutedEventArgs e)
        {
            LvLogMessages.AddLogMessage(new LogMessage());
            LvLogMessages.Focus();
        }

        private void PreferencesClicked(object sender, RoutedEventArgs e)
        {
        }

        private void FilterConfigClicked(object sender, RoutedEventArgs e)
        {
            var filterConfig = new FilterConfig(_root)
            {
                Owner = this
            };

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
            var messageQueuePumpTimeKey = _root.GetValue("MessageQueueTimer");
            if(messageQueuePumpTimeKey == null)
            {
                _root.SetValue("MessageQueueTimer", 500);
                _messagePumpTimer = new System.Timers.Timer(500);
            }
            else
            {
                _messagePumpTimer = new System.Timers.Timer(Convert.ToInt32(messageQueuePumpTimeKey));
            }

            _messagePumpTimer.Elapsed += PumpMessageQueue;
        }

        private void SetupUdpListener()
        {
            var listenPortKey = _root.GetValue("ListenPort");
            if (listenPortKey == null)
            {
                _root.SetValue("ListenPort", 514);
                _updListener = new UdpListener(514, ref _messageQueue);
            }
            else
            {
                _updListener = new UdpListener(Convert.ToInt32(listenPortKey), ref _messageQueue);
            }
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
            //Stop Listening Thread
            _updListener.StopListening();
            _messagePumpTimer.Enabled = false;

            //Save Config
            LvLogMessages.SaveConfig();
            _root.SetValue("WindowWidth", Width);
            _root.SetValue("WindowHeight", Height);

            //Close Registry Handle
            _root.Close();
            //_udpListenerThread.Join();
        }
    }
}
