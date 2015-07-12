using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using UPDLog.DataStructures;

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
            _root = root;
            InitializeComponent();
            BuildFilterColumns();
            BuildFilterActions();

            //Load Active Filter
            var activeFilter = _root.GetValue("ActiveFilter", "").ToString();
            if (!string.IsNullOrWhiteSpace(activeFilter)) { LoadActiveFilter(activeFilter); }
        }

        private void LoadActiveFilter(string activeFilter)
        {
            using (var activeFilterKey = _root.OpenSubKey(activeFilter))
            {
                if (activeFilterKey == null) { return; }

                cmbFilterCol.SelectedItem = activeFilterKey.GetValue("FilterColumn");
                cmbFilterAction.SelectedItem = activeFilterKey.GetValue("FilterAction", "");
                txtFilterContent.Text = activeFilterKey.GetValue("FilterContent", "").ToString();
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

            cmbFilterAction.ItemsSource = list;
        }

        private void BuildFilterColumns()
        {
            var lm = new LogMessage();
            var propertyList = new ObservableCollection<string>();
            foreach (var prop in lm.GetType().GetProperties())
            {
                propertyList.Add(prop.Name);
            }

            cmbFilterCol.ItemsSource = propertyList;
        }

        private void ApplyClicked(object sender, RoutedEventArgs e)
        {
            var filterName = "Default";

            var content = txtFilterContent.Text;
            var column = cmbFilterCol.SelectedItem.ToString();
            var action = cmbFilterAction.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(column) ||
                string.IsNullOrWhiteSpace(content) ||
                string.IsNullOrWhiteSpace(action))
            {
                //Highlight empty fields in red
                txtFilterContent.BorderBrush = Brushes.Red;
                txtFilterContent.BorderThickness = new Thickness(2);
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
