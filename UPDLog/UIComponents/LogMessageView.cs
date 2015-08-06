using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

using UPDLog.Utilities;
using UPDLog.DataStructures;

namespace UPDLog.UIComponents
{
    public class LogMessageView : ListView
    {
        private RegistryKey _root;
        private ScrollViewer _scrollViewer; 
        private readonly ContextMenu _contextMenu = new ContextMenu();
        private readonly LogFilterRule _logFilterRule = new LogFilterRule();

        public void LoadConfig(RegistryKey key)
        {
            _root = key;

            using (var logMessageViewKey = _root.GetOrCreateRegistryKey(@"Interface\LogMessageView", true))
            {
                foreach (var column in ((GridView)View).Columns)
                {
                    var colWidth = logMessageViewKey.GetValue(column.Header + "Width");

                    if (colWidth == null) { break; }
                    column.Width = Convert.ToDouble(colWidth);
                }
            }

            BuildContextMenu();
        }

        public void SaveConfig()
        {
            using (var logMessageViewKey = _root.GetOrCreateRegistryKey(@"Interface\LogMessageView", true))
            {
                foreach (var column in ((GridView)View).Columns)
                {
                    logMessageViewKey.SetValue(column.Header + "Width", column.Width);
                }
            }
        }

        public bool FilterEnabled()
        {
            return Items.Filter != null;
        }

        public void AddLogMessage(LogMessage lm)
        {
            Items.Add(lm);

            if (_scrollViewer != null &&
                _scrollViewer.VerticalOffset.Equals(_scrollViewer.ScrollableHeight))
            {
                _scrollViewer.ScrollToBottom(); 
            }
            
            if (FilterEnabled()) { Items.Filter = ShowMessage; }
        }

        public override void OnApplyTemplate()  
        {  
            base.OnApplyTemplate();  
  
            //Store a reference to the internal ScrollViewer for sticky scrolling
            _scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(this) as ScrollViewer;  
        }

        private static DependencyObject RecursiveVisualChildFinder<T>(DependencyObject rootObject)
        {
            var child = VisualTreeHelper.GetChild(rootObject, 0);
            if (child == null) { return null; }

            return child.GetType() == typeof(T) ? child : RecursiveVisualChildFinder<T>(child);
        }  

        public void ApplyFilter(bool enable)
        {
            if (!enable)
            {
                Items.Filter = null;
                return;
            }

            //Load active filter in registry
            string activeFilter;
            using (var activeFilterKey = _root.GetOrCreateRegistryKey("Filters", true))
            {
                activeFilter = activeFilterKey.GetValue("ActiveFilter", "").ToString();    
            }

            if (string.IsNullOrWhiteSpace(activeFilter)) { return; }
            using (var activeFilterKey = _root.OpenSubKey(@"Filters\" + activeFilter))
            {
                if (activeFilterKey == null)
                {
                    Items.Filter = null;
                    return;
                }

                _logFilterRule.Column = activeFilterKey.GetValue("FilterColumn", "").ToString();
                _logFilterRule.Action = activeFilterKey.GetValue("FilterAction", "").ToString();
                _logFilterRule.Content = activeFilterKey.GetValue("FilterContent", "").ToString();
                _logFilterRule.Acceptance = Convert.ToBoolean(activeFilterKey.GetValue("Acceptance", "").ToString());
            }

            Items.Filter = ShowMessage;
        }

        public bool ShowMessage(object obj)
        {
            var lm = obj as LogMessage;

            if (lm == null) { return true; }
            var propertyColumn = lm.GetType().GetProperty(_logFilterRule.Column);

            if (propertyColumn.GetValue(lm, null) == null) { return true; }

            //Parse filter action text
            var match = false;
            switch (_logFilterRule.Action)
            {
                case "Like":
                    var likeRegex = new Regex(_logFilterRule.Content);
                    match = likeRegex.Match((string)propertyColumn.GetValue(lm, null)).Success;
                    break;
                case "Not Like":
                    var notLikeRegex = new Regex(_logFilterRule.Content);
                    match = !notLikeRegex.Match((string)propertyColumn.GetValue(lm, null)).Success;
                    break;
                case "Equal To":
                    match = (string)propertyColumn.GetValue(lm, null) == _logFilterRule.Content;
                    break;
                case "Not Equal To":
                    match = (string)propertyColumn.GetValue(lm, null) != _logFilterRule.Content;
                    break;
                case "Greater Than":
                    match = true;
                    break;
                case "Less Than":
                    match = true;
                    break;
            }

            if(match && _logFilterRule.Acceptance) { return true; }
            if (match && !_logFilterRule.Acceptance) { return false; }
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) || e.Key != Key.C) { return; }

            var builder = new StringBuilder();
            foreach (var item in SelectedItems)
            {
                builder.AppendLine(((LogMessage)item).ToString());
            }

            Clipboard.SetText(builder.ToString());
        }

        private void BuildContextMenu()
        {
            ContextMenu = _contextMenu;
            foreach (var col in ((GridView)View).Columns)
            {
                var menuItem = new MenuItem()
                {
                    IsCheckable = true,
                    IsChecked = col.Width > 0,
                    Header = col.Header.ToString(),
                };

                menuItem.Click += ColumnHeaderClicked;
                _contextMenu.Items.Add(menuItem);
            }
        }

        void ColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) { return; }
            var col = ((GridView) View).Columns.First(x => x.Header == menuItem.Header);
            if (col == null) { return; }

            col.Width = menuItem.IsChecked ? 100 : 0;
        }
    }
}
