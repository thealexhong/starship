#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QA5_Trainer.CSharp.Interfaces;
using QA5COM;

#endregion

namespace QA5_Trainer.CSharp.UI
{
    public partial class DemoForm : Form, IQA5UI
    {
        public DemoForm()
        {
            InitializeComponent();
            PrepareCallsGrid();
        }

        #region IQA5UI Members

        public event EventHandler<FileSelectedArgs> FileSelected;
        public event EventHandler<SegmentClickedArgs> SegmentClicked;
        public event EventHandler<ColumnClickArgs> SegmentsListColumnClicked;
        public event EventHandler<NewSegmentEmotionArgs> NewSegmentEmotionAdded;
        public event EventHandler<NewCallEmotionalArgs> NewCallEmotionAdded;
        public event EventHandler<ClassifySegmentArgs> ClassifySegment;
        public event EventHandler<LioNetForgetArgs> LioNetForget;
        public event EventHandler<ClassifyCallArgs> ClassifyCall;
        public event EventHandler<ForgetCallClassificationArgs> ForgetCallClassification;
        public event EventHandler UiIsGoingToClose;
        public event EventHandler RetrieveLicenseDetails;
        public event EventHandler<ApplyCountersResetCodeArgs> ApplyCountersResetCode;
        public event EventHandler UpdateBordersFile;
        public event EventHandler ShowBordersOnGraph;

        private bool NoRealTimeUpdates
        {
            get { return noRealTimeUiUpdates.Checked; }
        }

        public bool HighlightAngerTrendLevel
        {
            set { angerTrendLevel.ForeColor = value ? Color.Red : Color.Black; }
        }

        public double MaxAngerTrandLevel
        {
            get { return double.Parse(maxAngerTrendLevel.Text); }
        }

        public string AngerTrendLevel
        {
            set { angerTrendLevel.Text = value; }
        }

        public bool AlarmIfStressTrendIsRaisingAndAbove
        {
            get { return alarmIfStressTrendIsRaisingAndAbove.Checked; }
        }

        public bool AlarmIfAngerTrendIsRaisingAndAbove
        {
            get { return alarmIfAngerTrendIsRaisingAndAbove.Checked; }
        }

        public bool HighlightStressTrendLevel
        {
            set { stressTrendLevel.ForeColor = value ? Color.Red : Color.Black; }
        }

        public double MaxStressTrendLevel
        {
            get { return double.Parse(maxStressTrendLevel.Text); }
        }

        public bool NotifyStressLevelIsRaising
        {
            set
            {
                if (value)
                {
                    stressLevelIsRaisingLabel.Visible = true;
                    stressLevelIsRaisingLabel.BringToFront();
                }
                else
                    stressLevelIsRaisingLabel.Visible = false;
            }
        }

        public string StressTrendLevel
        {
            set { stressTrendLevel.Text = value; }
        }

        public bool AlarmIfStressTrendIsLow
        {
            get { return alarmIfStressTrendIsLow.Checked; }
        }

        public bool HighlightEnergyTrendIsRaisingFor
        {
            set { energyTrendIsRaisingFor.ForeColor = value ? Color.Red : Color.Black; }
        }

        public bool HighlightEnergyLevelBelow
        {
            set { energyLevelBelow.ForeColor = value ? Color.Red : Color.Black; }
        }

        public string EnergyDifference
        {
            set { lbl_enrgyDif.Text = value; }
        }

        public string EnrgyHighSegments
        {
            set { lbl_enrgyHighSegs.Text = value; }
        }

        public string EnergyTrendIsRaisingFor
        {
            set { energyTrendIsRaisingFor.Text = value; }
        }

        public bool HighlightEnergyLevel
        {
            set { if (value) Shape2.BackColor = Color.Red; }
        }

        public bool NotifySpeakerIsTired
        {
            set
            {
                if (value)
                {
                    speakerIsTiredLabel.Visible = true;
                    speakerIsTiredLabel.BringToFront();
                }
                else
                    speakerIsTiredLabel.Visible = false;
            }
        }

        public string EnergyLevelBelow
        {
            set { energyLevelBelow.Text = value; }
        }

