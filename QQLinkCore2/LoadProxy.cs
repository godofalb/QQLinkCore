using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQLinkPlugIn;
using System.IO;
using System.Reflection;
using System.Diagnostics;
namespace QQLinkCore
{
    [Serializable]
    public class LoadProxy:MarshalByRefObject
    {
        
        List<QQPlugInBase> plugs;
        string dir;
        
        public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (QQPlugInBase plug in plugs)
            {
                yield return plug;
            }
        }
        public int Size
        {
            get { return plugs.Count; }
        }
        public QQPlugInBase.sendBack DoWork(QQPlugInBase.ReceiveMsg rmg,int index)
        {
           
            if ((plugs[index].ReceiveType & rmg.type) != polltype.none)
            {
                return plugs[index].ReceiveMessage(rmg);
            }
            else
            {
                return QQPlugInBase.DontSend;
            }
        }
#warning add filter
        public void load(string mdir,QQLinkCore.SQQLinker linker)
        {
            dir = mdir;
            plugs = new List<QQPlugInBase>();
            QQPlugInBase.QQLinker = linker;
         
            foreach (string file in Directory.GetFiles(dir))
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
                                QQPlugInBase temp=newassembly.CreateInstance(tt.FullName) as QQPlugInBase;
                                if (temp.LoadXML(string.Format("{0}\\{1}.xml", mdir, tt.Name), sp.NeedF, sp.NeedG, sp.NeedD))
                                {
                                    plugs.Add(temp);
                                }
                                
                               /* Console.WriteLine("F:{0},G:{1},D:{2}", plugs[plugs.Count - 1].ReceiveType & polltype.message, plugs[plugs.Count - 1].ReceiveType & polltype.group_message, plugs[plugs.Count - 1].ReceiveType & polltype.discu_message);
                                */
                               
                            }
                        }

                    }
                }
            }
        }
    }
}
