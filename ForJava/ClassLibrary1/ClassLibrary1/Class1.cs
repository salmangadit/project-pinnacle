using System;
using System.Dynamic;
using System.Windows.Forms;
using System.Drawing;

public class Class11
{
    public Class11() { }

    public void displayMessage()
    {
        Class2 c = new Class2();
        string message1 = c.message;
        Bitmap image = new Bitmap(@"C:\Users\VARUN\Desktop\Capture.png");
        MessageBox.Show(message1, "Sample");
    } 
}

public class Class2
{
    public string message = "Hello from C# Varun! Woahh with separate class and Bitmap!!";
}

