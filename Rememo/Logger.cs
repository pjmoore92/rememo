using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Rememo
{
    public class Logger
    {
        
            private String tag;

            public Logger(String tag)
            {
                this.tag = tag;
            }

            public void Write(String message)
            {
                Write(this.tag, message);
            }

            public static void Write(String tag, String message)
            {
                FileManager.Append((string)Application.Current.Resources["LogFile"], String.Format("({0} {1})\t{2}\t{3}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), tag, message));

                Console.WriteLine(String.Format("({0} {1}) [{2}] {3}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), tag, message));
            }
        }
    
}
