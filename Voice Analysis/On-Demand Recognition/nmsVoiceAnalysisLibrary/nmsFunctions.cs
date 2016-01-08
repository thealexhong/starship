using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using WaveLib;
using QA5_Trainer.CSharp.Interfaces;
using QA5COM;

namespace nmsVoiceAnalysisLibrary
{

    public interface nmsFunctions
    {
        bool Initialize();
        void StartA();
    };
    public class ManagedClass : nmsFunctions 
    {
        private const short DEF_BG_LEVEL = 0x3e8;
        private const bool FLAG_QA = true;
        private const bool FLAG_SD = true;
        private const bool FLAG_USEBORDERS = true;
        private const short NMS_FAILED = -1;
        private const short NMS_LIOERROR = -200;
        private const short NMS_LIOERROR_CANTMAKEFILE = -205;
        private const short NMS_LIOERROR_FILENOTFOUND = -201;
        private const short NMS_LIOERROR_FORGETFAILED1 = -203;
        private const short NMS_LIOERROR_FORGETNOBASICFOUND = -204;
        private const short NMS_LIOERROR_PROTECTIONERROR = -202;
        private const short NMS_LIOERROR_UNSPECIFIED = -210;
        private const short NMS_LIONET_CREATED_OK = 1;
        private const short NMS_OK = 0;
        private const short NMS_PROCESS_ANALYSISREADY = 2;
        private const short NMS_PROCESS_SILENCE = 0;
        private const short NMS_PROCESS_VOICEDETECTED = 1;
        private const short NMS_SDK_LICENSERENEWNEEDED = 1;
        private const short NMS_SDKERROR = -100;
        private const short NMS_SDKERROR_FILENOTFOUND = -101;
        private const short NMS_SDKERROR_LICENSERENEWNEEDEDNOW = -105;
        private const short NMS_SDKERROR_OPERATIONNOTALLOWED = -104;
        private const short NMS_SDKERROR_PROTECTIONERROR = -102;
        private const short NMS_SDKERROR_UNSPECIFIED = -110;
        private const short NMS_SDKERROR_WAVESMPRATEWRONG = -103;
        private readonly List<string> callEmotions = new List<string>(100);
        private readonly List<string> lioNetResultsCache = new List<string>(100);
        private readonly List<string> segmentEmotions = new List<string>(100);
        private readonly TrendAnalyzer[] trenders = new TrendAnalyzer[6];
        private short bufferSize;
        private IntPtr conversionBuffer = IntPtr.Zero;
        private string defOwner = "Autonomous Systems and Biomechatronics Lab";
        private string bordersFileName;
        private string directoryName;
        private string lioDataEmoSigFileName;
        private string lioDataSEGMENTfile;
        private nmsQA5core nmsCOMcallee;
        private nmsQA5core nmsQA5LicServer;
        //private short waveOutSamplesPerSecond;
        private nmsLioNetV6 nmsLioNet { get; set; }
        private readonly ManualResetEvent waitForPlaybackFinished = new ManualResetEvent(true);
    
        /* Buffer and Process Control variables declaration*/
        private int cStartPosSec = 0;
        private int cEndPosSec = 0;
        private int segmentID = 0;
        //private Array inpBuf;

        /* Historic .txt declaration */
        private StreamWriter HA;
        private StreamWriter tw;
        
        /* Main Variables and Parameters */
        private DateTime gamestart = DateTime.Now;
        private Array emoValsArray = null;
        //private Array emoValsArray = new Array[21];
        //private List<Int16> emoValsArray = new List<Int16>();
        private EmotionResults emoVals;
        private int dominantTypeOnly;
        private double brderS = -1.0;
        private string aIres = "?";
        private string bStr = "?";
        private Array testbuf = null;
        private int testBufLeng = 0;
        private int count = 0;
        private int processBufferResult;
        private short bufSize;
        private int countNum = Int32.MaxValue; //continous running
        //private int countNum = 80;  //Program running time

        private static int[] emotionSums;
        private int successSegNum;

