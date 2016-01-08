namespace QA5_Trainer.CSharp.Interfaces
{
    public class EnvelopData
    {
        public double LowBorder { get; private set; }
        public double HighBorder { get; private set; }
        public double LowEnvelope { get; private set; }
        public double HighEnvelope { get; private set; }
        public double ParmAvrg { get; private set; }

        public EnvelopData(double lowBorder, double highBorder, double lowEnvelope, double highEnvelope, double parmAvrg)
        {
            LowBorder = lowBorder;
            HighBorder = highBorder;
            LowEnvelope = lowEnvelope;
            HighEnvelope = highEnvelope;
            ParmAvrg = parmAvrg;
        }
    }
}