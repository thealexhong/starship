#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class SegmentClickedArgs : EventArgs
    {
        public SegmentClickedArgs(int clickedSegmentId)
        {
            ClickedSegmentId = clickedSegmentId;
        }

        public int ClickedSegmentId { get; private set; }
    }
}