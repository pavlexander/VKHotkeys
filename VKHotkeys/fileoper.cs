using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Text.RegularExpressions;

using System.Web;

namespace VKHotkeys
{
    class fileoper
    {
        public static void createFile(string fileLoc, string[] text)
        {

            if (!File.Exists(fileLoc))
            {

                using (FileStream stream = new FileStream(fileLoc, FileMode.Create))
                using (TextWriter writer = new StreamWriter(stream))
                {
                }
                System.IO.File.WriteAllLines(fileLoc, text, Encoding.UTF8);
            }
            else
            {
                ///string[] spaces = new string[] { "", "", ""};
                ///System.IO.File.AppendAllLines(fileLoc, spaces, Encoding.UTF8);
                System.IO.File.AppendAllLines(fileLoc, text, Encoding.UTF8);
            }
        }

		public static string[] loadFile(string fileLoc)
        {
           return File.ReadAllLines(fileLoc, Encoding.UTF8); ;
        }

        public static void writeLog(string fileLoc, string text)
        {
            
            if (!Directory.Exists(@"log\")) { Directory.CreateDirectory(@"log\"); }

            if (!File.Exists(fileLoc))
            {

                using (FileStream stream = new FileStream(fileLoc, FileMode.Create))
                using (TextWriter writer = new StreamWriter(stream))
                {
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileLoc, true))
            {
                DateTime t1 = DateTime.Now;
                file.WriteLine("Date: " + t1 + ". Message: " + text);

                //file.Close(); // nado li?
            }

            /*
            using (StreamWriter sw = new StreamWriter(new FileStream(location + @"log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                //Writes the method name with the exception and writes the exception underneath
                sw.WriteLine(String.Format("{0} ({1}) - Method: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), method.ToString()));
                sw.WriteLine(exception.ToString()); sw.WriteLine("");
            }
            */
        }

    }
}
