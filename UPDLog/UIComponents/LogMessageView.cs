using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Win32;

using UPDLog.DataStructures;
using UPDLog.Messaging;
using UPDLog.Utilities;

namespace UPDLog.UIComponents
{
    public class LogMessageView : ListView
    {
        private RegistryKey _root;
        private readonly LogFilterRule _logFilterRule = new LogFilterRule();
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public void LoadConfig(RegistryKey key)
        {
            ContextMenu = _contextMenu;
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
            if (FilterEnabled()) { Items.Filter = FilterMessage; }
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
            using (var activeFilterKey = _root.OpenSubKey("Filters"))
            {
                activeFilter = activeFilterKey.GetValue("ActiveFilter").ToString();    
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
            }

            Items.Filter = FilterMessage;
        }

        public bool FilterMessage(object obj)
        {
            var lm = obj as LogMessage;

            if (lm == null) { return false; }
            var propertyColumn = lm.GetType().GetProperty(_logFilterRule.Column);

            switch (_logFilterRule.Action)
            {
                case "Like":
                    var likeRegex = new Regex(_logFilterRule.Content);
                    return likeRegex.Match((string) propertyColumn.GetValue(lm, null)).Success;
                case "Not Like":
                    var notLikeRegex = new Regex(_logFilterRule.Content);
                    return !notLikeRegex.Match((string)propertyColumn.GetValue(lm, null)).Success;
                case "Equal To":
                    return (string)propertyColumn.GetValue(lm, null) == _logFilterRule.Content;
                case "Not Equal To":
                    return (string)propertyColumn.GetValue(lm, null) != _logFilterRule.Content;
                case "Greater Than":
                    return false;
                case "Less Than":
                    return false;
            }

            return false;
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            var item = this.GetGenericItemAt(Mouse.GetPosition(this));
            if (item is GridViewColumnHeader)
            {
                BuildContextMenu();
            }
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
            _contextMenu.Items.Clear();
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
            var col = ((GridView) View).Columns.First(x => x.Header == menuItem.Header);
            if (col == null) { return; }

            col.Width = menuItem.IsChecked ? 100 : 0;
        }
    }
}
