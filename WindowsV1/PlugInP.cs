using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace WindowsV1
{
    public class PlugInP
    {
        public string Class
        {
            get;
            set;
        }
        public string Name
        { get; set; }
        public bool Used
        { get; set; }
       
        public bool FChangeable
        {
            get;
            set;
        }
        public bool GChangeable
        {
            get;
            set;
        }
        public bool DChangeable
        {
            get;
            set;
        }
        public bool NeedF
        { get; set; }
        public bool NeedG
        { get; set; }
        public bool NeedD
        { get; set; }
        public string Groups
        { get; set; }
        public string Friends
        { get; set; }
        public string Dis
        { get; set; }
        public string GroupMode{ get; set; }
        public string FriendMode{ get; set; }
        public string DisMode { get; set; }
    }
}
