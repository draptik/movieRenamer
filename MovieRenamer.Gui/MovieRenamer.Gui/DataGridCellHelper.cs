using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace MovieRenamer.Gui
{
    /// <summary>
    ///     From http://www.muxtonmumbles.blogspot.de/2011/03/single-click-editing-for-checkboxes-in.html
    /// </summary>
    public static class DataGridCellHelper
    {
        public static readonly DependencyProperty IsSingeClickInCellProperty =
            DependencyProperty.RegisterAttached("IsSingleClickInCell", typeof (bool), typeof (DataGrid),
                new FrameworkPropertyMetadata(false, OnIsSingleClickInCellSet));

        public static void SetIsSingleClickInCell(UIElement element, bool value)
        {
            element.SetValue(IsSingeClickInCellProperty, value);
        }

        public static bool GetIsSingleClickInCell(UIElement element)
        {
            return (bool) element.GetValue(IsSingeClickInCellProperty);
        }

        private static void OnIsSingleClickInCellSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) (DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue)) {
                if ((bool) e.NewValue) {
                    var dataGrid = sender as DataGrid;
                    Debug.Assert(dataGrid != null);
                    EventManager.RegisterClassHandler(typeof (DataGridCell), UIElement.PreviewMouseLeftButtonUpEvent,
                        new RoutedEventHandler(OnPreviewMouseLeftButtonDown));
                }
            }
        }

        private static void OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly) {
                IEnumerable<CheckBox> checkboxestmp = FindVisualChildren<CheckBox>(cell);
                var checkboxes = checkboxestmp as IList<CheckBox> ?? checkboxestmp.ToList();
                if (checkboxes.Any()) {
                    foreach (CheckBox checkbox in checkboxes.Where(checkbox => checkbox.IsEnabled)) {
                        checkbox.Focus();
                        checkbox.IsChecked = !checkbox.IsChecked;
                        BindingExpression bindingExpression = checkbox.GetBindingExpression(ToggleButton.IsCheckedProperty);
                        if (bindingExpression != null) {
                            bindingExpression.UpdateSource();
                        }
                    }
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        yield return (T) child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}