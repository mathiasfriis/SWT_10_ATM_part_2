using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Logger
{
    public class FileOutput : IFileOutput
    {
        public void Write(string toFile)
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            string fileName = "FileOutputTest.txt";

            //Creating instance of StreamWriter
            System.IO.StreamWriter streamWriter = System.IO.File.AppendText(startupPath + fileName);

            streamWriter.WriteLine(toFile);

            //Closing streamWriter instance and file (I/O operation)
            streamWriter.Close();
        }
    }
}
