using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UPDLog.Utilities
{
    public static class ListViewExtensions
    {
        public static DependencyObject GetGenericItemAt(this ListView listView, Point clientRelativePosition)
        {
            var hitTestResult = VisualTreeHelper.HitTest(listView, clientRelativePosition);
            var selectedItem = hitTestResult.VisualHit;
            while (selectedItem != null)
            {
                if (selectedItem is ListViewItem)
                {
                    break;
                }
                if (selectedItem is GridViewColumnHeader)
                {
                    break;
                }

                selectedItem = VisualTreeHelper.GetParent(selectedItem);
            }

            return selectedItem;
        }

        public static ListViewItem GetItemAt(this ListView listView, Point clientRelativePosition)
        {
            var hitTestResult = VisualTreeHelper.HitTest(listView, clientRelativePosition);
            var selectedItem = hitTestResult.VisualHit;
            while (selectedItem != null)
            {
                if (selectedItem is ListViewItem)
                {
                    break;
                }

                selectedItem = VisualTreeHelper.GetParent(selectedItem);
            }

            return (ListViewItem) selectedItem;
        }

        public static FrameworkElement GetLastVisibleElement(this ListView listView)
        {
            double height = listView.ActualHeight;

            double currentHeight = 0;
            FrameworkElement previous = null;

            foreach (FrameworkElement item in listView.Items)
            {
                currentHeight += item.ActualHeight;
                if (currentHeight > height)
                {
                    return previous;
                }

                previous = item;
            }

            return previous;
        }
    }
}
