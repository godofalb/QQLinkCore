using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using QQLinkCore;
namespace QQLinkCore
{
    class Program
    {
        static void Main(string[] args)
        {
            SQQLinker linker = new SQQLinker(@"D:\腾讯创新俱乐部\QQLinkCore\PlugInDir");
            Stream picstream= linker.getContect();
            using (Bitmap bit = new Bitmap(picstream))
            {
                bit.Save(@"pic.png");
            }
            Console.WriteLine("Continue:");
          
            Console.ReadLine();
            linker.Unload();
            Console.WriteLine("Continue:");
            Console.ReadLine();
            Console.WriteLine("Continue:");
            linker.loadDomain(@"D:\腾讯创新俱乐部\QQLinkCore\PlugInDir");
            Console.ReadLine();
        }
    }
}
