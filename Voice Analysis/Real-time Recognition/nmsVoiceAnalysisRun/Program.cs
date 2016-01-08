using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nmsVoiceAnalysisLibrary;

namespace nmsVoiceAnalysisRun
{
    class Program
    {
        static void Main()
        {
            ManagedClass Bijan = new ManagedClass();
            if (Bijan.Initialize())
                Bijan.Start();
            return;
        }
    }
}
