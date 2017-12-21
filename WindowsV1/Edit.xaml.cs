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
        string des;
        public Edit(string path,PlugInP data)
        {

            InitializeComponent();
            filepath = path;
            Data = data;
            BaseData.DataContext = data;
            BaseData2.DataContext = data;
            Load(path);
        }
        public void Load(string path)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            XmlNodeList nodes = doc.SelectNodes("/GeneralSetting/UserSetting/add");
            Console.WriteLine("Loading");
            XmlNode gNode = doc.SelectSingleNode("/GeneralSetting/GroupList");
            XmlNode fNode = doc.SelectSingleNode("/GeneralSetting/FriendList");
            XmlNode dNode = doc.SelectSingleNode("/GeneralSetting/DisList");
            if (dNode != null)
            {
                Data.Dis = dNode.Attributes[0].Value;
                Data.DisMode = dNode.Attributes[1].Value;
            }
            if (gNode != null)
            {
                Data.Groups = gNode.Attributes[0].Value;
                Data.GroupMode = gNode.Attributes[1].Value;
            }
            if (fNode != null)
            {
                Data.Friends= fNode.Attributes[0].Value;
                Data.FriendMode = fNode.Attributes[1].Value;
            }
            foreach (XmlNode node in nodes)
            {
                userdata.Add(new Pair(){Key= node.Attributes[0].Value,Value=node.Attributes[1].Value});
                Console.WriteLine("Have{0} {1}", node.Attributes[0].Value, node.Attributes[1].Value);
            }
            XmlNode node1=doc.SelectSingleNode("/GeneralSetting/des");
            if (node1 != null)
            {
                Des.Text = node1.Attributes[0].Value;
                des = Des.Text;
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
            XmlNode node1 = doc.SelectSingleNode("/GeneralSetting");
            if (Data.Name == null)
            {
                XmlNode namenode = doc.CreateNode(XmlNodeType.Element, "name", null);
                XmlAttribute nameattribute = doc.CreateAttribute("value");
                nameattribute.Value = "未命名插件1";
                namenode.Attributes.Append(nameattribute);
                node1.AppendChild(namenode);

            }
            else
            {
                XmlNode node = node1.SelectSingleNode("name");
                node.Attributes[0].Value = Data.Name;

            }
            if (des == null)
            {
                XmlNode namenode = doc.CreateNode(XmlNodeType.Element, "des", null);
                XmlAttribute nameattribute = doc.CreateAttribute("value");
                nameattribute.Value = "无描述";
                namenode.Attributes.Append(nameattribute);
                node1.AppendChild(namenode);

            }
            else
            {
                XmlNode node = node1.SelectSingleNode("des");
                node.Attributes[0].Value = des;

            }
            XmlNode friendNode = node1.SelectSingleNode("FriendList");
            if (friendNode == null)
            {
                friendNode = doc.CreateNode(XmlNodeType.Element, "FriendList", null);
                XmlAttribute nameattribute = doc.CreateAttribute("value");
                nameattribute.Value = "";
                friendNode.Attributes.Append(nameattribute);
                XmlAttribute nameat2 = doc.CreateAttribute("Mode");
                nameat2.Value = "reject";
                friendNode.Attributes.Append(nameat2);
                node1.AppendChild(friendNode);

            }
            else
            {

                friendNode.Attributes[0].Value = Data.Friends;
                friendNode.Attributes[1].Value = Data.FriendMode;
            }
            XmlNode gnode = node1.SelectSingleNode("GroupList");
            if (gnode == null)
            {
                gnode = doc.CreateNode(XmlNodeType.Element, "GroupList", null);
                XmlAttribute nameattribute = doc.CreateAttribute("value");
                nameattribute.Value = "";
                gnode.Attributes.Append(nameattribute);
                XmlAttribute nameat2 = doc.CreateAttribute("Mode");
                nameat2.Value = "reject";
                gnode.Attributes.Append(nameat2);
                node1.AppendChild(gnode);

            }
            else
            {

                gnode.Attributes[0].Value = Data.Groups;
                gnode.Attributes[1].Value = Data.GroupMode;
            }
            XmlNode dnode = node1.SelectSingleNode("DisList");
            if (dnode == null)
            {
                dnode = doc.CreateNode(XmlNodeType.Element, "DisList", null);
                XmlAttribute nameattribute = doc.CreateAttribute("value");
                nameattribute.Value = "";
                dnode.Attributes.Append(nameattribute);
                XmlAttribute nameat2 = doc.CreateAttribute("Mode");
                nameat2.Value = "reject";
                dnode.Attributes.Append(nameat2);
                node1.AppendChild(dnode);

            }
            else
            {

                dnode.Attributes[0].Value = Data.Dis;
                dnode.Attributes[1].Value = Data.DisMode;
            }
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
