using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tesseract;
using System.Drawing;

namespace OCRTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum eOcrEngineMode : int
        {
            TESSERACT_ONLY = 0,
            CUBE_ONLY = 1,
            TESSERACT_CUBE_COMBINED = 2,
            DEFAULT = 3
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            const string language = "eng";
            const string TessractData = @"C:\Users\Salman\Documents\GitHub\project-pinnacle\Research\Salman\OCRTest\tessdata\";
            const string Path = @"C:\Users\Salman\NUS\EE4001\Research\trial.tif";

            TesseractProcessor processor = new TesseractProcessor();
            processor.DoMonitor = true;

            System.Drawing.Image bmp = System.Drawing.Image.FromFile(Path);

            bool x = processor.Init(TessractData, language, (int)eOcrEngineMode.TESSERACT_CUBE_COMBINED);

            processor.Clear();
            processor.ClearAdaptiveClassifier();
            string result = processor.Apply(bmp);
            List<Word> detectedWords = processor.RetriveResultDetail();
            int a = 2;
            //this.UpdateImageViewer(detectedWords);

            //using (var bmp = Bitmap.FromFile(Path) as Bitmap)
            //{
            //    var success = processor.Init(TessractData, language, (int)eOcrEngineMode.DEFAULT);
            //    if (!success)
            //    {
            //        Console.WriteLine("Failed to initialize tesseract.");
            //    }
            //    else
            //    {
            //        string text = processor.Recognize(bmp);
            //        Console.WriteLine("Text:");
            //        Console.WriteLine("*****************************");
            //        Console.WriteLine(text);
            //        Console.WriteLine("*****************************");
            //    }
            //}

            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }

            

    }
}
