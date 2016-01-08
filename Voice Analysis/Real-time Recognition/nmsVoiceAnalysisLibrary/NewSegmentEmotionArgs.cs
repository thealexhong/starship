#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class NewSegmentEmotionArgs : EventArgs
    {
        public NewSegmentEmotionArgs(string emotionName)
        {
            EmotionName = emotionName;
        }

        public string EmotionName { get; private set; }
    }
}