        public bool NotifyCallIsOutOfAcceptableLevels
        {
            set
            {
                if (value)
                {
                    callIsOutOfAcceptableLevelsLabel.Visible = true;
                    callIsOutOfAcceptableLevelsLabel.BringToFront();
                }
                else
                    callIsOutOfAcceptableLevelsLabel.Visible = false;
            }
        }

        public int LimitForEnergyTrendIsRaisingFor
        {
            get { return int.Parse(limitForEnergyTrendIsRaisingFor.Text); }
        }

        public double LimitForEnergyLevelBelow
        {
            get { return double.Parse(limitForEnergyLevelBelow.Text); }
        }

        public byte DataChannelNumber
        {
            get
            {
                return (byte) (dataChannelNumber.Checked ? 0 : 1);
            }
        }

        public void AddSegmentToList(int segCount, string bstring, int sPos, int fPos, EmotionResults emoVals,
                                     string onlineFlag,
                                     string comment)
        {
            if (InvokeRequired)
            {
                Invoke(new AddSegmentToListDelegate(AddSegmentToList),
                       new object[] {segCount, bstring, sPos, fPos, emoVals, onlineFlag, comment});
            }
            else
            {
                ListViewItem item = segmentsList.Items.Add(segCount.ToString());
                double position = (sPos)/100.0;
                item.SubItems.Add(position.ToString("F02"));
                position = (fPos)/100.0;
                item.SubItems.Add(position.ToString("F02"));
                item.SubItems.Add(emoVals.Energy.ToString());
                item.SubItems.Add(emoVals.content.ToString());
                item.SubItems.Add(emoVals.upset.ToString());
                item.SubItems.Add(emoVals.angry.ToString());
                item.SubItems.Add(emoVals.stress.ToString());
                item.SubItems.Add(emoVals.uncertainty.ToString());
                item.SubItems.Add(emoVals.excitement.ToString());
                item.SubItems.Add(emoVals.concentration_level.ToString());
                item.SubItems.Add(emoVals.EmoCogRatio.ToString());
                item.SubItems.Add(emoVals.hesitation.ToString());
                item.SubItems.Add(emoVals.BrainPower.ToString());
                item.SubItems.Add(emoVals.embarrassment.ToString());
                item.SubItems.Add(emoVals.intensive_thinking.ToString());
                item.SubItems.Add(emoVals.imagination_activity.ToString());
                item.SubItems.Add(emoVals.extremeState.ToString());
                item.SubItems.Add(emoVals.saf.ToString());
                item.SubItems.Add(emoVals.Atmos.ToString());
                item.SubItems.Add(onlineFlag);
                item.SubItems.Add(emoVals.AIres);
                item.SubItems.Add(emoVals.maxAmpVol.ToString());
                item.SubItems.Add(comment);
                item.SubItems.Add(bstring);

                segmentsList.Columns[0].Width = 60;
            }
        }

        public bool CreateSegmentAudioFiles()
        {
            return Ck_perVoice.Checked;
        }

        public void DrawData(double[] data, double min, double max, double avg, int scaleMin, int scaleMax, int column)
        {
            if (InvokeRequired)
            {
                Invoke(new DrawDataDelegate(DrawData),
                       new object[] {data, min, max, avg, scaleMin, scaleMax, column});
            }
            else
            {
                segmentStats.Text = string.Format("{3} (Min: {0}; Max: {1}; Avg: {2})", min, max, avg.ToString("F02"),
                                                  segmentsList.Columns[column].Text);

                var item = new ListViewItem(" ")
                               {
                                   BackColor = selectedColor.BackColor,
                                   ForeColor = Color.Black,
                                   UseItemStyleForSubItems = false
                               };
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, segmentStats.Text, Color.Black, Color.White,
                                                                   new Font("Times New Roman", 10f)));
                colorHistory.Items.Add(item);

                if (!useSameGraph.Checked)
                    graphView.Cls();

                double scaleHeight = max - min;
                graphView.ScaleHeight = scaleHeight + 1.0;
                graphView.ScaleWidth = data.Length - 1;

                graphView.MoveTo(0.0, ((float) ((scaleHeight + min) - data[0])));

                for (int i = 1; i < data.Length; i++)
                    graphView.LineTo(i, (float) ((scaleHeight + min) - data[i]),
                                     new object[] {selectedColor.BackColor});

