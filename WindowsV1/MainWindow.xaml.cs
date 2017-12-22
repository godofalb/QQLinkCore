using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QQLinkCore;
using QQLinkPlugIn;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Security.Permissions;
using System.Diagnostics;
namespace WindowsV1
{
   
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    public partial class MainWindow : Window
    {
        List<PlugInP> plugs = new List<PlugInP>();
        SQQLinker linker;
        delegate void l();
        FileIOPermission permission;
        string currentdir;
#warning 修改界面
        public MainWindow()
        {
            
            InitializeComponent();
            currentdir = ConfigurationManager.AppSettings[0];
            if (currentdir.ToLower()== "notdef")
            {
                currentdir = Directory.GetCurrentDirectory() + @"\pluginDir";
            }
            if (!Directory.Exists(currentdir))
            {
                Directory.CreateDirectory(currentdir);
            }
            dir.Text = currentdir;
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener(string.Format(@"{0}\d.log",  currentdir)));
         
            

            linker = new SQQLinker(currentdir);
            unload.IsEnabled = false;
            reload.IsEnabled = false;
            /*
            ss.Add(new PlugInP() { Name = "sad",Used=true });
           
            this.DataContext = ss;
              
             */
            permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, currentdir);
            permission.PermitOnly();
           
           
            Thread newt = new Thread(Load);
            Console.WriteLine("Start");
            newt.Start(currentdir);

        }
        private void Load(object p)
        {
            string path = (string)p;
           
            
            
            foreach (string file in Directory.GetFiles(path))
            {
                
                if (file.EndsWith(".dll"))
                {
                    Assembly newassembly = Assembly.LoadFrom(file);

                    foreach (Type tt in newassembly.GetTypes())
                    {
                      
                        foreach (object obj in tt.GetCustomAttributes(typeof(IsPlugIn), false))
                        {
                            IsPlugIn sp = obj as IsPlugIn;
                            if (sp.Use)
                            {
                                plugs.Add(new PlugInP() { Used=sp.Use,Class=tt.Name});
                                LoadXML(string.Format("{0}\\{1}.xml",path, tt.Name),plugs[plugs.Count-1],sp.NeedD,sp.NeedG,sp.NeedF);
                            }
                        }
                    }
                }

            }

            this.Dispatcher.Invoke(new l(() => { plugins.DataContext = plugs; }));
        }
   
       
      
        private void LoadXML(string path,PlugInP data,bool needf,bool needg,bool needd)
        {

            XmlDocument doc = new XmlDocument();
            
            doc.Load(path);
            data.Name = doc.SelectSingleNode("/GeneralSetting/name").Attributes[0].Value;
            data.FChangeable = needf;
            data.GChangeable = needg;
            data.DChangeable = needd;
            data.NeedF = false;
            data.NeedG = false;
            data.NeedD = false;
            XmlNodeList nodes = doc.SelectNodes("/GeneralSetting/add");
            
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes[1].Value.ToLower() == "need")
                {
                    switch (node.Attributes[0].Value)
                    {
                        case "NeedF": data.NeedF = needf; break;
                        case "NeedG": data.NeedG = needg; break;
                        case "NeedD": data.NeedD = needd; break;
                        default: break;
                    }
                }
            }
            
        }
        private void do_Click_1(object sender, RoutedEventArgs e)
        {
            
            Stream picstream = linker.getContect();
            
            Bitmap bitmap = Bitmap.FromStream(picstream) as Bitmap;
            BitmapSource IS = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            loginbox1.Source = IS;
            unload.IsEnabled = true;
            Sig.Content = "正常";
        }

        private void reload_Click_1(object sender, RoutedEventArgs e)
        {
            linker.loadDomain(currentdir);
            Sig.Content = "正常";
            reload.IsEnabled = false;
        }

        private void unload_Click_1(object sender, RoutedEventArgs e)
        {
            linker.Unload();
            Sig.Content = "需要刷新";
            reload.IsEnabled = true;
        }

       

        private void refresh_Click_1(object sender, RoutedEventArgs e)
        {
            plugs = new List<PlugInP>();
            plugins.DataContext = null;
            Thread newt = new Thread(Load);
            Console.WriteLine("Start");
            newt.Start(currentdir);
            linker.Unload();
            linker.loadDomain(currentdir);
            reload.IsEnabled = false;
            if (Sig.Content == "需要刷新")
            {
                Sig.Content = "正常";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PlugInP finder;
            string name=null;
            if (sender is Button)
            {
                name = ((FrameworkElement)sender).DataContext.ToString();
                var d = from plug in plugs
                        where plug.Class == name
                        select plug;
              
               
                IEnumerator<PlugInP> s = d.GetEnumerator();
                s.MoveNext();
                finder = s.Current;
            }
            else
            {
               
                finder = ((FrameworkElement)sender).DataContext as PlugInP;
                name = finder.Class;
               
            }
         
           
            Edit newweb = new Edit(string.Format("{0}\\{1}.xml", dir.Text, name),finder);
            string f = Sig.Content as string;
            Sig.Content = "需要刷新";
            //((Button)sender).DataContext.ToString()
            newweb.ShowDialog();
            linker.Unload();
            linker.loadDomain(currentdir);
            reload.IsEnabled = false;
             Sig.Content=f;
            /*
            Thread newt = new Thread(Load);
            Console.WriteLine("Start");
            newt.Start(dir.Text);
             * */
            
            

        }

        private void ChangeDir_Click_1(object sender, RoutedEventArgs e)
        {
            currentdir = dir.Text;
            permission.SetPathList(FileIOPermissionAccess.AllAccess,currentdir);
            linker.Unload();
            linker.loadDomain(currentdir);
            plugs = new List<PlugInP>();
            plugins.DataContext = null;
            Thread newt = new Thread(Load);
            Console.WriteLine("Start");
            newt.Start(currentdir);
            Console.WriteLine(currentdir);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {

            MessageBoxResult res= MessageBox.Show("是否保存插件路径","关闭选项",MessageBoxButton.YesNo);
            Console.WriteLine(res);
            if (res==MessageBoxResult.Yes)
            {

                Configuration config = ConfigurationManager.OpenExeConfiguration("WindowsV1.exe");
                Console.WriteLine(config.AppSettings.File);
                config.AppSettings.Settings.Clear();
                config.AppSettings.Settings.Add("PlugInDir", currentdir);
                config.Save();
            }
            return;
        }
    }
}
