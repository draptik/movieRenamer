using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MovieRenamer.Gui.Model;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MovieRenamer.Gui
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string destinationFolder;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string DestinationFolder
        {
            get { return destinationFolder; }
            set
            {
                destinationFolder = value;
                OnPropertyChanged("DestinationFolder");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            // http://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp
            var dlg = new OpenFileDialog {Multiselect = true};
            bool? showDialog = dlg.ShowDialog();
            if (showDialog == true) {
                BindToGrid(dlg.FileNames);
            }
        }


        private void BindToGrid(string[] filenamea)
        {
            var mediaFiles = new MediaFiles(filenamea);
            DgMediaFiles.ItemsSource = mediaFiles.Files;
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // http://stackoverflow.com/questions/13579034/how-do-you-rename-datagrid-columns-when-autogeneratecolumns-true
            string displayName = GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName)) {
                e.Column.Header = displayName;
            }
        }

        private string GetPropertyDisplayName(object descriptor)
        {
            var propertyDescriptor = descriptor as PropertyDescriptor;
            if (propertyDescriptor != null) {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = propertyDescriptor.Attributes[typeof (DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default)) {
                    return displayName.DisplayName;
                }
            }
            else {
                var propertyInfo = descriptor as PropertyInfo;
                if (propertyInfo != null) {
                    // Check for DisplayName attribute and set the column header accordingly
                    Object[] attributes = propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i) {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default) {
                            return displayName.DisplayName;
                        }
                    }
                }
            }
            return null;
        }

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Do_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var files = DgMediaFiles.ItemsSource as IEnumerable<MediaFile>;
            if (files != null)
                foreach (MediaFile file in files) {
                    file.DestinationFolder = DestinationFolder;
                    file.Rename();
                    file.Save();
                    AddToHistory("saving file " + file.GetRenamedFile());
                }
        }

        private void Do_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(DestinationFolder);
        }

        private void BtnSelectDestination_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            string selectedPath = dlg.SelectedPath;
            DestinationFolder = selectedPath;
            AddToHistory("setting selected path to " + selectedPath);
        }

        private ObservableCollection<string> history = new ObservableCollection<string>();

        public IEnumerable<string> History { get { return history; } }

        private void AddToHistory(string item)
        {
            if (!history.Contains(item)) {
                history.Add(item);
            }
        }
    }
}