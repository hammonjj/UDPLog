using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;

using Microsoft.Win32;

using UPDLog.DataStructures;
using UPDLog.Utilities;

namespace UPDLog.Windows
{
    /// <summary>
    /// Interaction logic for FilterConfig.xaml
    /// </summary>
    public partial class FilterConfig
    {
        private readonly RegistryKey _root;
        public FilterConfig(RegistryKey root)
        {
            _root = root.GetOrCreateRegistryKey("Filters", true); ;
            InitializeComponent();
            BuildFilterColumns();
            BuildFilterActions();
            BuildAcceptanceFilter();

            //Load Active Filter
            var activeFilter = _root.GetValue("ActiveFilter", "").ToString();
            if (!string.IsNullOrWhiteSpace(activeFilter)) { LoadActiveFilter(activeFilter); }
        }

        private void BuildAcceptanceFilter()
        {
            var list = new ObservableCollection<string>
            {
                "True", //Accept
                "False"  //Reject
            };

            CmbAcceptance.ItemsSource = list;
        }

        private void LoadActiveFilter(string activeFilter)
        {
            using (var activeFilterKey = _root.OpenSubKey(activeFilter))
            {
                if (activeFilterKey == null) { return; }

                CmbFilterCol.SelectedItem = activeFilterKey.GetValue("FilterColumn");
                CmbFilterAction.SelectedItem = activeFilterKey.GetValue("FilterAction", "");
                TxtFilterContent.Text = activeFilterKey.GetValue("FilterContent", "").ToString();
                CmbAcceptance.SelectedItem = activeFilterKey.GetValue("Acceptance", "");
            }
        }

        private void BuildFilterActions()
        {
            var list = new ObservableCollection<string>
            {
                "Like", 
                "Not Like",
                "Equal To",
                "Not Equal To"
                //"Greater Than", => Not yet implemented
                //"Less Than" => Not yet implemented
            };

            CmbFilterAction.ItemsSource = list;
        }

        private void BuildFilterColumns()
        {
            var lm = new LogMessage();
            var propertyList = new ObservableCollection<string>();
            foreach (var prop in lm.GetType().GetProperties())
            {
                propertyList.Add(prop.Name);
            }

            CmbFilterCol.ItemsSource = propertyList;
        }

        private void ApplyClicked(object sender, RoutedEventArgs e)
        {
            var filterName = "Default";

            var content = TxtFilterContent.Text;
            var column = CmbFilterCol.SelectedItem.ToString();
            var action = CmbFilterAction.SelectedItem.ToString();
            var acceptance = CmbAcceptance.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(column) ||
                string.IsNullOrWhiteSpace(content) ||
                string.IsNullOrWhiteSpace(action) ||
                string.IsNullOrWhiteSpace(acceptance))
            {
                //Highlight empty fields in red
                TxtFilterContent.BorderBrush = Brushes.Red;
                TxtFilterContent.BorderThickness = new Thickness(2);
                return;
            }

            //Save Filter to Registry
            var filterSubKey = _root.OpenSubKey(filterName, true) ?? _root.CreateSubKey(filterName);
            if (filterSubKey != null)
            {
                _root.SetValue("ActiveFilter", filterName);
                filterSubKey.SetValue("FilterColumn", column);
                filterSubKey.SetValue("FilterAction", action);
                filterSubKey.SetValue("FilterContent", content);
                filterSubKey.SetValue("Acceptance", acceptance);
            }
            else
            {
                MessageBox.Show(
                    this, 
                    "Error saving filter configuration", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _root.Close();
        }
    }
}