                TrendAnalyzer analyzer = new TrendAnalyzerClass();
                int degree = 1;
                analyzer.set_Degree(ref degree);
                analyzer.Init();

                double newX = 0.0;
                double x = 0.0;
                foreach (double sample in data)
                {
                    analyzer.AddPoint(newX, sample);
                    analyzer.AddVal(ref x);
                    double trendValue = analyzer.AddVal(ref newX);
                    newX++;
                    graphView.Circle(newX, (graphView.ScaleHeight - trendValue) + min, 4.0, selectedColor.BackColor);
                }

                graphView.Refresh();
            }
        }

        public void DrawSegment(byte[] segmentData)
        {
            segmentStats.Text = string.Empty;
            graphView.Cls();
            graphView.ScaleHeight = 256.0;
            graphView.ScaleWidth = segmentData.Length;
            graphView.MoveTo(0.0, ((float) ((graphView.ScaleHeight/2.0) - segmentData[0])));
            for (int i = 1; i < segmentData.Length; i++)
            {
                graphView.LineTo(i, (0x7f - segmentData[i]), new object[] {selectedColor.BackColor});
            }
            graphView.Refresh();
        }

        public void DrawSegment(short[] segmentData)
        {
            segmentStats.Text = string.Empty;
            graphView.Cls();
            graphView.ScaleHeight = 65535.0;
            graphView.ScaleWidth = segmentData.Length;
            graphView.MoveTo(0.0, segmentData[0]);
            for (int i = 1; i < segmentData.Length; i++)
            {
                graphView.LineTo(i, (float) ((graphView.ScaleHeight/2.0) - segmentData[i]), Color.Green);
            }
            graphView.Refresh();
        }

        public int GetAnalysisType()
        {
            return (ckAnalysisType.Checked ? 1 : 0);
        }

        public short GetBackgroundLevel()
        {
            short backgroundLevel;
            if (!short.TryParse(txt_BG.Text, out backgroundLevel))
            {
                backgroundLevel = 0x3e8;
            }
            return backgroundLevel;
        }

        public short GetCalibrationType()
        {
            return (ckCalType.Checked ? ((short) 1) : ((short) 0));
        }

        public double[] GetSegemtsDataFromColumn(int column, out double min, out double max, out double avg)
        {
            var list = new List<double>();
            foreach (ListViewItem item in segmentsList.Items)
            {
                list.Add(Convert.ToDouble(item.SubItems[column].Text));
            }
            double mi = list[0];
            double ma = list[0];
            double a = 0.0;
            list.ForEach(delegate(double val)
                             {
                                 if (mi > val)
                                 {
                                     mi = val;
                                 }
                                 if (ma < val)
                                 {
                                     ma = val;
                                 }
                                 a += val;
                             });
            min = mi;
            max = ma;
            avg = a/(list.Count);
            return list.ToArray();
        }

        public bool IsOneSecondBufferUsed()
        {
            return !ckSegLength.Checked;
        }

        public bool IsSegmentOutOfLimits()
        {
            return (callIsOutOfAcceptableLevelsLabel.Visible || stressLevelIsRaisingLabel.Visible);
        }

        public bool NeedToPlayEachSegment()
        {
            return ck_PlayBackRealTime.Checked;
        }

        public void Reset()
        {
            if (InvokeRequired)
            {
                Invoke(new ResetDelegate(Reset));
            }
            else
            {
                baseProfileEmotional.Value = 0;
                baseProfileLogical.Value = 0;
                baseProfileHesitant.Value = 0;
                baseProfileStressed.Value = 0;
                baseProfileEnergetic.Value = 0;
                baseProfileThoughtful.Value = 0;

                currentProfileEmotional.Value = 0;
                currentProfileLogical.Value = 0;
                currentProfileHesitant.Value = 0;
                currentProfileStressed.Value = 0;
                currentProfileEnergetic.Value = 0;
                currentProfileThoughtful.Value = 0;

                nmsHSAhistoryBar1.Reset();
                nmsHSAhistoryBar1.colorAtmos = (Color.White);
                nmsHSAhistoryBar1.colorContent = (Color.Green);
                nmsHSAhistoryBar1.colorUpset = (Color.Red);

                nmsHSAhistoryBar2.Reset();
                nmsHSAhistoryBar2.colorAtmos = (Color.FromArgb(0xff, 0xff, 0));
                nmsHSAhistoryBar2.colorContent = (Color.Red);
                nmsHSAhistoryBar2.colorUpset = (Color.Green);

                nmsAShistoryBar1.Reset();
                nmsAShistoryBar1.colorAnger = (Color.Red);
                nmsAShistoryBar1.colorStress = (Color.FromArgb(0xff, 0xff, 0));

                PrepareSementsListGrid();
                PrepareEnvelopAndBordersGrid();
            }
        }

        private void PrepareCallsGrid()
        {
            string[] strArray =
                "File Name|Processing Time|Call Max Priority|Call Priority|Agent Priority|Lionet Analysis|Overall Borders Distance|Avg. Voice Energy|Stress%|Upset%|Angry%"
                    .Split(new[] {'|'});
            callsGrid.Items.Clear();
            callsGrid.Columns.Clear();
            foreach (string str in strArray)
            {
                string text = str.Trim();
                callsGrid.Columns.Add(text, -2, HorizontalAlignment.Right);
            }
        }

        private void PrepareEnvelopAndBordersGrid()
        {
            string[] rowNames =
                "MIN|MAX|AVRG|Norm MIN|Norm MAX|Total Dif. Borders|   Low Border|   High Border|Total Dif. Envelope|   Low Envelope|   High Envelope"
                    .Split(new[] {'|'});

            conversationEnvelopAndBorders.Items.Clear();
            conversationEnvelopAndBorders.Columns.Clear();

            conversationEnvelopAndBorders.Columns.Add("", 120, HorizontalAlignment.Left);

            for (int i = 0; i < 51; i++)
                conversationEnvelopAndBorders.Columns.Add(String.Format("(Param{0})", i + 1), 80,
                                                          HorizontalAlignment.Right);

            foreach (string rowName in rowNames)
            {
                var item = new ListViewItem(rowName)
                               {
                                   BackColor = DefaultBackColor,
                                   Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold),
                                   UseItemStyleForSubItems = false
                               };

                conversationEnvelopAndBorders.Items.Add(item);
            }
        }

        public void SetBGLevel(int level)
        {
            if (InvokeRequired)
            {
                Invoke(new SetIntDelegate(SetBGLevel), new object[] {level});
                return;
            }

            txt_BG.Text = level.ToString();
        }

        public void SetLionetUserDefinedEmotions(string[] emotions)
        {
            if (InvokeRequired)
            {
                Invoke(new SetStringCollectionDelegate(SetLionetUserDefinedEmotions), new object[] {emotions});
                return;
            }

            lioNetEmotionsList.Items.Clear();

            foreach (string str in emotions)
                lioNetEmotionsList.Items.Add(str);
        }

        public void SetUserDefinedSegmentEmotions(string[] segmentEmotions)
        {
            if (InvokeRequired)
            {
                Invoke(new SetStringCollectionDelegate(SetUserDefinedSegmentEmotions), new object[] {segmentEmotions});
                return;
            }

            gridUserDefinedSegmentEmotions.Items.Clear();

            foreach (string str in segmentEmotions)
                gridUserDefinedSegmentEmotions.Items.Add(str);
        }

        public void ShowMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new SetControlTextDelegate(ShowMessage), new object[] {message});
                return;
            }

            MessageBox.Show(message);
        }

        public void ShowLicenseScreen()
        {
            tabControl1.SelectedIndex = 3;
        }

        public void ShowProfile(short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel,
                                short energeticLevel, short thinkingLevel)
        {
            if (NoRealTimeUpdates)
                return;

            if (InvokeRequired)
            {
                Invoke(new ShowProfileDelegate(ShowProfile),
                       new object[]
                           {emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel});
            }
            else
            {
                baseProfileEmotional.Value = emoLevel;
                baseProfileLogical.Value = logicalLevel;
                baseProfileHesitant.Value = hasitantLevel;
                baseProfileStressed.Value = stressLevel;
                baseProfileEnergetic.Value = energeticLevel;
                baseProfileThoughtful.Value = thinkingLevel;
            }
        }

        public void UpdateCallPriority(int segmentID, int callPriority, int stressSegs, int angerSegs, int upsetSegs)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateCallPriorityDelegate(UpdateCallPriority),
                       new object[] {segmentID, callPriority, stressSegs, angerSegs, upsetSegs});
            }
            else
            {
                lblCallPriority.Text = string.Format("Call Priority: {0} & (T:{1}, S:{2}, A:{3}, U:{4})", callPriority,
                                                     segmentID, stressSegs, angerSegs, upsetSegs);

                if (ck_AutoStartPlay.Checked)
                    ck_PlayBackRealTime.CheckState = (callPriority > 50) ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        public void UpdateHistoryBars(EmotionResults emoVals)
        {
            if (NoRealTimeUpdates)
                return;

            if (InvokeRequired)
            {
                Invoke(new UpdateHistoryBarsDelegate(UpdateHistoryBars), new object[] {emoVals});
            }
            else
            {
                nmsHSAhistoryBar1.addNewValues(emoVals.content, emoVals.upset, emoVals.Atmos);
                nmsHSAhistoryBar2.addNewValues((short) (emoVals.EmoCogRatio/20), (short) (emoVals.BrainPower/100),
                                               (short) (emoVals.Energy*2));
                nmsAShistoryBar1.addNewValues((short) (emoVals.stress*2), (short) (emoVals.angry*2));

                Application.DoEvents();
            }
        }

        public void UpdateProfile(short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel,
                                  short energeticLevel, short thinkingLevel)
        {
            if (NoRealTimeUpdates)
                return;

            if (InvokeRequired)
            {
                Invoke(new UpdateProfileDelegate(UpdateProfile),
                       new object[]
                           {emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel});
            }
            else
            {
                currentProfileEmotional.Value = emoLevel;
                currentProfileLogical.Value = logicalLevel;
                currentProfileHesitant.Value = hasitantLevel;
                currentProfileStressed.Value = stressLevel;
                currentProfileEnergetic.Value = energeticLevel;
                currentProfileThoughtful.Value = thinkingLevel;
            }
        }

        public string GetCurrentAiResultForSegment(int segmentId)
        {
            return segmentsList.Items[segmentId].SubItems[21].Text;
        }

        public void SetAiResultForSegment(int segmentId, string aiResult)
        {
            segmentsList.Items[segmentId].SubItems[21].Text = aiResult;
        }

        public void FreezTables()
        {
            if (InvokeRequired)
            {
                Invoke(new FreezTablesDelegate(FreezTables));
                return;
            }

            UseWaitCursor = true;

            segmentsList.Visible = false;
            segmentsListCover.Visible = true;
        }

        public void UnFreezTables()
        {
            if (InvokeRequired)
            {
                Invoke(new UnFreezTablesDelegate(UnFreezTables));
                return;
            }

            UseWaitCursor = false;

            segmentsList.Visible = true;
            segmentsListCover.Visible = false;
        }

        public void SetCallLioNetAnalysis(string lioNetCallAnalysis)
        {
            if (InvokeRequired)
            {
                Invoke(new SetControlTextDelegate(SetCallLioNetAnalysis), new object[] {lioNetCallAnalysis});
                return;
            }

            lioNetAnalysis.Text = lioNetCallAnalysis;
        }

        public void SetCallEmotionalSignature(string signature)
        {
            if (InvokeRequired)
            {
                Invoke(new SetControlTextDelegate(SetCallEmotionalSignature), new object[] {signature});
                return;
            }

            callEmotionSignature.Text = signature;
        }

        public void SetCallProfile(string callProfile)
        {
            if (InvokeRequired)
            {
                Invoke(new SetControlTextDelegate(SetCallProfile), new object[] {callProfile});
                return;
            }

            qaReport.Text = callProfile;
        }

        public void SetNetAccuracy(string netAccuracy)
        {
            if (InvokeRequired)
            {
                Invoke(new SetControlTextDelegate(SetNetAccuracy), new object[] {netAccuracy});
                return;
            }

            lioNetAccuracy.Text = netAccuracy;
        }

        public void SetLicenseDetails(string sysID, int callCounter, short postsLicensed, int runningProcesses,
                                      string coreVersion)
        {
            if (InvokeRequired)
            {
                Invoke(new SetLicenseDetailsDelegate(SetLicenseDetails),
                       new object[] {sysID, callCounter, postsLicensed, runningProcesses, coreVersion});
                return;
            }

            systemId.Text = sysID;
            currentCallsCounter.Text = callCounter.ToString();
            licensedPosts.Text = postsLicensed.ToString();
            numberOfProcessesRunning.Text = runningProcesses.ToString();
            qA5CoreVersion.Text = coreVersion;
        }

        public void SetConversationBordersData(Array callBordersData)
        {
            var font = new Font("Microsoft Sans Serif", 8.25F);
            for (int i = 0; i < 11; i++)
            {
                ListViewItem item = conversationEnvelopAndBorders.Items[i];
                for (int j = 0; j < 51; j++)
                    item.SubItems.Add(((double) callBordersData.GetValue(i, j)).ToString("F00"), Color.Black,
                                      Color.White, font);
            }
        }

        public void SetEnvelopAndBordersReport(string report)
        {
            envelopAndBordersReport.Text = report;
        }

        public void DrawBorders(double[,] callBordersData, EnvelopData[] envelopData)
        {
            graphView.Cls();
            graphView.ScaleHeight = 130;
            graphView.ScaleWidth = 51;
            graphView.ForeColor = Color.White;
            graphView.Line(0, 115, 53, 115);
            graphView.Line(0, 15, 53, 15);

            for (int i = 0; i <= 51; i++)
            {
                double parmMin = callBordersData[0, i];
                double parmMax = callBordersData[1, i];
                double parmMinN = callBordersData[2, i];
                double parmMaxN = callBordersData[3, i];
                double parmAvrg = callBordersData[4, i];

                double range = envelopData[i].HighBorder - envelopData[i].LowBorder;
                if (range > 0)
                {
                    graphView.ForeColor = Color.Red;
                    var tmpD = (float) ((parmMin - envelopData[i].LowBorder)/range*100 + 15);
                    var tmpD2 = (float) ((parmMax - envelopData[i].LowBorder)/range*100 + 15);
                    graphView.Box(i, tmpD, i + 1, tmpD2);

                    graphView.ForeColor = Color.Green;
                    tmpD = (float) ((parmMinN - envelopData[i].LowBorder)/range*100 + 15);
                    tmpD2 = (float) ((parmMaxN - envelopData[i].LowBorder)/range*100 + 15);
                    graphView.Box(i, tmpD, i + 1, tmpD2);

                    graphView.ForeColor = Color.Blue;
                    tmpD = (float) ((parmAvrg - envelopData[i].LowBorder)/range*100 + 15);
                    graphView.Line(i, tmpD, i + 1, tmpD);
                    //graphView.Box(i, tmpD, i + 1, tmpD + 1);
                }
            }
        }

        public void SetEnergyLevel(double enrgy, bool highlited)
        {
            var shape2Height = (int) ((((Picture10.Height - 4))/50.0)*enrgy);
            Shape2.Height = (shape2Height < 0) ? 1 : shape2Height;
            Shape2.Top = (Picture10.Bottom - Shape2.Height) - 2;
            Shape2.BackColor = highlited
                                   ? Color.FromArgb(0, 0xff, 0xff)
                                   : ColorTranslator.FromOle(0xffff);
        }

        public void AddCallRecord(string fileName, TimeSpan processingTime, int callMaxPriorityFlag, int callPriority,
                                  string lioRes, double overallBordersDistance, string angryPersent,
                                  string stressPersent, string upsetPersent, short agentRank, double avgVoiceEnergy)
        {
            callsGrid.Items.Add(fileName).SubItems.AddRange(new[]
                                                                {
                                                                    string.Format("{0}:{1}:{2}.{3}",
                                                                                  processingTime.Hours,
                                                                                  processingTime.Minutes,
                                                                                  processingTime.Seconds,
                                                                                  processingTime.Milliseconds),
                                                                    callMaxPriorityFlag.ToString(),
                                                                    callPriority.ToString(),
                                                                    agentRank.ToString(),
                                                                    String.IsNullOrEmpty(lioRes) ? "N/A" : lioRes,
                                                                    overallBordersDistance.ToString(),
                                                                    avgVoiceEnergy.ToString(),
                                                                    stressPersent,
                                                                    upsetPersent,
                                                                    angryPersent
                                                                });
        }

        #endregion

        private void browseForFile_Click(object sender, EventArgs e)
        {
            if ((FileSelected != null) && (openFileDialog.ShowDialog() == DialogResult.OK))
            {
                fileToProcess.Text = openFileDialog.FileName;
                FileSelected(this, new FileSelectedArgs(openFileDialog.FileName, false));
            }
        }

        private void cmdAddNewSegEmotion_Click(object sender, EventArgs e)
        {
            if (NewSegmentEmotionAdded != null)
                NewSegmentEmotionAdded(this, new NewSegmentEmotionArgs(txtSegEmotion.Text));
        }

        private void cmdClassifySeg_Click(object sender, EventArgs e)
        {
            if (ClassifySegment != null)
            {
                if (segmentsList.SelectedItems.Count == 0)
                    return;

                if (gridUserDefinedSegmentEmotions.SelectedItems.Count == 0)
                    return;


                string emotionName = gridUserDefinedSegmentEmotions.SelectedItems[0].Text;
                if (emotionName.Length == 0)
                    return;

                int segmentId = Int32.Parse(segmentsList.SelectedItems[0].Text);
                ClassifySegment(this, new ClassifySegmentArgs(segmentId, emotionName));
            }
        }

        private void cmdSegLioForget_Click(object sender, EventArgs e)
        {
            if (LioNetForget != null)
            {
                int segmentId = Int32.Parse(segmentsList.SelectedItems[0].Text);
                LioNetForget(this, new LioNetForgetArgs(segmentId));
            }
        }

        private void colorMap1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedColor.BackColor = colorMap1.ForeColor;
        }

        private void DemoForm_Resize(object sender, EventArgs e)
        {
            colorHistory.Items.Clear();
            graphView.Cls();
            graphView.Refresh();
        }

        private void PrepareSementsListGrid()
        {
            string[] strArray =
                "Seg N.|Start Pos (Sec.)| End Pos (Sec.)| Energy| Content | Upset | Angry | Stressed | Uncertain | Excited | Concentrated | EmoCogRatio| Hesitation | BrainPower | Embar. | I. Think | Imagin | ExtremeEmotion |  SAF | Atmos. | ONLINE Flag|  LioNet analysis | MaxAmpVol. | Comment | LioNet Info"
                    .Split(new[] {'|'});
            segmentsList.Items.Clear();
            segmentsList.Columns.Clear();
            foreach (string str in strArray)
            {
                string text = str.Trim();
                segmentsList.Columns.Add(text, -2, HorizontalAlignment.Right);
            }
        }

        private void segmentsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (SegmentsListColumnClicked != null)
            {
                SegmentsListColumnClicked(this, new ColumnClickArgs(e.Column));
            }
        }

        private void segmentsList_MouseClick(object sender, MouseEventArgs e)
        {
            if ((segmentsList.SelectedItems.Count != 0) && (SegmentClicked != null))
            {
                ListViewItem item = segmentsList.SelectedItems[0];
                SegmentClicked(this, new SegmentClickedArgs(Convert.ToInt32(item.Text)));
            }
        }

        private void useSameGraph_CheckedChanged(object sender, EventArgs e)
        {
            colorHistory.Visible = useSameGraph.Checked;
            colorHistory.Items.Clear();
            if (!useSameGraph.Checked)
            {
                graphView.Cls();
                graphView.Refresh();
            }
        }

        private void txtSegEmotion_TextChanged(object sender, EventArgs e)
        {
            cmdAddNewSegEmotion.Enabled = txtSegEmotion.Text.Length != 0;
        }

        private void gridUserDefinedEmotions_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdClassifySeg.Enabled = false;

            if (gridUserDefinedSegmentEmotions.SelectedItems.Count == 0)
                return;

            string text = gridUserDefinedSegmentEmotions.SelectedItems[0].Text;
            if (text.Length == 0)
                return;

            cmdClassifySeg.Enabled = true;
            cmdClassifySeg.Text = String.Format("Classify this segment as: {0}", text);
        }

        private void newEmotionSignature_TextChanged(object sender, EventArgs e)
        {
            addNewEmotionalSignature.Enabled = newEmotionSignature.Text.Length != 0;
        }

        private void addNewEmotionalSignature_Click(object sender, EventArgs e)
        {
            if (NewCallEmotionAdded != null)
                NewCallEmotionAdded(this, new NewCallEmotionalArgs(newEmotionSignature.Text));
        }

        private void classifyThisCall_Click(object sender, EventArgs e)
        {
            if (ClassifyCall != null)
                ClassifyCall(this,
                             new ClassifyCallArgs(callEmotionSignature.Text,
                                                  lioNetEmotionsList.Items[lioNetEmotionsList.SelectedIndex].ToString()));
        }

        private void lioNetForgetAnswer_Click(object sender, EventArgs e)
        {
            if (ForgetCallClassification != null)
                ForgetCallClassification(this, new ForgetCallClassificationArgs(callEmotionSignature.Text));
        }

        private void lioNetEmotionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            classifyThisCall.Text = "Classify This Call As " +
                                    lioNetEmotionsList.Items[lioNetEmotionsList.SelectedIndex];

            classifyThisCall.Enabled = true;
        }

        private void DemoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UiIsGoingToClose != null)
                UiIsGoingToClose(this, null);
        }

        private void retrieveLicenseDetails_Click(object sender, EventArgs e)
        {
            if (RetrieveLicenseDetails != null)
                RetrieveLicenseDetails(this, null);
        }

        private void applyCountersResetCode_Click(object sender, EventArgs e)
        {
            if (ApplyCountersResetCode != null)
                ApplyCountersResetCode(this, new ApplyCountersResetCodeArgs(countersResetCode.Text));
        }

        private void updateKnownEnvelopeAndBordersDataFile_Click(object sender, EventArgs e)
        {
            if (UpdateBordersFile != null)
                UpdateBordersFile(this, null);
        }

        private void showEnvelopAndBordesOnGraph_Click(object sender, EventArgs e)
        {
            if (ShowBordersOnGraph != null)
                ShowBordersOnGraph(this, null);
        }

        private void callsGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent("FileDrop") ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void callsGrid_DragDrop(object sender, DragEventArgs e)
        {
            var dropData = e.Data.GetData("FileDrop") as Array;
            if (dropData == null)
                return;

            for (int i = 0; i < dropData.Length; i++)
            {
                fileToProcess.Text = (string) dropData.GetValue(i);
                FileSelected(this, new FileSelectedArgs(fileToProcess.Text, i < (dropData.Length - 1)));
            }
        }

        private void callsGrid_DoubleClick(object sender, EventArgs e)
        {
            if (callsGrid.SelectedItems.Count <= 0) return;

            fileToProcess.Text = callsGrid.SelectedItems[0].Text;
            FileSelected(this, new FileSelectedArgs(fileToProcess.Text, false));
        }


        private delegate void AddSegmentToListDelegate(
            int segCount, string bstring, int sPos, int fPos, EmotionResults emoVals, string onlineFlag, string comment);

        private delegate void DrawDataDelegate(
            double[] data, double min, double max, double avg, int scaleMin, int scaleMax, int column);

        private delegate void FreezTablesDelegate();

        private delegate void ResetDelegate();

        private delegate void UnFreezTablesDelegate();

        private delegate void ShowProfileDelegate(
            short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel, short energeticLevel,
            short thinkingLevel);


        private delegate void SetIntDelegate(int SegCount);

        private delegate void UpdateHistoryBarsDelegate(EmotionResults emoVals);

        private delegate void UpdateProfileDelegate(
            short emoLevel, short logicalLevel, short hasitantLevel, short stressLevel, short energeticLevel,
            short thinkingLevel);

        private delegate void SetControlTextDelegate(string text);

        private delegate void SetStringCollectionDelegate(string[] strings);

        private delegate void SetLicenseDetailsDelegate(
            string sysID, int callCounter, short postsLicensed, int runningProcesses,
            string coreVersion);

        private delegate void UpdateCallPriorityDelegate(
            int segmentID, int callPriority, int stressSegs, int angerSegs, int upsetSegs);
    }
}