        private int energysum = 0;
        private int atmossum = 0;
        private int emocogsum = 0;
        private int safsum = 0;
        private int stressSum = 0;
        private int angrySum = 0;
        private int upsetSum = 0;
        private int excitSum = 0;
        private int concentrationSum = 0;

        /* New parameters */
        private int intenThinkSum = 0;
        private int contentsum = 0;
        private int embarrasSum = 0;
        private int hesitatSum = 0;
        private int imag_actSum = 0;
        private int extr_statSum = 0;
        private int uncertSum = 0;
        private int brainpowSum = 0;
        private int max_volSum = 0;
        private int voice_energSum = 0;
        
        /* Counters */
        private int silenceCount = 0;
        private int shortTermCount = 0;
        private short[] left;
        private double SecCount = 0.0;
        private double ncount = 0.0;
        private int TotalSegs = 0;
        private int folderConf; // 0 file directory, 1 folder directory...

        private int[] emt = new int[6];
        public Logistic Model = new Logistic();

        /* Read the initial time. */
        TimeSpan ProcDuration = TimeSpan.Zero;
        DateTime startTime;
        DateTime stopTime;
        
        //csv file 
        private StringBuilder csv;
        private static string audioFileName;
        private string pathName;
        private string resultsPath;


        public bool Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            lioDataEmoSigFileName = Path.Combine(ApplicationDirectoryPath, "QA5SigData.lio");
            lioDataSEGMENTfile = Path.Combine(ApplicationDirectoryPath, "EmoData.lio");
            bordersFileName = Path.Combine(ApplicationDirectoryPath, "QAbrdrs.dat");

            if (!InitQA5Core())
                return false;

            if (!InitLicenseAndCallee())
                return false;

            if (!ConfigureCoreForItsDesignatedTasks())
                return false;

           // InitTrenders();

            InitLioNetResultsCache();

            //ui.SetBGLevel(0x3e8);

            LoadUserDefinedEmoSegments();

            LoadLionetUserDefinedEmotions();

            if (!LoadLioNetKnowledgeAndEmotionalSignatures())
                return false;

            for (var i = 0; i < trenders.Length; i++)
                trenders[i] = new TrendAnalyzerClass();

            return true;
        }

        private bool LoadLioNetKnowledgeAndEmotionalSignatures()
        {
            string outMsg = "";
            if (nmsLioNet.nmslioLoadNet(lioDataEmoSigFileName, ref outMsg) != 0)
            {
                Console.WriteLine("Loading Lionet for Emotional Signatures failed. If this is the first time you run this application, this is expected. Creating new LioNet data file.");
                const int netInps = 108;
                if (nmsLioNet.nmslioNetCreate(lioDataEmoSigFileName, netInps, defOwner) != 0)
                {
                    Console.WriteLine("Unexpected Error occurred  while creating a new LioNet!");
                    return false;
                }
                nmsLioNet.nmslioSaveNet(lioDataEmoSigFileName, ref outMsg);
            }
            return true;
        }

        private void LoadLionetUserDefinedEmotions()
        {
            string path = Path.Combine(ApplicationDirectoryPath, "EmoUserDef.ini");
            if (File.Exists(path))
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

            if (callEmotions.Count == 0)
            {
                callEmotions.Add("Neutral");
            }
            else if (callEmotions[0] == "")
            {
                callEmotions[0] = "Neutral";
            }
        }

