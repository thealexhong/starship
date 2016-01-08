#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class FileSelectedArgs : EventArgs
    {
        public bool BatchProcess { get; set; }
        private readonly string fileName;

        public FileSelectedArgs(string fileName, bool batchProcess)
        {
            BatchProcess = batchProcess;
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }
    }
}