using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Win32;

namespace UPDLog.UIComponents
{
    public class LogMessageView : ListView
    {
        //private ItemCollection _hiddenColumns;
        public void LoadConfig(RegistryKey key)
        {    
        }

        public void ApplyFilter()
        {    
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            var item = this.GetGenericItemAt(Mouse.GetPosition(this));
            if (item is ListViewItem)
            {

            }
            else if(item is GridViewColumnHeader)
            {
                
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
#if false
            LvLogMessages.ContextMenu.Items.Clear();
            //Not yet implemented
            foreach (var col in ((GridView)LvLogMessages.View).Columns)
            {
                var name = col.Header.ToString();

                var obj = (DependencyObject)col;
                var vis = (bool)obj.GetValue(IsVisibleProperty);
                LvLogMessages.ContextMenu.Items.Add(name);
            }
#endif
        }
    }
}
