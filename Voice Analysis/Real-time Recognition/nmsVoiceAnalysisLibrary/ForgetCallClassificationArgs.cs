#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class ForgetCallClassificationArgs : EventArgs
    {
        public string CallSignature { get; private set; }

        public ForgetCallClassificationArgs(string callSignature)
        {
            CallSignature = callSignature;
        }
    }
}