        private void LoadUserDefinedEmoSegments()
        {
            string path = Path.Combine(ApplicationDirectoryPath, "EmoSigsDef.ini");
            if (File.Exists(path))
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
            if (segmentEmotions.Count == 0)
            {
                segmentEmotions.Add("Neutral");
            }
            else if (segmentEmotions[0] == "")
            {
                segmentEmotions[0] = "Neutral";
            }
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
                        Console.WriteLine("Plug Not found, or general security error.");
                    }
                    if (nmsSRV_INITResult != NMS_SDK_LICENSERENEWNEEDED)
                    {
                        Console.WriteLine("ERROR initializing SDK Server. Error Code:" + nmsSRV_INITResult);
                    }
                    if (nmsSRV_INITResult == NMS_SDKERROR_LICENSERENEWNEEDEDNOW)
                    {
                        Console.WriteLine("SDK Error. License renew needed");
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
                Console.WriteLine(exception.Message);
                return false;
            }
            return true;
        }

        private bool ConfigureCoreForItsDesignatedTasks()
        {
            int nmsConfigTestDataResult;
            short waveOutSamplesPerSecond = 11025;
            short backgroundLevel = 1000;
            short lengthOfSegmentInSeconds = 1;
            short calibrationType = 5;
            /* Configure the background noise. Default 1000 */
            
            nmsConfigTestDataResult = nmsCOMcallee.nmsConfigTestData(ref waveOutSamplesPerSecond,
                                                                     ref backgroundLevel,
                                                                     ref lengthOfSegmentInSeconds,
                                                                     ref calibrationType);

            if (nmsConfigTestDataResult != 0)
            {
                if (nmsConfigTestDataResult == NMS_SDKERROR_WAVESMPRATEWRONG)
                {
                    Console.WriteLine("The wave format provided is not supported. The operation will now abort.");
                    Marshal.ReleaseComObject(nmsCOMcallee);
                    nmsCOMcallee = null;
                    return false;
                }
                Console.WriteLine("An error was detected while configuring the core. The operation will now abort. Error = {0}", nmsConfigTestDataResult);
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
                Console.WriteLine("The system requires re-licensing to operate!");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            if (activationResult == NMS_SDKERROR_PROTECTIONERROR)
            {
                Console.WriteLine("The activation license could not be generated due to a protection error!");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            if (activationResult == NMS_SDKERROR_OPERATIONNOTALLOWED)
            {
                Console.WriteLine("The activation license could not be generated because too many processes are already running. Please wait until one of the other calls is ended, and obtain a license again");
                Marshal.ReleaseComObject(nmsCOMcallee);
                nmsCOMcallee = null;
                return false;
            }
            switch (nmsCOMcallee.nmsInitCore(ref actCode, ref lioDataSEGMENTfile, ref defOwner))
            {
                case NMS_OK:
                    break;

                case NMS_LIOERROR_FILENOTFOUND:
                    Console.WriteLine("Error LioNet file missing - Deal here with Missing LioNet files");
                    Marshal.ReleaseComObject(nmsCOMcallee);
                    nmsCOMcallee = null;
                    return false;

                case NMS_LIONET_CREATED_OK:
                    Console.WriteLine("New LioNet file created!");
                    break;

                default:
                    {
                        actCode = nmsQA5LicServer.nmsSRV_GetActivationLicense(ref tProcessId);
                        int nmsInitCoreResult = nmsCOMcallee.nmsInitCore(ref actCode, ref lioDataSEGMENTfile,
                                                                         ref defOwner);
                        switch (nmsInitCoreResult)
                        {
                            case NMS_SDKERROR_UNSPECIFIED:
                                Console.WriteLine("An unspecified type of error occurred. The operation will now abort.");
                                Marshal.ReleaseComObject(nmsCOMcallee);
                                nmsCOMcallee = null;
                                return false;

                            case NMS_SDKERROR_PROTECTIONERROR:
                                Console.WriteLine("The Activation License was not valid. The operation will now abort.");
                                Marshal.ReleaseComObject(nmsCOMcallee);
                                nmsCOMcallee = null;
                                return false;
                        }

                        Console.WriteLine("Error initializing QA5COM. Error: {0}.", nmsInitCoreResult);
                        Marshal.ReleaseComObject(nmsCOMcallee);
                        nmsCOMcallee = null;
                        return false;
                    }
            }
            return true;
        }

        /// <summary>
        /// Initialize Functions Above, Start Functions Below
        /// </summary>

        public void StartA() //int *R, int* H //jc
        {
            /* Creates Logistic model */
            Model.setMaxIts(2000);
            Model.buildClassifier();
            Console.ForegroundColor = ConsoleColor.White;
            //WaveLib.WaveFormat fmt = new WaveLib.WaveFormat(11025, 8, 1);
            //m_Player = new WaveLib.WaveOutPlayer(-1, fmt, 11025, 1, new WaveLib.BufferFillEventHandler(Filler));
            //m_Recorder = new WaveLib.WaveInRecorder(-1, fmt, 11025, 1, new WaveLib.BufferDoneEventHandler(DataArrived));
            Console.WriteLine();
            HA = File.CreateText("HumanAffectHistory.txt");
            tw = File.CreateText("VoiceAnalysisResults.txt");
            tw.Close();
            HA.Close();

            /* Reads and analyses the .wav file */
            openWav();

         }

        
        /* Higher Probability  
         * 
         * @param double[] array with probabilities 
         * @return index of the higher probability 
         */
        public int HigherProbability(double[] A)
        {
            double max = A[0];
            int index = 0;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] > max)
                {
                    max = A[i];
                    index = i;
                }
            }
            return index + 1;
        }

        /*
         * Builds the classifier
         *
         * @param train the training data to be used for generating the
         * boosted classifier.
         * @throws Exception if the classifier could not be built successfully
         */
        public int ClassifierLogisticModel()
        {
            /* Average sum */
            double[] TestData = {(energysum * 1.0 / shortTermCount),        (contentsum * 1.0 / shortTermCount),
                                 (upsetSum * 1.0 / shortTermCount),         (angrySum * 1.0 / shortTermCount),
                                 (stressSum * 1.0 / shortTermCount),        (concentrationSum * 1.0 / shortTermCount),
                                 (intenThinkSum * 1.0 / shortTermCount),    (safsum * 1.0 / shortTermCount),
                                 (excitSum * 1.0 / shortTermCount),         (atmossum * 1.0 / shortTermCount),
                                 (emocogsum * 1.0 / shortTermCount),        (embarrasSum * 1.0 / shortTermCount),
                                 (hesitatSum * 1.0 / shortTermCount),       (imag_actSum * 1.0 / shortTermCount),
                                 (extr_statSum * 1.0 / shortTermCount),     (uncertSum * 1.0 / shortTermCount),  
                                 (brainpowSum * 1.0 / shortTermCount),      (max_volSum * 1.0 / shortTermCount),
                                 (voice_energSum * 1.0 / shortTermCount)};

            double[] Prob = Model.distributionForInstance(TestData);

            return HigherProbability(Prob);
        }
        
        private void ProcessBuffer(ref Array inpBuf,
                                   ref int segmentID,
                                   ref int cEndPosSec,
                                   ref int cStartPosSec)
        //private void ProcessBuffer(ref int segmentID,
        //                       ref int cEndPosSec,
        //                       ref int cStartPosSec)
        {
            /* Read the initial time. */
            startTime = DateTime.Now;

            
            
            if (emoValsArray == null)
            {
                Console.WriteLine("ENTERED emovalsarray");
                emoValsArray = new short[21];
            }

            if (testbuf == null)
            {
                Console.WriteLine("ENTERED testbuf");
                testbuf = new short[22051];
            }
            bufSize = (short)(bufferSize - 1);
            emoValsArray = new short[21];
            aIres = "";
            bStr = "";
            testbuf = new short[22051];
            testBufLeng = 22050;
            brderS = -1.0;


            Array emoValsArray2 = new short[21];

            processBufferResult = nmsCOMcallee.nmsProcessBuffer( ref inpBuf,
                                                                    ref bufSize,
                                                                    ref emoValsArray2,
                                                                    ref aIres,
                                                                    ref bStr,
                                                                    ref testbuf,
                                                                    ref testBufLeng,
                                                                    ref brderS);
            
           

            cEndPosSec += 2;
            /* If Analysis is ready */
            Console.WriteLine("Is analysis ready? .... {0}", processBufferResult); 
            if (processBufferResult == NMS_PROCESS_ANALYSISREADY && bStr != null && aIres != null)
            {
                silenceCount = 0;
                emoVals = CopyValuesFromEmoArrayIntoEmotionsStructure(emoValsArray2, aIres);
                
                //csv
                //string newLine = CopyValuesFromEmoArrayIntoString(emoValsArray, aIres);
                // Create csv file to write it in
                //File.AppendAllText(resultsPath, newLine);

                //add values to the sum array
                CopyValuesFromEmoArrayIntoEmotionSums(emoValsArray2, aIres);
                successSegNum++;
                /* Store values of the curret buffer */
                energysum += emoVals.Energy;
                contentsum += emoVals.content;
                upsetSum += emoVals.upset;
                angrySum += emoVals.angry;
                stressSum += emoVals.stress;
                concentrationSum += emoVals.concentration_level;
                intenThinkSum += emoVals.intensive_thinking;
                safsum += emoVals.saf;
                excitSum += emoVals.excitement;
                atmossum += emoVals.Atmos;
                emocogsum += emoVals.EmoCogRatio;
                embarrasSum += emoVals.embarrassment;
                hesitatSum += emoVals.hesitation;
                imag_actSum += emoVals.imagination_activity;
                extr_statSum += emoVals.extremeState;
                uncertSum += emoVals.uncertainty;
                brainpowSum += emoVals.BrainPower;
                max_volSum += emoVals.maxAmpVol;
                voice_energSum += emoVals.VoiceEnergy;

                shortTermCount++;

                nmsCOMcallee.nmsQA_CollectAgentScoreData();

                if (segmentID >= lioNetResultsCache.Count)
                {
                    for (int i = 0; i <= 100; i++)
                        lioNetResultsCache.Add(string.Empty);
                }

                lioNetResultsCache[segmentID] = bStr;

                nmsCOMcallee.nmsQA_Logdata(ref segmentID, ref cStartPosSec, ref cEndPosSec);
                nmsCOMcallee.nmsSD_LogData();
                nmsCOMcallee.nmsCollectProfiler();
                cStartPosSec = 0;
                segmentID ++;

                /* Read the time. */
                stopTime = DateTime.Now;
                ProcDuration += stopTime - startTime;
            }
            /* Voice Detected */
            else if (processBufferResult == NMS_PROCESS_VOICEDETECTED && count < countNum && (cStartPosSec == 0))
            {
                ncount += 0.5;
                cEndPosSec -= 2;
                cStartPosSec = cEndPosSec;
            }
            /* The QA5Core fail to identify the buffer */
            else if (processBufferResult == -1 && count < countNum)
            {
                ncount += 0.5;
                cStartPosSec = 0;
            }
            /* Silence Detected*/
            else if (processBufferResult == NMS_PROCESS_SILENCE && count < countNum)
            {
                cStartPosSec = 0;
                silenceCount++;
            }
            /* Reset silenceCount if silence keeps being detected */
            if (shortTermCount == 0 && silenceCount == 2) silenceCount = 0;
            /* Return the Dominant Emotion after two non sequential silences */
            if (silenceCount == 2 && processBufferResult == NMS_PROCESS_SILENCE)
            {
                
                /* Dominant Human Emotion */
                dominantTypeOnly = ClassifierLogisticModel();
                //dominantTypeOnly = KNN_EmotionModel();
                
                /* Store reults */
                if (dominantTypeOnly == 1)
                    emt[0]++;
                else if (dominantTypeOnly == 2)
                    emt[1]++;
                else if (dominantTypeOnly == 3)
                    emt[2]++;
                else if (dominantTypeOnly == 4)
                    emt[3]++;
                else if (dominantTypeOnly == 5)
                    emt[4]++;
                else if (dominantTypeOnly == 6)
                    emt[5]++;
                /* Reset all the stored values */
                energysum = 0;
                contentsum = 0;
                upsetSum = 0;
                angrySum = 0;
                stressSum = 0;
                concentrationSum = 0;
                intenThinkSum = 0;
                safsum = 0;
                excitSum = 0;
                atmossum = 0;
                emocogsum = 0;
                embarrasSum = 0;
                hesitatSum = 0;
                imag_actSum = 0;
                extr_statSum = 0;
                uncertSum = 0;
                brainpowSum = 0;
                max_volSum = 0;
                voice_energSum = 0;
                shortTermCount = 0;

                /* Write file to send emotion to main controller */
                TextWriter UpdateHA = new StreamWriter("HumanEmo.txt");
                UpdateHA.Write(dominantTypeOnly);
                UpdateHA.Close();
                              
                /* Read the end time */
                DateTime stoptime = DateTime.Now;

                /* The processing duration*/
                Console.Write("Processing Time: "); Console.WriteLine(ProcDuration);
                ProcDuration = TimeSpan.Zero;

                /* Append to history files */
                HA = File.AppendText("HumanAffectHistory.txt");
                int min1 = Math.Abs((int)(SecCount-ncount) / 60),
                    min2 = Math.Abs((int)(SecCount) / 60),
                    sec1 = Math.Abs((int)(SecCount-ncount) % 60),
                    sec2 = Math.Abs((int)(SecCount) % 60);
                string  smin1 = String.Format("{0:D2}",min1),
                        smin2 = String.Format("{0:D2}",min2),
                        ssec1 = String.Format("{0:D2}",sec1),
                        ssec2 = String.Format("{0:D2}",sec2);
                
                HA.Write(smin1); HA.Write(":"); HA.Write(ssec1);
                HA.Write("~");
                HA.Write(smin2); HA.Write(":"); HA.Write(ssec2);
                HA.Write("   ");
                HA.WriteLine(dominantTypeOnly);
                HA.Close();
                
                /* Show in cmd window */
                Console.Write("Time: {0:D2}:{1:D2}~{2:D2}:{3:D2}", min1, sec1, min2, sec2);
                Console.Write("Human Affect: "); Console.WriteLine(dominantTypeOnly);
                ncount = 0.0;
            }
        }       
        
        /* Copy values to Emotion Structure */
        private static EmotionResults CopyValuesFromEmoArrayIntoEmotionsStructure(Array EmoArr, string AIres)
        {
            return new EmotionResults
            {
                AIres = AIres,
                angry = (short)EmoArr.GetValue(0),
                Atmos = (short)EmoArr.GetValue(1),
                concentration_level = (short)EmoArr.GetValue(2),
                embarrassment = (short)EmoArr.GetValue(3),
                excitement = (short)EmoArr.GetValue(4),
                hesitation = (short)EmoArr.GetValue(5),
                imagination_activity = (short)EmoArr.GetValue(6),
                intensive_thinking = (short)EmoArr.GetValue(7),
                content = (short)EmoArr.GetValue(8),
                saf = (short)EmoArr.GetValue(9),
                upset = (short)EmoArr.GetValue(10),
                extremeState = (short)EmoArr.GetValue(11),
                stress = (short)EmoArr.GetValue(12),
                uncertainty = (short)EmoArr.GetValue(13),
                Energy = (short)EmoArr.GetValue(14),
                BrainPower = (short)EmoArr.GetValue(15),
                EmoCogRatio = (short)EmoArr.GetValue(0x10),
                maxAmpVol = (short)EmoArr.GetValue(0x11),
                VoiceEnergy = (short)EmoArr.GetValue(18)
            };
        }

        /* Copy values to Emotion Structure */
        private static string CopyValuesFromEmoArrayIntoString(Array EmoArr, string AIres)
        {
            StringBuilder oneLine = new StringBuilder();

            oneLine.Append(audioFileName);
            oneLine.Append(",");

            for (int i = 0; i <= 18; i++)
            {
                oneLine.Append(EmoArr.GetValue(i));
                oneLine.Append(",");
            }

            oneLine.Append(Environment.NewLine);
            return oneLine.ToString();
        }

        private static void CopyValuesFromEmoArrayIntoEmotionSums(Array EmoArr, string AIres)
        {
            for (int i = 0; i <= 18; i++)
            {
                emotionSums[i] += (short) EmoArr.GetValue(i);
            }
        }


        /* Converts two bytes in short */
        static short bytesToShort(byte firstByte, byte secondByte)
        {
            int s = ((secondByte << 8) | firstByte);
            // This implicit conversion occurs fine since the int variable only has two bytes occupied.
            return (short)s;
        }
        /* Devide the .wave file in equally spaced bytes */
        private void openWav()
        {
            /* Directory of Files, folder and subfolder to be analyzed */
            //string path = @"C:\Users\ASB Workstation\Desktop\Audio files\Edited files\Final\";
            //string path = @"C:\Users\ASB Workstation\Desktop\Audio files\Separate files\Individual Files\Sorted by Video #";
            //string path = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\random samples";
            //resultsPath = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\results_randoms.csv";
            //string path = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\converted_8bit_11025hz";
            //resultsPath = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\results_500ms_8bit_5_11025hz.csv";
            //string path = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\emoDB_content_angry";
            //resultsPath = @"C:\Users\ASB Workstation\Desktop\emoDB\wav\results_content_angry_files_only.csv";
            string path = @"C:\Users\ASB Workstation\Desktop\Yuma\semaine_additional_12102015\sad_16bit_11025hz";
            resultsPath = @"C:\Users\ASB Workstation\Desktop\Yuma\semaine_additional_12102015\sad_16bit_11025hz_1s.csv";

            pathName = path;
            
            if(File.Exists(path)) 
            {
                // This path is a file
                ProcessFile(path); 
            }               
            else if(Directory.Exists(path)) 
            {
                // This path is a directory
                ProcessDirectory(path);
                HA = File.AppendText("HumanAffectHistory.txt");
                HA.Write("##### Processing Files in the Directory #####"); HA.Write("\n"); HA.WriteLine(path);
                HA.Close();
            }
            else 
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }        
            
        }
        public void ProcessFile(string path) 
        {
                /* File name */
                string Filename = Path.GetFileName(path);
                //csv
                audioFileName = Filename;
                if (!File.Exists(resultsPath))
                {

                    string header = "Filename,angry,Atmos,concentration_level,embarrassment," +
                                    "excitement,hesitation,imagination_activity,intensive_thinking," +
                                    "content,saf,upset,extremeState,stress,uncertainty,Energy," +
                                    "BrainPower,EmoCogRatio,maxAmpVol,VoiceEnergy" + Environment.NewLine;
                    File.WriteAllText(resultsPath, header);
                }
                /* Append to history files */
                Console.WriteLine("-> Analyzing file {0}", Filename);
                HA = File.AppendText("HumanAffectHistory.txt");
                HA.Write("-> File  "); HA.Write(Filename); HA.WriteLine(" ");
                HA.Close();

                /* Read all bytes from each wave file */
                byte[] wav = File.ReadAllBytes(path);
                /* OBS.: QA5.dll only recognizesa mono files with
                 * 6, 8 or 11KHz samples rates in 8 or 16 bit depth

                /* Get past all the other sub chunks to get to the data */
                int pos = 12;   // First Subchunk ID from 12 to 16
                
                //mono or stereo
                Console.WriteLine("Mono or stereo: {0}", wav[22]);

                //sample rate

                string h1 = wav[24].ToString("X");
                string h2 = wav[25].ToString("X");
                string h3 = wav[26].ToString("X");
                string h4 = wav[27].ToString("X");
                int samplingRate = int.Parse(h4 + h3 + h2 + h1, System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine("Sampling rate: {0}", samplingRate);
                //Console.WriteLine("Sampling rate: {0}", wav[27]*Math.Pow(16, 3) + wav[26]*Math.Pow(16, 2) + wav[25]*16 + wav[24]);

                // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
                while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
                {
                    pos += 4;
                    int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                    pos += 4 + chunkSize;
                }
                Console.WriteLine("We are at: {0}{1}{2}{3}", wav[pos],wav[pos+1],wav[pos+2],wav[pos+3]);

                pos += 8;

                // Now [pos] is positioned in the start of actual sound data.
                int samples = 0;
                int i = 0;
                //if mono
                if (wav[22] == 1)
                {
                    samples = wav.Length - pos;
                    left = new short[samples];
                    while (pos < wav.Length - 1)
                    {
                        left[i] = bytesToShort(wav[pos], wav[pos + 1]);
                        pos += 1;
                        i++;
                    }
                }
                //or if stereo
                else if (wav[22] == 2)
                {
                    samples = (wav.Length - pos) / 2;
                    left = new short[samples];
                    
                    while (pos < wav.Length - 1)
                    {
                        left[i] = bytesToShort(wav[pos], wav[pos + 1]);
                        pos += 2;
                        i++;
                    }
                }
                
                /* Bits per sample */
                h1 = wav[34].ToString("X").PadLeft(2, '0');
                h2 = wav[35].ToString("X").PadLeft(2, '0');
                Console.WriteLine("Bits per sample: "+h1+h2);

                /* Buffer Size */
                //bufferSize = 5512; //Number of bytes which correspond to half a seconds in the inpBuf array
                h1 = wav[28].ToString("X");
                h2 = wav[29].ToString("X");
                h3 = wav[30].ToString("X");
                h4 = wav[31].ToString("X");
                int byteRate = int.Parse(h4 + h3 + h2 + h1, System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine("Byte rate: {0}", byteRate);

                //bufferSize = (short)(byteRate * 1); //look at 2 sec segments
                bufferSize = (short) (0.02 * samplingRate);
                //bufferSize = (short)(1 * samplingRate);
                Console.WriteLine("Size of the buffer: {0}", bufferSize);
                short[] HoldAr = new short[bufferSize];
                Array inpBuf = new short[bufferSize];
                short[] inpBuf2 = new short[bufferSize];
                i = 0;
                int segmentNum = 1;
                int j;
                emotionSums = new int[19];
                successSegNum = 0;
                while ((i < samples))
                {
                    for (j = 0; (j < bufferSize) && (i != samples); j++)
                    {
                        HoldAr[j] = left[i];
                        i++;
                    }
                    /*
                    if (segmentNum == 1)
                    {
                        //inpBuf = HoldAr;
                        //Array.Copy(HoldAr, inpBuf2, bufferSize);
                        inpBuf2 = (short[])HoldAr.Clone();
                    }

                    //Array.Copy(inpBuf2, inpBuf, bufferSize);
                    inpBuf = (short[])inpBuf2.Clone(); 
                    */
                    inpBuf = (short[])HoldAr.Clone();

                    //Classifing function
                    Console.WriteLine("Segment #: {0}", segmentNum);
                    //ProcessBuffer(ref inpBuf, ref segmentID, ref cEndPosSec, ref cStartPosSec);
                    ProcessBuffer(ref inpBuf, ref segmentID, ref cEndPosSec, ref cStartPosSec);
                    segmentNum++;
                    SecCount += 2.0;
                }
                
                SecCount = 0;

                //write avg emotion values of file
                if (successSegNum > 0)
                {
                    writeEmotionAvgToFile();
                }
                Console.WriteLine("-> End of the file {0} yuma", Filename);
                Console.WriteLine();
            }

        public void writeEmotionAvgToFile()
        {
            StringBuilder oneLine = new StringBuilder();

            oneLine.Append(audioFileName);
            oneLine.Append(",");

            for (int i = 0; i <= 18; i++)
            {
                oneLine.Append((int)(emotionSums[i]/successSegNum));
                oneLine.Append(",");
            }

            oneLine.Append(Environment.NewLine);
            File.AppendAllText(resultsPath, oneLine.ToString());
        }
        public void ProcessDirectory(string targetDirectory) 
        {
            // Process the list of files found in the directory. 
            string [] fileEntries = Directory.GetFiles(targetDirectory);
            //initialize csv
            csv = new StringBuilder();

            foreach(string fileName in fileEntries)
                ProcessFile(fileName);
            // Recurse into subdirectories of this directory. 
            string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                //Directory Name
                Console.WriteLine("Analyzing Directory : {0}", Path.GetDirectoryName(subdirectory));
                ProcessDirectory(subdirectory);
            }
        }
      }
}