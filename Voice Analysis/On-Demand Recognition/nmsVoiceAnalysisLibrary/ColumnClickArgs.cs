#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class ColumnClickArgs : EventArgs
    {
        public ColumnClickArgs(int column)
        {
            Column = column;
        }

        public int Column { get; private set; }
    }
}