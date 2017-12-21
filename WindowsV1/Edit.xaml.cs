using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
namespace WindowsV1
{
    /// <summary>
    /// Edit.xaml 的交互逻辑
    /// </summary>
    public partial class Edit : Window
    {
        List<Pair> userdata = new List<Pair>();
        string filepath;
        PlugInP Data;
        public Edit(string path,PlugInP data)
        {

            InitializeComponent();
            filepath = path;
            Data = data;
            BaseData.DataContext = data;
            Load(path);
        }
        public void Load(string path)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            XmlNodeList nodes = doc.SelectNodes("/GeneralSetting/UserSetting/add");
            Console.WriteLine("Loading");
            foreach (XmlNode node in nodes)
            {
                userdata.Add(new Pair(){Key= node.Attributes[0].Value,Value=node.Attributes[1].Value});
                Console.WriteLine("Have{0} {1}", node.Attributes[0].Value, node.Attributes[1].Value);
            }
            XmlNode node1=doc.SelectSingleNode("/GeneralSetting/des");
            if (node1 != null)
            {
                Des.Text = node1.Attributes[0].Value;
            }
            else
            {
                Des.Text = "";
            }
            DataList.DataContext = userdata;
        }

        private void Save_Click_1(object sender, RoutedEventArgs e)
        {

            Save();
            
        }
        void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            XmlNodeList nodes = doc.SelectNodes("/GeneralSetting/add");

            foreach (XmlNode node in nodes)
            {
               
                    switch (node.Attributes[0].Value)
                    {
                        case "NeedF": node.Attributes[1].Value = Data.NeedF ? "Need" : "NotNeed"; break;
                        case "NeedG": node.Attributes[1].Value = Data.NeedG ? "Need" : "NotNeed"; break;
                        case "NeedD": node.Attributes[1].Value = Data.NeedD ? "Need" : "NotNeed"; break;
                        case "Used": node.Attributes[1].Value = Data.Used ? "Yes" : "No"; break;
                        default: break;
                    }
                
            }
            nodes = doc.SelectNodes("/GeneralSetting/UserSetting/add");
            foreach (XmlNode node in nodes)
            {
                foreach (Pair p in userdata)
                {
                    if (p.Key == node.Attributes[0].Value)
                    {
                        node.Attributes[1].Value = p.Value;
                        
                    }
                }
            }
            doc.Save(filepath);
        }
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }
    }
}
