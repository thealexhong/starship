using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nmsVoiceAnalysisLibrary;

namespace Database_test
{
    class Program
    {
        static void Main()
        {
            nmsFunctions DbTest = new ManagedClass();
            if (DbTest.Initialize())
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Database Analysis is running...");
                DbTest.StartA();
        }
    }

}