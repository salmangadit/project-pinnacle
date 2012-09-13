using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using AForgeForJava;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            PreProcessor P = new PreProcessor();
            string filename = @"Testing\card2.tif";
            string inputpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + filename;
            string outputpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + @"Testing\newfile2.tif";
            P.PreProcess(inputpath,40,180,outputpath);
        }
    }
}
