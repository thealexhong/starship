namespace QA5_Trainer.CSharp.Interfaces
{
    public struct DominantEmotion
    {
        public short Value;
        public string Type;

        public DominantEmotion(short Value1, string Type1)
        {
            Value = Value1;
            Type = Type1;
        }
    };
}