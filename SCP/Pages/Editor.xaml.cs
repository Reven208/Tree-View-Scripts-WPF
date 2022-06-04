using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;

namespace SCP.Pages
{
    /// <summary>
    /// Lógica de interacción para Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        private FileSystemWatcher fsw;
        private string watchingFolder;

        public Editor()
        {
            InitializeComponent();
            
            try { watch(); } catch { }
           
        }

        private void watch()
        {
            mainTreeView.Items.Clear();
            mainTreeView.Items.Add((object)this.CreateDirectoryNode(new DirectoryInfo("./Scripts")));
            watchingFolder = "./Scripts";
            fsw = new FileSystemWatcher(this.watchingFolder, "*.*");
            fsw.EnableRaisingEvents = true;
            fsw.IncludeSubdirectories = true;
            fsw.Created += new FileSystemEventHandler(this.changed);
            fsw.Changed += new FileSystemEventHandler(this.changed);
            fsw.Renamed += new RenamedEventHandler(this.renamed);
            fsw.Deleted += new FileSystemEventHandler(this.changed);
        }
        private void changed(object source, FileSystemEventArgs e) => this.mainTreeView.Dispatcher.Invoke((Action)(() =>
        {
            mainTreeView.Items.Clear();
            mainTreeView.Items.Add((object)this.CreateDirectoryNode(new DirectoryInfo("./Scripts")));
        }));

        private void renamed(object source, RenamedEventArgs e) => this.mainTreeView.Dispatcher.Invoke((Action)(() =>
        {
            mainTreeView.Items.Clear();
            mainTreeView.Items.Add((object)this.CreateDirectoryNode(new DirectoryInfo("./Scripts")));
        }));

        private TreeViewItem GetTreeView(string tag, string text, string imagePath)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Foreground = (Brush)new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFB4B4B4"));
            treeViewItem.Tag = (object)tag;
         
            treeViewItem.IsExpanded = false;
            StackPanel stackPanel = new StackPanel();
            stackPanel.CanHorizontallyScroll = false;
           
            stackPanel.Orientation = Orientation.Horizontal;
            Image image = new Image();
            image.Source = (ImageSource)new BitmapImage(new Uri("pack://application:,,/Pngs/" + imagePath));
            image.Width = 16.0;
            image.Height = 16.0;
            RenderOptions.SetBitmapScalingMode((DependencyObject)image, BitmapScalingMode.HighQuality);
            Label label = new Label();
            label.Content = (object)text;
            label.Foreground = (Brush)new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFB4B4B4"));
            stackPanel.Children.Add((UIElement)image);
            stackPanel.Children.Add((UIElement)label);
            treeViewItem.Header = (object)stackPanel;
            treeViewItem.ToolTip = (object)imagePath;
            ToolTipService.SetIsEnabled((DependencyObject)treeViewItem, false);
            return treeViewItem;
        }


        private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeViewItem treeView = this.GetTreeView(directoryInfo.FullName, directoryInfo.Name, "Folder.png");
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                //ADD SCRIPTS
                treeView.Items.Add((object)this.CreateDirectoryNode(directory));
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".lua")
                    //Lua PNG
                    treeView.Items.Add((object)this.GetTreeView(file.FullName, file.Name, "Script.png"));
                else if (file.Extension == ".txt")
                    //TXT PNG
                    treeView.Items.Add((object)this.GetTreeView(file.FullName, file.Name, "Script.png"));
              else
                    //NOT TXT OR  LUA PNG
                    treeView.Items.Add((object)this.GetTreeView(file.FullName, file.Name, "File.png"));

            }
            return treeView;
        }

        private void mainTreeView_SelectedItemChanged(object sender,RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                //If Is Selected
                if (this.mainTreeView.SelectedItem == null)
                    return;
                TreeViewItem selectedItem = this.mainTreeView.SelectedItem as TreeViewItem;
                string str = selectedItem.Tag.ToString();
                //Dont Select Folder
                if (selectedItem.ToolTip.ToString().EndsWith("Folder.png"))
                {
                    return;
                }
                //If Select TXT OR LUA
                if (str.EndsWith(".txt") || str.EndsWith(".lua") && !selectedItem.ToolTip.ToString().EndsWith("Folder.png"))
                {
                    StreamReader streamReader = new StreamReader(selectedItem.Tag.ToString());
                 //Charge File To Text Editor   //GetCurrent().Text = streamReader.ReadToEnd();
                }
                else
                //IF NO IS TXT O LUA
                {
                    MessageBox.Show("Invalide");
                }
            }
            catch (Exception ex)
            {
                //INT ERROR
                int num = (int)MessageBox.Show(ex.ToString());
            }
        }

   
    }

}
