using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilCloudClientConsole.Classes
{
    public class FileToSend
    {
        public string fileName { get; set; } = string.Empty;
        public string fileOriginPath { get; set; } = string.Empty;
        public string fileSavePath { get; set; } = string.Empty;

        public FileToSend() { }
    }
}
