#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class ClassifyCallArgs : EventArgs
    {
        public string CallSignature { get; private set; }
        public string EmotionName { get; private set; }

        public ClassifyCallArgs(string callSignature, string emotionName)
        {
            CallSignature = callSignature;
            EmotionName = emotionName;
        }
    }
}