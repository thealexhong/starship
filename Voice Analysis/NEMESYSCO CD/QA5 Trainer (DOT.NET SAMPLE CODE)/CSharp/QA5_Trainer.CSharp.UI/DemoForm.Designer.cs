using System.ComponentModel;
using System.Windows.Forms;
using HistoryBarsControls.HistoryBarsControls;
using System.Drawing;

namespace QA5_Trainer.CSharp.UI
{
    public partial class DemoForm
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.browseForFile = new System.Windows.Forms.Button();
            this.fileToProcess = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lbl_enrgyDif = new System.Windows.Forms.Label();
            this.lbl_enrgyHighSegs = new System.Windows.Forms.Label();
            this.Shape2 = new VB6PictureControlEmu.PictureBox();
            this.Picture10 = new VB6PictureControlEmu.PictureBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.nmsHSAhistoryBar2 = new HistoryBarsControls.HistoryBarsControls.nmsHSAhistoryBar();
            this.nmsHSAhistoryBar1 = new HistoryBarsControls.HistoryBarsControls.nmsHSAhistoryBar();
            this.nmsAShistoryBar1 = new HistoryBarsControls.HistoryBarsControls.nmsAShistoryBar();
            this._Label8_0 = new System.Windows.Forms.Label();
            this._Label8_1 = new System.Windows.Forms.Label();
            this._Label8_2 = new System.Windows.Forms.Label();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.lblCallPriority = new System.Windows.Forms.Label();
            this.ck_AutoStartPlay = new System.Windows.Forms.CheckBox();
            this.ck_PlayBackRealTime = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.energyLevelBelow = new System.Windows.Forms.Label();
            this.limitForEnergyLevelBelow = new System.Windows.Forms.TextBox();
            this.energyTrendIsRaisingFor = new System.Windows.Forms.Label();
            this.speakerIsTiredLabel = new System.Windows.Forms.Label();
            this.callIsOutOfAcceptableLevelsLabel = new System.Windows.Forms.Label();
            this.stressLevelIsRaisingLabel = new System.Windows.Forms.Label();
            this.callWithinAcceptableLevelsLabel = new System.Windows.Forms.Label();
            this.limitForEnergyTrendIsRaisingFor = new System.Windows.Forms.TextBox();
            this.angerTrendLevel = new System.Windows.Forms.Label();
            this.maxAngerTrendLevel = new System.Windows.Forms.TextBox();
            this.stressTrendLevel = new System.Windows.Forms.Label();
            this.maxStressTrendLevel = new System.Windows.Forms.TextBox();
            this.alarmIfStressTrendIsLow = new System.Windows.Forms.CheckBox();
            this.alarmIfStressTrendIsRaising = new System.Windows.Forms.CheckBox();
            this.alarmIfStressTrendIsRaisingAndAbove = new System.Windows.Forms.CheckBox();
            this.alarmIfAngerTrendIsRaisingAndAbove = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.currentProfileThoughtful = new System.Windows.Forms.ProgressBar();
            this._Label15_6 = new System.Windows.Forms.Label();
            this.currentProfileEnergetic = new System.Windows.Forms.ProgressBar();
            this.currentProfileStressed = new System.Windows.Forms.ProgressBar();
            this._Label15_7 = new System.Windows.Forms.Label();
            this.currentProfileHesitant = new System.Windows.Forms.ProgressBar();
            this.currentProfileLogical = new System.Windows.Forms.ProgressBar();
            this._Label15_8 = new System.Windows.Forms.Label();
            this.currentProfileEmotional = new System.Windows.Forms.ProgressBar();
            this._Label15_9 = new System.Windows.Forms.Label();
            this._Label15_10 = new System.Windows.Forms.Label();
            this._Label15_11 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._Label15_1 = new System.Windows.Forms.Label();
            this.baseProfileEmotional = new System.Windows.Forms.ProgressBar();
            this._Label15_2 = new System.Windows.Forms.Label();
            this.baseProfileLogical = new System.Windows.Forms.ProgressBar();
            this._Label15_3 = new System.Windows.Forms.Label();
            this.baseProfileHesitant = new System.Windows.Forms.ProgressBar();
            this._Label15_4 = new System.Windows.Forms.Label();
            this.baseProfileStressed = new System.Windows.Forms.ProgressBar();
            this._Label15_5 = new System.Windows.Forms.Label();
            this.baseProfileEnergetic = new System.Windows.Forms.ProgressBar();
            this._Label15_0 = new System.Windows.Forms.Label();
            this.baseProfileThoughtful = new System.Windows.Forms.ProgressBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.segmentsListCover = new System.Windows.Forms.Label();
            this.colorHistory = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.useSameGraph = new System.Windows.Forms.CheckBox();
            this.selectedColor = new System.Windows.Forms.PictureBox();
            this.colorMap1 = new QA5_Trainer.CSharp.UI.ColorMap();
            this.segmentsList = new System.Windows.Forms.ListView();
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this.gridUserDefinedSegmentEmotions = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.cmdSegLioForget = new System.Windows.Forms.Button();
            this.cmdClassifySeg = new System.Windows.Forms.Button();
            this.cmdAddNewSegEmotion = new System.Windows.Forms.Button();
            this.txtSegEmotion = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lioNetAccuracy = new System.Windows.Forms.Label();
            this.lioNetForgetAnswer = new System.Windows.Forms.Button();
            this.classifyThisCall = new System.Windows.Forms.Button();
            this.lioNetEmotionsList = new System.Windows.Forms.ListBox();
            this.addNewEmotionalSignature = new System.Windows.Forms.Button();
            this.newEmotionSignature = new System.Windows.Forms.TextBox();
            this.lioNetAnalysis = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.callEmotionSignature = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.qaReport = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.showEnvelopAndBordesOnGraph = new System.Windows.Forms.Button();
            this.updateKnownEnvelopeAndBordersDataFile = new System.Windows.Forms.Button();
            this.conversationEnvelopAndBorders = new System.Windows.Forms.ListView();
            this.conversationEnvelopAndBordersLabel = new System.Windows.Forms.Label();
            this.envelopAndBordersReport = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.callsGrid = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.noRealTimeUiUpdates = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.applyCountersResetCode = new System.Windows.Forms.Button();
            this.countersResetCode = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.retrieveLicenseDetails = new System.Windows.Forms.Button();
            this.qA5CoreVersion = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numberOfProcessesRunning = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.currentCallsCounter = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.licensedPosts = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.systemId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.txt_BG = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.ckCalType = new System.Windows.Forms.CheckBox();
            this.ckSegLength = new System.Windows.Forms.CheckBox();
            this.ckAnalysisType = new System.Windows.Forms.CheckBox();
            this.ckStressTest = new System.Windows.Forms.CheckBox();
            this.Ck_perVoice = new System.Windows.Forms.CheckBox();
            this.lioNetEmotions = new System.Windows.Forms.ColumnHeader();
            this.lioNetAnalyses = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.segmentStats = new System.Windows.Forms.Label();
            this.graphView = new VB6PictureControlEmu.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.pictureBox1 = new VB6PictureControlEmu.PictureBox();
            this.pictureBox2 = new VB6PictureControlEmu.PictureBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.nmsHSAhistoryBar3 = new HistoryBarsControls.HistoryBarsControls.nmsHSAhistoryBar();
            this.nmsHSAhistoryBar4 = new HistoryBarsControls.HistoryBarsControls.nmsHSAhistoryBar();
            this.nmsAShistoryBar2 = new HistoryBarsControls.HistoryBarsControls.nmsAShistoryBar();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label29 = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.label30 = new System.Windows.Forms.Label();
            this.progressBar4 = new System.Windows.Forms.ProgressBar();
            this.progressBar5 = new System.Windows.Forms.ProgressBar();
            this.label31 = new System.Windows.Forms.Label();
            this.progressBar6 = new System.Windows.Forms.ProgressBar();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label35 = new System.Windows.Forms.Label();
            this.progressBar7 = new System.Windows.Forms.ProgressBar();
            this.label36 = new System.Windows.Forms.Label();
            this.progressBar8 = new System.Windows.Forms.ProgressBar();
            this.label37 = new System.Windows.Forms.Label();
            this.progressBar9 = new System.Windows.Forms.ProgressBar();
            this.label38 = new System.Windows.Forms.Label();
            this.progressBar10 = new System.Windows.Forms.ProgressBar();
            this.label39 = new System.Windows.Forms.Label();
            this.progressBar11 = new System.Windows.Forms.ProgressBar();
            this.label40 = new System.Windows.Forms.Label();
            this.progressBar12 = new System.Windows.Forms.ProgressBar();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.label41 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.colorMap2 = new QA5_Trainer.CSharp.UI.ColorMap();
            this.listView2 = new System.Windows.Forms.ListView();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.label42 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.listView4 = new System.Windows.Forms.ListView();
            this.label48 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.listView5 = new System.Windows.Forms.ListView();
            this.tabPage12 = new System.Windows.Forms.TabPage();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.button9 = new System.Windows.Forms.Button();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.dataChannelNumber = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedColor)).BeginInit();
            this.Frame3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.tabPage11.SuspendLayout();
            this.tabPage12.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.browseForFile);
            this.groupBox1.Controls.Add(this.fileToProcess);
            this.groupBox1.Location = new System.Drawing.Point(12, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(858, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // browseForFile
            // 
            this.browseForFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseForFile.Location = new System.Drawing.Point(729, 17);
            this.browseForFile.Name = "browseForFile";
            this.browseForFile.Size = new System.Drawing.Size(116, 23);
            this.browseForFile.TabIndex = 1;
            this.browseForFile.Text = "Select File";
            this.browseForFile.UseVisualStyleBackColor = true;
            this.browseForFile.Click += new System.EventHandler(this.browseForFile_Click);
            // 
            // fileToProcess
            // 
            this.fileToProcess.AcceptsReturn = true;
            this.fileToProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileToProcess.Location = new System.Drawing.Point(6, 19);
            this.fileToProcess.Name = "fileToProcess";
            this.fileToProcess.ReadOnly = true;
            this.fileToProcess.Size = new System.Drawing.Size(718, 20);
            this.fileToProcess.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 60);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(858, 562);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbl_enrgyDif);
            this.tabPage1.Controls.Add(this.lbl_enrgyHighSegs);
            this.tabPage1.Controls.Add(this.Shape2);
            this.tabPage1.Controls.Add(this.Picture10);
            this.tabPage1.Controls.Add(this.GroupBox5);
            this.tabPage1.Controls.Add(this.GroupBox4);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(850, 536);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Real Time";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lbl_enrgyDif
            // 
            this.lbl_enrgyDif.BackColor = System.Drawing.Color.Black;
            this.lbl_enrgyDif.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_enrgyDif.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_enrgyDif.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbl_enrgyDif.Location = new System.Drawing.Point(409, 20);
            this.lbl_enrgyDif.Name = "lbl_enrgyDif";
            this.lbl_enrgyDif.Size = new System.Drawing.Size(29, 19);
            this.lbl_enrgyDif.TabIndex = 44;
            this.lbl_enrgyDif.Text = "-";
            this.lbl_enrgyDif.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbl_enrgyHighSegs
            // 
            this.lbl_enrgyHighSegs.BackColor = System.Drawing.Color.Black;
            this.lbl_enrgyHighSegs.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_enrgyHighSegs.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_enrgyHighSegs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbl_enrgyHighSegs.Location = new System.Drawing.Point(409, 40);
            this.lbl_enrgyHighSegs.Name = "lbl_enrgyHighSegs";
            this.lbl_enrgyHighSegs.Size = new System.Drawing.Size(29, 19);
            this.lbl_enrgyHighSegs.TabIndex = 45;
            this.lbl_enrgyHighSegs.Text = "-";
            this.lbl_enrgyHighSegs.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Shape2
            // 
            this.Shape2.BackColor = System.Drawing.Color.Teal;
            this.Shape2.DrawWidth = 1;
            this.Shape2.Location = new System.Drawing.Point(400, 137);
            this.Shape2.Margin = new System.Windows.Forms.Padding(0);
            this.Shape2.Name = "Shape2";
            this.Shape2.ScaleHeight = 150;
            this.Shape2.ScaleLeft = 0;
            this.Shape2.ScaleTop = 0;
            this.Shape2.ScaleWidth = 150;
            this.Shape2.Size = new System.Drawing.Size(49, 68);
            this.Shape2.TabIndex = 180;
            // 
            // Picture10
            // 
            this.Picture10.BackColor = System.Drawing.Color.Black;
            this.Picture10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Picture10.Cursor = System.Windows.Forms.Cursors.Default;
            this.Picture10.DrawWidth = 1;
            this.Picture10.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Picture10.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Picture10.Location = new System.Drawing.Point(398, 20);
            this.Picture10.Margin = new System.Windows.Forms.Padding(0);
            this.Picture10.Name = "Picture10";
            this.Picture10.ScaleHeight = 150;
            this.Picture10.ScaleLeft = 0;
            this.Picture10.ScaleTop = 0;
            this.Picture10.ScaleWidth = 150;
            this.Picture10.Size = new System.Drawing.Size(53, 187);
            this.Picture10.TabIndex = 179;
            // 
            // GroupBox5
            // 
            this.GroupBox5.BackColor = System.Drawing.Color.White;
            this.GroupBox5.Controls.Add(this.nmsHSAhistoryBar2);
            this.GroupBox5.Controls.Add(this.nmsHSAhistoryBar1);
            this.GroupBox5.Controls.Add(this.nmsAShistoryBar1);
            this.GroupBox5.Controls.Add(this._Label8_0);
            this.GroupBox5.Controls.Add(this._Label8_1);
            this.GroupBox5.Controls.Add(this._Label8_2);
            this.GroupBox5.Location = new System.Drawing.Point(8, 213);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(374, 234);
            this.GroupBox5.TabIndex = 178;
            this.GroupBox5.TabStop = false;
            // 
            // nmsHSAhistoryBar2
            // 
            this.nmsHSAhistoryBar2.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsHSAhistoryBar2.colorAtmos = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar2.colorContent = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar2.colorUpset = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsHSAhistoryBar2.Location = new System.Drawing.Point(11, 160);
            this.nmsHSAhistoryBar2.Name = "nmsHSAhistoryBar2";
            this.nmsHSAhistoryBar2.NumberOfSegmentsToShow = ((short)(100));
            this.nmsHSAhistoryBar2.Size = new System.Drawing.Size(353, 43);
            this.nmsHSAhistoryBar2.TabIndex = 188;
            // 
            // nmsHSAhistoryBar1
            // 
            this.nmsHSAhistoryBar1.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsHSAhistoryBar1.colorAtmos = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar1.colorContent = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar1.colorUpset = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsHSAhistoryBar1.Location = new System.Drawing.Point(11, 87);
            this.nmsHSAhistoryBar1.Name = "nmsHSAhistoryBar1";
            this.nmsHSAhistoryBar1.NumberOfSegmentsToShow = ((short)(100));
            this.nmsHSAhistoryBar1.Size = new System.Drawing.Size(353, 43);
            this.nmsHSAhistoryBar1.TabIndex = 187;
            // 
            // nmsAShistoryBar1
            // 
            this.nmsAShistoryBar1.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsAShistoryBar1.colorAnger = System.Drawing.Color.Black;
            this.nmsAShistoryBar1.colorStress = System.Drawing.Color.Black;
            this.nmsAShistoryBar1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsAShistoryBar1.Location = new System.Drawing.Point(11, 15);
            this.nmsAShistoryBar1.Name = "nmsAShistoryBar1";
            this.nmsAShistoryBar1.showNoSegments = ((short)(100));
            this.nmsAShistoryBar1.Size = new System.Drawing.Size(353, 43);
            this.nmsAShistoryBar1.TabIndex = 186;
            // 
            // _Label8_0
            // 
            this._Label8_0.AutoSize = true;
            this._Label8_0.BackColor = System.Drawing.Color.White;
            this._Label8_0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._Label8_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label8_0.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label8_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label8_0.Location = new System.Drawing.Point(11, 58);
            this._Label8_0.Name = "_Label8_0";
            this._Label8_0.Size = new System.Drawing.Size(154, 16);
            this._Label8_0.TabIndex = 185;
            this._Label8_0.Text = "Yellow = Stress, Red = Anger";
            // 
            // _Label8_1
            // 
            this._Label8_1.AutoSize = true;
            this._Label8_1.BackColor = System.Drawing.Color.White;
            this._Label8_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._Label8_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label8_1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label8_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label8_1.Location = new System.Drawing.Point(11, 130);
            this._Label8_1.Name = "_Label8_1";
            this._Label8_1.Size = new System.Drawing.Size(255, 16);
            this._Label8_1.TabIndex = 184;
            this._Label8_1.Text = "Green = Content, Red = Upset, White Line = Atmos.";
            // 
            // _Label8_2
            // 
            this._Label8_2.AutoSize = true;
            this._Label8_2.BackColor = System.Drawing.Color.White;
            this._Label8_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._Label8_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label8_2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label8_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label8_2.Location = new System.Drawing.Point(11, 203);
            this._Label8_2.Name = "_Label8_2";
            this._Label8_2.Size = new System.Drawing.Size(276, 16);
            this._Label8_2.TabIndex = 183;
            this._Label8_2.Text = "Green = Brain P., Red = Emo/Cog, Yellow Line = Energy";
            // 
            // GroupBox4
            // 
            this.GroupBox4.BackColor = System.Drawing.Color.White;
            this.GroupBox4.Controls.Add(this.lblCallPriority);
            this.GroupBox4.Controls.Add(this.ck_AutoStartPlay);
            this.GroupBox4.Controls.Add(this.ck_PlayBackRealTime);
            this.GroupBox4.Location = new System.Drawing.Point(6, 453);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(835, 80);
            this.GroupBox4.TabIndex = 177;
            this.GroupBox4.TabStop = false;
            // 
            // lblCallPriority
            // 
            this.lblCallPriority.BackColor = System.Drawing.Color.White;
            this.lblCallPriority.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCallPriority.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblCallPriority.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCallPriority.Location = new System.Drawing.Point(6, 16);
            this.lblCallPriority.Name = "lblCallPriority";
            this.lblCallPriority.Size = new System.Drawing.Size(816, 18);
            this.lblCallPriority.TabIndex = 125;
            this.lblCallPriority.Text = "Call Priority:";
            this.lblCallPriority.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ck_AutoStartPlay
            // 
            this.ck_AutoStartPlay.BackColor = System.Drawing.Color.White;
            this.ck_AutoStartPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.ck_AutoStartPlay.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ck_AutoStartPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ck_AutoStartPlay.Location = new System.Drawing.Point(419, 53);
            this.ck_AutoStartPlay.Name = "ck_AutoStartPlay";
            this.ck_AutoStartPlay.Size = new System.Drawing.Size(165, 21);
            this.ck_AutoStartPlay.TabIndex = 167;
            this.ck_AutoStartPlay.Text = "Auto start play if high Priority";
            this.ck_AutoStartPlay.UseVisualStyleBackColor = false;
            // 
            // ck_PlayBackRealTime
            // 
            this.ck_PlayBackRealTime.BackColor = System.Drawing.Color.White;
            this.ck_PlayBackRealTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.ck_PlayBackRealTime.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ck_PlayBackRealTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ck_PlayBackRealTime.Location = new System.Drawing.Point(623, 53);
            this.ck_PlayBackRealTime.Name = "ck_PlayBackRealTime";
            this.ck_PlayBackRealTime.Size = new System.Drawing.Size(199, 21);
            this.ck_PlayBackRealTime.TabIndex = 115;
            this.ck_PlayBackRealTime.Text = "Play Segments as they are created";
            this.ck_PlayBackRealTime.UseVisualStyleBackColor = false;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.White;
            this.groupBox6.Controls.Add(this.energyLevelBelow);
            this.groupBox6.Controls.Add(this.limitForEnergyLevelBelow);
            this.groupBox6.Controls.Add(this.energyTrendIsRaisingFor);
            this.groupBox6.Controls.Add(this.speakerIsTiredLabel);
            this.groupBox6.Controls.Add(this.callIsOutOfAcceptableLevelsLabel);
            this.groupBox6.Controls.Add(this.stressLevelIsRaisingLabel);
            this.groupBox6.Controls.Add(this.callWithinAcceptableLevelsLabel);
            this.groupBox6.Controls.Add(this.limitForEnergyTrendIsRaisingFor);
            this.groupBox6.Controls.Add(this.angerTrendLevel);
            this.groupBox6.Controls.Add(this.maxAngerTrendLevel);
            this.groupBox6.Controls.Add(this.stressTrendLevel);
            this.groupBox6.Controls.Add(this.maxStressTrendLevel);
            this.groupBox6.Controls.Add(this.alarmIfStressTrendIsLow);
            this.groupBox6.Controls.Add(this.alarmIfStressTrendIsRaising);
            this.groupBox6.Controls.Add(this.alarmIfStressTrendIsRaisingAndAbove);
            this.groupBox6.Controls.Add(this.alarmIfAngerTrendIsRaisingAndAbove);
            this.groupBox6.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(390, 213);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(451, 234);
            this.groupBox6.TabIndex = 176;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Define Real-Time alarm settings:";
            // 
            // energyLevelBelow
            // 
            this.energyLevelBelow.BackColor = System.Drawing.SystemColors.Window;
            this.energyLevelBelow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.energyLevelBelow.Cursor = System.Windows.Forms.Cursors.Default;
            this.energyLevelBelow.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.energyLevelBelow.ForeColor = System.Drawing.SystemColors.WindowText;
            this.energyLevelBelow.Location = new System.Drawing.Point(400, 149);
            this.energyLevelBelow.Name = "energyLevelBelow";
            this.energyLevelBelow.Size = new System.Drawing.Size(35, 20);
            this.energyLevelBelow.TabIndex = 173;
            // 
            // limitForEnergyLevelBelow
            // 
            this.limitForEnergyLevelBelow.AcceptsReturn = true;
            this.limitForEnergyLevelBelow.BackColor = System.Drawing.SystemColors.Window;
            this.limitForEnergyLevelBelow.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.limitForEnergyLevelBelow.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limitForEnergyLevelBelow.ForeColor = System.Drawing.SystemColors.WindowText;
            this.limitForEnergyLevelBelow.Location = new System.Drawing.Point(369, 149);
            this.limitForEnergyLevelBelow.MaxLength = 0;
            this.limitForEnergyLevelBelow.Name = "limitForEnergyLevelBelow";
            this.limitForEnergyLevelBelow.Size = new System.Drawing.Size(23, 20);
            this.limitForEnergyLevelBelow.TabIndex = 165;
            this.limitForEnergyLevelBelow.Text = "2";
            // 
            // energyTrendIsRaisingFor
            // 
            this.energyTrendIsRaisingFor.BackColor = System.Drawing.SystemColors.Window;
            this.energyTrendIsRaisingFor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.energyTrendIsRaisingFor.Cursor = System.Windows.Forms.Cursors.Default;
            this.energyTrendIsRaisingFor.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.energyTrendIsRaisingFor.ForeColor = System.Drawing.SystemColors.WindowText;
            this.energyTrendIsRaisingFor.Location = new System.Drawing.Point(400, 113);
            this.energyTrendIsRaisingFor.Name = "energyTrendIsRaisingFor";
            this.energyTrendIsRaisingFor.Size = new System.Drawing.Size(35, 20);
            this.energyTrendIsRaisingFor.TabIndex = 174;
            // 
            // speakerIsTiredLabel
            // 
            this.speakerIsTiredLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.speakerIsTiredLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.speakerIsTiredLabel.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.speakerIsTiredLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.speakerIsTiredLabel.Location = new System.Drawing.Point(18, 191);
            this.speakerIsTiredLabel.Name = "speakerIsTiredLabel";
            this.speakerIsTiredLabel.Size = new System.Drawing.Size(417, 28);
            this.speakerIsTiredLabel.TabIndex = 177;
            this.speakerIsTiredLabel.Text = "Speaker is tired or depressd";
            this.speakerIsTiredLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.speakerIsTiredLabel.Visible = false;
            // 
            // callIsOutOfAcceptableLevelsLabel
            // 
            this.callIsOutOfAcceptableLevelsLabel.BackColor = System.Drawing.Color.Red;
            this.callIsOutOfAcceptableLevelsLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.callIsOutOfAcceptableLevelsLabel.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.callIsOutOfAcceptableLevelsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.callIsOutOfAcceptableLevelsLabel.Location = new System.Drawing.Point(18, 191);
            this.callIsOutOfAcceptableLevelsLabel.Name = "callIsOutOfAcceptableLevelsLabel";
            this.callIsOutOfAcceptableLevelsLabel.Size = new System.Drawing.Size(417, 28);
            this.callIsOutOfAcceptableLevelsLabel.TabIndex = 130;
            this.callIsOutOfAcceptableLevelsLabel.Text = "Call is out of acceptable levels";
            this.callIsOutOfAcceptableLevelsLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.callIsOutOfAcceptableLevelsLabel.Visible = false;
            // 
            // stressLevelIsRaisingLabel
            // 
            this.stressLevelIsRaisingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.stressLevelIsRaisingLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.stressLevelIsRaisingLabel.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.stressLevelIsRaisingLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.stressLevelIsRaisingLabel.Location = new System.Drawing.Point(18, 191);
            this.stressLevelIsRaisingLabel.Name = "stressLevelIsRaisingLabel";
            this.stressLevelIsRaisingLabel.Size = new System.Drawing.Size(417, 28);
            this.stressLevelIsRaisingLabel.TabIndex = 162;
            this.stressLevelIsRaisingLabel.Text = "Stress level is raising...";
            this.stressLevelIsRaisingLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.stressLevelIsRaisingLabel.Visible = false;
            // 
            // callWithinAcceptableLevelsLabel
            // 
            this.callWithinAcceptableLevelsLabel.BackColor = System.Drawing.Color.Green;
            this.callWithinAcceptableLevelsLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.callWithinAcceptableLevelsLabel.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.callWithinAcceptableLevelsLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.callWithinAcceptableLevelsLabel.Location = new System.Drawing.Point(18, 191);
            this.callWithinAcceptableLevelsLabel.Name = "callWithinAcceptableLevelsLabel";
            this.callWithinAcceptableLevelsLabel.Size = new System.Drawing.Size(415, 28);
            this.callWithinAcceptableLevelsLabel.TabIndex = 163;
            this.callWithinAcceptableLevelsLabel.Text = "Call is within acceptable levels";
            this.callWithinAcceptableLevelsLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // limitForEnergyTrendIsRaisingFor
            // 
            this.limitForEnergyTrendIsRaisingFor.AcceptsReturn = true;
            this.limitForEnergyTrendIsRaisingFor.BackColor = System.Drawing.SystemColors.Window;
            this.limitForEnergyTrendIsRaisingFor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.limitForEnergyTrendIsRaisingFor.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limitForEnergyTrendIsRaisingFor.ForeColor = System.Drawing.SystemColors.WindowText;
            this.limitForEnergyTrendIsRaisingFor.Location = new System.Drawing.Point(369, 113);
            this.limitForEnergyTrendIsRaisingFor.MaxLength = 0;
            this.limitForEnergyTrendIsRaisingFor.Name = "limitForEnergyTrendIsRaisingFor";
            this.limitForEnergyTrendIsRaisingFor.Size = new System.Drawing.Size(23, 20);
            this.limitForEnergyTrendIsRaisingFor.TabIndex = 166;
            this.limitForEnergyTrendIsRaisingFor.Text = "6";
            // 
            // angerTrendLevel
            // 
            this.angerTrendLevel.BackColor = System.Drawing.SystemColors.Window;
            this.angerTrendLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.angerTrendLevel.Cursor = System.Windows.Forms.Cursors.Default;
            this.angerTrendLevel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.angerTrendLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.angerTrendLevel.Location = new System.Drawing.Point(400, 75);
            this.angerTrendLevel.Name = "angerTrendLevel";
            this.angerTrendLevel.Size = new System.Drawing.Size(35, 20);
            this.angerTrendLevel.TabIndex = 175;
            // 
            // maxAngerTrendLevel
            // 
            this.maxAngerTrendLevel.AcceptsReturn = true;
            this.maxAngerTrendLevel.BackColor = System.Drawing.SystemColors.Window;
            this.maxAngerTrendLevel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.maxAngerTrendLevel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxAngerTrendLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.maxAngerTrendLevel.Location = new System.Drawing.Point(369, 75);
            this.maxAngerTrendLevel.MaxLength = 0;
            this.maxAngerTrendLevel.Name = "maxAngerTrendLevel";
            this.maxAngerTrendLevel.Size = new System.Drawing.Size(23, 20);
            this.maxAngerTrendLevel.TabIndex = 167;
            this.maxAngerTrendLevel.Text = "1.5";
            // 
            // stressTrendLevel
            // 
            this.stressTrendLevel.BackColor = System.Drawing.SystemColors.Window;
            this.stressTrendLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stressTrendLevel.Cursor = System.Windows.Forms.Cursors.Default;
            this.stressTrendLevel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stressTrendLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.stressTrendLevel.Location = new System.Drawing.Point(400, 39);
            this.stressTrendLevel.Name = "stressTrendLevel";
            this.stressTrendLevel.Size = new System.Drawing.Size(35, 20);
            this.stressTrendLevel.TabIndex = 176;
            // 
            // maxStressTrendLevel
            // 
            this.maxStressTrendLevel.AcceptsReturn = true;
            this.maxStressTrendLevel.BackColor = System.Drawing.SystemColors.Window;
            this.maxStressTrendLevel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.maxStressTrendLevel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxStressTrendLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.maxStressTrendLevel.Location = new System.Drawing.Point(369, 39);
            this.maxStressTrendLevel.MaxLength = 0;
            this.maxStressTrendLevel.Name = "maxStressTrendLevel";
            this.maxStressTrendLevel.Size = new System.Drawing.Size(23, 20);
            this.maxStressTrendLevel.TabIndex = 168;
            this.maxStressTrendLevel.Text = "6";
            // 
            // alarmIfStressTrendIsLow
            // 
            this.alarmIfStressTrendIsLow.BackColor = System.Drawing.Color.White;
            this.alarmIfStressTrendIsLow.Checked = true;
            this.alarmIfStressTrendIsLow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alarmIfStressTrendIsLow.Cursor = System.Windows.Forms.Cursors.Default;
            this.alarmIfStressTrendIsLow.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.alarmIfStressTrendIsLow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.alarmIfStressTrendIsLow.Location = new System.Drawing.Point(22, 149);
            this.alarmIfStressTrendIsLow.Name = "alarmIfStressTrendIsLow";
            this.alarmIfStressTrendIsLow.Size = new System.Drawing.Size(286, 18);
            this.alarmIfStressTrendIsLow.TabIndex = 169;
            this.alarmIfStressTrendIsLow.Text = "Alarm if ENERGY level is below (Tiredness):";
            this.alarmIfStressTrendIsLow.UseVisualStyleBackColor = false;
            // 
            // alarmIfStressTrendIsRaising
            // 
            this.alarmIfStressTrendIsRaising.BackColor = System.Drawing.Color.White;
            this.alarmIfStressTrendIsRaising.Checked = true;
            this.alarmIfStressTrendIsRaising.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alarmIfStressTrendIsRaising.Cursor = System.Windows.Forms.Cursors.Default;
            this.alarmIfStressTrendIsRaising.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.alarmIfStressTrendIsRaising.ForeColor = System.Drawing.SystemColors.ControlText;
            this.alarmIfStressTrendIsRaising.Location = new System.Drawing.Point(22, 109);
            this.alarmIfStressTrendIsRaising.Name = "alarmIfStressTrendIsRaising";
            this.alarmIfStressTrendIsRaising.Size = new System.Drawing.Size(336, 28);
            this.alarmIfStressTrendIsRaising.TabIndex = 170;
            this.alarmIfStressTrendIsRaising.Text = "Alarm if ENERGY trend is raising for (no.) segments :";
            this.alarmIfStressTrendIsRaising.UseVisualStyleBackColor = false;
            // 
            // alarmIfStressTrendIsRaisingAndAbove
            // 
            this.alarmIfStressTrendIsRaisingAndAbove.AutoSize = true;
            this.alarmIfStressTrendIsRaisingAndAbove.BackColor = System.Drawing.Color.White;
            this.alarmIfStressTrendIsRaisingAndAbove.Checked = true;
            this.alarmIfStressTrendIsRaisingAndAbove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alarmIfStressTrendIsRaisingAndAbove.Cursor = System.Windows.Forms.Cursors.Default;
            this.alarmIfStressTrendIsRaisingAndAbove.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.alarmIfStressTrendIsRaisingAndAbove.ForeColor = System.Drawing.SystemColors.ControlText;
            this.alarmIfStressTrendIsRaisingAndAbove.Location = new System.Drawing.Point(22, 38);
            this.alarmIfStressTrendIsRaisingAndAbove.Name = "alarmIfStressTrendIsRaisingAndAbove";
            this.alarmIfStressTrendIsRaisingAndAbove.Size = new System.Drawing.Size(315, 20);
            this.alarmIfStressTrendIsRaisingAndAbove.TabIndex = 172;
            this.alarmIfStressTrendIsRaisingAndAbove.Text = "Alarm if STRESS trend level is raising and above :";
            this.alarmIfStressTrendIsRaisingAndAbove.UseVisualStyleBackColor = false;
            // 
            // alarmIfAngerTrendIsRaisingAndAbove
            // 
            this.alarmIfAngerTrendIsRaisingAndAbove.BackColor = System.Drawing.Color.White;
            this.alarmIfAngerTrendIsRaisingAndAbove.Checked = true;
            this.alarmIfAngerTrendIsRaisingAndAbove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alarmIfAngerTrendIsRaisingAndAbove.Cursor = System.Windows.Forms.Cursors.Default;
            this.alarmIfAngerTrendIsRaisingAndAbove.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.alarmIfAngerTrendIsRaisingAndAbove.ForeColor = System.Drawing.SystemColors.ControlText;
            this.alarmIfAngerTrendIsRaisingAndAbove.Location = new System.Drawing.Point(22, 77);
            this.alarmIfAngerTrendIsRaisingAndAbove.Name = "alarmIfAngerTrendIsRaisingAndAbove";
            this.alarmIfAngerTrendIsRaisingAndAbove.Size = new System.Drawing.Size(299, 18);
            this.alarmIfAngerTrendIsRaisingAndAbove.TabIndex = 171;
            this.alarmIfAngerTrendIsRaisingAndAbove.Text = "Alarm if ANGER trend level is above :";
            this.alarmIfAngerTrendIsRaisingAndAbove.UseVisualStyleBackColor = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.currentProfileThoughtful);
            this.groupBox3.Controls.Add(this._Label15_6);
            this.groupBox3.Controls.Add(this.currentProfileEnergetic);
            this.groupBox3.Controls.Add(this.currentProfileStressed);
            this.groupBox3.Controls.Add(this._Label15_7);
            this.groupBox3.Controls.Add(this.currentProfileHesitant);
            this.groupBox3.Controls.Add(this.currentProfileLogical);
            this.groupBox3.Controls.Add(this._Label15_8);
            this.groupBox3.Controls.Add(this.currentProfileEmotional);
            this.groupBox3.Controls.Add(this._Label15_9);
            this.groupBox3.Controls.Add(this._Label15_10);
            this.groupBox3.Controls.Add(this._Label15_11);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(465, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(376, 201);
            this.groupBox3.TabIndex = 167;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current Profile: (online)";
            // 
            // currentProfileThoughtful
            // 
            this.currentProfileThoughtful.Location = new System.Drawing.Point(128, 174);
            this.currentProfileThoughtful.Name = "currentProfileThoughtful";
            this.currentProfileThoughtful.Size = new System.Drawing.Size(231, 20);
            this.currentProfileThoughtful.TabIndex = 47;
            // 
            // _Label15_6
            // 
            this._Label15_6.BackColor = System.Drawing.Color.White;
            this._Label15_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_6.Location = new System.Drawing.Point(18, 172);
            this._Label15_6.Name = "_Label15_6";
            this._Label15_6.Size = new System.Drawing.Size(104, 22);
            this._Label15_6.TabIndex = 48;
            this._Label15_6.Text = "Thoughful:";
            // 
            // currentProfileEnergetic
            // 
            this.currentProfileEnergetic.Location = new System.Drawing.Point(128, 146);
            this.currentProfileEnergetic.Name = "currentProfileEnergetic";
            this.currentProfileEnergetic.Size = new System.Drawing.Size(231, 20);
            this.currentProfileEnergetic.TabIndex = 46;
            // 
            // currentProfileStressed
            // 
            this.currentProfileStressed.Location = new System.Drawing.Point(128, 118);
            this.currentProfileStressed.Name = "currentProfileStressed";
            this.currentProfileStressed.Size = new System.Drawing.Size(231, 20);
            this.currentProfileStressed.TabIndex = 45;
            // 
            // _Label15_7
            // 
            this._Label15_7.BackColor = System.Drawing.Color.White;
            this._Label15_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_7.Location = new System.Drawing.Point(18, 144);
            this._Label15_7.Name = "_Label15_7";
            this._Label15_7.Size = new System.Drawing.Size(104, 22);
            this._Label15_7.TabIndex = 49;
            this._Label15_7.Text = "Energetic:";
            // 
            // currentProfileHesitant
            // 
            this.currentProfileHesitant.Location = new System.Drawing.Point(128, 90);
            this.currentProfileHesitant.Name = "currentProfileHesitant";
            this.currentProfileHesitant.Size = new System.Drawing.Size(231, 20);
            this.currentProfileHesitant.TabIndex = 44;
            // 
            // currentProfileLogical
            // 
            this.currentProfileLogical.Location = new System.Drawing.Point(128, 62);
            this.currentProfileLogical.Name = "currentProfileLogical";
            this.currentProfileLogical.Size = new System.Drawing.Size(231, 20);
            this.currentProfileLogical.TabIndex = 43;
            // 
            // _Label15_8
            // 
            this._Label15_8.BackColor = System.Drawing.Color.White;
            this._Label15_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_8.Location = new System.Drawing.Point(18, 116);
            this._Label15_8.Name = "_Label15_8";
            this._Label15_8.Size = new System.Drawing.Size(104, 22);
            this._Label15_8.TabIndex = 50;
            this._Label15_8.Text = "Stressed:";
            // 
            // currentProfileEmotional
            // 
            this.currentProfileEmotional.Location = new System.Drawing.Point(128, 34);
            this.currentProfileEmotional.Name = "currentProfileEmotional";
            this.currentProfileEmotional.Size = new System.Drawing.Size(231, 20);
            this.currentProfileEmotional.TabIndex = 42;
            // 
            // _Label15_9
            // 
            this._Label15_9.BackColor = System.Drawing.Color.White;
            this._Label15_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_9.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_9.Location = new System.Drawing.Point(18, 88);
            this._Label15_9.Name = "_Label15_9";
            this._Label15_9.Size = new System.Drawing.Size(104, 22);
            this._Label15_9.TabIndex = 51;
            this._Label15_9.Text = "Hesitant:";
            // 
            // _Label15_10
            // 
            this._Label15_10.BackColor = System.Drawing.Color.White;
            this._Label15_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_10.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_10.Location = new System.Drawing.Point(18, 60);
            this._Label15_10.Name = "_Label15_10";
            this._Label15_10.Size = new System.Drawing.Size(104, 22);
            this._Label15_10.TabIndex = 52;
            this._Label15_10.Text = "Logical:";
            // 
            // _Label15_11
            // 
            this._Label15_11.BackColor = System.Drawing.Color.White;
            this._Label15_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_11.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_11.Location = new System.Drawing.Point(18, 32);
            this._Label15_11.Name = "_Label15_11";
            this._Label15_11.Size = new System.Drawing.Size(104, 22);
            this._Label15_11.TabIndex = 53;
            this._Label15_11.Text = "Emotional:";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this._Label15_1);
            this.groupBox2.Controls.Add(this.baseProfileEmotional);
            this.groupBox2.Controls.Add(this._Label15_2);
            this.groupBox2.Controls.Add(this.baseProfileLogical);
            this.groupBox2.Controls.Add(this._Label15_3);
            this.groupBox2.Controls.Add(this.baseProfileHesitant);
            this.groupBox2.Controls.Add(this._Label15_4);
            this.groupBox2.Controls.Add(this.baseProfileStressed);
            this.groupBox2.Controls.Add(this._Label15_5);
            this.groupBox2.Controls.Add(this.baseProfileEnergetic);
            this.groupBox2.Controls.Add(this._Label15_0);
            this.groupBox2.Controls.Add(this.baseProfileThoughtful);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 201);
            this.groupBox2.TabIndex = 166;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Base Emotional Profile:";
            // 
            // _Label15_1
            // 
            this._Label15_1.BackColor = System.Drawing.Color.White;
            this._Label15_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_1.Location = new System.Drawing.Point(18, 62);
            this._Label15_1.Name = "_Label15_1";
            this._Label15_1.Size = new System.Drawing.Size(104, 22);
            this._Label15_1.TabIndex = 36;
            this._Label15_1.Text = "Logical:";
            // 
            // baseProfileEmotional
            // 
            this.baseProfileEmotional.Location = new System.Drawing.Point(128, 34);
            this.baseProfileEmotional.Name = "baseProfileEmotional";
            this.baseProfileEmotional.Size = new System.Drawing.Size(231, 20);
            this.baseProfileEmotional.TabIndex = 29;
            // 
            // _Label15_2
            // 
            this._Label15_2.BackColor = System.Drawing.Color.White;
            this._Label15_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_2.Location = new System.Drawing.Point(18, 90);
            this._Label15_2.Name = "_Label15_2";
            this._Label15_2.Size = new System.Drawing.Size(104, 22);
            this._Label15_2.TabIndex = 37;
            this._Label15_2.Text = "Hesitant:";
            // 
            // baseProfileLogical
            // 
            this.baseProfileLogical.Location = new System.Drawing.Point(128, 62);
            this.baseProfileLogical.Name = "baseProfileLogical";
            this.baseProfileLogical.Size = new System.Drawing.Size(231, 20);
            this.baseProfileLogical.TabIndex = 30;
            // 
            // _Label15_3
            // 
            this._Label15_3.BackColor = System.Drawing.Color.White;
            this._Label15_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_3.Location = new System.Drawing.Point(18, 118);
            this._Label15_3.Name = "_Label15_3";
            this._Label15_3.Size = new System.Drawing.Size(104, 22);
            this._Label15_3.TabIndex = 38;
            this._Label15_3.Text = "Stressed:";
            // 
            // baseProfileHesitant
            // 
            this.baseProfileHesitant.Location = new System.Drawing.Point(128, 90);
            this.baseProfileHesitant.Name = "baseProfileHesitant";
            this.baseProfileHesitant.Size = new System.Drawing.Size(231, 20);
            this.baseProfileHesitant.TabIndex = 31;
            // 
            // _Label15_4
            // 
            this._Label15_4.BackColor = System.Drawing.Color.White;
            this._Label15_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_4.Location = new System.Drawing.Point(18, 146);
            this._Label15_4.Name = "_Label15_4";
            this._Label15_4.Size = new System.Drawing.Size(104, 22);
            this._Label15_4.TabIndex = 39;
            this._Label15_4.Text = "Energetic:";
            // 
            // baseProfileStressed
            // 
            this.baseProfileStressed.Location = new System.Drawing.Point(128, 118);
            this.baseProfileStressed.Name = "baseProfileStressed";
            this.baseProfileStressed.Size = new System.Drawing.Size(231, 20);
            this.baseProfileStressed.TabIndex = 32;
            // 
            // _Label15_5
            // 
            this._Label15_5.BackColor = System.Drawing.Color.White;
            this._Label15_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_5.Location = new System.Drawing.Point(18, 174);
            this._Label15_5.Name = "_Label15_5";
            this._Label15_5.Size = new System.Drawing.Size(104, 22);
            this._Label15_5.TabIndex = 40;
            this._Label15_5.Text = "Thoughtful:";
            // 
            // baseProfileEnergetic
            // 
            this.baseProfileEnergetic.Location = new System.Drawing.Point(128, 146);
            this.baseProfileEnergetic.Name = "baseProfileEnergetic";
            this.baseProfileEnergetic.Size = new System.Drawing.Size(231, 20);
            this.baseProfileEnergetic.TabIndex = 33;
            // 
            // _Label15_0
            // 
            this._Label15_0.BackColor = System.Drawing.Color.White;
            this._Label15_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label15_0.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._Label15_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label15_0.Location = new System.Drawing.Point(18, 34);
            this._Label15_0.Name = "_Label15_0";
            this._Label15_0.Size = new System.Drawing.Size(104, 22);
            this._Label15_0.TabIndex = 35;
            this._Label15_0.Text = "Emotional:";
            // 
            // baseProfileThoughtful
            // 
            this.baseProfileThoughtful.Location = new System.Drawing.Point(128, 174);
            this.baseProfileThoughtful.Name = "baseProfileThoughtful";
            this.baseProfileThoughtful.Size = new System.Drawing.Size(231, 20);
            this.baseProfileThoughtful.TabIndex = 34;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.segmentsListCover);
            this.tabPage2.Controls.Add(this.colorHistory);
            this.tabPage2.Controls.Add(this.useSameGraph);
            this.tabPage2.Controls.Add(this.selectedColor);
            this.tabPage2.Controls.Add(this.colorMap1);
            this.tabPage2.Controls.Add(this.segmentsList);
            this.tabPage2.Controls.Add(this.Frame3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(850, 536);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Segment View";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // segmentsListCover
            // 
            this.segmentsListCover.BackColor = System.Drawing.Color.Black;
            this.segmentsListCover.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.segmentsListCover.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.segmentsListCover.Location = new System.Drawing.Point(6, 11);
            this.segmentsListCover.Name = "segmentsListCover";
            this.segmentsListCover.Size = new System.Drawing.Size(640, 454);
            this.segmentsListCover.TabIndex = 83;
            this.segmentsListCover.Text = "Processing Data";
            this.segmentsListCover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.segmentsListCover.Visible = false;
            // 
            // colorHistory
            // 
            this.colorHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.colorHistory.FullRowSelect = true;
            this.colorHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.colorHistory.Location = new System.Drawing.Point(6, 474);
            this.colorHistory.Name = "colorHistory";
            this.colorHistory.Size = new System.Drawing.Size(269, 54);
            this.colorHistory.TabIndex = 82;
            this.colorHistory.UseCompatibleStateImageBehavior = false;
            this.colorHistory.View = System.Windows.Forms.View.Details;
            this.colorHistory.Visible = false;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 200;
            // 
            // useSameGraph
            // 
            this.useSameGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.useSameGraph.Appearance = System.Windows.Forms.Appearance.Button;
            this.useSameGraph.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.useSameGraph.Location = new System.Drawing.Point(281, 472);
            this.useSameGraph.Name = "useSameGraph";
            this.useSameGraph.Size = new System.Drawing.Size(63, 56);
            this.useSameGraph.TabIndex = 81;
            this.useSameGraph.Text = "Use same graph";
            this.useSameGraph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.useSameGraph.UseVisualStyleBackColor = true;
            this.useSameGraph.Click += new System.EventHandler(this.useSameGraph_CheckedChanged);
            this.useSameGraph.CheckedChanged += new System.EventHandler(this.useSameGraph_CheckedChanged);
            // 
            // selectedColor
            // 
            this.selectedColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectedColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.selectedColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.selectedColor.Location = new System.Drawing.Point(588, 474);
            this.selectedColor.Name = "selectedColor";
            this.selectedColor.Size = new System.Drawing.Size(57, 56);
            this.selectedColor.TabIndex = 79;
            this.selectedColor.TabStop = false;
            // 
            // colorMap1
            // 
            this.colorMap1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorMap1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorMap1.Location = new System.Drawing.Point(350, 474);
            this.colorMap1.Name = "colorMap1";
            this.colorMap1.Size = new System.Drawing.Size(229, 56);
            this.colorMap1.TabIndex = 80;
            this.colorMap1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.colorMap1_MouseUp);
            // 
            // segmentsList
            // 
            this.segmentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.segmentsList.FullRowSelect = true;
            this.segmentsList.GridLines = true;
            this.segmentsList.HideSelection = false;
            this.segmentsList.Location = new System.Drawing.Point(6, 11);
            this.segmentsList.Name = "segmentsList";
            this.segmentsList.ShowGroups = false;
            this.segmentsList.Size = new System.Drawing.Size(640, 454);
            this.segmentsList.TabIndex = 77;
            this.segmentsList.UseCompatibleStateImageBehavior = false;
            this.segmentsList.View = System.Windows.Forms.View.Details;
            this.segmentsList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.segmentsList_MouseClick);
            this.segmentsList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.segmentsList_ColumnClick);
            // 
            // Frame3
            // 
            this.Frame3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Frame3.BackColor = System.Drawing.SystemColors.Window;
            this.Frame3.Controls.Add(this.gridUserDefinedSegmentEmotions);
            this.Frame3.Controls.Add(this.cmdSegLioForget);
            this.Frame3.Controls.Add(this.cmdClassifySeg);
            this.Frame3.Controls.Add(this.cmdAddNewSegEmotion);
            this.Frame3.Controls.Add(this.txtSegEmotion);
            this.Frame3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame3.Location = new System.Drawing.Point(652, 11);
            this.Frame3.Name = "Frame3";
            this.Frame3.Padding = new System.Windows.Forms.Padding(0);
            this.Frame3.Size = new System.Drawing.Size(189, 519);
            this.Frame3.TabIndex = 76;
            this.Frame3.TabStop = false;
            this.Frame3.Text = " Train LioNet System ";
            // 
            // gridUserDefinedSegmentEmotions
            // 
            this.gridUserDefinedSegmentEmotions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.gridUserDefinedSegmentEmotions.FullRowSelect = true;
            this.gridUserDefinedSegmentEmotions.GridLines = true;
            this.gridUserDefinedSegmentEmotions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.gridUserDefinedSegmentEmotions.Location = new System.Drawing.Point(7, 74);
            this.gridUserDefinedSegmentEmotions.MultiSelect = false;
            this.gridUserDefinedSegmentEmotions.Name = "gridUserDefinedSegmentEmotions";
            this.gridUserDefinedSegmentEmotions.ShowGroups = false;
            this.gridUserDefinedSegmentEmotions.Size = new System.Drawing.Size(177, 326);
            this.gridUserDefinedSegmentEmotions.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.gridUserDefinedSegmentEmotions.TabIndex = 80;
            this.gridUserDefinedSegmentEmotions.UseCompatibleStateImageBehavior = false;
            this.gridUserDefinedSegmentEmotions.View = System.Windows.Forms.View.Details;
            this.gridUserDefinedSegmentEmotions.SelectedIndexChanged += new System.EventHandler(this.gridUserDefinedEmotions_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 172;
            // 
            // cmdSegLioForget
            // 
            this.cmdSegLioForget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.cmdSegLioForget.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdSegLioForget.Enabled = false;
            this.cmdSegLioForget.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.cmdSegLioForget.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdSegLioForget.Location = new System.Drawing.Point(6, 463);
            this.cmdSegLioForget.Name = "cmdSegLioForget";
            this.cmdSegLioForget.Size = new System.Drawing.Size(178, 48);
            this.cmdSegLioForget.TabIndex = 79;
            this.cmdSegLioForget.Text = "Net Forget!";
            this.cmdSegLioForget.UseVisualStyleBackColor = false;
            this.cmdSegLioForget.Click += new System.EventHandler(this.cmdSegLioForget_Click);
            // 
            // cmdClassifySeg
            // 
            this.cmdClassifySeg.BackColor = System.Drawing.SystemColors.Control;
            this.cmdClassifySeg.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdClassifySeg.Enabled = false;
            this.cmdClassifySeg.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.cmdClassifySeg.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdClassifySeg.Location = new System.Drawing.Point(7, 406);
            this.cmdClassifySeg.Name = "cmdClassifySeg";
            this.cmdClassifySeg.Size = new System.Drawing.Size(178, 48);
            this.cmdClassifySeg.TabIndex = 78;
            this.cmdClassifySeg.Text = "Classify this segment";
            this.cmdClassifySeg.UseVisualStyleBackColor = false;
            this.cmdClassifySeg.Click += new System.EventHandler(this.cmdClassifySeg_Click);
            // 
            // cmdAddNewSegEmotion
            // 
            this.cmdAddNewSegEmotion.BackColor = System.Drawing.SystemColors.Control;
            this.cmdAddNewSegEmotion.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdAddNewSegEmotion.Enabled = false;
            this.cmdAddNewSegEmotion.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddNewSegEmotion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdAddNewSegEmotion.Location = new System.Drawing.Point(6, 42);
            this.cmdAddNewSegEmotion.Name = "cmdAddNewSegEmotion";
            this.cmdAddNewSegEmotion.Size = new System.Drawing.Size(178, 26);
            this.cmdAddNewSegEmotion.TabIndex = 77;
            this.cmdAddNewSegEmotion.Text = "Add as new emotion";
            this.cmdAddNewSegEmotion.UseVisualStyleBackColor = false;
            this.cmdAddNewSegEmotion.Click += new System.EventHandler(this.cmdAddNewSegEmotion_Click);
            // 
            // txtSegEmotion
            // 
            this.txtSegEmotion.AcceptsReturn = true;
            this.txtSegEmotion.BackColor = System.Drawing.SystemColors.Window;
            this.txtSegEmotion.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSegEmotion.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSegEmotion.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtSegEmotion.Location = new System.Drawing.Point(6, 16);
            this.txtSegEmotion.MaxLength = 0;
            this.txtSegEmotion.Name = "txtSegEmotion";
            this.txtSegEmotion.Size = new System.Drawing.Size(178, 20);
            this.txtSegEmotion.TabIndex = 76;
            this.txtSegEmotion.TextChanged += new System.EventHandler(this.txtSegEmotion_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.Controls.Add(this.lioNetAnalysis);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.callEmotionSignature);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.qaReport);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(850, 536);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Emotional Signature";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lioNetAccuracy);
            this.groupBox7.Controls.Add(this.lioNetForgetAnswer);
            this.groupBox7.Controls.Add(this.classifyThisCall);
            this.groupBox7.Controls.Add(this.lioNetEmotionsList);
            this.groupBox7.Controls.Add(this.addNewEmotionalSignature);
            this.groupBox7.Controls.Add(this.newEmotionSignature);
            this.groupBox7.Location = new System.Drawing.Point(634, 40);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(206, 482);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = " Train LioNet System - Complete Calls ";
            // 
            // lioNetAccuracy
            // 
            this.lioNetAccuracy.BackColor = System.Drawing.Color.Black;
            this.lioNetAccuracy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lioNetAccuracy.ForeColor = System.Drawing.SystemColors.Info;
            this.lioNetAccuracy.Location = new System.Drawing.Point(7, 431);
            this.lioNetAccuracy.Name = "lioNetAccuracy";
            this.lioNetAccuracy.Size = new System.Drawing.Size(193, 39);
            this.lioNetAccuracy.TabIndex = 5;
            this.lioNetAccuracy.Text = "LioNet Accuracy:0%";
            this.lioNetAccuracy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lioNetForgetAnswer
            // 
            this.lioNetForgetAnswer.BackColor = System.Drawing.Color.LightCoral;
            this.lioNetForgetAnswer.Location = new System.Drawing.Point(7, 397);
            this.lioNetForgetAnswer.Name = "lioNetForgetAnswer";
            this.lioNetForgetAnswer.Size = new System.Drawing.Size(193, 27);
            this.lioNetForgetAnswer.TabIndex = 4;
            this.lioNetForgetAnswer.Text = "Make LioNet forget this answer";
            this.lioNetForgetAnswer.UseVisualStyleBackColor = false;
            this.lioNetForgetAnswer.Click += new System.EventHandler(this.lioNetForgetAnswer_Click);
            // 
            // classifyThisCall
            // 
            this.classifyThisCall.Enabled = false;
            this.classifyThisCall.Location = new System.Drawing.Point(7, 351);
            this.classifyThisCall.Name = "classifyThisCall";
            this.classifyThisCall.Size = new System.Drawing.Size(193, 40);
            this.classifyThisCall.TabIndex = 3;
            this.classifyThisCall.Text = "Classify This Call";
            this.classifyThisCall.UseVisualStyleBackColor = true;
            this.classifyThisCall.Click += new System.EventHandler(this.classifyThisCall_Click);
            // 
            // lioNetEmotionsList
            // 
            this.lioNetEmotionsList.FormattingEnabled = true;
            this.lioNetEmotionsList.Location = new System.Drawing.Point(7, 81);
            this.lioNetEmotionsList.Name = "lioNetEmotionsList";
            this.lioNetEmotionsList.Size = new System.Drawing.Size(193, 264);
            this.lioNetEmotionsList.TabIndex = 2;
            this.lioNetEmotionsList.SelectedIndexChanged += new System.EventHandler(this.lioNetEmotionsList_SelectedIndexChanged);
            // 
            // addNewEmotionalSignature
            // 
            this.addNewEmotionalSignature.Enabled = false;
            this.addNewEmotionalSignature.Location = new System.Drawing.Point(7, 47);
            this.addNewEmotionalSignature.Name = "addNewEmotionalSignature";
            this.addNewEmotionalSignature.Size = new System.Drawing.Size(193, 27);
            this.addNewEmotionalSignature.TabIndex = 1;
            this.addNewEmotionalSignature.Text = "Add New Call Profile";
            this.addNewEmotionalSignature.UseVisualStyleBackColor = true;
            this.addNewEmotionalSignature.Click += new System.EventHandler(this.addNewEmotionalSignature_Click);
            // 
            // newEmotionSignature
            // 
            this.newEmotionSignature.Location = new System.Drawing.Point(7, 20);
            this.newEmotionSignature.Name = "newEmotionSignature";
            this.newEmotionSignature.Size = new System.Drawing.Size(193, 20);
            this.newEmotionSignature.TabIndex = 0;
            this.newEmotionSignature.TextChanged += new System.EventHandler(this.newEmotionSignature_TextChanged);
            // 
            // lioNetAnalysis
            // 
            this.lioNetAnalysis.BackColor = System.Drawing.Color.Black;
            this.lioNetAnalysis.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lioNetAnalysis.ForeColor = System.Drawing.SystemColors.Info;
            this.lioNetAnalysis.Location = new System.Drawing.Point(12, 481);
            this.lioNetAnalysis.Name = "lioNetAnalysis";
            this.lioNetAnalysis.Size = new System.Drawing.Size(611, 42);
            this.lioNetAnalysis.TabIndex = 7;
            this.lioNetAnalysis.Text = "-No analysis-";
            this.lioNetAnalysis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lioNetAnalysis, "LioNet analysis for this Emotional Signature");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 464);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "QA5 LioNet analysis:";
            // 
            // callEmotionSignature
            // 
            this.callEmotionSignature.Location = new System.Drawing.Point(9, 361);
            this.callEmotionSignature.Multiline = true;
            this.callEmotionSignature.Name = "callEmotionSignature";
            this.callEmotionSignature.Size = new System.Drawing.Size(614, 96);
            this.callEmotionSignature.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 344);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "LioNet \"Emotional Signature\" for this call";
            // 
            // qaReport
            // 
            this.qaReport.Location = new System.Drawing.Point(6, 56);
            this.qaReport.Multiline = true;
            this.qaReport.Name = "qaReport";
            this.qaReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.qaReport.Size = new System.Drawing.Size(617, 281);
            this.qaReport.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "QA5 Report";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(850, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "LioNet Call (Emotional Signature) training system";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.showEnvelopAndBordesOnGraph);
            this.tabPage5.Controls.Add(this.updateKnownEnvelopeAndBordersDataFile);
            this.tabPage5.Controls.Add(this.conversationEnvelopAndBorders);
            this.tabPage5.Controls.Add(this.conversationEnvelopAndBordersLabel);
            this.tabPage5.Controls.Add(this.envelopAndBordersReport);
            this.tabPage5.Controls.Add(this.label14);
            this.tabPage5.Controls.Add(this.label13);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(850, 536);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Envelop & Borders";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // showEnvelopAndBordesOnGraph
            // 
            this.showEnvelopAndBordesOnGraph.Location = new System.Drawing.Point(656, 186);
            this.showEnvelopAndBordesOnGraph.Name = "showEnvelopAndBordesOnGraph";
            this.showEnvelopAndBordesOnGraph.Size = new System.Drawing.Size(185, 45);
            this.showEnvelopAndBordesOnGraph.TabIndex = 7;
            this.showEnvelopAndBordesOnGraph.Text = "Show On Graph";
            this.showEnvelopAndBordesOnGraph.UseVisualStyleBackColor = true;
            this.showEnvelopAndBordesOnGraph.Click += new System.EventHandler(this.showEnvelopAndBordesOnGraph_Click);
            // 
            // updateKnownEnvelopeAndBordersDataFile
            // 
            this.updateKnownEnvelopeAndBordersDataFile.Location = new System.Drawing.Point(8, 474);
            this.updateKnownEnvelopeAndBordersDataFile.Name = "updateKnownEnvelopeAndBordersDataFile";
            this.updateKnownEnvelopeAndBordersDataFile.Size = new System.Drawing.Size(265, 45);
            this.updateKnownEnvelopeAndBordersDataFile.TabIndex = 6;
            this.updateKnownEnvelopeAndBordersDataFile.Text = "Update the \"Known Envelope && Borders\" Data File";
            this.updateKnownEnvelopeAndBordersDataFile.UseVisualStyleBackColor = true;
            this.updateKnownEnvelopeAndBordersDataFile.Click += new System.EventHandler(this.updateKnownEnvelopeAndBordersDataFile_Click);
            // 
            // conversationEnvelopAndBorders
            // 
            this.conversationEnvelopAndBorders.FullRowSelect = true;
            this.conversationEnvelopAndBorders.GridLines = true;
            this.conversationEnvelopAndBorders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.conversationEnvelopAndBorders.Location = new System.Drawing.Point(11, 259);
            this.conversationEnvelopAndBorders.MultiSelect = false;
            this.conversationEnvelopAndBorders.Name = "conversationEnvelopAndBorders";
            this.conversationEnvelopAndBorders.Size = new System.Drawing.Size(830, 209);
            this.conversationEnvelopAndBorders.TabIndex = 5;
            this.conversationEnvelopAndBorders.UseCompatibleStateImageBehavior = false;
            this.conversationEnvelopAndBorders.View = System.Windows.Forms.View.Details;
            // 
            // conversationEnvelopAndBordersLabel
            // 
            this.conversationEnvelopAndBordersLabel.AutoSize = true;
            this.conversationEnvelopAndBordersLabel.Location = new System.Drawing.Point(8, 243);
            this.conversationEnvelopAndBordersLabel.Name = "conversationEnvelopAndBordersLabel";
            this.conversationEnvelopAndBordersLabel.Size = new System.Drawing.Size(177, 13);
            this.conversationEnvelopAndBordersLabel.TabIndex = 4;
            this.conversationEnvelopAndBordersLabel.Text = "Conversation Envelope and Borders";
            // 
            // envelopAndBordersReport
            // 
            this.envelopAndBordersReport.Location = new System.Drawing.Point(8, 52);
            this.envelopAndBordersReport.Multiline = true;
            this.envelopAndBordersReport.Name = "envelopAndBordersReport";
            this.envelopAndBordersReport.ReadOnly = true;
            this.envelopAndBordersReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.envelopAndBordersReport.Size = new System.Drawing.Size(642, 179);
            this.envelopAndBordersReport.TabIndex = 3;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 35);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(147, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Envelope and Borders Report";
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.Black;
            this.label13.Dock = System.Windows.Forms.DockStyle.Top;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.Info;
            this.label13.Location = new System.Drawing.Point(0, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(850, 31);
            this.label13.TabIndex = 1;
            this.label13.Text = "Envelop && Borders";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.callsGrid);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(850, 536);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Tests Database";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // callsGrid
            // 
            this.callsGrid.AllowDrop = true;
            this.callsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.callsGrid.FullRowSelect = true;
            this.callsGrid.GridLines = true;
            this.callsGrid.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.callsGrid.HideSelection = false;
            this.callsGrid.Location = new System.Drawing.Point(0, 0);
            this.callsGrid.MultiSelect = false;
            this.callsGrid.Name = "callsGrid";
            this.callsGrid.Size = new System.Drawing.Size(850, 536);
            this.callsGrid.TabIndex = 0;
            this.callsGrid.UseCompatibleStateImageBehavior = false;
            this.callsGrid.View = System.Windows.Forms.View.Details;
            this.callsGrid.DoubleClick += new System.EventHandler(this.callsGrid_DoubleClick);
            this.callsGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.callsGrid_DragDrop);
            this.callsGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.callsGrid_DragOver);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.noRealTimeUiUpdates);
            this.tabPage3.Controls.Add(this.groupBox9);
            this.tabPage3.Controls.Add(this.groupBox8);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(850, 536);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings & License Data";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // noRealTimeUiUpdates
            // 
            this.noRealTimeUiUpdates.Appearance = System.Windows.Forms.Appearance.Button;
            this.noRealTimeUiUpdates.Location = new System.Drawing.Point(652, 493);
            this.noRealTimeUiUpdates.Name = "noRealTimeUiUpdates";
            this.noRealTimeUiUpdates.Size = new System.Drawing.Size(189, 27);
            this.noRealTimeUiUpdates.TabIndex = 173;
            this.noRealTimeUiUpdates.Text = "No UI Updates (For Speed Tests)";
            this.noRealTimeUiUpdates.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noRealTimeUiUpdates.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.applyCountersResetCode);
            this.groupBox9.Controls.Add(this.countersResetCode);
            this.groupBox9.Controls.Add(this.label12);
            this.groupBox9.Controls.Add(this.label11);
            this.groupBox9.Controls.Add(this.retrieveLicenseDetails);
            this.groupBox9.Controls.Add(this.qA5CoreVersion);
            this.groupBox9.Controls.Add(this.label10);
            this.groupBox9.Controls.Add(this.numberOfProcessesRunning);
            this.groupBox9.Controls.Add(this.label9);
            this.groupBox9.Controls.Add(this.currentCallsCounter);
            this.groupBox9.Controls.Add(this.label8);
            this.groupBox9.Controls.Add(this.licensedPosts);
            this.groupBox9.Controls.Add(this.label7);
            this.groupBox9.Controls.Add(this.systemId);
            this.groupBox9.Controls.Add(this.label6);
            this.groupBox9.Location = new System.Drawing.Point(7, 174);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(834, 241);
            this.groupBox9.TabIndex = 167;
            this.groupBox9.TabStop = false;
            // 
            // applyCountersResetCode
            // 
            this.applyCountersResetCode.Location = new System.Drawing.Point(639, 187);
            this.applyCountersResetCode.Name = "applyCountersResetCode";
            this.applyCountersResetCode.Size = new System.Drawing.Size(75, 37);
            this.applyCountersResetCode.TabIndex = 14;
            this.applyCountersResetCode.Text = "Apply";
            this.applyCountersResetCode.UseVisualStyleBackColor = true;
            this.applyCountersResetCode.Click += new System.EventHandler(this.applyCountersResetCode_Click);
            // 
            // countersResetCode
            // 
            this.countersResetCode.Location = new System.Drawing.Point(21, 204);
            this.countersResetCode.Name = "countersResetCode";
            this.countersResetCode.Size = new System.Drawing.Size(612, 20);
            this.countersResetCode.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 187);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Counters Reset Code";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(18, 150);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(465, 30);
            this.label11.TabIndex = 11;
            this.label11.Text = "To reset the counters, you will need to obtain an unique code from Nemesysco. Ple" +
                "ase contact Nemesysco\'s representative to obtain the code.";
            // 
            // retrieveLicenseDetails
            // 
            this.retrieveLicenseDetails.Location = new System.Drawing.Point(419, 75);
            this.retrieveLicenseDetails.Name = "retrieveLicenseDetails";
            this.retrieveLicenseDetails.Size = new System.Drawing.Size(147, 36);
            this.retrieveLicenseDetails.TabIndex = 10;
            this.retrieveLicenseDetails.Text = "Retrieve License Details";
            this.retrieveLicenseDetails.UseVisualStyleBackColor = true;
            this.retrieveLicenseDetails.Click += new System.EventHandler(this.retrieveLicenseDetails_Click);
            // 
            // qA5CoreVersion
            // 
            this.qA5CoreVersion.Location = new System.Drawing.Point(419, 41);
            this.qA5CoreVersion.Name = "qA5CoreVersion";
            this.qA5CoreVersion.ReadOnly = true;
            this.qA5CoreVersion.Size = new System.Drawing.Size(147, 20);
            this.qA5CoreVersion.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(416, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "QA5 Core Version";
            // 
            // numberOfProcessesRunning
            // 
            this.numberOfProcessesRunning.Location = new System.Drawing.Point(215, 91);
            this.numberOfProcessesRunning.Name = "numberOfProcessesRunning";
            this.numberOfProcessesRunning.ReadOnly = true;
            this.numberOfProcessesRunning.Size = new System.Drawing.Size(147, 20);
            this.numberOfProcessesRunning.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(212, 75);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "No. of Processes Running";
            // 
            // currentCallsCounter
            // 
            this.currentCallsCounter.Location = new System.Drawing.Point(215, 41);
            this.currentCallsCounter.Name = "currentCallsCounter";
            this.currentCallsCounter.ReadOnly = true;
            this.currentCallsCounter.Size = new System.Drawing.Size(147, 20);
            this.currentCallsCounter.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(212, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Current Calls Counter";
            // 
            // licensedPosts
            // 
            this.licensedPosts.Location = new System.Drawing.Point(21, 91);
            this.licensedPosts.Name = "licensedPosts";
            this.licensedPosts.ReadOnly = true;
            this.licensedPosts.Size = new System.Drawing.Size(147, 20);
            this.licensedPosts.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Licensed Posts";
            // 
            // systemId
            // 
            this.systemId.Location = new System.Drawing.Point(21, 41);
            this.systemId.Name = "systemId";
            this.systemId.ReadOnly = true;
            this.systemId.Size = new System.Drawing.Size(147, 20);
            this.systemId.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "System ID";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dataChannelNumber);
            this.groupBox8.Controls.Add(this.txt_BG);
            this.groupBox8.Controls.Add(this.Label4);
            this.groupBox8.Controls.Add(this.ckCalType);
            this.groupBox8.Controls.Add(this.ckSegLength);
            this.groupBox8.Controls.Add(this.ckAnalysisType);
            this.groupBox8.Controls.Add(this.ckStressTest);
            this.groupBox8.Controls.Add(this.Ck_perVoice);
            this.groupBox8.Location = new System.Drawing.Point(7, 31);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(834, 125);
            this.groupBox8.TabIndex = 166;
            this.groupBox8.TabStop = false;
            // 
            // txt_BG
            // 
            this.txt_BG.AcceptsReturn = true;
            this.txt_BG.BackColor = System.Drawing.SystemColors.Window;
            this.txt_BG.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_BG.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_BG.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txt_BG.Location = new System.Drawing.Point(171, 81);
            this.txt_BG.MaxLength = 0;
            this.txt_BG.Name = "txt_BG";
            this.txt_BG.Size = new System.Drawing.Size(39, 20);
            this.txt_BG.TabIndex = 173;
            this.txt_BG.Text = "1000";
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.Color.Transparent;
            this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label4.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Label4.Location = new System.Drawing.Point(18, 84);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(131, 16);
            this.Label4.TabIndex = 174;
            this.Label4.Text = "Background Noise level";
            // 
            // ckCalType
            // 
            this.ckCalType.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckCalType.BackColor = System.Drawing.SystemColors.Control;
            this.ckCalType.Checked = true;
            this.ckCalType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckCalType.Cursor = System.Windows.Forms.Cursors.Default;
            this.ckCalType.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckCalType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ckCalType.Location = new System.Drawing.Point(427, 78);
            this.ckCalType.Name = "ckCalType";
            this.ckCalType.Size = new System.Drawing.Size(189, 27);
            this.ckCalType.TabIndex = 170;
            this.ckCalType.Text = "Use Calibration Type 2";
            this.ckCalType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckCalType.UseVisualStyleBackColor = false;
            // 
            // ckSegLength
            // 
            this.ckSegLength.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckSegLength.BackColor = System.Drawing.SystemColors.Control;
            this.ckSegLength.Checked = true;
            this.ckSegLength.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckSegLength.Cursor = System.Windows.Forms.Cursors.Default;
            this.ckSegLength.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSegLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ckSegLength.Location = new System.Drawing.Point(427, 27);
            this.ckSegLength.Name = "ckSegLength";
            this.ckSegLength.Size = new System.Drawing.Size(189, 27);
            this.ckSegLength.TabIndex = 172;
            this.ckSegLength.Text = "2 Seconds segments";
            this.ckSegLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckSegLength.UseVisualStyleBackColor = false;
            // 
            // ckAnalysisType
            // 
            this.ckAnalysisType.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckAnalysisType.BackColor = System.Drawing.SystemColors.Control;
            this.ckAnalysisType.Checked = true;
            this.ckAnalysisType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckAnalysisType.Cursor = System.Windows.Forms.Cursors.Default;
            this.ckAnalysisType.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckAnalysisType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ckAnalysisType.Location = new System.Drawing.Point(224, 78);
            this.ckAnalysisType.Name = "ckAnalysisType";
            this.ckAnalysisType.Size = new System.Drawing.Size(189, 27);
            this.ckAnalysisType.TabIndex = 168;
            this.ckAnalysisType.Text = "Use Analysis Type 2";
            this.ckAnalysisType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckAnalysisType.UseVisualStyleBackColor = false;
            // 
            // ckStressTest
            // 
            this.ckStressTest.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckStressTest.BackColor = System.Drawing.SystemColors.Control;
            this.ckStressTest.Checked = true;
            this.ckStressTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckStressTest.Cursor = System.Windows.Forms.Cursors.Default;
            this.ckStressTest.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckStressTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ckStressTest.Location = new System.Drawing.Point(21, 27);
            this.ckStressTest.Name = "ckStressTest";
            this.ckStressTest.Size = new System.Drawing.Size(189, 27);
            this.ckStressTest.TabIndex = 169;
            this.ckStressTest.Text = "Test CL Stress ";
            this.ckStressTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ckStressTest.UseVisualStyleBackColor = false;
            // 
            // Ck_perVoice
            // 
            this.Ck_perVoice.Appearance = System.Windows.Forms.Appearance.Button;
            this.Ck_perVoice.BackColor = System.Drawing.SystemColors.Control;
            this.Ck_perVoice.Checked = true;
            this.Ck_perVoice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ck_perVoice.Cursor = System.Windows.Forms.Cursors.Default;
            this.Ck_perVoice.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ck_perVoice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Ck_perVoice.Location = new System.Drawing.Point(224, 27);
            this.Ck_perVoice.Name = "Ck_perVoice";
            this.Ck_perVoice.Size = new System.Drawing.Size(189, 27);
            this.Ck_perVoice.TabIndex = 171;
            this.Ck_perVoice.Text = "Prepare voice segments";
            this.Ck_perVoice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Ck_perVoice.UseVisualStyleBackColor = false;
            // 
            // lioNetEmotions
            // 
            this.lioNetEmotions.Text = "LioNet Emotions";
            this.lioNetEmotions.Width = 173;
            // 
            // lioNetAnalyses
            // 
            this.lioNetAnalyses.Text = "LioNet Analyses";
            this.lioNetAnalyses.Width = 205;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Controls.Add(this.segmentStats);
            this.panel1.Controls.Add(this.graphView);
            this.panel1.Location = new System.Drawing.Point(12, 623);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(858, 103);
            this.panel1.TabIndex = 2;
            // 
            // segmentStats
            // 
            this.segmentStats.AutoSize = true;
            this.segmentStats.ForeColor = System.Drawing.Color.White;
            this.segmentStats.Location = new System.Drawing.Point(1, 2);
            this.segmentStats.Name = "segmentStats";
            this.segmentStats.Size = new System.Drawing.Size(0, 13);
            this.segmentStats.TabIndex = 1;
            this.segmentStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // graphView
            // 
            this.graphView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graphView.DrawWidth = 1;
            this.graphView.Location = new System.Drawing.Point(0, 18);
            this.graphView.Margin = new System.Windows.Forms.Padding(0);
            this.graphView.Name = "graphView";
            this.graphView.ScaleHeight = 150;
            this.graphView.ScaleLeft = 0;
            this.graphView.ScaleTop = 0;
            this.graphView.ScaleWidth = 150;
            this.graphView.Size = new System.Drawing.Size(858, 82);
            this.graphView.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "WAVE Files|*.wav";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 10;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 210;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.label15);
            this.tabPage7.Controls.Add(this.label16);
            this.tabPage7.Controls.Add(this.pictureBox1);
            this.tabPage7.Controls.Add(this.pictureBox2);
            this.tabPage7.Controls.Add(this.groupBox10);
            this.tabPage7.Controls.Add(this.groupBox11);
            this.tabPage7.Controls.Add(this.groupBox12);
            this.tabPage7.Controls.Add(this.groupBox13);
            this.tabPage7.Controls.Add(this.groupBox14);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(850, 536);
            this.tabPage7.TabIndex = 0;
            this.tabPage7.Text = "Real Time";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.Black;
            this.label15.Cursor = System.Windows.Forms.Cursors.Default;
            this.label15.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.label15.Location = new System.Drawing.Point(409, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(29, 19);
            this.label15.TabIndex = 44;
            this.label15.Text = "-";
            this.label15.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.Black;
            this.label16.Cursor = System.Windows.Forms.Cursors.Default;
            this.label16.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.label16.Location = new System.Drawing.Point(409, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 19);
            this.label16.TabIndex = 45;
            this.label16.Text = "-";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Teal;
            this.pictureBox1.DrawWidth = 1;
            this.pictureBox1.Location = new System.Drawing.Point(400, 137);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.ScaleHeight = 150;
            this.pictureBox1.ScaleLeft = 0;
            this.pictureBox1.ScaleTop = 0;
            this.pictureBox1.ScaleWidth = 150;
            this.pictureBox1.Size = new System.Drawing.Size(49, 68);
            this.pictureBox1.TabIndex = 180;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Black;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox2.DrawWidth = 1;
            this.pictureBox2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pictureBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pictureBox2.Location = new System.Drawing.Point(398, 20);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.ScaleHeight = 150;
            this.pictureBox2.ScaleLeft = 0;
            this.pictureBox2.ScaleTop = 0;
            this.pictureBox2.ScaleWidth = 150;
            this.pictureBox2.Size = new System.Drawing.Size(53, 187);
            this.pictureBox2.TabIndex = 179;
            // 
            // groupBox10
            // 
            this.groupBox10.BackColor = System.Drawing.Color.White;
            this.groupBox10.Controls.Add(this.nmsHSAhistoryBar3);
            this.groupBox10.Controls.Add(this.nmsHSAhistoryBar4);
            this.groupBox10.Controls.Add(this.nmsAShistoryBar2);
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Controls.Add(this.label18);
            this.groupBox10.Controls.Add(this.label19);
            this.groupBox10.Location = new System.Drawing.Point(8, 213);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(374, 234);
            this.groupBox10.TabIndex = 178;
            this.groupBox10.TabStop = false;
            // 
            // nmsHSAhistoryBar3
            // 
            this.nmsHSAhistoryBar3.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsHSAhistoryBar3.colorAtmos = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar3.colorContent = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar3.colorUpset = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsHSAhistoryBar3.Location = new System.Drawing.Point(11, 160);
            this.nmsHSAhistoryBar3.Name = "nmsHSAhistoryBar3";
            this.nmsHSAhistoryBar3.NumberOfSegmentsToShow = ((short)(100));
            this.nmsHSAhistoryBar3.Size = new System.Drawing.Size(353, 43);
            this.nmsHSAhistoryBar3.TabIndex = 188;
            // 
            // nmsHSAhistoryBar4
            // 
            this.nmsHSAhistoryBar4.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsHSAhistoryBar4.colorAtmos = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar4.colorContent = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar4.colorUpset = System.Drawing.Color.Black;
            this.nmsHSAhistoryBar4.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsHSAhistoryBar4.Location = new System.Drawing.Point(11, 87);
            this.nmsHSAhistoryBar4.Name = "nmsHSAhistoryBar4";
            this.nmsHSAhistoryBar4.NumberOfSegmentsToShow = ((short)(100));
            this.nmsHSAhistoryBar4.Size = new System.Drawing.Size(353, 43);
            this.nmsHSAhistoryBar4.TabIndex = 187;
            // 
            // nmsAShistoryBar2
            // 
            this.nmsAShistoryBar2.BorderType = System.Windows.Forms.BorderStyle.None;
            this.nmsAShistoryBar2.colorAnger = System.Drawing.Color.Black;
            this.nmsAShistoryBar2.colorStress = System.Drawing.Color.Black;
            this.nmsAShistoryBar2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nmsAShistoryBar2.Location = new System.Drawing.Point(11, 15);
            this.nmsAShistoryBar2.Name = "nmsAShistoryBar2";
            this.nmsAShistoryBar2.showNoSegments = ((short)(100));
            this.nmsAShistoryBar2.Size = new System.Drawing.Size(353, 43);
            this.nmsAShistoryBar2.TabIndex = 186;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.White;
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.Cursor = System.Windows.Forms.Cursors.Default;
            this.label17.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label17.Location = new System.Drawing.Point(11, 58);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(154, 16);
            this.label17.TabIndex = 185;
            this.label17.Text = "Yellow = Stress, Red = Anger";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.White;
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label18.Cursor = System.Windows.Forms.Cursors.Default;
            this.label18.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label18.Location = new System.Drawing.Point(11, 130);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(255, 16);
            this.label18.TabIndex = 184;
            this.label18.Text = "Green = Content, Red = Upset, White Line = Atmos.";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.White;
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label19.Cursor = System.Windows.Forms.Cursors.Default;
            this.label19.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label19.Location = new System.Drawing.Point(11, 203);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(276, 16);
            this.label19.TabIndex = 183;
            this.label19.Text = "Green = Brain P., Red = Emo/Cog, Yellow Line = Energy";
            // 
            // groupBox11
            // 
            this.groupBox11.BackColor = System.Drawing.Color.White;
            this.groupBox11.Controls.Add(this.label20);
            this.groupBox11.Controls.Add(this.checkBox1);
            this.groupBox11.Controls.Add(this.checkBox2);
            this.groupBox11.Location = new System.Drawing.Point(6, 453);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(835, 80);
            this.groupBox11.TabIndex = 177;
            this.groupBox11.TabStop = false;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.White;
            this.label20.Cursor = System.Windows.Forms.Cursors.Default;
            this.label20.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.Location = new System.Drawing.Point(6, 16);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(816, 18);
            this.label20.TabIndex = 125;
            this.label20.Text = "Call Priority:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // checkBox1
            // 
            this.checkBox1.BackColor = System.Drawing.Color.White;
            this.checkBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox1.Location = new System.Drawing.Point(419, 53);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(165, 21);
            this.checkBox1.TabIndex = 167;
            this.checkBox1.Text = "Auto start play if high Priority";
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // checkBox2
            // 
            this.checkBox2.BackColor = System.Drawing.Color.White;
            this.checkBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox2.Location = new System.Drawing.Point(623, 53);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(199, 21);
            this.checkBox2.TabIndex = 115;
            this.checkBox2.Text = "Play Segments as they are created";
            this.checkBox2.UseVisualStyleBackColor = false;
            // 
            // groupBox12
            // 
            this.groupBox12.BackColor = System.Drawing.Color.White;
            this.groupBox12.Controls.Add(this.label21);
            this.groupBox12.Controls.Add(this.textBox1);
            this.groupBox12.Controls.Add(this.label22);
            this.groupBox12.Controls.Add(this.label23);
            this.groupBox12.Controls.Add(this.label24);
            this.groupBox12.Controls.Add(this.label25);
            this.groupBox12.Controls.Add(this.label26);
            this.groupBox12.Controls.Add(this.textBox2);
            this.groupBox12.Controls.Add(this.label27);
            this.groupBox12.Controls.Add(this.textBox3);
            this.groupBox12.Controls.Add(this.label28);
            this.groupBox12.Controls.Add(this.textBox4);
            this.groupBox12.Controls.Add(this.checkBox3);
            this.groupBox12.Controls.Add(this.checkBox4);
            this.groupBox12.Controls.Add(this.checkBox5);
            this.groupBox12.Controls.Add(this.checkBox6);
            this.groupBox12.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox12.Location = new System.Drawing.Point(390, 213);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(451, 234);
            this.groupBox12.TabIndex = 176;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Define Real-Time alarm settings:";
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.SystemColors.Window;
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label21.Cursor = System.Windows.Forms.Cursors.Default;
            this.label21.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label21.Location = new System.Drawing.Point(400, 149);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 20);
            this.label21.TabIndex = 173;
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.Location = new System.Drawing.Point(369, 149);
            this.textBox1.MaxLength = 0;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(23, 20);
            this.textBox1.TabIndex = 165;
            this.textBox1.Text = "2";
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.SystemColors.Window;
            this.label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label22.Cursor = System.Windows.Forms.Cursors.Default;
            this.label22.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label22.Location = new System.Drawing.Point(400, 113);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(35, 20);
            this.label22.TabIndex = 174;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label23.Cursor = System.Windows.Forms.Cursors.Default;
            this.label23.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label23.Location = new System.Drawing.Point(18, 191);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(417, 28);
            this.label23.TabIndex = 177;
            this.label23.Text = "Speaker is tired or depressd";
            this.label23.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label23.Visible = false;
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.Red;
            this.label24.Cursor = System.Windows.Forms.Cursors.Default;
            this.label24.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label24.Location = new System.Drawing.Point(18, 191);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(417, 28);
            this.label24.TabIndex = 130;
            this.label24.Text = "Call is out of acceptable levels";
            this.label24.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label24.Visible = false;
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label25.Cursor = System.Windows.Forms.Cursors.Default;
            this.label25.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label25.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label25.Location = new System.Drawing.Point(18, 191);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(417, 28);
            this.label25.TabIndex = 162;
            this.label25.Text = "Stress level is raising...";
            this.label25.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label25.Visible = false;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.Green;
            this.label26.Cursor = System.Windows.Forms.Cursors.Default;
            this.label26.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label26.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label26.Location = new System.Drawing.Point(18, 191);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(415, 28);
            this.label26.TabIndex = 163;
            this.label26.Text = "Call is within acceptable levels";
            this.label26.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBox2
            // 
            this.textBox2.AcceptsReturn = true;
            this.textBox2.BackColor = System.Drawing.SystemColors.Window;
            this.textBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox2.Location = new System.Drawing.Point(369, 113);
            this.textBox2.MaxLength = 0;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(23, 20);
            this.textBox2.TabIndex = 166;
            this.textBox2.Text = "6";
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.SystemColors.Window;
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label27.Cursor = System.Windows.Forms.Cursors.Default;
            this.label27.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label27.Location = new System.Drawing.Point(400, 75);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(35, 20);
            this.label27.TabIndex = 175;
            // 
            // textBox3
            // 
            this.textBox3.AcceptsReturn = true;
            this.textBox3.BackColor = System.Drawing.SystemColors.Window;
            this.textBox3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox3.Location = new System.Drawing.Point(369, 75);
            this.textBox3.MaxLength = 0;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(23, 20);
            this.textBox3.TabIndex = 167;
            this.textBox3.Text = "1.5";
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.SystemColors.Window;
            this.label28.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label28.Cursor = System.Windows.Forms.Cursors.Default;
            this.label28.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label28.Location = new System.Drawing.Point(400, 39);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(35, 20);
            this.label28.TabIndex = 176;
            // 
            // textBox4
            // 
            this.textBox4.AcceptsReturn = true;
            this.textBox4.BackColor = System.Drawing.SystemColors.Window;
            this.textBox4.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox4.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox4.Location = new System.Drawing.Point(369, 39);
            this.textBox4.MaxLength = 0;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(23, 20);
            this.textBox4.TabIndex = 168;
            this.textBox4.Text = "6";
            // 
            // checkBox3
            // 
            this.checkBox3.BackColor = System.Drawing.Color.White;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.checkBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox3.Location = new System.Drawing.Point(22, 149);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(286, 18);
            this.checkBox3.TabIndex = 169;
            this.checkBox3.Text = "Alarm if ENERGY level is below (Tiredness):";
            this.checkBox3.UseVisualStyleBackColor = false;
            // 
            // checkBox4
            // 
            this.checkBox4.BackColor = System.Drawing.Color.White;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.checkBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox4.Location = new System.Drawing.Point(22, 109);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(336, 28);
            this.checkBox4.TabIndex = 170;
            this.checkBox4.Text = "Alarm if ENERGY trend is raising for (no.) segments :";
            this.checkBox4.UseVisualStyleBackColor = false;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.BackColor = System.Drawing.Color.White;
            this.checkBox5.Checked = true;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.checkBox5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox5.Location = new System.Drawing.Point(22, 38);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(315, 20);
            this.checkBox5.TabIndex = 172;
            this.checkBox5.Text = "Alarm if STRESS trend level is raising and above :";
            this.checkBox5.UseVisualStyleBackColor = false;
            // 
            // checkBox6
            // 
            this.checkBox6.BackColor = System.Drawing.Color.White;
            this.checkBox6.Checked = true;
            this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox6.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.checkBox6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox6.Location = new System.Drawing.Point(22, 77);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(299, 18);
            this.checkBox6.TabIndex = 171;
            this.checkBox6.Text = "Alarm if ANGER trend level is above :";
            this.checkBox6.UseVisualStyleBackColor = false;
            // 
            // groupBox13
            // 
            this.groupBox13.BackColor = System.Drawing.Color.White;
            this.groupBox13.Controls.Add(this.progressBar1);
            this.groupBox13.Controls.Add(this.label29);
            this.groupBox13.Controls.Add(this.progressBar2);
            this.groupBox13.Controls.Add(this.progressBar3);
            this.groupBox13.Controls.Add(this.label30);
            this.groupBox13.Controls.Add(this.progressBar4);
            this.groupBox13.Controls.Add(this.progressBar5);
            this.groupBox13.Controls.Add(this.label31);
            this.groupBox13.Controls.Add(this.progressBar6);
            this.groupBox13.Controls.Add(this.label32);
            this.groupBox13.Controls.Add(this.label33);
            this.groupBox13.Controls.Add(this.label34);
            this.groupBox13.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox13.Location = new System.Drawing.Point(465, 6);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(376, 201);
            this.groupBox13.TabIndex = 167;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Current Profile: (online)";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(128, 174);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(231, 20);
            this.progressBar1.TabIndex = 47;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.White;
            this.label29.Cursor = System.Windows.Forms.Cursors.Default;
            this.label29.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label29.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label29.Location = new System.Drawing.Point(18, 172);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(104, 22);
            this.label29.TabIndex = 48;
            this.label29.Text = "Thoughful:";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(128, 146);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(231, 20);
            this.progressBar2.TabIndex = 46;
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(128, 118);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(231, 20);
            this.progressBar3.TabIndex = 45;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.White;
            this.label30.Cursor = System.Windows.Forms.Cursors.Default;
            this.label30.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label30.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label30.Location = new System.Drawing.Point(18, 144);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(104, 22);
            this.label30.TabIndex = 49;
            this.label30.Text = "Energetic:";
            // 
            // progressBar4
            // 
            this.progressBar4.Location = new System.Drawing.Point(128, 90);
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.Size = new System.Drawing.Size(231, 20);
            this.progressBar4.TabIndex = 44;
            // 
            // progressBar5
            // 
            this.progressBar5.Location = new System.Drawing.Point(128, 62);
            this.progressBar5.Name = "progressBar5";
            this.progressBar5.Size = new System.Drawing.Size(231, 20);
            this.progressBar5.TabIndex = 43;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.White;
            this.label31.Cursor = System.Windows.Forms.Cursors.Default;
            this.label31.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label31.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label31.Location = new System.Drawing.Point(18, 116);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(104, 22);
            this.label31.TabIndex = 50;
            this.label31.Text = "Stressed:";
            // 
            // progressBar6
            // 
            this.progressBar6.Location = new System.Drawing.Point(128, 34);
            this.progressBar6.Name = "progressBar6";
            this.progressBar6.Size = new System.Drawing.Size(231, 20);
            this.progressBar6.TabIndex = 42;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.White;
            this.label32.Cursor = System.Windows.Forms.Cursors.Default;
            this.label32.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label32.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label32.Location = new System.Drawing.Point(18, 88);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(104, 22);
            this.label32.TabIndex = 51;
            this.label32.Text = "Hesitant:";
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.White;
            this.label33.Cursor = System.Windows.Forms.Cursors.Default;
            this.label33.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label33.Location = new System.Drawing.Point(18, 60);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(104, 22);
            this.label33.TabIndex = 52;
            this.label33.Text = "Logical:";
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.White;
            this.label34.Cursor = System.Windows.Forms.Cursors.Default;
            this.label34.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label34.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label34.Location = new System.Drawing.Point(18, 32);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(104, 22);
            this.label34.TabIndex = 53;
            this.label34.Text = "Emotional:";
            // 
            // groupBox14
            // 
            this.groupBox14.BackColor = System.Drawing.Color.White;
            this.groupBox14.Controls.Add(this.label35);
            this.groupBox14.Controls.Add(this.progressBar7);
            this.groupBox14.Controls.Add(this.label36);
            this.groupBox14.Controls.Add(this.progressBar8);
            this.groupBox14.Controls.Add(this.label37);
            this.groupBox14.Controls.Add(this.progressBar9);
            this.groupBox14.Controls.Add(this.label38);
            this.groupBox14.Controls.Add(this.progressBar10);
            this.groupBox14.Controls.Add(this.label39);
            this.groupBox14.Controls.Add(this.progressBar11);
            this.groupBox14.Controls.Add(this.label40);
            this.groupBox14.Controls.Add(this.progressBar12);
            this.groupBox14.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox14.Location = new System.Drawing.Point(6, 6);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(376, 201);
            this.groupBox14.TabIndex = 166;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Base Emotional Profile:";
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.White;
            this.label35.Cursor = System.Windows.Forms.Cursors.Default;
            this.label35.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label35.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label35.Location = new System.Drawing.Point(18, 62);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(104, 22);
            this.label35.TabIndex = 36;
            this.label35.Text = "Logical:";
            // 
            // progressBar7
            // 
            this.progressBar7.Location = new System.Drawing.Point(128, 34);
            this.progressBar7.Name = "progressBar7";
            this.progressBar7.Size = new System.Drawing.Size(231, 20);
            this.progressBar7.TabIndex = 29;
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.White;
            this.label36.Cursor = System.Windows.Forms.Cursors.Default;
            this.label36.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label36.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label36.Location = new System.Drawing.Point(18, 90);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(104, 22);
            this.label36.TabIndex = 37;
            this.label36.Text = "Hesitant:";
            // 
            // progressBar8
            // 
            this.progressBar8.Location = new System.Drawing.Point(128, 62);
            this.progressBar8.Name = "progressBar8";
            this.progressBar8.Size = new System.Drawing.Size(231, 20);
            this.progressBar8.TabIndex = 30;
            // 
            // label37
            // 
            this.label37.BackColor = System.Drawing.Color.White;
            this.label37.Cursor = System.Windows.Forms.Cursors.Default;
            this.label37.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label37.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label37.Location = new System.Drawing.Point(18, 118);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(104, 22);
            this.label37.TabIndex = 38;
            this.label37.Text = "Stressed:";
            // 
            // progressBar9
            // 
            this.progressBar9.Location = new System.Drawing.Point(128, 90);
            this.progressBar9.Name = "progressBar9";
            this.progressBar9.Size = new System.Drawing.Size(231, 20);
            this.progressBar9.TabIndex = 31;
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.White;
            this.label38.Cursor = System.Windows.Forms.Cursors.Default;
            this.label38.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label38.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label38.Location = new System.Drawing.Point(18, 146);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(104, 22);
            this.label38.TabIndex = 39;
            this.label38.Text = "Energetic:";
            // 
            // progressBar10
            // 
            this.progressBar10.Location = new System.Drawing.Point(128, 118);
            this.progressBar10.Name = "progressBar10";
            this.progressBar10.Size = new System.Drawing.Size(231, 20);
            this.progressBar10.TabIndex = 32;
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.White;
            this.label39.Cursor = System.Windows.Forms.Cursors.Default;
            this.label39.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label39.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label39.Location = new System.Drawing.Point(18, 174);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(104, 22);
            this.label39.TabIndex = 40;
            this.label39.Text = "Thoughtful:";
            // 
            // progressBar11
            // 
            this.progressBar11.Location = new System.Drawing.Point(128, 146);
            this.progressBar11.Name = "progressBar11";
            this.progressBar11.Size = new System.Drawing.Size(231, 20);
            this.progressBar11.TabIndex = 33;
            // 
            // label40
            // 
            this.label40.BackColor = System.Drawing.Color.White;
            this.label40.Cursor = System.Windows.Forms.Cursors.Default;
            this.label40.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label40.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label40.Location = new System.Drawing.Point(18, 34);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(104, 22);
            this.label40.TabIndex = 35;
            this.label40.Text = "Emotional:";
            // 
            // progressBar12
            // 
            this.progressBar12.Location = new System.Drawing.Point(128, 174);
            this.progressBar12.Name = "progressBar12";
            this.progressBar12.Size = new System.Drawing.Size(231, 20);
            this.progressBar12.TabIndex = 34;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.label41);
            this.tabPage8.Controls.Add(this.listView1);
            this.tabPage8.Controls.Add(this.checkBox7);
            this.tabPage8.Controls.Add(this.pictureBox3);
            this.tabPage8.Controls.Add(this.colorMap2);
            this.tabPage8.Controls.Add(this.listView2);
            this.tabPage8.Controls.Add(this.groupBox15);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(850, 536);
            this.tabPage8.TabIndex = 1;
            this.tabPage8.Text = "Segment View";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            this.label41.BackColor = System.Drawing.Color.Black;
            this.label41.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label41.Location = new System.Drawing.Point(6, 11);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(640, 454);
            this.label41.TabIndex = 83;
            this.label41.Text = "Processing Data";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label41.Visible = false;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(6, 474);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(269, 54);
            this.listView1.TabIndex = 82;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Width = 200;
            // 
            // checkBox7
            // 
            this.checkBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox7.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox7.Location = new System.Drawing.Point(281, 472);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(63, 56);
            this.checkBox7.TabIndex = 81;
            this.checkBox7.Text = "Use same graph";
            this.checkBox7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox3.Location = new System.Drawing.Point(588, 474);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(57, 56);
            this.pictureBox3.TabIndex = 79;
            this.pictureBox3.TabStop = false;
            // 
            // colorMap2
            // 
            this.colorMap2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorMap2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorMap2.Location = new System.Drawing.Point(350, 474);
            this.colorMap2.Name = "colorMap2";
            this.colorMap2.Size = new System.Drawing.Size(229, 56);
            this.colorMap2.TabIndex = 80;
            // 
            // listView2
            // 
            this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(6, 11);
            this.listView2.Name = "listView2";
            this.listView2.ShowGroups = false;
            this.listView2.Size = new System.Drawing.Size(640, 454);
            this.listView2.TabIndex = 77;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // groupBox15
            // 
            this.groupBox15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox15.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox15.Controls.Add(this.listView3);
            this.groupBox15.Controls.Add(this.button1);
            this.groupBox15.Controls.Add(this.button2);
            this.groupBox15.Controls.Add(this.button3);
            this.groupBox15.Controls.Add(this.textBox5);
            this.groupBox15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.groupBox15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox15.Location = new System.Drawing.Point(652, 11);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox15.Size = new System.Drawing.Size(189, 519);
            this.groupBox15.TabIndex = 76;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = " Train LioNet System ";
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8});
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView3.Location = new System.Drawing.Point(7, 74);
            this.listView3.MultiSelect = false;
            this.listView3.Name = "listView3";
            this.listView3.ShowGroups = false;
            this.listView3.Size = new System.Drawing.Size(177, 326);
            this.listView3.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView3.TabIndex = 80;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Width = 172;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(6, 463);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 48);
            this.button1.TabIndex = 79;
            this.button1.Text = "Net Forget!";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Cursor = System.Windows.Forms.Cursors.Default;
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Location = new System.Drawing.Point(7, 406);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 48);
            this.button2.TabIndex = 78;
            this.button2.Text = "Classify this segment";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Cursor = System.Windows.Forms.Cursors.Default;
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(6, 42);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 26);
            this.button3.TabIndex = 77;
            this.button3.Text = "Add as new emotion";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // textBox5
            // 
            this.textBox5.AcceptsReturn = true;
            this.textBox5.BackColor = System.Drawing.SystemColors.Window;
            this.textBox5.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox5.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox5.Location = new System.Drawing.Point(6, 16);
            this.textBox5.MaxLength = 0;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(178, 20);
            this.textBox5.TabIndex = 76;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.groupBox16);
            this.tabPage9.Controls.Add(this.label43);
            this.tabPage9.Controls.Add(this.label44);
            this.tabPage9.Controls.Add(this.textBox7);
            this.tabPage9.Controls.Add(this.label45);
            this.tabPage9.Controls.Add(this.textBox8);
            this.tabPage9.Controls.Add(this.label46);
            this.tabPage9.Controls.Add(this.label47);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(850, 536);
            this.tabPage9.TabIndex = 3;
            this.tabPage9.Text = "Emotional Signature";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.label42);
            this.groupBox16.Controls.Add(this.button4);
            this.groupBox16.Controls.Add(this.button5);
            this.groupBox16.Controls.Add(this.listBox1);
            this.groupBox16.Controls.Add(this.button6);
            this.groupBox16.Controls.Add(this.textBox6);
            this.groupBox16.Location = new System.Drawing.Point(634, 40);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(206, 482);
            this.groupBox16.TabIndex = 8;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = " Train LioNet System - Complete Calls ";
            // 
            // label42
            // 
            this.label42.BackColor = System.Drawing.Color.Black;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ForeColor = System.Drawing.SystemColors.Info;
            this.label42.Location = new System.Drawing.Point(7, 431);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(193, 39);
            this.label42.TabIndex = 5;
            this.label42.Text = "LioNet Accuracy:0%";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.LightCoral;
            this.button4.Location = new System.Drawing.Point(7, 397);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(193, 27);
            this.button4.TabIndex = 4;
            this.button4.Text = "Make LioNet forget this answer";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(7, 351);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(193, 40);
            this.button5.TabIndex = 3;
            this.button5.Text = "Classify This Call";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 81);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(193, 264);
            this.listBox1.TabIndex = 2;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(7, 47);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(193, 27);
            this.button6.TabIndex = 1;
            this.button6.Text = "Add New Call Profile";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(7, 20);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(193, 20);
            this.textBox6.TabIndex = 0;
            // 
            // label43
            // 
            this.label43.BackColor = System.Drawing.Color.Black;
            this.label43.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ForeColor = System.Drawing.SystemColors.Info;
            this.label43.Location = new System.Drawing.Point(12, 481);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(611, 42);
            this.label43.TabIndex = 7;
            this.label43.Text = "-No analysis-";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label43, "LioNet analysis for this Emotional Signature");
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(9, 464);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(105, 13);
            this.label44.TabIndex = 5;
            this.label44.Text = "QA5 LioNet analysis:";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(9, 361);
            this.textBox7.Multiline = true;
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(614, 96);
            this.textBox7.TabIndex = 4;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(6, 344);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(198, 13);
            this.label45.TabIndex = 3;
            this.label45.Text = "LioNet \"Emotional Signature\" for this call";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(6, 56);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox8.Size = new System.Drawing.Size(617, 281);
            this.textBox8.TabIndex = 2;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(3, 40);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(63, 13);
            this.label46.TabIndex = 1;
            this.label46.Text = "QA5 Report";
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.Black;
            this.label47.Dock = System.Windows.Forms.DockStyle.Top;
            this.label47.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.ForeColor = System.Drawing.SystemColors.Info;
            this.label47.Location = new System.Drawing.Point(0, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(850, 31);
            this.label47.TabIndex = 0;
            this.label47.Text = "LioNet Call (Emotional Signature) training system";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.button7);
            this.tabPage10.Controls.Add(this.button8);
            this.tabPage10.Controls.Add(this.listView4);
            this.tabPage10.Controls.Add(this.label48);
            this.tabPage10.Controls.Add(this.textBox9);
            this.tabPage10.Controls.Add(this.label49);
            this.tabPage10.Controls.Add(this.label50);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Size = new System.Drawing.Size(850, 536);
            this.tabPage10.TabIndex = 4;
            this.tabPage10.Text = "Envelop & Borders";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(656, 186);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(185, 45);
            this.button7.TabIndex = 7;
            this.button7.Text = "Show On Graph";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(8, 474);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(265, 45);
            this.button8.TabIndex = 6;
            this.button8.Text = "Update the \"Known Envelope && Borders\" Data File";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // listView4
            // 
            this.listView4.FullRowSelect = true;
            this.listView4.GridLines = true;
            this.listView4.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView4.Location = new System.Drawing.Point(11, 259);
            this.listView4.MultiSelect = false;
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(830, 209);
            this.listView4.TabIndex = 5;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(8, 243);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(177, 13);
            this.label48.TabIndex = 4;
            this.label48.Text = "Conversation Envelope and Borders";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(8, 52);
            this.textBox9.Multiline = true;
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox9.Size = new System.Drawing.Size(642, 179);
            this.textBox9.TabIndex = 3;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(5, 35);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(147, 13);
            this.label49.TabIndex = 2;
            this.label49.Text = "Envelope and Borders Report";
            // 
            // label50
            // 
            this.label50.BackColor = System.Drawing.Color.Black;
            this.label50.Dock = System.Windows.Forms.DockStyle.Top;
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ForeColor = System.Drawing.SystemColors.Info;
            this.label50.Location = new System.Drawing.Point(0, 0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(850, 31);
            this.label50.TabIndex = 1;
            this.label50.Text = "Envelop && Borders";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage11
            // 
            this.tabPage11.Controls.Add(this.listView5);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Size = new System.Drawing.Size(850, 536);
            this.tabPage11.TabIndex = 5;
            this.tabPage11.Text = "Tests Database";
            this.tabPage11.UseVisualStyleBackColor = true;
            // 
            // listView5
            // 
            this.listView5.AllowDrop = true;
            this.listView5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView5.FullRowSelect = true;
            this.listView5.GridLines = true;
            this.listView5.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView5.HideSelection = false;
            this.listView5.Location = new System.Drawing.Point(0, 0);
            this.listView5.MultiSelect = false;
            this.listView5.Name = "listView5";
            this.listView5.Size = new System.Drawing.Size(850, 536);
            this.listView5.TabIndex = 0;
            this.listView5.UseCompatibleStateImageBehavior = false;
            this.listView5.View = System.Windows.Forms.View.Details;
            // 
            // tabPage12
            // 
            this.tabPage12.Controls.Add(this.checkBox8);
            this.tabPage12.Controls.Add(this.groupBox17);
            this.tabPage12.Controls.Add(this.groupBox18);
            this.tabPage12.Location = new System.Drawing.Point(4, 22);
            this.tabPage12.Name = "tabPage12";
            this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage12.Size = new System.Drawing.Size(850, 536);
            this.tabPage12.TabIndex = 2;
            this.tabPage12.Text = "Settings & License Data";
            this.tabPage12.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            this.checkBox8.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox8.Location = new System.Drawing.Point(652, 493);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(189, 27);
            this.checkBox8.TabIndex = 173;
            this.checkBox8.Text = "No UI Updates (For Speed Tests)";
            this.checkBox8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.button9);
            this.groupBox17.Controls.Add(this.textBox10);
            this.groupBox17.Controls.Add(this.label51);
            this.groupBox17.Controls.Add(this.label52);
            this.groupBox17.Controls.Add(this.button10);
            this.groupBox17.Controls.Add(this.textBox11);
            this.groupBox17.Controls.Add(this.label53);
            this.groupBox17.Controls.Add(this.textBox12);
            this.groupBox17.Controls.Add(this.label54);
            this.groupBox17.Controls.Add(this.textBox13);
            this.groupBox17.Controls.Add(this.label55);
            this.groupBox17.Controls.Add(this.textBox14);
            this.groupBox17.Controls.Add(this.label56);
            this.groupBox17.Controls.Add(this.textBox15);
            this.groupBox17.Controls.Add(this.label57);
            this.groupBox17.Location = new System.Drawing.Point(7, 174);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(834, 241);
            this.groupBox17.TabIndex = 167;
            this.groupBox17.TabStop = false;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(639, 187);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 37);
            this.button9.TabIndex = 14;
            this.button9.Text = "Apply";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(21, 204);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(612, 20);
            this.textBox10.TabIndex = 13;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(18, 187);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(108, 13);
            this.label51.TabIndex = 12;
            this.label51.Text = "Counters Reset Code";
            // 
            // label52
            // 
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.Red;
            this.label52.Location = new System.Drawing.Point(18, 150);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(465, 30);
            this.label52.TabIndex = 11;
            this.label52.Text = "To reset the counters, you will need to obtain an unique code from Nemesysco. Ple" +
                "ase contact Nemesysco\'s representative to obtain the code.";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(419, 75);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(147, 36);
            this.button10.TabIndex = 10;
            this.button10.Text = "Retrieve License Details";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(419, 41);
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(147, 20);
            this.textBox11.TabIndex = 9;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(416, 25);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(91, 13);
            this.label53.TabIndex = 8;
            this.label53.Text = "QA5 Core Version";
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(215, 91);
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.Size = new System.Drawing.Size(147, 20);
            this.textBox12.TabIndex = 7;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(212, 75);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(131, 13);
            this.label54.TabIndex = 6;
            this.label54.Text = "No. of Processes Running";
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(215, 41);
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(147, 20);
            this.textBox13.TabIndex = 5;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(212, 25);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(106, 13);
            this.label55.TabIndex = 4;
            this.label55.Text = "Current Calls Counter";
            // 
            // textBox14
            // 
            this.textBox14.Location = new System.Drawing.Point(21, 91);
            this.textBox14.Name = "textBox14";
            this.textBox14.ReadOnly = true;
            this.textBox14.Size = new System.Drawing.Size(147, 20);
            this.textBox14.TabIndex = 3;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(18, 75);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(79, 13);
            this.label56.TabIndex = 2;
            this.label56.Text = "Licensed Posts";
            // 
            // textBox15
            // 
            this.textBox15.Location = new System.Drawing.Point(21, 41);
            this.textBox15.Name = "textBox15";
            this.textBox15.ReadOnly = true;
            this.textBox15.Size = new System.Drawing.Size(147, 20);
            this.textBox15.TabIndex = 1;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(18, 25);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(55, 13);
            this.label57.TabIndex = 0;
            this.label57.Text = "System ID";
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.textBox16);
            this.groupBox18.Controls.Add(this.label58);
            this.groupBox18.Controls.Add(this.checkBox9);
            this.groupBox18.Controls.Add(this.checkBox10);
            this.groupBox18.Controls.Add(this.checkBox11);
            this.groupBox18.Controls.Add(this.checkBox12);
            this.groupBox18.Controls.Add(this.checkBox13);
            this.groupBox18.Location = new System.Drawing.Point(7, 31);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(834, 125);
            this.groupBox18.TabIndex = 166;
            this.groupBox18.TabStop = false;
            // 
            // textBox16
            // 
            this.textBox16.AcceptsReturn = true;
            this.textBox16.BackColor = System.Drawing.SystemColors.Window;
            this.textBox16.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox16.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox16.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox16.Location = new System.Drawing.Point(171, 81);
            this.textBox16.MaxLength = 0;
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new System.Drawing.Size(39, 20);
            this.textBox16.TabIndex = 173;
            this.textBox16.Text = "1000";
            // 
            // label58
            // 
            this.label58.BackColor = System.Drawing.Color.Transparent;
            this.label58.Cursor = System.Windows.Forms.Cursors.Default;
            this.label58.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label58.Location = new System.Drawing.Point(18, 84);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(131, 16);
            this.label58.TabIndex = 174;
            this.label58.Text = "Background Noise level";
            // 
            // checkBox9
            // 
            this.checkBox9.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox9.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox9.Checked = true;
            this.checkBox9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox9.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox9.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.checkBox9.Location = new System.Drawing.Point(460, 78);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(189, 27);
            this.checkBox9.TabIndex = 170;
            this.checkBox9.Text = "Use Calibration Type 2";
            this.checkBox9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox9.UseVisualStyleBackColor = false;
            // 
            // checkBox10
            // 
            this.checkBox10.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox10.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox10.Checked = true;
            this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox10.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.checkBox10.Location = new System.Drawing.Point(460, 27);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(189, 27);
            this.checkBox10.TabIndex = 172;
            this.checkBox10.Text = "2 Seconds segments";
            this.checkBox10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox10.UseVisualStyleBackColor = false;
            // 
            // checkBox11
            // 
            this.checkBox11.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox11.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox11.Checked = true;
            this.checkBox11.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox11.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox11.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.checkBox11.Location = new System.Drawing.Point(245, 78);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(189, 27);
            this.checkBox11.TabIndex = 168;
            this.checkBox11.Text = "Use Analysis Type 2";
            this.checkBox11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox11.UseVisualStyleBackColor = false;
            // 
            // checkBox12
            // 
            this.checkBox12.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox12.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox12.Checked = true;
            this.checkBox12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox12.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox12.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.checkBox12.Location = new System.Drawing.Point(21, 27);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(189, 27);
            this.checkBox12.TabIndex = 169;
            this.checkBox12.Text = "Test CL Stress ";
            this.checkBox12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox12.UseVisualStyleBackColor = false;
            // 
            // checkBox13
            // 
            this.checkBox13.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox13.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox13.Checked = true;
            this.checkBox13.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox13.Cursor = System.Windows.Forms.Cursors.Default;
            this.checkBox13.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.checkBox13.Location = new System.Drawing.Point(245, 27);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(189, 27);
            this.checkBox13.TabIndex = 171;
            this.checkBox13.Text = "Prepare voice segments";
            this.checkBox13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox13.UseVisualStyleBackColor = false;
            // 
            // dataChannelNumber
            // 
            this.dataChannelNumber.Appearance = System.Windows.Forms.Appearance.Button;
            this.dataChannelNumber.BackColor = System.Drawing.SystemColors.Control;
            this.dataChannelNumber.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataChannelNumber.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataChannelNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.dataChannelNumber.Location = new System.Drawing.Point(632, 27);
            this.dataChannelNumber.Name = "dataChannelNumber";
            this.dataChannelNumber.Size = new System.Drawing.Size(189, 78);
            this.dataChannelNumber.TabIndex = 175;
            this.dataChannelNumber.Text = "In Case of Stereo Input, Data is on the Right Channel";
            this.dataChannelNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dataChannelNumber.UseVisualStyleBackColor = false;
            // 
            // DemoForm
            // 
            this.ClientSize = new System.Drawing.Size(882, 732);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "DemoForm";
            this.Text = "QA5 Sample Implementation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DemoForm_FormClosing);
            this.Resize += new System.EventHandler(this.DemoForm_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectedColor)).EndInit();
            this.Frame3.ResumeLayout(false);
            this.Frame3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.tabPage10.ResumeLayout(false);
            this.tabPage10.PerformLayout();
            this.tabPage11.ResumeLayout(false);
            this.tabPage12.ResumeLayout(false);
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.ResumeLayout(false);

        }

        public CheckBox alarmIfStressTrendIsRaisingAndAbove;
        public CheckBox alarmIfAngerTrendIsRaisingAndAbove;
        public CheckBox alarmIfStressTrendIsRaising;
        public CheckBox alarmIfStressTrendIsLow;
        public Label _Label15_0;
        public Label _Label15_1;
        public Label _Label15_10;
        public Label _Label15_11;
        public Label _Label15_2;
        public Label _Label15_3;
        public Label _Label15_4;
        public Label _Label15_5;
        public Label _Label15_6;
        public Label _Label15_7;
        public Label _Label15_8;
        public Label _Label15_9;
        public Label callWithinAcceptableLevelsLabel;
        public Label callIsOutOfAcceptableLevelsLabel;
        public Label stressLevelIsRaisingLabel;
        public Label speakerIsTiredLabel;
        public Label _Label8_0;
        public Label _Label8_1;
        public Label _Label8_2;
        public Label stressTrendLevel;
        public Label angerTrendLevel;
        public Label energyTrendIsRaisingFor;
        public Label energyLevelBelow;
        public ProgressBar baseProfileEmotional;
        public ProgressBar baseProfileLogical;
        public ProgressBar baseProfileHesitant;
        public ProgressBar baseProfileStressed;
        public ProgressBar baseProfileEnergetic;
        public ProgressBar baseProfileThoughtful;
        public ProgressBar currentProfileEmotional;
        public ProgressBar currentProfileLogical;
        public ProgressBar currentProfileHesitant;
        public ProgressBar currentProfileStressed;
        public ProgressBar currentProfileEnergetic;
        public ProgressBar currentProfileThoughtful;
        public TextBox maxStressTrendLevel;
        public TextBox maxAngerTrendLevel;
        public TextBox limitForEnergyTrendIsRaisingFor;
        public TextBox limitForEnergyLevelBelow;
        private Button browseForFile;
        public CheckBox ck_AutoStartPlay;
        public CheckBox ck_PlayBackRealTime;
        public Button cmdAddNewSegEmotion;
        public Button cmdClassifySeg;
        public Button cmdSegLioForget;
        private ListView colorHistory;
        private ColorMap colorMap1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private TextBox fileToProcess;
        public GroupBox Frame3;
        private ListView gridUserDefinedSegmentEmotions;
        private GroupBox groupBox1;
        internal GroupBox groupBox2;
        internal GroupBox groupBox3;
        internal GroupBox GroupBox4;
        internal GroupBox GroupBox5;
        internal GroupBox groupBox6;
        public Label lbl_enrgyDif;
        public Label lbl_enrgyHighSegs;
        public Label lblCallPriority;
        private ColumnHeader lioNetAnalyses;
        private ColumnHeader lioNetEmotions;
        internal nmsAShistoryBar nmsAShistoryBar1;
        internal nmsHSAhistoryBar nmsHSAhistoryBar1;
        internal nmsHSAhistoryBar nmsHSAhistoryBar2;
        private OpenFileDialog openFileDialog;
        private Panel panel1;
        public VB6PictureControlEmu.PictureBox Picture10;
        private ListView segmentsList;
        private Label segmentStats;
        private VB6PictureControlEmu.PictureBox graphView;
        private PictureBox selectedColor;
        private VB6PictureControlEmu.PictureBox Shape2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        public TextBox txtSegEmotion;
        private CheckBox useSameGraph;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private Label segmentsListCover;
        private TabPage tabPage4;
        private Label label1;
        private TextBox callEmotionSignature;
        private Label label3;
        private TextBox qaReport;
        private Label label2;
        private Label lioNetAnalysis;
        private ToolTip toolTip1;
        private Label label5;
        private GroupBox groupBox7;
        private TextBox newEmotionSignature;
        private Label lioNetAccuracy;
        private Button lioNetForgetAnswer;
        private Button classifyThisCall;
        private ListBox lioNetEmotionsList;
        private Button addNewEmotionalSignature;
        private GroupBox groupBox8;
        private CheckBox noRealTimeUiUpdates;
        public CheckBox ckCalType;
        public CheckBox ckSegLength;
        public CheckBox ckAnalysisType;
        public CheckBox ckStressTest;
        public CheckBox Ck_perVoice;
        private GroupBox groupBox9;
        private Label label6;
        private Button retrieveLicenseDetails;
        private TextBox qA5CoreVersion;
        private Label label10;
        private TextBox numberOfProcessesRunning;
        private Label label9;
        private TextBox currentCallsCounter;
        private Label label8;
        private TextBox licensedPosts;
        private Label label7;
        private TextBox systemId;
        private Button applyCountersResetCode;
        private TextBox countersResetCode;
        private Label label12;
        private Label label11;
        public TextBox txt_BG;
        public Label Label4;
        private TabPage tabPage5;
        private TabPage tabPage6;
        private Label label13;
        private Button showEnvelopAndBordesOnGraph;
        private Button updateKnownEnvelopeAndBordersDataFile;
        private ListView conversationEnvelopAndBorders;
        private Label conversationEnvelopAndBordersLabel;
        private TextBox envelopAndBordersReport;
        private Label label14;
        private ListView callsGrid;
        public CheckBox dataChannelNumber;
        private TabPage tabPage7;
        public Label label15;
        public Label label16;
        private VB6PictureControlEmu.PictureBox pictureBox1;
        public VB6PictureControlEmu.PictureBox pictureBox2;
        internal GroupBox groupBox10;
        internal nmsHSAhistoryBar nmsHSAhistoryBar3;
        internal nmsHSAhistoryBar nmsHSAhistoryBar4;
        internal nmsAShistoryBar nmsAShistoryBar2;
        public Label label17;
        public Label label18;
        public Label label19;
        internal GroupBox groupBox11;
        public Label label20;
        public CheckBox checkBox1;
        public CheckBox checkBox2;
        internal GroupBox groupBox12;
        public Label label21;
        public TextBox textBox1;
        public Label label22;
        public Label label23;
        public Label label24;
        public Label label25;
        public Label label26;
        public TextBox textBox2;
        public Label label27;
        public TextBox textBox3;
        public Label label28;
        public TextBox textBox4;
        public CheckBox checkBox3;
        public CheckBox checkBox4;
        public CheckBox checkBox5;
        public CheckBox checkBox6;
        internal GroupBox groupBox13;
        public ProgressBar progressBar1;
        public Label label29;
        public ProgressBar progressBar2;
        public ProgressBar progressBar3;
        public Label label30;
        public ProgressBar progressBar4;
        public ProgressBar progressBar5;
        public Label label31;
        public ProgressBar progressBar6;
        public Label label32;
        public Label label33;
        public Label label34;
        internal GroupBox groupBox14;
        public Label label35;
        public ProgressBar progressBar7;
        public Label label36;
        public ProgressBar progressBar8;
        public Label label37;
        public ProgressBar progressBar9;
        public Label label38;
        public ProgressBar progressBar10;
        public Label label39;
        public ProgressBar progressBar11;
        public Label label40;
        public ProgressBar progressBar12;
        private TabPage tabPage8;
        private Label label41;
        private ListView listView1;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private CheckBox checkBox7;
        private PictureBox pictureBox3;
        private ColorMap colorMap2;
        private ListView listView2;
        public GroupBox groupBox15;
        private ListView listView3;
        private ColumnHeader columnHeader8;
        public Button button1;
        public Button button2;
        public Button button3;
        public TextBox textBox5;
        private TabPage tabPage9;
        private GroupBox groupBox16;
        private Label label42;
        private Button button4;
        private Button button5;
        private ListBox listBox1;
        private Button button6;
        private TextBox textBox6;
        private Label label43;
        private Label label44;
        private TextBox textBox7;
        private Label label45;
        private TextBox textBox8;
        private Label label46;
        private Label label47;
        private TabPage tabPage10;
        private Button button7;
        private Button button8;
        private ListView listView4;
        private Label label48;
        private TextBox textBox9;
        private Label label49;
        private Label label50;
        private TabPage tabPage11;
        private ListView listView5;
        private TabPage tabPage12;
        private CheckBox checkBox8;
        private GroupBox groupBox17;
        private Button button9;
        private TextBox textBox10;
        private Label label51;
        private Label label52;
        private Button button10;
        private TextBox textBox11;
        private Label label53;
        private TextBox textBox12;
        private Label label54;
        private TextBox textBox13;
        private Label label55;
        private TextBox textBox14;
        private Label label56;
        private TextBox textBox15;
        private Label label57;
        private GroupBox groupBox18;
        public TextBox textBox16;
        public Label label58;
        public CheckBox checkBox9;
        public CheckBox checkBox10;
        public CheckBox checkBox11;
        public CheckBox checkBox12;
        public CheckBox checkBox13;

    }
}