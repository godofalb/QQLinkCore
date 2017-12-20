using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace QQLinkPlugIn
{
    [AttributeUsage(AttributeTargets.Class,Inherited=false,AllowMultiple=false)]
    public class IsPlugIn:System.Attribute
    {
        
        public bool Use
        {
            get;
            set;
        }
        public bool NeedF
        {
            get;
            set;
        }
        public bool NeedG
        {
            get;
            set;
        }
        public bool NeedD
        {
            get;
            set;
        }

        public IsPlugIn()
        { 
            
        }
    }
    
    public delegate string GetSomething();
    public enum polltype:byte
    { 
        message=1,
        discu_message=2,
        group_message=4,
        none=0

    }
    [Serializable]
   
    [IsPlugIn(Use = false,NeedD=false,NeedF=false,NeedG=false)]
    public abstract class QQPlugInBase
    {
       
        [Serializable]
        public struct sendBack
        {
            public bool send;
            public string qq;
            public string message;
            public string font;
            public string color;
            public string size;
            public polltype type;
            public sendBack(bool s, string q, string m, polltype t)
            {
                send = s;
                qq = q;
                message=m;
                size ="10";
                color = "000000";
                font = "宋体";
                type = t;
            }
        }
        [Serializable]
        public struct ReceiveMsg
        {
           
            public string to_qq;
            public string from_qq;
            public string message;
            public string font;
            public string color;
            public string size;
            public string senderUin;
            public bool called;
            public polltype type;
            public ReceiveMsg(bool call,string tq,string fq, string m,polltype t)
            {
                type = t;
                called = call;
                to_qq = tq;
                from_qq = fq;
                message=m;
                size = "10";
                color = "000000";
                font = "宋体";
                
                senderUin = from_qq;
            }
            public ReceiveMsg(bool call, string tq, string fq, string m, string t)
            {
                
                switch (t)
                {
                    case "\"message\"":type=polltype.message;break;
                    case "\"discu_message\"": type = polltype.discu_message; break;
                    case "\"group_message\"": type = polltype.group_message; break;
                    default: type = polltype.none; break;
                }
                called = call;
                to_qq = tq;
                from_qq = fq;
                message = m;
                size = "10";
                color = "000000";
                font = "宋体";
                
                senderUin = from_qq;
            }
        }

        private static sendBack dontsend = new sendBack(false, null, null,polltype.none);
        public static sendBack DontSend
        {
            get { return dontsend; }
        }
        protected static IQQlinker qqLinker;
        protected Dictionary<string, string> userArg
        {
            get;
            set;
        }
     
        public polltype ReceiveType
        {
            get;
            set;
        }
        public static IQQlinker QQLinker
        {
            set { qqLinker = value; }
            get { return qqLinker; }
        }
        
        protected QQPlugInBase()
        {
           
        }
        public bool LoadXML(string path,bool needf,bool needg,bool needd)
        {
            XmlDocument doc = new XmlDocument();
            ReceiveType = polltype.none;
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("/GeneralSetting/add");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes[1].Value.ToLower() == "need")
                {
                    switch (node.Attributes[0].Value)
                    {
                        case "NeedF": if (needf) { ReceiveType |= polltype.message; } break;
                        case "NeedG": if (needg) { ReceiveType |= polltype.group_message; } break;
                        case "NeedD": if (needd) { ReceiveType |= polltype.discu_message; } break;
                        
                        default: break;
                    }
                }
                if (node.Attributes[0].Value == "Used")
                {
                    if (node.Attributes[1].Value != "Yes")
                    {
                        return false;
                    }

                }
            }
            nodes = doc.SelectNodes("/GeneralSetting/UserSetting/*");
            if (nodes != null && nodes.Count > 0)
            {
                userArg = new Dictionary<string, string>();
            }
            foreach (XmlNode node in nodes)
            {
                userArg.Add(node.Attributes[0].Value, node.Attributes[1].Value);
            }
            return true;
            
        }
        /// <summary>
        ///     用来重载的接收方法
        /// </summary>
        /// <param name="rmg"></param>
        /// <returns>返回sendBack，可以调用packSendBack,如果需要设置字体，字号和颜色则需要手动设置(目前不支持)</returns>
        public virtual sendBack ReceiveMessage(ReceiveMsg rmg)
        {

            return packSendBack(polltype.none);
        }
        /// <summary>
        /// 可以用来打包返回类型
        /// </summary>
        /// <param name="t">返回消息的类型，polltype枚举</param>
        /// <param name="send">是否发送消息，如果不需要发送消息则设置false，或者直接返回默认包dontsent</param>
        /// <param name="qq">发送给的qq，如果是群或讨论组则为群或组号</param>
        /// <param name="message">需要发送的消息，注意有些特殊符号需要解码</param>
        /// <returns></returns>
        protected sendBack  packSendBack(polltype t,bool send=false,string qq="",string message="")
        {
            return new sendBack(send, qq, message,t);
        }
        /// <summary>
        ///  用来处理某些字符显示问题，比如将"转化为\\\",如果输出需要转义字符则用这个处理。（目前只处理引号）
        /// </summary>
        /// <param name="ori"></param>
        /// <returns></returns>
        protected string Coding(string ori)
        {
            return ori.Replace("\"", "\\\\\\\"");
        }
      
    }
}
