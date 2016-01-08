#region Usings

using System;

#endregion

namespace QA5_Trainer.CSharp.Interfaces
{
    public interface IQA5UI
    {
        event EventHandler<FileSelectedArgs> FileSelected;

        event EventHandler<SegmentClickedArgs> SegmentClicked;

        event EventHandler<ColumnClickArgs> SegmentsListColumnClicked;

        event EventHandler<NewSegmentEmotionArgs> NewSegmentEmotionAdded;

        event EventHandler<NewCallEmotionalArgs> NewCallEmotionAdded;

        event EventHandler<ClassifySegmentArgs> ClassifySegment;

        event EventHandler<LioNetForgetArgs> LioNetForget;

        event EventHandler<ClassifyCallArgs> ClassifyCall;

        event EventHandler<ForgetCallClassificationArgs> ForgetCallClassification;

        event EventHandler UiIsGoingToClose;

        event EventHandler RetrieveLicenseDetails;

        //event EventHandler<ApplyCountersResetCodeArgs> ApplyCountersResetCode;

        event EventHandler UpdateBordersFile;

        event EventHandler ShowBordersOnGraph;

        bool HighlightAngerTrendLevel { set; }
        double MaxAngerTrandLevel { get; }
        string AngerTrendLevel { set; }
        bool AlarmIfStressTrendIsRaisingAndAbove { get; }
        bool AlarmIfAngerTrendIsRaisingAndAbove { get; }
        bool HighlightStressTrendLevel { set; }
        double MaxStressTrendLevel { get; }
        bool NotifyStressLevelIsRaising { set; }
        string StressTrendLevel { set; }
        bool AlarmIfStressTrendIsLow { get; }
        bool HighlightEnergyTrendIsRaisingFor { set; }
        bool HighlightEnergyLevelBelow { set; }
        string EnergyDifference { set; }
        string EnrgyHighSegments { set; }
        string EnergyTrendIsRaisingFor { set; }
        bool HighlightEnergyLevel { set; }
        bool NotifySpeakerIsTired { set; }
        string EnergyLevelBelow { set; }
        bool NotifyCallIsOutOfAcceptableLevels { set; }
        int LimitForEnergyTrendIsRaisingFor { get; }
        double LimitForEnergyLevelBelow { get; }
        byte DataChannelNumber { get; }

        void AddSegmentToList(int segCount, string bstring, int sPos, int fPos, EmotionResults emoVals,
                              string onlineFlag, string comment);

        bool CreateSegmentAudioFiles();
        void DrawData(double[] data, double min, double max, double avg, int scaleMin, int scaleMax, int column);
        void DrawSegment(byte[] segmentData);
        void DrawSegment(short[] segmentData);
        int GetAnalysisType();
        short GetBackgroundLevel();
        short GetCalibrationType();
        double[] GetSegemtsDataFromColumn(int column, out double min, out double max, out double avg);
        bool IsOneSecondBufferUsed();
        bool IsSegmentOutOfLimits();
        bool NeedToPlayEachSegment();
        void Reset();
        void SetBGLevel(int level);
        void SetLionetUserDefinedEmotions(string[] emotions);
        void SetUserDefinedSegmentEmotions(string[] segmentEmotions);
        void ShowMessage(string message);
        void ShowLicenseScreen();

        void ShowProfile(short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel,
                         short energeticLevel, short thinkingLevel);

        void UpdateCallPriority(int segmentID, int callPriority, int stressSegs, int angerSegs, int upsetSegs);
        void UpdateHistoryBars(EmotionResults emoVals);

        void UpdateProfile(short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel,
                           short energeticLevel, short thinkingLevel);

        string GetCurrentAiResultForSegment(int segmentId);
        void SetAiResultForSegment(int segmentId, string aiResult);
        void FreezTables();
        void UnFreezTables();
        void SetCallLioNetAnalysis(string lioNetCallAnalysis);
        void SetCallEmotionalSignature(string signature);
        void SetCallProfile(string callProfile);
        void SetNetAccuracy(string netAccuracy);

        void SetLicenseDetails(string sysID, int callCounter, short postsLicensed, int runningProcesses,
                               string coreVersion);

        void SetConversationBordersData(Array callBordersData);
        void SetEnvelopAndBordersReport(string report);
        void DrawBorders(double[,] callBordersData, EnvelopData[] envelopData);
        void SetEnergyLevel(double enrgy, bool highlited);

        void AddCallRecord(string fileName, TimeSpan processingTime, int callMaxPriorityFlag, int callPriority,
                           string lioRes, double overallBordersDistance, string angryPersent, string stressPersent,
                           string upsetPersent, short agentRank, double avgVoiceEnergy);
    }
}