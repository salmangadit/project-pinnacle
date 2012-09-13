using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Imaging.Filters;

namespace AForgeForJava
{
    public class PreProcessor
    {
        public PreProcessor()
        {}

        public void displaymessage()
        {
            Console.WriteLine("Hello Bro");
        }
        public void PreProcess(string input, int b, int c, string output)
        {
            Image img = Image.FromFile(input);
            Bitmap bitimage = (Bitmap)img;
            BrightnessCorrection b_filter = new BrightnessCorrection(b);
            ContrastCorrection c_filter = new ContrastCorrection(c);

            b_filter.ApplyInPlace(bitimage);
            c_filter.ApplyInPlace(bitimage);
            img.Save(output);
        }
    }
}
