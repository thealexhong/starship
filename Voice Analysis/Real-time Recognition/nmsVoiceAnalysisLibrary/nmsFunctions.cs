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
        void Start();
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
        private short waveOutSamplesPerSecond;
        private nmsLioNetV6 nmsLioNet { get; set; }
        private readonly ManualResetEvent waitForPlaybackFinished = new ManualResetEvent(true);
        
        /* Wave Lib variables declaratioin */
        private byte[] m_PlayBuffer;
        private byte[] m_RecBuffer;
        private WaveLib.WaveOutPlayer m_Player;
        private WaveLib.WaveInRecorder m_Recorder;
        private WaveLib.FifoStream A_Fifo = new WaveLib.FifoStream();
        private WaveLib.FifoStream m_Fifo = new WaveLib.FifoStream();
        private int newRecordedTime = 0;
        private int oldRecordedTime = 0;
        private int sampleRate = 11025;
        private int definition = 16;
        private int segmentLength = 2;
        
        /* Buffer and Process Control variables declaration*/
        private int cStartPosSec = 0;
        private int cEndPosSec = 0;
        private int segmentID = 0;
        private Array inpBuf;

        /* Historic .txt declaration */
        private StreamWriter log;
        private StreamWriter v_arff;
        private StreamWriter a_arff;
        private StreamWriter voiceOutput;
        private string valence = "-3";
        private string arousal = "-3";
        
        /* Main Variables and Parameters */
        private DateTime gamestart = DateTime.Now;
        private Array emoValsArray = null;
        private EmotionResults emoVals;
        //private int dominantTypeOnly;
        private double brderS = -1.0;
        private string aIres = "";
        private string bStr = "";
        private Array testbuf = null;
        private int testBufLeng = 0;
        private int count = 0;
        private int processBufferResult;
        private short bufSize;
        private int countNum = Int32.MaxValue; //continous running
        //private int countNum = 80;  //Program running time
        /* Counters */
        private int silenceCount = 0;
        private int shortTermCount = 0;

        /* Read the initial time. */
        TimeSpan ProcDuration = TimeSpan.Zero;
        DateTime startTime;
        DateTime stopTime;
         
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
            short lengthOfSegmentInSeconds;
            short calibrationType = 5;
            /* Configure the background noise. Default 1000 */
            short backgroundLevel = 1000;
            waveOutSamplesPerSecond = 11025;
            lengthOfSegmentInSeconds = 2;
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

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.White;

            WaveLib.WaveFormat fmt = new WaveLib.WaveFormat(sampleRate, definition, 1);
            m_Recorder = new WaveLib.WaveInRecorder(-1, fmt, sampleRate*segmentLength*(definition/8), 1, new WaveLib.BufferDoneEventHandler(DataArrived));
            
            log = File.CreateText("LogFeatureVector.txt");
            log.Close();

            v_arff = File.CreateText("FeatureVectorValence.arff");
            v_arff.WriteLine("@relation valence\n");
            v_arff.WriteLine("@attribute Angry numeric");
            v_arff.WriteLine("@attribute ConcentrationLevel numeric");
            v_arff.WriteLine("@attribute Embarrassment numeric");
            v_arff.WriteLine("@attribute Excitement numeric");
            v_arff.WriteLine("@attribute Hesitation numeric");
            v_arff.WriteLine("@attribute ImaginationActivity numeric");
            v_arff.WriteLine("@attribute IntensiveThinking numeric");
            v_arff.WriteLine("@attribute Content numeric");
            v_arff.WriteLine("@attribute SAF numeric");
            v_arff.WriteLine("@attribute Upset numeric");
            v_arff.WriteLine("@attribute ExtremeState numeric");
            v_arff.WriteLine("@attribute Stress numeric");
            v_arff.WriteLine("@attribute Uncertainty numeric");
            v_arff.WriteLine("@attribute Energy numeric");
            v_arff.WriteLine("@attribute BrainPower numeric");
            v_arff.WriteLine("@attribute EmoCogRatio numeric");
            v_arff.WriteLine("@attribute MaxAmpVol numeric");
            v_arff.WriteLine("@attribute VoiceEnergy numeric");
            v_arff.WriteLine("@attribute valence {-2, -1, 0, 1, 2}\n");
            v_arff.WriteLine("@data\n");
            v_arff.Close();

            a_arff = File.CreateText("FeatureVectorArousal.arff");
            a_arff.WriteLine("@relation arousal\n");
            a_arff.WriteLine("@attribute Angry numeric");
            a_arff.WriteLine("@attribute ConcentrationLevel numeric");
            a_arff.WriteLine("@attribute Embarrassment numeric");
            a_arff.WriteLine("@attribute Excitement numeric");
            a_arff.WriteLine("@attribute Hesitation numeric");
            a_arff.WriteLine("@attribute ImaginationActivity numeric");
            a_arff.WriteLine("@attribute IntensiveThinking numeric");
            a_arff.WriteLine("@attribute Content numeric");
            a_arff.WriteLine("@attribute SAF numeric");
            a_arff.WriteLine("@attribute Upset numeric");
            a_arff.WriteLine("@attribute ExtremeState numeric");
            a_arff.WriteLine("@attribute Stress numeric");
            a_arff.WriteLine("@attribute Uncertainty numeric");
            a_arff.WriteLine("@attribute Energy numeric");
            a_arff.WriteLine("@attribute BrainPower numeric");
            a_arff.WriteLine("@attribute EmoCogRatio numeric");
            a_arff.WriteLine("@attribute MaxAmpVol numeric");
            a_arff.WriteLine("@attribute VoiceEnergy numeric");
            a_arff.WriteLine("@attribute arousal {-2, -1, 0, 1, 2}\n");
            a_arff.WriteLine("@data\n");
            a_arff.Close();

            voiceOutput = File.CreateText("..\\..\\voiceOutput.txt");
            voiceOutput.Close();

            Console.WriteLine("Recording...");
        }

        private void Filler(IntPtr data, int size)
        {
            if (m_PlayBuffer == null || m_PlayBuffer.Length < size)
                m_PlayBuffer = new byte[size];
            if (m_Fifo.Length >= size)
                m_Fifo.Read(m_PlayBuffer, 0, size);
            else
                for (int i = 0; i < m_PlayBuffer.Length; i++)
                    m_PlayBuffer[i] = 0;
            System.Runtime.InteropServices.Marshal.Copy(m_PlayBuffer, 0, data, size);
        }
     
        private void DataArrived(IntPtr data, int size)
        {
            oldRecordedTime = newRecordedTime;
            newRecordedTime = Int32.Parse(DateTime.Now.ToString("HHmmssfff"));
            Console.WriteLine("Time Elapsed: " + (newRecordedTime - oldRecordedTime).ToString());
            
            if (m_RecBuffer == null || m_RecBuffer.Length < size)
            {
                inpBuf = new short[sampleRate*segmentLength];
                m_RecBuffer = new byte[size];
            }
            System.Runtime.InteropServices.Marshal.Copy(data, m_RecBuffer, 0, size);
            m_Fifo.Write(m_RecBuffer, 0, m_RecBuffer.Length);
            /* Converts bytes array do short array */
            MiniRead(m_RecBuffer, (short[])inpBuf, size);
            bufferSize = (short) (sampleRate*segmentLength);
            /* Buffer Analyser */
            ProcessBuffer(ref inpBuf, ref segmentID, ref cEndPosSec, ref cStartPosSec);
        }

        private short bytesToShort(byte firstByte, byte secondByte)
        {
            int s = ((secondByte << 8) | firstByte);
            // This implicit conversion occurs fine since the int variable only has two bytes occupied.
            return (short)s;
        }

        private void MiniRead(byte[] m_RecBuf, short[] soundBuf, int size1)
        {
            if (definition == 8)
            {
                for (int i = 0; i < size1; i++)
                    soundBuf[i] = (short)((m_RecBuf[i] - 127) * 127);
            }
            else if (definition == 16)
            {
                int i = 0;
                int j = 0;
                while (i < size1)
                {
                    soundBuf[j] = bytesToShort(m_RecBuf[i], m_RecBuf[i + 1]);
                    i += 2;
                    j++;
                }
            }
        }  

        private void ProcessBuffer(ref Array inpBuf,
                                   ref int segmentID,
                                   ref int cEndPosSec,
                                   ref int cStartPosSec)
        {
            /* Read the initial time. */
            startTime = DateTime.Now;
            
            //bufSize = (short)(bufferSize - 1);
            bufSize = bufferSize;
            Console.WriteLine("Size of the buffer is: " + bufSize.ToString());
            processBufferResult = nmsCOMcallee.nmsProcessBuffer(ref inpBuf,
                                                                    ref bufSize,
                                                                    ref emoValsArray,
                                                                    ref aIres,
                                                                    ref bStr,
                                                                    ref testbuf,
                                                                    ref testBufLeng,
                                                                    ref brderS);

            cEndPosSec += 2;
            /* If Analysis is ready */
            Console.WriteLine("Sound captured and processed");
            Console.WriteLine(processBufferResult);
            if (processBufferResult == NMS_PROCESS_ANALYSISREADY)
            {
                silenceCount = 0;        
                emoVals = CopyValuesFromEmoArrayIntoEmotionsStructure(emoValsArray, aIres);
                String fvStr = CopyValuesFromEmoArrayIntoString(emoValsArray, aIres);
                Console.WriteLine("Features extracted!");
                Console.WriteLine(fvStr);

                string[] lines = System.IO.File.ReadAllLines("FeatureVectorValence.arff");
                lines[23] = fvStr;
                System.IO.File.WriteAllLines("FeatureVectorValence.arff", lines);
                lines = System.IO.File.ReadAllLines("FeatureVectorArousal.arff");
                lines[23] = fvStr;
                System.IO.File.WriteAllLines("FeatureVectorArousal.arff", lines);



                //Run command prompt command
                // Start the child process.
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "classify.bat";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                //System.Console.WriteLine(output);
                string[] tokens = output.Split(':');
                //for(int i=0; i<tokens.Length; i++)
                //{
                //    Console.WriteLine("#"+i+"-"+tokens[i]);
                //}

                //Parse out the valence and arousal
                if(tokens[3][0] == '-')
                    valence = "-" + tokens[3][1].ToString();
                else
                    valence = tokens[3][0].ToString();
                if (tokens[6][0] == '-')
                    arousal = "-" + tokens[6][1].ToString();
                else
                    arousal = tokens[6][0].ToString();
                
                Console.WriteLine("Valence: " + valence);
                Console.WriteLine("Arousal: " + arousal);

                log = File.AppendText("LogFeatureVector.txt");
                log.WriteLine(newRecordedTime + "\t" + fvStr + "\t" + valence + "\t" + arousal + "\t");
                log.Close();
                
                voiceOutput = File.AppendText("..\\..\\voiceOutput.txt");
                voiceOutput.WriteLine(valence.ToString() + "," + arousal.ToString());
                voiceOutput.Close();

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
                
                cEndPosSec -= 2;
                cStartPosSec = cEndPosSec;
            }
            /* The QA5Core fail to identify the buffer */
            else if (processBufferResult == -1 && count < countNum)
            {
                cStartPosSec = 0;
            }
            /* Silence Detected*/
            else if (processBufferResult == NMS_PROCESS_SILENCE && count < countNum)
            {
                cStartPosSec = 0;
                silenceCount++;
            }
            /* Reset silenceCount if no voice was detected */
            if (shortTermCount == 0 && silenceCount == 2) silenceCount = 0;
            /* Return the Dominant Emotion after two non sequential silences */
            if (silenceCount == 2 && processBufferResult == NMS_PROCESS_SILENCE)
            {
                /* Read the end time */
                DateTime stoptime = DateTime.Now;

                /* The processing duration*/
                Console.Write("Processing Time: "); Console.WriteLine(ProcDuration);
                ProcDuration = TimeSpan.Zero;
                
            }
            /* If Program is running with determined time */
            if (count == countNum)
            {
                cEndPosSec = 0;
                cStartPosSec = 0;
                /* Stop audio output */
                if ((m_Player != null)) 
                    try
                    {
                        m_Player.Dispose();
                    }
                    finally
                    {
                        m_Player = null;
                    }
                /* Stop audio input */
                if ((m_Recorder != null))
                    try
                    {
                        m_Recorder.Dispose();
                    }
                    finally
                    {
                        m_Recorder = null;
                    }
                /* Clear All Pending Data */
                m_Fifo.Flush(); 
            }
            /* Running during defined time*/
            count++;
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
            oneLine.Append(EmoArr.GetValue(0)); //Angry
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(2)); //ConcentrationLevel
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(3)); //Embarrassment
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(4)); //Excitement
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(5)); //Hesitation
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(6)); //ImaginationActivity
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(7)); //IntensiveThinking
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(8)); //Content
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(9)); //SAF
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(10)); //Upset
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(11)); //ExtremeState
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(12)); //Stress
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(13)); //Uncertainty
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(14)); //Energy
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(15)); //BrainPower
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(16)); //EmoCogRatio
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(17)); //MaxAmpVol
            oneLine.Append(",");
            oneLine.Append(EmoArr.GetValue(18)); //VoiceEnergy
            oneLine.Append(",?");
            return oneLine.ToString();
        }         
    }
}