#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class ApplyCountersResetCodeArgs : EventArgs
    {
        public string ResetCode { get; private set; }

        public ApplyCountersResetCodeArgs(string resetCode)
        {
            ResetCode = resetCode;
        }
    }
}