#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class ClassifySegmentArgs : EventArgs
    {
        public int SegmentId { get; private set; }
        public string EmotionName { get; private set; }

        public ClassifySegmentArgs(int segmentId, string emotionName)
        {
            SegmentId = segmentId;
            EmotionName = emotionName;
        }
    }
}