using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VLCRadioLoggerSeeSharp
{
    class Program
    {
        public static String madeByRemco = "This list is generated with the VLC media player radio logger. Made by Peam_ 2017.\n";
        public static String filename = "D:\\Programming\\VLC media player radio song logger\\logs.txt";
        private static bool skipQuestion = false;
        static void Main(string[] args)
        {            
            if(args[0] != null)
            {
                filename = args[0];
            }
            if(args[1]!=null && args[1].Equals("true"))
            {
                skipQuestion = true;
            }
            setFileLocation();
            startProgram();
        }

        public static void setFileLocation()
        {
            bool done = false;
            while (!done && !skipQuestion)
            {           
                Console.WriteLine("The current path for the output is: " + filename + ".\n Would you like to change this? (y/N)");
                if (Console.ReadLine().Equals("y"))
                {
                    Console.WriteLine("Enter the new path for the output:");
                    filename = Console.ReadLine();
                }
                else
                {
                    done = true;
                }
            }
            if(!File.Exists(filename))
            {
                TextWriter tw = new StreamWriter(filename, true);
                //tw.WriteLine(madeByRemco);           
                tw.Close();
            }
        }

        public static void startProgram()
        {
            Console.WriteLine("Saving your logs into " + filename);
            String currentTitle = "";
            while(true)
            {
                IEnumerable windows = FindWindowsWithText("VLC media player");
                foreach(IntPtr window in windows)
                {
                    String windowname = GetWindowText(window);
                    if(windowname.EndsWith(" - VLC media player"))
                    {
                        if (!windowname.Equals(currentTitle))
                        {
                            currentTitle = windowname;
                            windowname = windowname.Substring(0, windowname.Length - 19);

                            Console.WriteLine(windowname);                            
                            saveToFile(windowname);
                        }
                    }
                                      
                }
                Thread.Sleep(2000);
            }
        }
        
        public static void saveToFile(String name)
        {
            //String title = name.Split('-')[1];
            //String artist = name.Split('-')[0];

            if (!File.Exists(filename))
            {
                TextWriter tw = new StreamWriter(filename, true);
                //tw.WriteLine(madeByRemco);
                tw.Close();
            }

            using (StreamWriter w = File.AppendText(filename))
            {
                w.WriteLine(name);
            }
            
        }


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                return GetWindowText(wnd).Contains(titleText);
            });
        }




    }
}
