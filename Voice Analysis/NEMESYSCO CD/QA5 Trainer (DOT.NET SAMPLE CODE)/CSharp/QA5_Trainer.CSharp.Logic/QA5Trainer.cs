#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Nemesysco.Std.FullDuplexStub;
using Nemesysco.Std.WaveLib.Native;
using QA5_Trainer.CSharp.Interfaces;
using QA5COM;

#endregion

namespace QA5_Trainer.CSharp.Logic
{
    public partial class QA5Trainer
    {
        private readonly List<string> callEmotions = new List<string>(100);
        private readonly FullDplxClass fullDuplex = new FullDplxClass();
        private readonly List<string> lioNetResultsCache = new List<string>(100);
        private readonly List<string> segmentEmotions = new List<string>(100);
        private readonly TrendAnalyzer[] trenders = new TrendAnalyzer[6];
        private readonly IQA5UI ui;
        private int bitsPerSample;
        private short bufferSize;
        private IntPtr conversionBuffer = IntPtr.Zero;
        private short[] splitBuffer;
        private int trainingsCount;
        private string defOwner = "Nemesysco Ltd";
        private string bordersFileName;
        private string directoryName;
        private string fileName;
        private string lioDataEmoSigFileName;
        private string lioDataSEGMENTfile;
        private nmsQA5core nmsCOMcallee;
        private nmsQA5core nmsQA5LicServer;
        private string segmentsFolder;
        private short waveOutSamplesPerSecond;
        private nmsLioNetV6 nmsLioNet { get; set; }
        private readonly ManualResetEvent waitForPlaybackFinished = new ManualResetEvent(true);
        private Array callBordersData;

        private int upsetSegments;
        private string onlineFlag;
        private int stressSegments;
        private int enrgyStaysHighSegments;
        private int angerSegments;
        private int midEnergySegments;
        private int lowEnergySegments;
        private int callMaxPriorityFlag;
        private double overallBordersDistance;
        private int callPriority;
        private string lioNetResult;
        private bool processingBatch;
        private string dataFolder;
        private TextWriter cSvWriter;
        private int processingFileChannels;
        private short avgVoiceEnergy;


        public QA5Trainer(IQA5UI iQa5Ui)
        {
            if (iQa5Ui == null)
                throw new ArgumentNullException("iQa5Ui");

            ui = iQa5Ui;
            SubscribeToUiEvents();

            fullDuplex.WaveOutEndOfFile += fullDuplex_WaveOutEndOfFile;
        }

        private void fullDuplex_WaveOutEndOfFile()
        {
            waitForPlaybackFinished.Set();
        }

        private void SubscribeToUiEvents()
        {
            ui.FileSelected += ui_FileSelected;
            ui.SegmentsListColumnClicked += (SegmentsList_ColumnClicked);
            ui.SegmentClicked += (SegmentsList_SegmentClicked);
            ui.NewSegmentEmotionAdded += ui_NewSegmentEmotionAdded;
            ui.ClassifySegment += ui_ClassifySegment;
            ui.LioNetForget += ui_LioNetForget;
            ui.ClassifyCall += ui_ClassifyCall;
            ui.NewCallEmotionAdded += ui_NewCallEmotionAdded;
            ui.UiIsGoingToClose += ui_UiIsGoingToClose;
            ui.ForgetCallClassification += ui_ForgetCallClassification;
            ui.RetrieveLicenseDetails += ui_RetrieveLicenseDetails;
            ui.ApplyCountersResetCode += ui_ApplyCountersResetCode;
            ui.UpdateBordersFile += ui_UpdateBordersFile;
            ui.ShowBordersOnGraph += ui_ShowBordersOnGraph;
        }

        private void ui_ShowBordersOnGraph(object sender, EventArgs e)
        {
            var envelopAndBordersRanges = new List<EnvelopData>(52);

            for (short parmN = 0; parmN <= 51; parmN++)
            {
                double lowBorder = 0;
                double parmAvrg = 0;
                double highEnvelope = 0;
                double lowEnvelope = 0;
                double highBorder = 0;

                nmsCOMcallee.nmsBordersGetEnvelopesData(ref parmN, ref lowBorder, ref highBorder, ref lowEnvelope,
                                                        ref highEnvelope, ref parmAvrg);

                envelopAndBordersRanges.Add(new EnvelopData(lowBorder, highBorder, lowEnvelope, highEnvelope, parmAvrg));
            }

            ui.DrawBorders((double[,]) callBordersData, envelopAndBordersRanges.ToArray());
        }

        private void ui_UpdateBordersFile(object sender, EventArgs e)
        {
            nmsCOMcallee.nmsBordersUpdateWithCurrent(ref bordersFileName);
        }

        private void ui_ApplyCountersResetCode(object sender, ApplyCountersResetCodeArgs e)
        {
            string resetCode = e.ResetCode;
            int rc = nmsQA5LicServer.nmsSRV_ResetCounter(ref resetCode);

            if (rc == NMS_SDKERROR_OPERATIONNOTALLOWED)
                ui.ShowMessage("Illegal license or operation failed");
            else
                ui.ShowMessage("Operation Succeeded");
        }

        private void ui_RetrieveLicenseDetails(object sender, EventArgs e)
        {
            int callCounter = 0;
            string sysID = string.Empty;
            short postsLicensed = 0;
            int runningProcesses = 0;
            nmsQA5LicServer.nmsSRV_GetOpDetails(ref callCounter, ref sysID, ref postsLicensed, ref runningProcesses);

            string coreVersion = "version";
            nmsQA5LicServer.nmsSRV_ResetCounter(ref coreVersion);

            ui.SetLicenseDetails(sysID, callCounter, postsLicensed, runningProcesses, coreVersion);
        }

