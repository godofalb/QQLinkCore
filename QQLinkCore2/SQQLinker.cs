using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using QQLinkPlugIn;
using System.Reflection;
using System.Diagnostics;
namespace QQLinkCore
   
{
    
   [Serializable]
    public class SQQLinker:MarshalByRefObject,IQQlinker
   
    {
        const int MaxTCount = 100;
        //HashSet<string> rejectUin = new HashSet<string>();
        delegate string smallrequest(params string[] s);
        delegate QQPlugInBase.sendBack Work(QQPlugInBase.ReceiveMsg rmg,int index);
        CookieContainer cc;
        string ptwebqq = "";
        string psessionid = "";
        string uin = "";
        string vfwebqq = "";
        static string UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
      //  List<QQPlugInBase> plugs = new List<QQPlugInBase>();
        AppDomain proxyDomain;
        Work w;
        LoadProxy loadP;
        int currendTCount = 0;
        public SQQLinker(string dir)
        {
            cc = new CookieContainer();
            proxyDomain = null;
            loadDomain(dir);
          //  Trace.Assert(false, "啊实打实的");
           // QQPlugInBase.QQLinker = this;
           // plugs.Add(new DemoPlugIn());
           // Console.WriteLine(getContect());
        }
        public void PerTest()
        {
            FileStream sd = new FileStream(@"C:\Users\xwl99\ans.txt", FileMode.Open, FileAccess.Write);
            StreamWriter ss = new StreamWriter(sd);

            ss.WriteLine("--++++++++");
            ss.Close();
        }
        public void PerTest(string path)
        {
            FileStream sd = new FileStream(path, FileMode.Open, FileAccess.Write);
            StreamWriter ss = new StreamWriter(sd);

            ss.WriteLine("--++++++++");
            ss.Close();
        }
        public void Unload()
        {
            if (proxyDomain != null)
            {
                AppDomain.Unload(proxyDomain);
                w = null;
                proxyDomain = null;
               
            }
        }
        public void loadDomain(string dir)
        {
            
            proxyDomain = AppDomain.CreateDomain("proxy");
            Type proxytype=typeof(LoadProxy);
            Assembly assembly=Assembly.GetAssembly(proxytype);
            loadP = proxyDomain.CreateInstance(assembly.FullName,proxytype.FullName).Unwrap() as LoadProxy;
            loadP.load(dir, this);
            w = loadP.DoWork;
        }
        public Stream getContect()
        {
            try
            {
                Console.WriteLine("Get contect: "+Thread.CurrentThread.ManagedThreadId);  
                HttpWebResponse response = getRequest("https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001").GetResponse() as HttpWebResponse;

                Stream picStream = getPic("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=2&l=M&s=3&d=72&v=4&t=0.6250925222411752&daid=164&pt_3rd_aid=0");//show it
                string sig = cc.GetCookies(new Uri("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=2&l=M&s=3&d=72&v=4&t=0.6250925222411752&daid=164&pt_3rd_aid=0"))["qrsig"].Value;
                string check = "https://ssl.ptlogin2.qq.com/ptqrlogin?ptqrtoken={0}&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=0-0-32750&mibao_css=m_webqq&t=undefined&g=1&js_type=0&js_ver=10197&login_sig=&pt_randsalt=0";
                string real = string.Format(check, Time33(sig));
                smallrequest waitlogin = new smallrequest(waitLogin);
                
                waitlogin.BeginInvoke(new string[]{real},new AsyncCallback(endLogin),waitlogin);


                Console.WriteLine("Start Login...");
                return picStream;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        string waitLogin(params string[] s)//this part
        {
            
            Console.WriteLine("Waiting: " + Thread.CurrentThread.ManagedThreadId);  
            HttpWebResponse response;
            CookieCollection cookies;
            string reformat = @"'(http://.*?)'";
            string nexturl = "";
            
            Regex pattern = new Regex(reformat);
            while (true)
            {

                response = getRequest(s[0]).GetResponse() as HttpWebResponse;//s[0]=realurl
                cookies = response.Cookies;
                string context = GetContent(response);
                Console.WriteLine(context);
                foreach (Cookie cookie in cookies)
                {
                    Console.WriteLine("{0}:{1}", cookie.Name, cookie.Value);
                }

                if (cookies.Count > 0)
                {

                    Match match = pattern.Match(context);
                    nexturl = match.Groups[1].Value;
                    ptwebqq = cookies["ptwebqq"].Value;
                    break;
                }

                Console.WriteLine("=============");
                Thread.Sleep(1000);
            }
            return nexturl;
        }
        HttpWebRequest send_d1(string url, string datastring)
        {
            HttpWebRequest request = getRequest(url);
            SetHeaderValue(request.Headers, "Host", "d1.web2.qq.com");
            SetHeaderValue(request.Headers, "Referer", "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
           // request.Headers.Set("Origin", "http://d1.web2.qq.com");
          
            datastring = "r=" + UrlEncode(datastring, System.Text.UTF8Encoding.UTF8);
            
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(datastring);
            postData(request, datas);
            return request;
        }
        string getMessage(SJsonSolver json,out bool called)
        {
            string message = null;
            called = false;
            if (json.Contains("poll_type"))
            {
                SJsonSolver res = json["value"] as SJsonSolver;
                message = res["style"] as string;

                Match match = System.Text.RegularExpressions.Regex.Match(message, "\"(.*)\"");
                message = match.Groups[1].Value;
                if (message[0] == '@')
                {
                    int last = message.LastIndexOf('\"');
                    if (last > 0 && last < message.Length - 1)
                    {
                        message = message.Substring(last + 1);
                        called = true;
                    }
                }
                return message;
            }
            return "";
        }
        public string getSingleLongNick(string uin)
        {
            HttpWebRequest request = getRequest(string.Format("http://s.web2.qq.com/api/get_single_long_nick2?tuin={0}&vfwebqq={1}&t=1510213814193",uin,vfwebqq));
            SetHeaderValue(request.Headers, "Host", "s.web2.qq.com");
            SetHeaderValue(request.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string content = GetContent(response);
            try
            {
                Console.WriteLine(content);
                SJsonSolver s = SJsonSolver.Creste(content);
                string message = s["lnick"] as string;
                Match match = System.Text.RegularExpressions.Regex.Match(message, "\"(.*)\"");
                message = match.Groups[1].Value;
                
                return message;
            }
            catch
            {
                return "";
            }
        }
        public string getDisInfo(string din)
        {
            HttpWebRequest request = getRequest(string.Format("http://d1.web2.qq.com/channel/get_discu_info?did={0}&vfwebqq={1}&clientid=53999199&psessionid={2}", din, vfwebqq, psessionid));
            SetHeaderValue(request.Headers, "Host", "d1.web2.qq.com");
            SetHeaderValue(request.Headers, "Referer", "http://d1.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string content = GetContent(response);
            return content;
        }
        public string getGroupInfo(string gin)
        {
            HttpWebRequest request = getRequest(string.Format("http://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}", gin, vfwebqq));
            SetHeaderValue(request.Headers, "Host", "s.web2.qq.com");
            SetHeaderValue(request.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string content = GetContent(response);
            return content;
        }
        public string getFrindInfo(string uin)
        {
            HttpWebRequest request = getRequest(string.Format("http://s.web2.qq.com/api/get_friend_info2?tuin={0}&vfwebqq={1}&clientid=53999199&psessionid={2}", uin, vfwebqq,psessionid));
            SetHeaderValue(request.Headers, "Host", "s.web2.qq.com");
            SetHeaderValue(request.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string content = GetContent(response);
            return content;
            
        }
        string poll(params string[] ps)
        {
           // Console.WriteLine("Polling: " + Thread.CurrentThread.ManagedThreadId);
            string polldatastring = "{\"ptwebqq\":\"" + ptwebqq + "\",\"clientid\":53999199,\"psessionid\":\"" + psessionid + "\",\"key\":\"\"}";
            /*
            Console.WriteLine("{0} : {1}","uin",uin);
            Console.WriteLine("{0} : {1}","vf",vfwebqq);
            Console.WriteLine("{0} : {1}", "p", psessionid);
            Console.WriteLine("{0} : {1}", "ptw", ptwebqq);
            */
            while (true)
            {

                try
                {
                    Console.WriteLine("Asking new");
                    HttpWebRequest pollrequest = send_d1("http://d1.web2.qq.com/channel/poll2", polldatastring);

                    HttpWebResponse pollresponse = null;
                    pollrequest.Timeout = 1000000;


                    pollresponse = pollrequest.GetResponse() as HttpWebResponse;
                    string message = GetContent(pollresponse);
                    string qq = "515102224";
                   
                    SJsonSolver json = SJsonSolver.Creste(message);
                    if ((string)json["retcode"] == "0" && !json.Contains("errmsg")&&proxyDomain!=null)
                    {
                        bool called;
                        string messae = getMessage(json, out called);
                       
                        string fromUin = json["from_uin"] as string;

                        string toUin = json["to_uin"] as string;
                        string poll_type = json["poll_type"] as string;

                        QQPlugInBase.ReceiveMsg rmg = new QQPlugInBase.ReceiveMsg(called, toUin, fromUin, messae, poll_type);
                        rmg.font = json["name"] as string;
                        rmg.size = json["size"] as string;
                        rmg.color = json["color"] as string;
                        string senduin;
                        if (poll_type != "\"message\"")
                        {
                            senduin = json["send_uin"] as string;

                        }
                        else
                        {
                            senduin = fromUin;
                        }
                        if (senduin == uin)
                        {
                            continue;
                        }
                        rmg.senderUin = senduin;
#warning 获得昵称   //getSingleLongNick(senduin);
                        
                        for(int i=0;i<loadP.Size;i++)
                        {

                           
                            w.BeginInvoke(rmg, i, new AsyncCallback(sendBack), w);
                            /*
                            QQPlugInBase.sendBack ddd = loadP.DoWork(rmg, i);
                            if (ddd.send)
                            {

                                SendMessage(ddd);
                            }
                           */
                          
                        }
                       
                    }
                }
                catch(Exception ex)
                {
                    Trace.Assert(false,ex.Message);
                    
                }
            }
            return null;
        }
        void sendBack(IAsyncResult res)
        {
           
            Work work = res.AsyncState as Work;
            QQPlugInBase.sendBack sendback=work.EndInvoke(res);
            if (sendback.send)
            {

                SendMessage(sendback);
            }
        }
        public static string packMessage(QQPlugInBase.sendBack send, string psessionid)
        {
            switch (send.type)
            {
                case polltype.message: return packPersonMessage(send, psessionid); break;
                case polltype.discu_message: return packDisMessage(send, psessionid); break;
                case polltype.group_message: return packGroupMessage(send, psessionid); break;
                default: break;
            }
            return null;

        }

        public static string packDisMessage(QQPlugInBase.sendBack send, string psessionid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\"did\":");
            stringBuilder.Append(send.qq);
            stringBuilder.Append(",\"content\":\"[\\\"");
            stringBuilder.Append(send.message);
            stringBuilder.Append("\\\",[\\\"font\\\",{\\\"name\\\":\\\"");
            stringBuilder.Append(send.font);
            stringBuilder.Append("\\\",\\\"size\\\":");
            stringBuilder.Append(send.size);
            stringBuilder.Append(",\\\"style\\\":[0,0,0],\\\"color\\\":\\\"");
            stringBuilder.Append(send.color);
            stringBuilder.Append("\\\"}]]\",\"face\":594,\"clientid\":53999199,\"msg_id\":3600001,\"psessionid\":\"");
            stringBuilder.Append(psessionid);
            stringBuilder.Append("\"}");
            return stringBuilder.ToString();
        }
        public static string packGroupMessage(QQPlugInBase.sendBack send, string psessionid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\"group_uin\":");
            stringBuilder.Append(send.qq);
            stringBuilder.Append(",\"content\":\"[\\\"");
            stringBuilder.Append(send.message);
            stringBuilder.Append("\\\",[\\\"font\\\",{\\\"name\\\":\\\"");
            stringBuilder.Append(send.font);
            stringBuilder.Append("\\\",\\\"size\\\":");
            stringBuilder.Append(send.size);
            stringBuilder.Append(",\\\"style\\\":[0,0,0],\\\"color\\\":\\\"");
            stringBuilder.Append(send.color);
            stringBuilder.Append("\\\"}]]\",\"face\":594,\"clientid\":53999199,\"msg_id\":3600001,\"psessionid\":\"");
            stringBuilder.Append(psessionid);
            stringBuilder.Append("\"}");
            return stringBuilder.ToString();
        }

        public static string packPersonMessage(QQPlugInBase.sendBack send, string psessionid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\"to\":");
            stringBuilder.Append(send.qq);
            stringBuilder.Append(",\"content\":\"[\\\"");
            stringBuilder.Append(send.message);
            stringBuilder.Append("\\\",[\\\"font\\\",{\\\"name\\\":\\\"");
            stringBuilder.Append(send.font);
            stringBuilder.Append("\\\",\\\"size\\\":");
            stringBuilder.Append(send.size);
            stringBuilder.Append(",\\\"style\\\":[0,0,0],\\\"color\\\":\\\"");
            stringBuilder.Append(send.color);
            stringBuilder.Append("\\\"}]]\",\"face\":594,\"clientid\":53999199,\"msg_id\":3600001,\"psessionid\":\"");
            stringBuilder.Append(psessionid);
            stringBuilder.Append("\"}");
            return stringBuilder.ToString();
        }
        public void SendMessage(QQPlugInBase.sendBack sendback)
        {
            
                string datastring = packMessage(sendback, psessionid);
                Console.WriteLine(datastring);
                //string datastring = "r=" + UrlEncode(sendback.message, System.Text.UTF8Encoding.UTF8);

                switch (sendback.type)
                {
                    case polltype.message:
                        {
                            HttpWebRequest sendbackRequest = send_d1("http://d1.web2.qq.com/channel/send_buddy_msg2", datastring);

                            HttpWebResponse response = sendbackRequest.GetResponse() as HttpWebResponse;
                            Console.WriteLine(GetContent(response)); break;
                        }
                    case polltype.discu_message:
                        {
                            HttpWebRequest sendbackRequest = send_d1("http://d1.web2.qq.com/channel/send_discu_msg2", datastring);

                            HttpWebResponse response = sendbackRequest.GetResponse() as HttpWebResponse;
                            Console.WriteLine(GetContent(response));
                            break;
                        }
                    case polltype.group_message:
                        {
                            HttpWebRequest sendbackRequest = send_d1("http://d1.web2.qq.com/channel/send_qun_msg2", datastring);

                            HttpWebResponse response = sendbackRequest.GetResponse() as HttpWebResponse;
                            Console.WriteLine(GetContent(response));
                            break;
                        }
                    default: break;
                }
                Console.WriteLine("{0}____________________________",sendback.type);
                // Console.WriteLine(sendback.message);
            
        }
        void endLogin(IAsyncResult res)
        {
            Console.WriteLine("endLogin : "+Thread.CurrentThread.ManagedThreadId);  
            if (res == null)
            {
                throw new ArgumentException("res");
            }
            smallrequest d1 = res.AsyncState as smallrequest;
            string nexturl=d1.EndInvoke(res);
            HttpWebResponse response = getRequest(nexturl).GetResponse() as HttpWebResponse;
            HttpWebRequest vfrequest = getRequest(string.Format("http://s.web2.qq.com/api/getvfwebqq?ptwebqq={0}&clientid=53999199&psessionid=&t=1488053293431", ptwebqq)) as HttpWebRequest;
            vfrequest.Referer = "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1";
            HttpWebResponse vfresponse = (HttpWebResponse)vfrequest.GetResponse();
            string jsoncontext = GetContent(vfresponse);
            Console.WriteLine(jsoncontext);
            SJsonSolver json1 =SJsonSolver.Creste(jsoncontext);
            vfwebqq = json1.Find("vfwebqq") as string;
            vfwebqq = vfwebqq.Substring(1, vfwebqq.Length - 3);
#warning change this part to send_2
            string datastring = "{\"ptwebqq\":\"" + ptwebqq + "\",\"clientid\":53999199,\"psessionid\":\"\",\"status\":\"online\"}";
            /*
            HttpWebRequest loginrequest = getRequest("http://d1.web2.qq.com/channel/login2");
            loginrequest.Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2";
            loginrequest.Headers.Set("Origin", "http://d1.web2.qq.com");
            SetHeaderValue(loginrequest.Headers, "Host", "d1.web2.qq.com");
             // newrequest.Headers.Set("Host", "d1.web2.qq.com");
              
            datastring = "r=" + UrlEncode(datastring, System.Text.UTF8Encoding.UTF8);
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(datastring);
            postData(loginrequest, datas);
              */

            HttpWebRequest loginrequest = send_d1("http://d1.web2.qq.com/channel/login2",datastring);
            
#warning build the json
         
      
            HttpWebResponse loginresponse = loginrequest.GetResponse() as HttpWebResponse;
            string logincontext = GetContent(loginresponse);
            SJsonSolver json2 = SJsonSolver.Creste(logincontext);
            psessionid = json2.Find("psessionid") as string;
            psessionid = psessionid.Substring(1, psessionid.Length - 2);
            uin = json2.Find("uin") as string;
            
            getFriend();
            getG();
            getDisG();
            getR();
            getOnLine();
            //smallrequest polling = new smallrequest(poll);
            //polling.BeginInvoke(null, null, null);
            
            //ssssssss
            /*
             *  SJsonSolver jsonsolver = new SJsonSolver(context2);
            string vfwebqq = jsonsolver.Find("vfwebqq");
             */
            Console.WriteLine("EndLogin");
            poll();
        }
        void getG()
        {
            Console.WriteLine("Getting Gropus");
            Console.WriteLine(uin);
            Thread.Sleep(1000);
            HttpWebRequest newrequest = (HttpWebRequest)HttpWebRequest.Create("http://s.web2.qq.com/api/get_group_name_list_mask2");
            newrequest.CookieContainer = cc;

            newrequest.Method = "POST";
            newrequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.122 Safari/537.36 SE 2.X MetaSr 1.0";
            newrequest.KeepAlive = true;
            SetHeaderValue(newrequest.Headers, "Host", "s.web2.qq.com");

            SetHeaderValue(newrequest.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            string s = hash(uint.Parse(uin), "");
            Console.WriteLine(uin);
            Console.WriteLine(ptwebqq);
            string getdatastring = "{\"vfwebqq\":\"" + vfwebqq + "\",\"hash\":\"" + s + "\"}";
            Console.WriteLine(getdatastring);
            getdatastring = "r=" + UrlEncode(getdatastring, System.Text.UTF8Encoding.UTF8);
            Console.WriteLine(getdatastring);
            Byte[] getdatas = System.Text.Encoding.UTF8.GetBytes(getdatastring);
            Stream gets = newrequest.GetRequestStream();
            gets.Write(getdatas, 0, getdatas.Length);
            gets.Close();

            HttpWebResponse response = newrequest.GetResponse() as HttpWebResponse;
            SJsonSolver sjser = SJsonSolver.Creste(GetContent(response));
            Console.WriteLine("_______________________________");
            Console.WriteLine(sjser);
            Console.WriteLine("_______________________________");

        }
        void getFriend()
        {
            Console.WriteLine("Getting Friends");
#warning verify this part
            Thread.Sleep(1000);
            HttpWebRequest newrequest = (HttpWebRequest)HttpWebRequest.Create("http://s.web2.qq.com/api/get_user_friends2");
            newrequest.CookieContainer = cc;

            newrequest.Method = "POST";
            newrequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.122 Safari/537.36 SE 2.X MetaSr 1.0";
            newrequest.KeepAlive = true;
            SetHeaderValue(newrequest.Headers, "Host", "s.web2.qq.com");

            SetHeaderValue(newrequest.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
#warning 就是hash有错
            string s = hash(uint.Parse(uin), ptwebqq);
            Console.WriteLine(uin);
            Console.WriteLine(ptwebqq);

            string getdatastring = "{\"vfwebqq\":\"" + vfwebqq + "\",\"hash\":\"" + s + "\"}";
            Console.WriteLine(getdatastring);
            getdatastring = "r=" + UrlEncode(getdatastring, System.Text.UTF8Encoding.UTF8);
            Console.WriteLine(getdatastring);

            //   string getdatastring = string.Format("r=%7b%22h%22%3a%22hello%22%2c%22hash%22%3a%22{0}%22%2c%22vfwebqq%22%3a%22{1}%22%7d",s, vfwebqq);
            byte[] getdatas = System.Text.Encoding.UTF8.GetBytes(getdatastring);
            Stream gets = newrequest.GetRequestStream();
            gets.Write(getdatas, 0, getdatas.Length);
            gets.Close();

            HttpWebResponse response = newrequest.GetResponse() as HttpWebResponse;
            SJsonSolver sjser = SJsonSolver.Creste(GetContent(response));
            Console.WriteLine("_______________________________");
            Console.WriteLine(sjser);
            Console.WriteLine("_______________________________");
        }
        void getOnLine()
        {
            Console.WriteLine("ON Lines");
            string url = string.Format("http://d1.web2.qq.com/channel/get_online_buddies2?vfwebqq={0}&clientid=53999199&psessionid={1}&t=1488268527333", vfwebqq, psessionid);
            HttpWebRequest newrequest = (HttpWebRequest)WebRequest.Create(url);
            newrequest.CookieContainer = cc;
            newrequest.KeepAlive = true;
            SetHeaderValue(newrequest.Headers, "Host", "d1.web2.qq.com");
            newrequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            newrequest.Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2";
            HttpWebResponse response = newrequest.GetResponse() as HttpWebResponse;
            SJsonSolver sjser = SJsonSolver.Creste(GetContent(response));
            Console.WriteLine("_______________________________");
            Console.WriteLine(sjser);
            Console.WriteLine("_______________________________");

        }
        void getR()
        {
            Console.WriteLine("Get recent");
            Thread.Sleep(1000);
            HttpWebRequest newrequest = (HttpWebRequest)HttpWebRequest.Create("http://d1.web2.qq.com/channel/get_recent_list2");
            newrequest.CookieContainer = cc;

            newrequest.Method = "POST";
            newrequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.122 Safari/537.36 SE 2.X MetaSr 1.0";
            newrequest.KeepAlive = true;
            SetHeaderValue(newrequest.Headers, "Origin", "http://d1.web2.qq.com");
            SetHeaderValue(newrequest.Headers, "Host", "d1.web2.qq.com");
            SetHeaderValue(newrequest.Headers, "Referer", "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
            Console.WriteLine(psessionid);
            foreach (Cookie cookie in cc.GetCookies(new Uri("http://d1.web2.qq.com")))
            {
                Console.WriteLine(cookie.Name + ":" + cookie.Value);
            }
            string getdatastring = "{\"vfwebqq\":\"" + vfwebqq + "\",\"clientid\":53999199,\"psessionid\":\"" +"\"}";
            Console.WriteLine(getdatastring);
            getdatastring = "r=" + UrlEncode(getdatastring, System.Text.UTF8Encoding.UTF8);
            Console.WriteLine(getdatastring);
            byte[] getdatas = System.Text.Encoding.UTF8.GetBytes(getdatastring);
            Stream gets = newrequest.GetRequestStream();
            int s = 0;
            while (s + 8 < getdatas.Length)
            {
                gets.Write(getdatas, s, 8);
                s += 8;
            }
            gets.Write(getdatas, s, getdatas.Length - s);
            gets.Close();

            HttpWebResponse response = newrequest.GetResponse() as HttpWebResponse;
            SJsonSolver sjser = SJsonSolver.Creste(GetContent(response));
            Console.WriteLine("_______________________________");
            Console.WriteLine(sjser);
            Console.WriteLine("_______________________________");
        }
        void getDisG()
        {
            Thread.Sleep(1000);
            Console.WriteLine("GetD");
            
            Console.WriteLine(string.Format("http://s.web2.qq.com/api/get_discus_list?clientid=53999199&psessionid={0}&vfwebqq={1}&t=1508055730349", psessionid, vfwebqq));
            HttpWebRequest newrequest = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://s.web2.qq.com/api/get_discus_list?clientid=53999199&psessionid={0}&vfwebqq={1}&t=1508055730349", psessionid, vfwebqq));
            newrequest.CookieContainer = cc;

            newrequest.Method = "GET";
            newrequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.122 Safari/537.36 SE 2.X MetaSr 1.0";
            newrequest.KeepAlive = true;
            SetHeaderValue(newrequest.Headers, "Host", "s.web2.qq.com");
            SetHeaderValue(newrequest.Headers, "Origin", "http://s.web2.qq.com");
            SetHeaderValue(newrequest.Headers, "Referer", "http://s.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");




            HttpWebResponse response = newrequest.GetResponse() as HttpWebResponse;
            SJsonSolver sjser = SJsonSolver.Creste(GetContent(response));
            Console.WriteLine("_______________________________");
            Console.WriteLine(sjser);
            Console.WriteLine("_______________________________");
        }
        Stream getPic(string url)
        { 
            HttpWebResponse response=getRequest(url).GetResponse() as HttpWebResponse;
            return response.GetResponseStream();
        }
        public HttpWebRequest getRequest(string url)
        {
            HttpWebRequest w = HttpWebRequest.Create(url) as HttpWebRequest;
            w.CookieContainer = cc;
            w.UserAgent = SQQLinker.UserAgent;
            w.KeepAlive = true;
            return w;

        }
        static string hash(uint uin, string ptvfwebqq)
        {

            uint[] ptb = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                ptb[i] = 0;
            }
            for (int i = 0; i < ptvfwebqq.Length; i++)
            {
                int ptbindex = i % 4;

                ptb[ptbindex] ^= (uint)ptvfwebqq[i];

            }
            string[] salt = new string[2] { "EC", "OK" };
            uint[] uinByte = new uint[4];
            uinByte[0] = (((uin >> 24) & 0xFF) ^ salt[0][0]);
            uinByte[1] = (((uin >> 16) & 0xFF) ^ salt[0][1]);
            uinByte[2] = (((uin >> 8) & 0xFF) ^ salt[1][0]);
            uinByte[3] = ((uin & 0xFF) ^ salt[1][1]);
            uint[] result = new uint[8];
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    result[i] = ptb[i >> 1];
                else
                    result[i] = uinByte[i >> 1];
            }


            return byte2hex(result);
        }
        static string byte2hex(uint[] bytes)
        {//bytes array
            string hex = "0123456789ABCDEF";
            string buf = "";

            for (int i = 0; i < bytes.Length; i++)
            {

                buf += (hex[(int)((bytes[i] >> 4) & 0xF)]);
                buf += (hex[(int)(bytes[i] & 0xF)]);
            }
            return buf;
        }
        static void postData(HttpWebRequest request,byte[] data)
        {
            request.Method = "POST";
            Stream sw=request.GetRequestStream();
            /*
            int s = 0;
            while (s + 8 < data.Length)
            {
                sw.Write(data, s, 8);
                s += 8;
            }
           
            sw.Write(data,s,data.Length-s);*/
            sw.Write(data,0,data.Length);
            sw.Close();
        }
        static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        static string UrlEncode(string temp, Encoding encoding)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                string t = temp[i].ToString();
                string k;
                switch (t)
                {
                    case "'":
                        t = "%27";
                        builder.Append(t);
                        break;

                    case " ":
                        t = "%20";
                        builder.Append(t);
                        break;

                    case "(":
                        t = "%28";
                        builder.Append(t);
                        break;

                    case ")":
                        t = "%29";
                        builder.Append(t);
                        break;

                    case "!":
                        t = "%21";
                        builder.Append(t);
                        break;

                    case "*":
                        t = "%2A";
                        builder.Append(t);
                        break;

                    default:
                        k = HttpUtility.UrlEncode(t, encoding);
                        if (t == k)
                        {
                            builder.Append(t);
                        }
                        else
                        {
                            builder.Append(k.ToUpper());
                        }
                        break;
                }
            }
            return builder.ToString();
        }
        static string GetContent(HttpWebResponse res)
        {
            StreamReader reader = new StreamReader(res.GetResponseStream());
            string s= reader.ReadToEnd();
            reader.Close();
            return s;
        }
        static int Time33(string str)
        {
            int hash = 0;
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                hash = hash*33 +(int)str[i];
          //      Console.WriteLine("{0}  {1}  {2}", str[i], (int)str[i],hash);
            }
            return hash & 2147483647;
        }
        
    }
}
