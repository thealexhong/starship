#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public class NewCallEmotionalArgs : EventArgs
    {
        public string Emotion { get; private set; }

        public NewCallEmotionalArgs(string emotion)
        {
            Emotion = emotion;
        }
    }
}