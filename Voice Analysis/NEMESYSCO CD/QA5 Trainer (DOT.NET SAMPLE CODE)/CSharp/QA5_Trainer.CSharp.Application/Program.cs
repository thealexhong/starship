#region Usings

using System;
using System.Windows.Forms;
using QA5_Trainer.CSharp.Interfaces;
using QA5_Trainer.CSharp.UI;

#if VBLogic
    using QA5_Trainer.VB.Logic;
#else 
    #if CSharpLogic
        using QA5_Trainer.CSharp.Logic;
    #endif
#endif

#endregion

namespace QA5_Trainer.CSharp.Application
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Form ui = new DemoForm();

#if VBLogic
            var qa5Trainer = new QA5Trainer(ui as IQA5UI);
#else
#if CSharpLogic
            var qa5Trainer = new QA5Trainer(ui as IQA5UI);
    #endif
#endif
            qa5Trainer.Initialize();

            System.Windows.Forms.Application.Run(ui);
        }
    }
}