        private string ApplicationDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(directoryName))
                {
                    Assembly executingAssembly = Assembly.GetExecutingAssembly();
                    directoryName = Path.GetDirectoryName(executingAssembly.Location);
                }
                return directoryName;
            }
        }

        private void ui_ForgetCallClassification(object sender, ForgetCallClassificationArgs e)
        {
            string callSignature = e.CallSignature;
            if (callSignature.IndexOf('=') == -1)
                return;

            int rc = nmsLioNet.nmslioNetForget(ref callSignature);

            if (rc != 0)
                ui.ShowMessage("Error 'Forgetting': " + rc);
            else
                ui.ShowMessage("'Forgetting' succeeded");

            callSignature = callSignature.Substring(0, callSignature.IndexOf('='));
            ui.SetCallEmotionalSignature(callSignature);
        }

        private void ui_UiIsGoingToClose(object sender, EventArgs e)
        {
            if (conversionBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(conversionBuffer);

            string errorMessage = string.Empty;
            if (nmsCOMcallee != null)
                nmsCOMcallee.nmsLioNetSave(ref errorMessage);
        }

        private void ui_NewCallEmotionAdded(object sender, NewCallEmotionalArgs e)
        {
            string emotionName = e.Emotion;

            if (callEmotions.Contains(emotionName))
                return;

            emotionName = emotionName.Replace(' ', '_');
            emotionName = emotionName.Replace('=', '_');

            callEmotions.Add(emotionName);

            string path = Path.Combine(ApplicationDirectoryPath, "EmoUserDef.ini");
            using (TextWriter textWriter = new StreamWriter(path))
            {
                foreach (string emotion in callEmotions)
                {
                    textWriter.WriteLine(emotion);
                }
            }

            ui.SetLionetUserDefinedEmotions(callEmotions.ToArray());
        }

        private void ui_ClassifyCall(object sender, ClassifyCallArgs e)
        {
            if (e.CallSignature.IndexOf('=') != -1)
            {
                ui.ShowMessage("This Emotional Signature was already trained.");
                return;
            }

            string callEmotionSignature = string.Format("{0} ={1}", e.CallSignature, e.EmotionName);
            string res = String.Empty;
            double tOutOfKnownRange = 0;
            double tOdist = 0;
            double tOprob = 0;
            double tOrisk = 0;
            short resAccuracy = 0;
            int rcl = nmsLioNet.nmslioNetGo(callEmotionSignature, ref res, ref tOdist, ref tOprob, ref tOrisk,
                                            ref tOutOfKnownRange, ref resAccuracy);
            if (rcl == -1)
            {
                ui.ShowMessage("Unknown problem - Can't teach.");
                return;
            }

            //Calculate and show the current accuracy level
            string LioNetRep = string.Empty;
            ui.SetNetAccuracy("Net accuracy:" +
                              nmsLioNet.nmslioIsNetReady(ref lioDataEmoSigFileName, ref LioNetRep).ToString("F02") +
                              "%");
            ui.ShowMessage("LioNet Response (Call): " + res);

            //every 5 trainings it is advised to send the LioNet com to sleep, and every 10 for deep sleep...
            trainingsCount++;
            string outMsg = string.Empty;
            if ((trainingsCount%5) == 0)
            {
                if ((trainingsCount%10) == 0)
                    nmsLioNet.nmslioTakeDeepSleep();

                nmsLioNet.nmslioNetSleep();
            }

            //after each training, save the Net data
            nmsLioNet.nmslioSaveNet(lioDataEmoSigFileName, ref outMsg);

            ui.SetCallEmotionalSignature(callEmotionSignature);
        }

        private void ui_LioNetForget(object sender, LioNetForgetArgs e)
        {
            string esig = lioNetResultsCache[e.SegmentId];
            int rc = nmsCOMcallee.nmslioNetForget(ref esig);

            if (rc != 0)
                ui.ShowMessage("Error 'Forgetting' :" + rc);
            else
                ui.ShowMessage("'Forgetting' succeeded");

            esig = esig.Substring(0, esig.IndexOf('='));
            lioNetResultsCache[e.SegmentId] = esig;

            string aiResultForSegment = ui.GetCurrentAiResultForSegment(e.SegmentId);
            string lionetResponse = "";
            double dist = 0;
            double risk = 0;
            double prob = 0;
            double tOutOfKnownRange = 0;
            short resAccuracy = 0;
            nmsCOMcallee.nmslioNetGo(ref esig, ref lionetResponse, ref dist, ref risk, ref prob,
                                     ref tOutOfKnownRange, ref resAccuracy);
            if (lionetResponse != aiResultForSegment)
            {
                if (String.IsNullOrEmpty(lionetResponse))
                    ui.SetAiResultForSegment(e.SegmentId, "?");
                else
                    ui.SetAiResultForSegment(e.SegmentId, "R_" + lionetResponse);
            }
        }

        private void ui_ClassifySegment(object sender, ClassifySegmentArgs e)
        {
            string esig = string.Format("{0} ={1}", lioNetResultsCache[e.SegmentId], e.EmotionName);
            string res = "";
            double tOdist = 0;
            double tOprob = 0;
            double tOrisk = 0;
            double tOutOfKnownRange = 0;
            short resAccuracy = 0;
            int rcl = nmsCOMcallee.nmslioNetGo(ref esig, ref res, ref tOdist, ref tOprob, ref tOrisk,
                                               ref tOutOfKnownRange, ref resAccuracy);
            if (rcl == -1)
            {
                ui.ShowMessage("Unknown problem - Can't teach.");
                return;
            }

            ui.ShowMessage(string.Format("LioNet Response: {0}", res));

            ui.SetAiResultForSegment(e.SegmentId, "C_" + e.EmotionName);
        }

        private void ui_NewSegmentEmotionAdded(object sender, NewSegmentEmotionArgs e)
        {
            string emotionName = e.EmotionName;

            if (segmentEmotions.Contains(emotionName))
                return;

            emotionName = emotionName.Replace(' ', '_');
            emotionName = emotionName.Replace('=', '_');

            segmentEmotions.Add(emotionName);

            string path = Path.Combine(ApplicationDirectoryPath, "EmoSigsDef.ini");
            using (TextWriter textWriter = new StreamWriter(path))
            {
                foreach (string emotion in segmentEmotions)
                {
                    textWriter.WriteLine(emotion);
                }
            }

            ui.SetUserDefinedSegmentEmotions(segmentEmotions.ToArray());
        }

        private void DoAnalysis()
        {
            WaveStream stream;
            Exception exception;
            try
            {
                stream = new WaveStream(File.Open(fileName, FileMode.Open, FileAccess.Read));
            }
            catch (Exception exception1)
            {
                exception = exception1;
                ui.ShowMessage(exception.Message);
                return;
            }
            try
            {
                try
                {
                    ResetStatistics();

                    onlineFlag = onlineFlag = "---";

                    int cStartPosSec = 0;
                    int cEndPosSec = 0;
                    int segmentID = 0;
                    Array inpBuf = new short[bufferSize];

                    DateTime startProcessingTime = DateTime.Now;

                    while (ReadBuffer(stream, (short[]) inpBuf) > 0)
                    {

                        ProcessBuffer(ref inpBuf, ref segmentID, ref cEndPosSec, ref cStartPosSec);
                    }

                    DateTime endProcessingTime = DateTime.Now;

                    TimeSpan processingTime = endProcessingTime - startProcessingTime;

                    UpdateCallProfile(segmentID, processingTime);
                    AnalyzeConversationBorders();
                    UpdateTestsDatabase(processingTime, segmentID, (double)avgVoiceEnergy / segmentID);
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    ui.ShowMessage(exception.Message + Environment.NewLine + exception.StackTrace);
                }
            }
            finally
            {
                stream.Close();
                ui.UnFreezTables();

                if (!processingBatch)
                    ui.ShowMessage("Done");
            }
        }

        private void ProcessBuffer(ref Array inpBuf, ref int segmentID, ref int cEndPosSec, ref int cStartPosSec)
        {
            double brderS = -1.0;
            Array emoValsArray = null;
            string aIres = "";
            string bStr = "";
            Array testbuf = null;
            int testBufLeng = 0;
            var bufSize = (short) (bufferSize - 1);
            int processBufferResult = nmsCOMcallee.nmsProcessBuffer(ref inpBuf,
                                                                    ref bufSize,
                                                                    ref emoValsArray,
                                                                    ref aIres,
                                                                    ref bStr,
                                                                    ref testbuf,
                                                                    ref testBufLeng,
                                                                    ref brderS);
            if (processBufferResult == NMS_SDKERROR)
            {
                throw new Exception(
                    "A critical error ocured inside the SDK, and the analysis can not proceed. This is mostly caused by an internal protection error. Please close this application and start again, make sure your plug is connected, and try again.");
            }

            if ((processBufferResult == NMS_PROCESS_VOICEDETECTED) && (cStartPosSec == 0))
                cStartPosSec = cEndPosSec;

            if (processBufferResult == NMS_OK)
                cStartPosSec = 0;

            if (processBufferResult == NMS_FAILED)
                cStartPosSec = 0;

            cEndPosSec += 2;

            if (processBufferResult == NMS_PROCESS_ANALYSISREADY)
            {
                EmotionResults emoVals = CopyValuesFromEmoArrayIntoEmotionsStructure(emoValsArray, aIres);

                avgVoiceEnergy += emoVals.VoiceEnergy;

                nmsCOMcallee.nmsQA_CollectAgentScoreData();

                UpdateAlarms(emoVals, segmentID);

                ui.UpdateHistoryBars(emoVals);

                if (segmentID >= lioNetResultsCache.Count)
                {
                    for (int i = 0; i <= 100; i++)
                        lioNetResultsCache.Add(string.Empty);
                }

                lioNetResultsCache[segmentID] = bStr;

                nmsCOMcallee.nmsQA_Logdata(ref segmentID, ref cStartPosSec, ref cEndPosSec);
                nmsCOMcallee.nmsSD_LogData();

                string comment = string.Empty;
                UpdateSegmentsListAndCsv(segmentID, bStr, cStartPosSec, cEndPosSec, emoVals, comment);

                UpdateProfiles(segmentID);

                nmsCOMcallee.nmsCollectProfiler();
                if (ui.CreateSegmentAudioFiles())
                {
                    SaveSegment(segmentID, testbuf);
                    if (ui.NeedToPlayEachSegment())
                        PlaySegment(segmentID);
                }

                UpdateCallPriority(segmentID);

                cStartPosSec = 0;
                segmentID++;
            }
        }

        private void UpdateTestsDatabase(TimeSpan processingTime, int segmentID, double avgVoiceEnergy)
        {
            ui.AddCallRecord(fileName,
                             processingTime,
                             callMaxPriorityFlag,
                             callPriority,
                             lioNetResult,
                             overallBordersDistance,
                             ((angerSegments*100.0)/segmentID).ToString("F02"),
                             ((stressSegments*100.0)/segmentID).ToString("F02"),
                             ((upsetSegments*100.0)/segmentID).ToString("F02"),
                             (short)nmsCOMcallee.nmsQA_CollectAgentScoreData(),
                             avgVoiceEnergy
                );
        }

        private void ResetStatistics()
        {
            enrgyStaysHighSegments = 0;
            midEnergySegments = 0;
            lowEnergySegments = 0;
            stressSegments = 0;
            angerSegments = 0;
            upsetSegments = 0;
            callMaxPriorityFlag = 0;
        }

        private void UpdateCallPriority(int segmentID)
        {
            callPriority = (int) (((((stressSegments*0.25) + (angerSegments*2.5)) + (upsetSegments*0.25))*100.0)/(segmentID));
            if (callPriority > 100)
                callPriority = 100;

            if (callMaxPriorityFlag < callPriority)
                callMaxPriorityFlag = callPriority;

            ui.UpdateCallPriority(segmentID, callPriority, stressSegments, angerSegments, upsetSegments);
        }

        private void UpdateAlarms(EmotionResults EmoVals, int segmentID)
        {
            try
            {
                int rotateCycle;
                double rotatingAvrg = 0;
                bool callIsOutOfAcceptableLevels = false;

                if (EmoVals.upset > 0)
                {
                    upsetSegments++;
                }

                if (ui.AlarmIfAngerTrendIsRaisingAndAbove)
                {
                    ui.HighlightAngerTrendLevel = false;
                    double angry = EmoVals.angry;
                    rotateCycle = 4;
                    rotatingAvrg = trenders[1].regNewValRotatingAvrg(ref angry, ref rotateCycle);
                    if (ui.AlarmIfAngerTrendIsRaisingAndAbove && (rotatingAvrg > ui.MaxAngerTrandLevel))
                    {
                        callIsOutOfAcceptableLevels = true;
                        onlineFlag = "angry " + rotatingAvrg.ToString("F02");
                        ui.HighlightAngerTrendLevel = true;
                    }

                    ui.AngerTrendLevel = rotatingAvrg.ToString("F02");
                }

                if (ui.AlarmIfStressTrendIsRaisingAndAbove)
                {
                    ui.HighlightStressTrendLevel = false;
                    double stress = EmoVals.stress;
                    rotateCycle = 4;
                    rotatingAvrg = trenders[0].regNewValRotatingAvrg(ref stress, ref rotateCycle);
                    if (rotatingAvrg > ui.MaxStressTrendLevel)
                    {
                        ui.NotifyStressLevelIsRaising = true;
                        ui.HighlightStressTrendLevel = true;
                        onlineFlag = "STRESS " + rotatingAvrg.ToString("F02");
                        stressSegments++;
                    }
                    else if (rotatingAvrg < (ui.MaxAngerTrandLevel - 1.0))
                    {
                        ui.NotifyStressLevelIsRaising = false;
                        ui.HighlightStressTrendLevel = false;
                    }
                    else
                    {
                        ui.HighlightStressTrendLevel = false;
                    }

                    ui.StressTrendLevel = rotatingAvrg.ToString("F02");
                }

                if (ui.AlarmIfStressTrendIsRaisingAndAbove || ui.AlarmIfStressTrendIsLow)
                {
                    ui.HighlightEnergyTrendIsRaisingFor = false;
                    ui.HighlightEnergyLevelBelow = false;

                    double difEnrgy = 0;
                    if (segmentID > 5)
                    {
                        rotateCycle = 8;
                        double energy = EmoVals.Energy;
                        rotatingAvrg = trenders[2].regNewValRotatingAvrg(ref energy, ref rotateCycle);
                        double num6 = trenders[2].getFirstValuesRotating() + 3.0;

                        difEnrgy = (rotatingAvrg - num6)*2.0;
                        ui.EnergyDifference = difEnrgy.ToString("F02");

                        if (difEnrgy < 7.0)
                        {
                            enrgyStaysHighSegments = 0;
                        }
                        else
                        {
                            enrgyStaysHighSegments++;
                        }

                        ui.EnrgyHighSegments = enrgyStaysHighSegments.ToString("");

                        ui.SetEnergyLevel(difEnrgy, (difEnrgy >= 7.0));
                    }

                    if (ui.AlarmIfStressTrendIsRaisingAndAbove)
                    {
                        if (enrgyStaysHighSegments > ui.LimitForEnergyTrendIsRaisingFor && (EmoVals.content == 0))
                        {
                            callIsOutOfAcceptableLevels = true;
                            ui.HighlightEnergyTrendIsRaisingFor = true;
                            onlineFlag = "ANGRY " + rotatingAvrg.ToString("F02");
                            angerSegments++;

                            ui.HighlightEnergyLevel = true;
                        }
                        else
                        {
                            ui.HighlightEnergyTrendIsRaisingFor = false;
                            if (difEnrgy > 0.0)
                            {
                                midEnergySegments++;
                            }
                        }

                        ui.EnergyTrendIsRaisingFor = enrgyStaysHighSegments.ToString();
                    }

                    if (ui.AlarmIfStressTrendIsLow && (rotatingAvrg <= ui.LimitForEnergyLevelBelow))
                    {
                        callIsOutOfAcceptableLevels = true;
                        ui.HighlightEnergyLevelBelow = true;
                        lowEnergySegments++;

                        ui.NotifySpeakerIsTired = true;
                    }
                    else
                    {
                        ui.NotifySpeakerIsTired = false;
                    }

                    ui.EnergyLevelBelow = rotatingAvrg.ToString("F02");
                }

                ui.NotifyCallIsOutOfAcceptableLevels = callIsOutOfAcceptableLevels;
            }
            catch (Exception exception)
            {
                ui.ShowMessage(exception.Message);
            }
        }

        private void AnalyzeConversationBorders()
        {
            nmsCOMcallee.nmsBordersCalculate(ref callBordersData);

            ui.SetConversationBordersData(callBordersData);

            overallBordersDistance = 0;
            double overallNormalInternalEnvelopeDistance = 0;

            var significantRep = new StringBuilder();

            for (int i = 0; i <= 50; i++)
            {
                if ((double) callBordersData.GetValue(5, i) > 0)
                {
                    overallBordersDistance = overallBordersDistance + (double) callBordersData.GetValue(5, i);
                    if ((double) callBordersData.GetValue(5, i) > 15) //Dif is 15% or more
                    {
                        significantRep.AppendFormat(
                            ">Parameter '{0}' is {1}% far from the Border. (Low: {2}%, High: {3}%){4}",
                            i + 1, ((double) callBordersData.GetValue(5, i)).ToString("F02"),
                            ((double) callBordersData.GetValue(6, i)).ToString("F02"),
                            ((double) callBordersData.GetValue(7, i)).ToString("F02"),
                            Environment.NewLine);
                    }
                }

                if ((double) callBordersData.GetValue(8, i) > 0)
                {
                    overallNormalInternalEnvelopeDistance = overallNormalInternalEnvelopeDistance +
                                                            (double) callBordersData.GetValue(8, i);
                    if ((double) callBordersData.GetValue(8, i) > 15) //' Dif is 15% or more
                    {
                        significantRep.AppendFormat(
                            ">Parameter '{0}' is {1}% far from the normal envelop. (Low: {2}%, High: {3}%){4}",
                            i + 1,
                            ((double) callBordersData.GetValue(8, i)).ToString("F02"),
                            ((double) callBordersData.GetValue(9, i)).ToString("F02"),
                            ((double) callBordersData.GetValue(10, i)).ToString("F02"),
                            Environment.NewLine);
                    }
                }
            }

            significantRep.Insert(0,
                                  String.Format(
                                      "Overall Borders Distance: {0}{2}Overall Normal Internal Envelope Distance:{1}{2}",
                                      overallBordersDistance.ToString("F02"),
                                      overallNormalInternalEnvelopeDistance.ToString("F02"),
                                      Environment.NewLine)
                );

            ui.SetEnvelopAndBordersReport(significantRep.ToString());

            if (overallBordersDistance + overallNormalInternalEnvelopeDistance > 0)
                ui.ShowMessage(
                    String.Format(
                        "This conversation demonstrated differences from the current Borders settings. If you want to update the 'Known Borders', please see the 'Envelope and Borders' tab. ({0}/{1})",
                        overallBordersDistance, overallNormalInternalEnvelopeDistance));
        }

        private void UpdateCallProfile(int segCount, TimeSpan processingTime)
        {
            var nmsCallProfile = new StringBuilder(nmsCOMcallee.nmsCallProfiler(ref segCount));
            nmsCallProfile.AppendLine();
            nmsCallProfile.AppendLine(String.Format("Processing Time: {0}:{1}:{2}.{3}", processingTime.Hours,
                                                    processingTime.Minutes,
                                                    processingTime.Seconds, processingTime.Milliseconds));
            nmsCallProfile.AppendLine();

            //QAsig will receive the Emotional Signature string that can be used to flag the call using LioNet, and to
            //train the system to identify types of CALLS (unlike the basic LioNet used to analyze the SEGMENT level)
            //YOU MUST enter the number of segments for analysis detected in the call (SegCount) collected in this
            //function. All the rest are outputs.
            int nStressL = 0;
            double nAVJstressD = 0;
            double nAVJemoD = 0;
            string RepQA = String.Empty;
            string qa5Signature = nmsCOMcallee.nmsQA_CreateSignature(ref segCount, ref nStressL, ref nAVJstressD,
                                                                     ref nAVJemoD, ref RepQA);

            if (qa5Signature.Length < 10)
                //if this is the case, an error was detected, or the file had less than 5 voice segments. Normally, you will
                //prefer to analyze files containing more than 30 segments at least.
                ui.SetCallLioNetAnalysis("LioNet Analysis: unavailable due to short file or error");
                //the QA5sig will contain the error code.
            else
            {
                //Let LioNet object test the Emotional Signature:
                lioNetResult = "";
                double outDist = 0;
                double outProb = 0;
                double outRisk = 0;
                double outOfRange = 0;
                short resAccuracy = 0;
                nmsLioNet.nmslioNetGo(qa5Signature, ref lioNetResult, ref outDist, ref outProb, ref outRisk,
                                      ref outOfRange, ref resAccuracy);
                //"LioRes" is actually the final decision of the system in Offline, and should be added to your calls
                //database.
                if (String.IsNullOrEmpty(lioNetResult))
                    ui.SetCallLioNetAnalysis("LioNet Analysis: " + "Not Available");
                else
                    ui.SetCallLioNetAnalysis("LioNet Analysis: " + lioNetResult);
                //few more bits of information are returned, such as:
                // nStressL   = Number of high stress segments detected in the call
                // nAVJstressD = Average Stress level in the call
                // nAVJemoD = Average Emotional level in the call
                // RepQA - the full QA report (textualized)
                nmsCallProfile.AppendLine(" QA5 data:");
                nmsCallProfile.AppendLine(RepQA);
            }
            ui.SetCallEmotionalSignature(qa5Signature); //display the emotional Signature data

            ui.SetCallProfile(nmsCallProfile.ToString());
        }

        private bool CheckFileAndGetItsParameters(string processingFileName, out short bufSize, out int BPS,
                                                  out short SPS)
        {
            bufSize = 0;
            BPS = 0;
            SPS = 0;
            fullDuplex.WaveOutFilename = processingFileName;
            fullDuplex.WaveOutGetFileInfo();
            if (fullDuplex.WaveOutFileLengthMilliseconds < 10000)
            {
                ui.ShowMessage("File is too short");
                return false;
            }

            processingFileChannels = fullDuplex.WaveOutChannels;
            BPS = fullDuplex.WaveOutBitsPerSample;
            SPS = (short) fullDuplex.WaveOutSamplesPerSecond;
            fullDuplex.WaveOutClose();

            if ((BPS != 8) && (BPS != 16))
            {
                ui.ShowMessage("Only files with 8 or 16 bit per sample are supported");
                return false;
            }

            if ((((SPS != 8000) && (SPS != 11025)) &&
                 (SPS != 11000)) && (SPS != 6000))
            {
                ui.ShowMessage(string.Format("Files with {0}Hz sampling rate are not supported",
                                             SPS));
                return false;
            }
            switch (SPS)
            {
                case 11025:
                case 11000:
                    bufSize = 220;
                    break;

                case 6000:
                    bufSize = 120;
                    break;

                case 8000:
                    bufSize = 160;
                    break;
            }

            if (conversionBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(conversionBuffer);

            conversionBuffer = Marshal.AllocHGlobal((BPS == 8 ? bufSize : bufSize * 2) * processingFileChannels);
            splitBuffer = new short[bufSize * processingFileChannels];

            return true;
        }

        private bool ConfigureCoreForItsDesignatedTasks()
        {
            int nmsConfigTestDataResult;
            short lengthOfSegmentInSeconds;
            short calibrationType = ui.GetCalibrationType();
            if (ui.GetAnalysisType() == 1)
                calibrationType = (short) (calibrationType + 2);

            short backgroundLevel = ui.GetBackgroundLevel();

            if (ui.IsOneSecondBufferUsed())
            {
                lengthOfSegmentInSeconds = 1;
                nmsConfigTestDataResult = nmsCOMcallee.nmsConfigTestData(ref waveOutSamplesPerSecond,
                                                                         ref backgroundLevel,
                                                                         ref lengthOfSegmentInSeconds,
                                                                         ref calibrationType);
            }
            else
            {
                lengthOfSegmentInSeconds = 2;
                nmsConfigTestDataResult = nmsCOMcallee.nmsConfigTestData(ref waveOutSamplesPerSecond,
                                                                         ref backgroundLevel,
                                                                         ref lengthOfSegmentInSeconds,
                                                                         ref calibrationType);
            }

            if (nmsConfigTestDataResult != 0)
            {
                if (nmsConfigTestDataResult == NMS_SDKERROR_WAVESMPRATEWRONG)
                {
                    ui.ShowMessage("The wave format provided is not supported. The operation will now abort.");
                    Marshal.ReleaseComObject(nmsCOMcallee);
                    nmsCOMcallee = null;
                    return false;
                }

                ui.ShowMessage(
                    string.Format(
                        "An error was detected while configuring the core. The operation will now abort. Error = {0}",
                        nmsConfigTestDataResult));
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            nmsCOMcallee.nmsQA_ConfigUse();
            nmsCOMcallee.nmsSD_ConfigStressCL();
            nmsCOMcallee.nmsBordersLoad(ref bordersFileName);
            bool bActive = true;
            nmsCOMcallee.nmsBordersSetConfig(ref bActive);
            return true;
        }

        private static EmotionResults CopyValuesFromEmoArrayIntoEmotionsStructure(Array EmoArr, string AIres)
        {
            return new EmotionResults
                       {
                           AIres = AIres,
                           angry = (short) EmoArr.GetValue(0),
                           Atmos = (short) EmoArr.GetValue(1),
                           concentration_level = (short) EmoArr.GetValue(2),
                           embarrassment = (short) EmoArr.GetValue(3),
                           excitement = (short) EmoArr.GetValue(4),
                           hesitation = (short) EmoArr.GetValue(5),
                           imagination_activity = (short) EmoArr.GetValue(6),
                           intensive_thinking = (short) EmoArr.GetValue(7),
                           content = (short) EmoArr.GetValue(8),
                           saf = (short) EmoArr.GetValue(9),
                           upset = (short) EmoArr.GetValue(10),
                           extremeState = (short) EmoArr.GetValue(11),
                           stress = (short) EmoArr.GetValue(12),
                           uncertainty = (short) EmoArr.GetValue(13),
                           Energy = (short) EmoArr.GetValue(14),
                           BrainPower = (short) EmoArr.GetValue(15),
                           EmoCogRatio = (short) EmoArr.GetValue(0x10),
                           maxAmpVol = (short) EmoArr.GetValue(0x11),
                           VoiceEnergy = (short) EmoArr.GetValue(18)
                       };
        }

        private void CreateDataFolders()
        {
            dataFolder = Path.Combine(Path.GetDirectoryName(fileName),
                                      Path.GetFileNameWithoutExtension(fileName) + " Data");

            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            segmentsFolder = Path.Combine(dataFolder, "Segments");
            if (!Directory.Exists(segmentsFolder))
                Directory.CreateDirectory(segmentsFolder);
        }

        private void DrawSegmentWaveform(string segmentFileName)
        {
            lock (this)
            {
                using (var stream = new WaveStream(segmentFileName))
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int) stream.Length);
                    if (stream.FormatEx.wBitsPerSample == 8)
                    {
                        ui.DrawSegment(buffer);
                    }
                    else
                    {
                        IntPtr zero = IntPtr.Zero;
                        try
                        {
                            var destination = new short[buffer.Length/2];
                            zero = Marshal.AllocHGlobal(((Marshal.SizeOf(typeof (short))*buffer.Length)/2));
                            Marshal.Copy(buffer, 0, zero, buffer.Length);
                            Marshal.Copy(zero, destination, 0, buffer.Length/2);
                            ui.DrawSegment(destination);
                        }
                        finally
                        {
                            if (zero != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(zero);
                            }
                        }
                    }
                }
            }
        }

        public bool Initialize()
        {
            lioDataEmoSigFileName = Path.Combine(ApplicationDirectoryPath, "QA5SigData.lio");
            lioDataSEGMENTfile = Path.Combine(ApplicationDirectoryPath, "EmoData.lio");
            bordersFileName = Path.Combine(ApplicationDirectoryPath, "QAbrdrs.dat");

            if (!InitQA5Core())
                return false;

            ui.SetBGLevel(0x3e8);

            LoadUserDefinedEmoSegments();

            LoadLionetUserDefinedEmotions();

            if (!LoadLioNetKnowledgeAndEmotionalSignatures())
                return false;

            for (var i = 0; i < trenders.Length; i++)
                trenders[i] = new TrendAnalyzerClass();

            return true;
        }

        private bool InitLicenseAndCallee()
        {
            short activationResult;
            nmsCOMcallee = new nmsQA5coreClass();
            int tProcessId = nmsCOMcallee.nmsSDKgetProcessId();
            string actCode = nmsQA5LicServer.nmsSRV_GetActivationLicense(ref tProcessId);
            if (!short.TryParse(actCode.Substring(0, 4), out activationResult))
                activationResult = 0;

            if (activationResult == NMS_SDKERROR_LICENSERENEWNEEDEDNOW)
            {
                ui.ShowMessage("The system requires re-licensing to operate!");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            if (activationResult == NMS_SDKERROR_PROTECTIONERROR)
            {
                ui.ShowMessage("The activation license could not be generated due to a protection error!");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            if (activationResult == NMS_SDKERROR_OPERATIONNOTALLOWED)
            {
                ui.ShowMessage(
                    "The activation license could not be generated because too many processes are already running. Please wait until one of the other calls is ended, and obtain a license again");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            switch (nmsCOMcallee.nmsInitCore(ref actCode, ref lioDataSEGMENTfile, ref defOwner))
            {
                case NMS_OK:
                    break;

                case NMS_LIOERROR_FILENOTFOUND:
                    ui.ShowMessage("Error LioNet file missing - Deal here with Missing LioNet files");
                    Marshal.ReleaseComObject(nmsCOMcallee);
                    nmsCOMcallee = null;
                    return false;

                case NMS_LIONET_CREATED_OK:
                    ui.ShowMessage("New LioNet file created!");
                    break;

                default:
                    {
                        actCode = nmsQA5LicServer.nmsSRV_GetActivationLicense(ref tProcessId);
                        int nmsInitCoreResult = nmsCOMcallee.nmsInitCore(ref actCode, ref lioDataSEGMENTfile,
                                                                         ref defOwner);
                        switch (nmsInitCoreResult)
                        {
                            case NMS_SDKERROR_UNSPECIFIED:
                                ui.ShowMessage("An unspecified type of error occurred. The operation will now abort.");
                                Marshal.ReleaseComObject(nmsCOMcallee);
                                nmsCOMcallee = null;
                                return false;

                            case NMS_SDKERROR_PROTECTIONERROR:
                                ui.ShowMessage("The Activation License was not valid. The operation will now abort.");
                                Marshal.ReleaseComObject(nmsCOMcallee);
                                nmsCOMcallee = null;
                                return false;
                        }

                        ui.ShowMessage(string.Format("Error initializing QA5COM. Error: {0}.", nmsInitCoreResult));
                        Marshal.ReleaseComObject(nmsCOMcallee);
                        nmsCOMcallee = null;
                        return false;
                    }
            }
            return true;
        }

        private void InitLioNetResultsCache()
        {
            lioNetResultsCache.Clear();
            for (int i = 0; i < 100; i++)
            {
                lioNetResultsCache.Add(string.Empty);
            }
        }

        private bool InitQA5Core()
        {
            try
            {
                nmsQA5LicServer = new nmsQA5coreClass();
                short nmsSRV_INITResult = nmsQA5LicServer.nmsSRV_INIT(ref defOwner);
                if (nmsSRV_INITResult != 0)
                {
                    if (nmsSRV_INITResult == NMS_SDKERROR_PROTECTIONERROR)
                    {
                        ui.ShowMessage("(Plug Not found, or general security error.)");
                    }
                    if (nmsSRV_INITResult != NMS_SDK_LICENSERENEWNEEDED)
                    {
                        ui.ShowMessage("ERROR initializing SDK Server. Error Code:" + nmsSRV_INITResult);
                    }
                    if (nmsSRV_INITResult == NMS_SDKERROR_LICENSERENEWNEEDEDNOW)
                    {
                        ui.ShowLicenseScreen();
                    }
                    if (nmsSRV_INITResult != NMS_SDK_LICENSERENEWNEEDED)
                    {
                        return false;
                    }
                }

                nmsLioNet = new nmsLioNetV6Class();
            }
            catch (Exception exception)
            {
                ui.ShowMessage(exception.Message);
                return false;
            }
            return true;
        }

        private void InitTrenders()
        {
            foreach (TrendAnalyzer analyzer in trenders)
            {
                analyzer.Init();
                int degree = 1;
                analyzer.set_Degree(ref degree);
            }
        }

        private bool LoadLioNetKnowledgeAndEmotionalSignatures()
        {
            string outMsg = "";
            if (nmsLioNet.nmslioLoadNet(lioDataEmoSigFileName, ref outMsg) != 0)
            {
                ui.ShowMessage("Loading Lionet for Emotional Signatures failed: " + outMsg +
                               ". If this is the first time you run this application, this is expected. Creating new LioNet data file.");
                const int netInps = 108;
                if (nmsLioNet.nmslioNetCreate(lioDataEmoSigFileName, netInps, defOwner) != 0)
                {
                    ui.ShowMessage("Unexpected Error occurred  while creating a new LioNet!");
                    return false;
                }
                nmsLioNet.nmslioSaveNet(lioDataEmoSigFileName, ref outMsg);
                ui.SetNetAccuracy("NEW NET");
            }
            else
            {
                string sNetReport = "";
                ui.SetNetAccuracy(string.Format("Net accuracy: {0}%",
                                                nmsLioNet.nmslioIsNetReady(ref lioDataEmoSigFileName,
                                                                           ref sNetReport).ToString("F02",
                                                                                                    CultureInfo
                                                                                                        .
                                                                                                        InvariantCulture)));
            }
            return true;
        }

        private void LoadLionetUserDefinedEmotions()
        {
            string path = Path.Combine(ApplicationDirectoryPath, "EmoUserDef.ini");
            if (File.Exists(path))
            {
                try
                {
                    using (TextReader reader = new StreamReader(path))
                    {
                        string emotion;
                        while ((emotion = reader.ReadLine()) != null)
                        {
                            callEmotions.Add(emotion);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ui.ShowMessage("Error reading EmoUserDef.ini: " + exception.Message);
                }
            }

            if (callEmotions.Count == 0)
            {
                callEmotions.Add("Neutral");
            }
            else if (callEmotions[0] == "")
            {
                callEmotions[0] = "Neutral";
            }
            ui.SetLionetUserDefinedEmotions(callEmotions.ToArray());
        }

        private void LoadUserDefinedEmoSegments()
        {
            string path = Path.Combine(ApplicationDirectoryPath, "EmoSigsDef.ini");
            if (File.Exists(path))
            {
                try
                {
                    using (TextReader reader = new StreamReader(path))
                    {
                        string emotion;
                        while ((emotion = reader.ReadLine()) != null)
                        {
                            segmentEmotions.Add(emotion);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ui.ShowMessage("Error reading EmoSigsDef.ini: " + exception.Message);
                }
            }
            if (segmentEmotions.Count == 0)
            {
                segmentEmotions.Add("Neutral");
            }
            else if (segmentEmotions[0] == "")
            {
                segmentEmotions[0] = "Neutral";
            }

            ui.SetUserDefinedSegmentEmotions(segmentEmotions.ToArray());
        }

        private void ui_FileSelected(object sender, FileSelectedArgs e)
        {
            processingBatch = e.BatchProcess;
            fileName = e.FileName;
            avgVoiceEnergy = 0;
            ui.Reset();
            ui.FreezTables();

            if (
                CheckFileAndGetItsParameters(e.FileName, out bufferSize, out bitsPerSample,
                                             out waveOutSamplesPerSecond) && InitLicenseAndCallee())
            {
                if (ConfigureCoreForItsDesignatedTasks())
                {
                    InitTrenders();
                    
                    InitLioNetResultsCache();
                    
                    CreateDataFolders();

                    CreateAndInitializeCsvFile();

                    DoAnalysis();

                    FinalizeCsvFile();
                }
            }
        }

        private void FinalizeCsvFile()
        {
            cSvWriter.Close();
        }

        private void CreateAndInitializeCsvFile()
        {
            cSvWriter = new StreamWriter(Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(fileName) + ".csv"));

            cSvWriter.WriteLine("Seg N.,Start Pos (Sec.),End Pos (Sec.),Energy,Content,Upset,Angry,Stressed,Uncertain,Excited,Concentrated,EmoCogRatio,Hesitation,BrainPower,Embar.,I. Think,Imagin,ExtremeEmotion,SAF,Atmos.,ONLINE Flag,LioNet analysis,MaxAmpVol.,Comment,LioNet Info");
        }

        private void PlaySegment(int segCount)
        {
            waitForPlaybackFinished.WaitOne();

            waitForPlaybackFinished.Reset();

            string segmentFileName = Path.Combine(segmentsFolder,
                                                  string.Format("Segment_{0}.wav", segCount.ToString("D3")));
            DrawSegmentWaveform(segmentFileName);

            fullDuplex.WaveOutFilename = segmentFileName;
            fullDuplex.WaveOutPlayFile(0, 0);
        }

        private int ReadBuffer(Stream waveStream, short[] soundBuffer)
        {
            int readBufferSize = ((bitsPerSample == 8) ? bufferSize : (bufferSize * 2)) * processingFileChannels;
            var buffer = new byte[readBufferSize];
            int cb = waveStream.Read(buffer, 0, readBufferSize);
            if (cb == 0)
                return 0;

            if (bitsPerSample == 8)
            {
                int j = 0;
                for (int i = 0; i < cb; i+=processingFileChannels)
                    soundBuffer[j++] = (short) ((buffer[i] - 127)*127);

                return cb;
            }
            
            if (processingFileChannels != 1)
            {
                Marshal.Copy(buffer, 0, conversionBuffer, cb);
                Marshal.Copy(conversionBuffer, splitBuffer, 0, cb / 2);

                int j = 0;
                for (int i = ui.DataChannelNumber; i < cb / 2; i += processingFileChannels)
                    soundBuffer[j++] = splitBuffer[i];
            }
            else
            {
                Marshal.Copy(buffer, 0, conversionBuffer, cb);
                Marshal.Copy(conversionBuffer, soundBuffer, 0, cb/2);
            }

            return cb / processingFileChannels;
        }

        private void SaveSegment(int segCount, Array testbuf)
        {
            var writer =
                new WaveWriter(
                    File.Create(Path.Combine(segmentsFolder,
                                             string.Format("Segment_{0}.wav", segCount.ToString("D3")))),
                    new WaveFormatEx(waveOutSamplesPerSecond, 16, 1));
            writer.Write((short[]) testbuf);
            writer.Close();
        }

        private void SegmentsList_ColumnClicked(object sender, ColumnClickArgs e)
        {
            if ((e.Column >= 3) && (e.Column < 20))
            {
                double min;
                double max;
                double avg;
                int scaleMin;
                int scaleMax;
                double[] segmentsData = ui.GetSegemtsDataFromColumn(e.Column, out min, out max, out avg);
                if (e.Column == 3)
                {
                    scaleMin = 0;
                    scaleMax = 50;
                }
                else if (e.Column == 0x13)
                {
                    scaleMin = -80;
                    scaleMax = 80;
                }
                else if (e.Column == 13)
                {
                    scaleMin = 300;
                    scaleMax = 0x5dc;
                }
                else if (e.Column == 11)
                {
                    scaleMin = 30;
                    scaleMax = 500;
                }
                else
                {
                    scaleMin = 0;
                    scaleMax = 30;
                }
                ui.DrawData(segmentsData, min, max, avg, scaleMin, scaleMax, e.Column);
            }
        }

        private void SegmentsList_SegmentClicked(object sender, SegmentClickedArgs e)
        {
            PlaySegment(e.ClickedSegmentId);

            //Test the lionet to see whether or not LioNet would change its original analysis based on the last training sessions
            string lionetString = lioNetResultsCache[e.ClickedSegmentId];
            string Ress = "";
            double Dist = 0;
            double Riss = 0;
            double Probb = 0;
            double OutR = 0;

            string CurRes = ui.GetCurrentAiResultForSegment(e.ClickedSegmentId);

            short resAccuracy = 0;
            nmsCOMcallee.nmslioNetGo(ref lionetString, ref Ress, ref Dist, ref Riss, ref Probb, ref OutR, ref resAccuracy);
            if (Ress != CurRes && !string.IsNullOrEmpty(Ress))
                ui.SetAiResultForSegment(e.ClickedSegmentId, "R_" + Ress);
            // Yes - Lionet decided a new analysis is more appropriate...
        }

        private void UpdateProfiles(int SegCount)
        {
            short emoLevel = 0;
            short logicalLevel = 0;
            short hasitantLevel = 0;
            short stressLevel = 0;
            short energeticLevel = 0;
            short thinkingLevel = 0;
            if (SegCount == 6)
            {
                nmsCOMcallee.nmsQA_getProfilerData(ref emoLevel, ref logicalLevel, ref hasitantLevel,
                                                   ref stressLevel, ref energeticLevel, ref thinkingLevel);
                ui.ShowProfile(emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel);
            }
            else if (SegCount > 6)
            {
                nmsCOMcallee.nmsQA_getChangesFromProfiler(ref emoLevel, ref logicalLevel, ref hasitantLevel,
                                                          ref stressLevel, ref energeticLevel, ref thinkingLevel);
                ui.UpdateProfile(emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel);
            }
        }

        private void UpdateSegmentsListAndCsv(int segCount, string bstring, int sPos, int fPos, EmotionResults emotionResults,
                                              string comment)
        {
            ui.AddSegmentToList(segCount, bstring, sPos, fPos, emotionResults, onlineFlag, comment);

            var stringBuilder = new StringBuilder();
            
            stringBuilder.Append(segCount.ToString());
            double position = (sPos) / 100.0;
            stringBuilder.Append("," + position.ToString("F02"));
            position = (fPos) / 100.0;
            stringBuilder.Append("," + position.ToString("F02"));
            stringBuilder.Append("," + emotionResults.Energy);
            stringBuilder.Append("," + emotionResults.content);
            stringBuilder.Append("," + emotionResults.upset);
            stringBuilder.Append("," + emotionResults.angry);
            stringBuilder.Append("," + emotionResults.stress);
            stringBuilder.Append("," + emotionResults.uncertainty);
            stringBuilder.Append("," + emotionResults.excitement);
            stringBuilder.Append("," + emotionResults.concentration_level);
            stringBuilder.Append("," + emotionResults.EmoCogRatio);
            stringBuilder.Append("," + emotionResults.hesitation);
            stringBuilder.Append("," + emotionResults.BrainPower);
            stringBuilder.Append("," + emotionResults.embarrassment);
            stringBuilder.Append("," + emotionResults.intensive_thinking);
            stringBuilder.Append("," + emotionResults.imagination_activity);
            stringBuilder.Append("," + emotionResults.extremeState);
            stringBuilder.Append("," + emotionResults.saf);
            stringBuilder.Append("," + emotionResults.Atmos);
            stringBuilder.Append("," + onlineFlag);
            stringBuilder.Append("," + emotionResults.AIres);
            stringBuilder.Append("," + emotionResults.maxAmpVol);
            stringBuilder.Append("," + comment);
            stringBuilder.Append("," + bstring);

            cSvWriter.WriteLine(stringBuilder.ToString());

        }
    }
}