using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQLinkPlugIn
{
    public interface IQQlinker
    {
       string getSingleLongNick(string uin);
       string getGroupInfo(string gin);
       string getDisInfo(string din);
       string getFrindInfo(string uin);
       void SendMessage(QQPlugInBase.sendBack sendback);
    }
}
