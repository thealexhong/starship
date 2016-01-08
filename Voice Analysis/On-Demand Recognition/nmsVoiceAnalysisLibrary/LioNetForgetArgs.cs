#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class LioNetForgetArgs : EventArgs
    {
        public int SegmentId { get; private set; }

        public LioNetForgetArgs(int segmentId)
        {
            SegmentId = segmentId;
        }
    }
}