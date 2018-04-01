using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQLinkPlugIn;
using System.Configuration;
using System.Threading;
namespace PlugInDemo
{
    /*
     * 注意到这个示例的命名空间名，类名，文件名都不一样。这样可以有助于展示各种名字间的对应关系。才不是懒得改名字呢。
     * 
     * IsPlugIn特性描述插件是否可用，是否接受好友消息（NeedF),组消息(NeedD)和群消息(NeedG)。请根据实际需要进行修改。
     * 
     * 将生成的dll文件和对应的xml文件拷贝到插件目录下即可工作。
     */ 
    [Serializable]
    [IsPlugIn(Use=true,NeedD=true,NeedF=true,NeedG=true)]
    public class DemoPlugIn:QQPlugInBase
    {

        
        ///

            /*
             * 这个方法为接受信息时调用的方法。为了避免堵塞将在新线程中处理每条消息，如果需要在接受信息时修改数据请自行保证线程安全性，可以认为每次调用该方法都是一个新线程。
             * 
             * 
             */ 
      
            public override sendBack ReceiveMessage(ReceiveMsg rmg)
            {
                if (rmg.called)
                {
                    QQLinker.SendMessage(packSendBack(rmg.type, true, rmg.from_qq, Coding("被@")));
                }
                    Console.WriteLine("|{0}|", rmg.message);
                    Console.WriteLine(rmg.type);
                    Console.WriteLine("Called? :{0}", rmg.called);
                    string message = qqLinker.getSingleLongNick(rmg.senderUin) + "|";

                    switch (rmg.type)
                    {
                        case polltype.discu_message:
                            message += "\n" + qqLinker.getDisInfo(rmg.from_qq);
                            break;
                        case polltype.group_message:
                            message += "\n" + qqLinker.getGroupInfo(rmg.from_qq);
                            break;
                        case polltype.message:
                            message += "\n" + qqLinker.getFrindInfo(rmg.from_qq);
                            break;
                        default:
                            break;
                    }
                    message = message.Replace("\"", "\\\\\\\"");
                    Console.WriteLine(message);

                    //  sendBack s = packSendBack(rmg.type,true, rmg.from_qq, message);
                    QQLinker.SendMessage(packSendBack(rmg.type, true, rmg.from_qq, rmg.from_qq+"|"+rmg.senderUin));
                //    return DontSend;
                    return packSendBack(rmg.type, true, rmg.from_qq, Coding("测试\"成功")); //s;
                
               
            }
            
            public DemoPlugIn()

            {
                
            }
        
    }
}
