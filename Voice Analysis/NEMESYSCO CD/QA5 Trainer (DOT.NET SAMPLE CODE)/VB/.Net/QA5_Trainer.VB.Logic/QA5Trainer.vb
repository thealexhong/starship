#Region "Usings"

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Globalization
Imports Nemesysco.Std.WaveLib.Native
Imports QA5_Trainer.CSharp.Interfaces
Imports Nemesysco.Std.FullDuplexStub
Imports QA5COM
Imports System.Threading
Imports System.Reflection
Imports System.Text

#End Region

Partial Public Class QA5Trainer
    Private ReadOnly callEmotions As New List(Of String)(100)
    Private WithEvents fullDuplex As New FullDplxClass()
    Private ReadOnly lioNetResultsCache As New List(Of String)(100)
    Private ReadOnly segmentEmotions As New List(Of String)(100)
    Private ReadOnly trenders As TrendAnalyzer() = New TrendAnalyzer(5) {}
    Private WithEvents ui As IQA5UI
    Private bitsPerSample As Integer
    Private bufferSize As Short
    Private conversionBuffer As IntPtr = IntPtr.Zero
    Private trainigsCount As Integer
    Private DEF_OWNER As String = "Nemesysco Ltd"
    Private bordersFileName As String
    Private directoryName As String
    Private fileName As String
    Private lioDataEmoSigFileName As String
    Private LioDataSEGMENTfile As String
    Private lioNetWasTrained As Boolean
    Private nmsCOMcallee As nmsQA5core
    Private nmsQA5LicServer As nmsQA5core
    Private segmentsFolder As String
    Private waveOutSamplesPerSecond As Short
    Private _nmsLioNet As nmsLioNetV6

    Private Property NmsLioNet() As nmsLioNetV6
        Get
            NmsLioNet = _nmsLioNet
        End Get
        Set(ByVal value As nmsLioNetV6)
            _nmsLioNet = value
        End Set
    End Property

    Private ReadOnly waitForPlaybackFinished As New ManualResetEvent(True)
    Private callBordersData As Array

    Private upsetSegments As Integer
    Private onlineFlag As String
    Private stressSegments As Integer
    Private enrgyStaysHighSegments As Integer
    Private angerSegments As Integer
    Private midEnergySegments As Integer
    Private lowEnergySegments As Integer
    Private callMaxPriorityFlag As Integer
    Private overallBordersDistance As Double
    Private callPriority As Integer
    Private lioNetResult As String
    Private processingBatch As Boolean
    Private cSvWriter As TextWriter
    Private dataFolder As String
    Private processingFileChannels As Integer
    Private splitBuffer() As Short


    Public Sub New(ByVal iQa5Ui As IQA5UI)
        If iQa5Ui Is Nothing Then
            Throw New ArgumentNullException("iQa5Ui")
        End If

        ui = iQa5Ui
    End Sub

    Private Sub fullDuplex_WaveOutEndOfFile() Handles fullDuplex.WaveOutEndOfFile
        waitForPlaybackFinished.[Set]()
    End Sub

    Private Sub ui_ShowBordersOnGraph(ByVal sender As Object, ByVal e As EventArgs) Handles ui.ShowBordersOnGraph
        Dim envelopAndBordersRanges As List(Of EnvelopData) = New List(Of EnvelopData)(52)

        For parmN As Short = 0 To 51
            Dim lowBorder As Double = 0
            Dim parmAvrg As Double = 0
            Dim highEnvelope As Double = 0
            Dim lowEnvelope As Double = 0
            Dim highBorder As Double = 0

            nmsCOMcallee.nmsBordersGetEnvelopesData(parmN, lowBorder, highBorder, lowEnvelope, highEnvelope, parmAvrg)

            envelopAndBordersRanges.Add(New EnvelopData(lowBorder, highBorder, lowEnvelope, highEnvelope, parmAvrg))
        Next

        ui.DrawBorders(DirectCast(callBordersData, Double(,)), envelopAndBordersRanges.ToArray())
    End Sub

    Private Sub ui_UpdateBordersFile(ByVal sender As Object, ByVal e As EventArgs) Handles ui.UpdateBordersFile
        nmsCOMcallee.nmsBordersUpdateWithCurrent(bordersFileName)
    End Sub

    Private Sub ui_ApplyCountersResetCode(ByVal sender As Object, ByVal e As ApplyCountersResetCodeArgs) _
        Handles ui.ApplyCountersResetCode
        Dim resetCode As String = e.ResetCode
        Dim rc As Integer = nmsQA5LicServer.nmsSRV_ResetCounter(resetCode)

        If rc = NMS_SDKERROR_OPERATIONNOTALLOWED Then
            ui.ShowMessage("Illegal license or operation failed")
        Else
            ui.ShowMessage("Operation Succeeded")
        End If
    End Sub

    Private Sub ui_RetrieveLicenseDetails(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ui.RetrieveLicenseDetails
        Dim callCounter As Integer = 0
        Dim sysID As String = String.Empty
        Dim postsLicensed As Short = 0
        Dim runningProcesses As Integer = 0
        nmsQA5LicServer.nmsSRV_GetOpDetails(callCounter, sysID, postsLicensed, runningProcesses)

        Dim coreVersion As String = "version"
        nmsQA5LicServer.nmsSRV_ResetCounter(coreVersion)

        ui.SetLicenseDetails(sysID, callCounter, postsLicensed, runningProcesses, coreVersion)
    End Sub

    Private ReadOnly Property ApplicationDirectoryPath() As String
        Get
            If String.IsNullOrEmpty(directoryName) Then
                Dim executingAssembly As Assembly = Assembly.GetExecutingAssembly()
                directoryName = Path.GetDirectoryName(executingAssembly.Location)
            End If
            Return directoryName
        End Get
    End Property

    Private Sub ui_ForgetCallClassification(ByVal sender As Object, ByVal e As ForgetCallClassificationArgs) _
        Handles ui.ForgetCallClassification
        Dim callSignature As String = e.CallSignature
        If callSignature.IndexOf("="c) = -1 Then
            Return
        End If

        Dim rc As Integer = NmsLioNet.nmslioNetForget(callSignature)

        If rc <> 0 Then
            ui.ShowMessage("Error 'Forgetting': " & rc)
        Else
            ui.ShowMessage("'Forgetting' succeeded")
        End If

        callSignature = callSignature.Substring(0, callSignature.IndexOf("="c))
        ui.SetCallEmotionalSignature(callSignature)
    End Sub

    Private Sub ui_UiIsGoingToClose(ByVal sender As Object, ByVal e As EventArgs) Handles ui.UiIsGoingToClose
        If conversionBuffer <> IntPtr.Zero Then
            Marshal.FreeHGlobal(conversionBuffer)
        End If

        Dim errorMessage As String = String.Empty
        If nmsCOMcallee IsNot Nothing Then
            nmsCOMcallee.nmsLioNetSave(errorMessage)
        End If
    End Sub

    Private Sub ui_NewCallEmotionAdded(ByVal sender As Object, ByVal e As NewCallEmotionalArgs) _
        Handles ui.NewCallEmotionAdded
        Dim emotionName As String = e.Emotion

        If callEmotions.Contains(emotionName) Then
            Return
        End If

        emotionName = emotionName.Replace(" "c, "_"c)
        emotionName = emotionName.Replace("="c, "_"c)

        callEmotions.Add(emotionName)

        Dim emoUserDefPath As String = Path.Combine(ApplicationDirectoryPath, "EmoUserDef.ini")
        Using textWriter As TextWriter = New StreamWriter(emoUserDefPath)
            For Each emotion As String In callEmotions
                textWriter.WriteLine(emotion)
            Next
        End Using

        ui.SetLionetUserDefinedEmotions(callEmotions.ToArray())
    End Sub

    Private Sub ui_ClassifyCall(ByVal sender As Object, ByVal e As ClassifyCallArgs) Handles ui.ClassifyCall
        If e.CallSignature.IndexOf("="c) <> -1 Then
            ui.ShowMessage("This Emotional Signature was already trained.")
            Return
        End If

        Dim callEmotionSignature As String = String.Format("{0} ={1}", e.CallSignature, e.EmotionName)
        Dim res As String = [String].Empty
        Dim tOutOfKnownRange As Double = 0
        Dim outDist As Double = 0
        Dim outProb As Double = 0
        Dim outRisk As Double = 0
        Dim resAccuracy As Short
        Dim rcl As Integer = NmsLioNet.nmslioNetGo(callEmotionSignature, res, outDist, outProb, outRisk, tOutOfKnownRange, resAccuracy)
        If rcl = -1 Then
            ui.ShowMessage("Unknown problem - Can't teach.")
            Return
        End If

        'Calculate and show the current accuracy level
        Dim netReport As String = String.Empty
        ui.SetNetAccuracy( _
                           "Net accuracy:" & _
                           NmsLioNet.nmslioIsNetReady(lioDataEmoSigFileName, netReport).ToString("F02") & "%")
        ui.ShowMessage("LioNet Response (Call): " & res)

        'every 5 trainings it is advised to send the LioNet com to sleep, and every 10 for deep sleep...
        trainigsCount += 1
        Dim dreams As String = String.Empty
        If (trainigsCount Mod 5) = 0 Then
            If (trainigsCount Mod 10) = 0 Then
                NmsLioNet.nmslioTakeDeepSleep()
            End If

            Dim usedBasic As Integer = 0
            Dim usedHidden As Integer = 0
            NmsLioNet.nmslioNetSleep()
        End If

        'after each training, save the Net data
        Dim doSleep As Boolean = False
        NmsLioNet.nmslioSaveNet(lioDataEmoSigFileName, dreams)

        ui.SetCallEmotionalSignature(callEmotionSignature)
    End Sub

    Private Sub ui_LioNetForget(ByVal sender As Object, ByVal e As LioNetForgetArgs) Handles ui.LioNetForget
        Dim esig As String = lioNetResultsCache(e.SegmentId)
        Dim rc As Integer = nmsCOMcallee.nmslioNetForget(esig)

        If rc <> 0 Then
            ui.ShowMessage("Error 'Forgetting' :" & rc)
        Else
            ui.ShowMessage("'Forgetting' succeeded")
        End If

        esig = esig.Substring(0, esig.IndexOf("="c))
        lioNetResultsCache(e.SegmentId) = esig

        Dim aiResultForSegment As String = ui.GetCurrentAiResultForSegment(e.SegmentId)
        Dim lionetResponse As String = ""
        Dim dist As Double = 0
        Dim risk As Double = 0
        Dim prob As Double = 0
        Dim tOutOfKnownRange As Double = 0
        Dim resAccuracy As Short
        nmsCOMcallee.nmslioNetGo(esig, lionetResponse, dist, risk, prob, tOutOfKnownRange, resAccuracy)
        If lionetResponse <> aiResultForSegment Then
            If [String].IsNullOrEmpty(lionetResponse) Then
                ui.SetAiResultForSegment(e.SegmentId, "?")
            Else
                ui.SetAiResultForSegment(e.SegmentId, "R_" & lionetResponse)
            End If
        End If
    End Sub

    Private Sub ui_ClassifySegment(ByVal sender As Object, ByVal e As ClassifySegmentArgs) Handles ui.ClassifySegment
        Dim esig As String = String.Format("{0} ={1}", lioNetResultsCache(e.SegmentId), e.EmotionName)
        Dim res As String = ""
        Dim tOdist As Double = 0
        Dim tOprob As Double = 0
        Dim tOrisk As Double = 0
        Dim tOutOfKnownRange As Double = 0
        Dim resAccuracy As Short
        Dim rcl As Integer = nmsCOMcallee.nmslioNetGo(esig, res, tOdist, tOprob, tOrisk, tOutOfKnownRange, resAccuracy)
        If rcl = -1 Then
            ui.ShowMessage("Unknown problem - Can't teach.")
            Return
        End If

        ui.ShowMessage(String.Format("LioNet Response: {0}", res))

        ui.SetAiResultForSegment(e.SegmentId, "C_" & e.EmotionName)

        lioNetWasTrained = True
    End Sub

    Private Sub ui_NewSegmentEmotionAdded(ByVal sender As Object, ByVal e As NewSegmentEmotionArgs) _
        Handles ui.NewSegmentEmotionAdded
        Dim emotionName As String = e.EmotionName

        If segmentEmotions.Contains(emotionName) Then
            Return
        End If

        emotionName = emotionName.Replace(" "c, "_"c)
        emotionName = emotionName.Replace("="c, "_"c)

        segmentEmotions.Add(emotionName)

        Dim path__1 As String = Path.Combine(ApplicationDirectoryPath, "EmoSigsDef.ini")
        Using textWriter As TextWriter = New StreamWriter(path__1)
            For Each emotion As String In segmentEmotions
                textWriter.WriteLine(emotion)
            Next
        End Using

        ui.SetUserDefinedSegmentEmotions(segmentEmotions.ToArray())
    End Sub

    Private Sub DoAnalysis()
        Dim stream As WaveStream
        Dim exception As Exception
        Try
            stream = New WaveStream(File.Open(fileName, FileMode.Open, FileAccess.Read))
        Catch exception1 As Exception
            exception = exception1
            ui.ShowMessage(exception.Message)
            Return
        End Try
        Try
            Try
                ResetStatistics()

                onlineFlag = "---"

                Dim cStartPosSec As Integer = 0
                Dim cEndPosSec As Integer = 0
                Dim segmentID As Integer = 0
                Dim inpBuf As Array = New Short(bufferSize - 1) {}

                Dim startProcessingTime As DateTime = DateTime.Now

                Dim avgVoiceEnergy As Short

                While ReadBuffer(stream, DirectCast(inpBuf, Short())) > 0
                    Dim brderS As Double = -1
                    Dim emoValsArray As Array = Nothing
                    Dim aIres As String = ""
                    Dim bStr As String = ""
                    Dim testbuf As Array = Nothing
                    Dim testBufLeng As Integer = 0
                    Dim bufSize As Short = CShort((bufferSize - 1))
                    Dim _
                        processBufferResult As Integer = _
                            nmsCOMcallee.nmsProcessBuffer(inpBuf, bufSize, emoValsArray, aIres, bStr, testbuf, _
                                                           testBufLeng, brderS)
                    If processBufferResult = NMS_SDKERROR Then
                        Throw _
                            New Exception( _
                                           "A critical error ocured inside the SDK, and the analysis can not proceed. This is mostly caused by an internal protection error. Please close this application and start again, make sure your plug is connected, and try again.")
                    End If

                    If (processBufferResult = NMS_PROCESS_VOICEDETECTED) AndAlso (cStartPosSec = 0) Then
                        cStartPosSec = cEndPosSec
                    End If

                    If processBufferResult = NMS_OK Then
                        cStartPosSec = 0
                    End If

                    If processBufferResult = NMS_FAILED Then
                        cStartPosSec = 0
                    End If

                    cEndPosSec += 2

                    If processBufferResult = NMS_PROCESS_ANALYSISREADY Then
                        Dim _
                            emoVals As EmotionResults = _
                                CopyValuesFromEmoArrayIntoEmotionsStructure(emoValsArray, aIres)

                        avgVoiceEnergy += emoVals.VoiceEnergy

                        nmsCOMcallee.nmsQA_CollectAgentScoreData()

                        UpdateAlarms(emoVals, segmentID)

                        ui.UpdateHistoryBars(emoVals)

                        If segmentID >= lioNetResultsCache.Count Then
                            For i As Integer = 0 To 100
                                lioNetResultsCache.Add(String.Empty)
                            Next
                        End If

                        lioNetResultsCache(segmentID) = bStr

                        nmsCOMcallee.nmsQA_Logdata(segmentID, cStartPosSec, cEndPosSec)
                        nmsCOMcallee.nmsSD_LogData()

                        Dim comment As String = String.Empty
                        UpdateSegmentsListAndCsv(segmentID, bStr, cStartPosSec, cEndPosSec, emoVals, comment)

                        UpdateProfiles(segmentID)

                        nmsCOMcallee.nmsCollectProfiler()
                        If ui.CreateSegmentAudioFiles() Then
                            SaveSegment(segmentID, testbuf)
                            If ui.NeedToPlayEachSegment() Then
                                PlaySegment(segmentID)
                            End If
                        End If

                        UpdateCallPriority(segmentID)

                        cStartPosSec = 0
                        segmentID += 1
                    End If
                End While

                Dim endProcessingTime As DateTime = DateTime.Now

                Dim processingTime As TimeSpan = endProcessingTime - startProcessingTime

                UpdateCallProfile(segmentID, processingTime)

                AnalyzeConversationBorders()

                UpdateTestsDatabase(processingTime, segmentID, avgVoiceEnergy / segmentID)
            Catch exception2 As Exception
                exception = exception2
                ui.ShowMessage(exception.Message & Environment.NewLine & exception.StackTrace)
            End Try
        Finally
            stream.Close()
            ui.UnFreezTables()

            If Not processingBatch Then
                ui.ShowMessage("Done")
            End If
        End Try
    End Sub

    Private Sub UpdateTestsDatabase(ByVal processingTime As TimeSpan, ByVal segmentID As Integer, ByVal avgVoiceEnergy As Double)
        ui.AddCallRecord(fileName, processingTime, callMaxPriorityFlag, callPriority, lioNetResult, overallBordersDistance, _
                          ((angerSegments * 100) / segmentID).ToString("F02"), ((stressSegments * 100) / segmentID).ToString("F02"), _
                          ((upsetSegments * 100) / segmentID).ToString("F02"), nmsCOMcallee.nmsQA_GetAgentScore(), avgVoiceEnergy)
    End Sub

    Private Sub ResetStatistics()
        enrgyStaysHighSegments = 0
        midEnergySegments = 0
        lowEnergySegments = 0
        stressSegments = 0
        angerSegments = 0
        upsetSegments = 0
        callMaxPriorityFlag = 0
    End Sub

    Private Sub UpdateCallPriority(ByVal segmentID As Integer)
        Try
            callPriority = CInt((((((stressSegments * 0.25) + (angerSegments * 2.5)) + (upsetSegments * 0.25)) * 100) / (segmentID + 1)))
            If callPriority > 100 Then
                callPriority = 100
            End If

            If callMaxPriorityFlag < callPriority Then
                callMaxPriorityFlag = callPriority
            End If

            ui.UpdateCallPriority(segmentID, callPriority, stressSegments, angerSegments, upsetSegments)
        Catch

        End Try
    End Sub

    Private Sub UpdateAlarms(ByVal EmoVals As EmotionResults, ByVal segmentID As Integer)
        Try
            Dim rotateCycle As Integer
            Dim rotatingAvrg As Double = 0
            Dim callIsOutOfAcceptableLevels As Boolean = False

            If EmoVals.upset > 0 Then
                upsetSegments += 1
            End If

            If ui.AlarmIfAngerTrendIsRaisingAndAbove Then
                ui.HighlightAngerTrendLevel = False
                Dim angry As Double = EmoVals.angry
                rotateCycle = 4
                rotatingAvrg = trenders(1).regNewValRotatingAvrg(angry, rotateCycle)
                If ui.AlarmIfAngerTrendIsRaisingAndAbove AndAlso (rotatingAvrg > ui.MaxAngerTrandLevel) Then
                    callIsOutOfAcceptableLevels = True
                    onlineFlag = "angry " & rotatingAvrg.ToString("F02")
                    ui.HighlightAngerTrendLevel = True
                End If

                ui.AngerTrendLevel = rotatingAvrg.ToString("F02")
            End If

            If ui.AlarmIfStressTrendIsRaisingAndAbove Then
                ui.HighlightStressTrendLevel = False
                Dim stress As Double = EmoVals.stress
                rotateCycle = 4
                rotatingAvrg = trenders(0).regNewValRotatingAvrg(stress, rotateCycle)
                If rotatingAvrg > ui.MaxStressTrendLevel Then
                    ui.NotifyStressLevelIsRaising = True
                    ui.HighlightStressTrendLevel = True
                    onlineFlag = "STRESS " & rotatingAvrg.ToString("F02")
                    stressSegments += 1
                ElseIf rotatingAvrg < (ui.MaxAngerTrandLevel - 1) Then
                    ui.NotifyStressLevelIsRaising = False
                    ui.HighlightStressTrendLevel = False
                Else
                    ui.HighlightStressTrendLevel = False
                End If

                ui.StressTrendLevel = rotatingAvrg.ToString("F02")
            End If

            If ui.AlarmIfStressTrendIsRaisingAndAbove OrElse ui.AlarmIfStressTrendIsLow Then
                ui.HighlightEnergyTrendIsRaisingFor = False
                ui.HighlightEnergyLevelBelow = False

                Dim difEnrgy As Double = 0
                If segmentID > 5 Then
                    rotateCycle = 8
                    Dim energy As Double = EmoVals.Energy
                    rotatingAvrg = trenders(2).regNewValRotatingAvrg(energy, rotateCycle)
                    Dim num6 As Double = trenders(2).getFirstValuesRotating() + 3

                    difEnrgy = (rotatingAvrg - num6) * 2
                    ui.EnergyDifference = difEnrgy.ToString("F02")

                    If difEnrgy < 7 Then
                        enrgyStaysHighSegments = 0
                    Else
                        enrgyStaysHighSegments += 1
                    End If

                    ui.EnrgyHighSegments = enrgyStaysHighSegments.ToString("")

                    ui.SetEnergyLevel(difEnrgy, (difEnrgy >= 7))
                End If

                If ui.AlarmIfStressTrendIsRaisingAndAbove Then
                    If enrgyStaysHighSegments > ui.LimitForEnergyTrendIsRaisingFor AndAlso (EmoVals.content = 0) Then
                        callIsOutOfAcceptableLevels = True
                        ui.HighlightEnergyTrendIsRaisingFor = True
                        onlineFlag = "ANGRY " & rotatingAvrg.ToString("F02")
                        angerSegments += 1

                        ui.HighlightEnergyLevel = True
                    Else
                        ui.HighlightEnergyTrendIsRaisingFor = False
                        If difEnrgy > 0 Then
                            midEnergySegments += 1
                        End If
                    End If

                    ui.EnergyTrendIsRaisingFor = enrgyStaysHighSegments.ToString()
                End If

                If ui.AlarmIfStressTrendIsLow AndAlso (rotatingAvrg <= ui.LimitForEnergyLevelBelow) Then
                    callIsOutOfAcceptableLevels = True
                    ui.HighlightEnergyLevelBelow = True
                    lowEnergySegments += 1

                    ui.NotifySpeakerIsTired = True
                Else
                    ui.NotifySpeakerIsTired = False
                End If

                ui.EnergyLevelBelow = rotatingAvrg.ToString("F02")
            End If

            ui.NotifyCallIsOutOfAcceptableLevels = callIsOutOfAcceptableLevels
        Catch exception As Exception
            ui.ShowMessage(exception.Message)
        End Try
    End Sub

    Private Sub AnalyzeConversationBorders()
        nmsCOMcallee.nmsBordersCalculate(callBordersData)

        ui.SetConversationBordersData(callBordersData)

        overallBordersDistance = 0
        Dim overallNormalInternalEnvelopeDistance As Double = 0

        Dim significantRep As StringBuilder = New StringBuilder()

        For i As Integer = 0 To 50
            If CDbl(callBordersData.GetValue(5, i)) > 0 Then
                overallBordersDistance = overallBordersDistance + CDbl(callBordersData.GetValue(5, i))
                If CDbl(callBordersData.GetValue(5, i)) > 15 Then
                    'Dif is 15% or more
                    significantRep.AppendFormat( _
                                                 ">Parameter '{0}' is {1}% far from the Border. (Low: {2}%, High: {3}%){4}", _
                                                 i + 1, CDbl(callBordersData.GetValue(5, i)).ToString("F02"), _
                                                 CDbl(callBordersData.GetValue(6, i)).ToString("F02"), _
                                                 CDbl(callBordersData.GetValue(7, i)).ToString("F02"), _
                                                 Environment.NewLine)
                End If
            End If

            If CDbl(callBordersData.GetValue(8, i)) > 0 Then
                overallNormalInternalEnvelopeDistance = overallNormalInternalEnvelopeDistance + _
                                                        CDbl(callBordersData.GetValue(8, i))
                If CDbl(callBordersData.GetValue(8, i)) > 15 Then
                    '' Dif is 15% or more
                    significantRep.AppendFormat( _
                                                 ">Parameter '{0}' is {1}% far from the normal envelop. (Low: {2}%, High: {3}%){4}", _
                                                 i + 1, CDbl(callBordersData.GetValue(8, i)).ToString("F02"), _
                                                 CDbl(callBordersData.GetValue(9, i)).ToString("F02"), _
                                                 CDbl(callBordersData.GetValue(10, i)).ToString("F02"), _
                                                 Environment.NewLine)
                End If
            End If
        Next

        significantRep.Insert(0, _
                               [String].Format( _
                                                "Overall Borders Distance: {0}{2}Overall Normal Internal Envelope Distance:{1}{2}", _
                                                overallBordersDistance.ToString("F02"), _
                                                overallNormalInternalEnvelopeDistance.ToString("F02"), _
                                                Environment.NewLine))

        ui.SetEnvelopAndBordersReport(significantRep.ToString())

        If overallBordersDistance + overallNormalInternalEnvelopeDistance > 0 Then
            ui.ShowMessage( _
                            [String].Format( _
                                             "This conversation demonstrated differences from the current Borders settings. If you want to update the 'Known Borders', please see the 'Envelope and Borders' tab. ({0}/{1})", _
                                             overallBordersDistance, overallNormalInternalEnvelopeDistance))
        End If
    End Sub

    Private Sub UpdateCallProfile(ByVal segCount As Integer, ByVal processingTime As TimeSpan)
        Dim nmsCallProfile As StringBuilder = New StringBuilder(nmsCOMcallee.nmsCallProfiler(segCount))
        nmsCallProfile.AppendLine()
        nmsCallProfile.AppendLine( _
                                   [String].Format("Processing Time: {0}:{1}:{2}.{3}", processingTime.Hours, _
                                                    processingTime.Minutes, processingTime.Seconds, _
                                                    processingTime.Milliseconds))
        nmsCallProfile.AppendLine()

        'QAsig will receive the Emotional Signature string that can be used to flag the call using LioNet, and to
        'train the system to identify types of CALLS (unlike the basic LioNet used to analyze the SEGMENT level)
        'YOU MUST enter the number of segments for analysis detected in the call (SegCount) collected in this
        'function. All the rest are outputs.
        Dim nStressL As Integer = 0
        Dim nAVJstressD As Double = 0
        Dim nAVJemoD As Double = 0
        Dim RepQA As String = [String].Empty
        Dim _
            qa5Signature As String = _
                nmsCOMcallee.nmsQA_CreateSignature(segCount, nStressL, nAVJstressD, nAVJemoD, RepQA)

        If qa5Signature.Length < 10 Then
            'if this is the case, an error was detected, or the file had less than 5 voice segments. Normally, you will
            'prefer to analyze files containing more than 30 segments at least.
            ui.SetCallLioNetAnalysis("LioNet Analysis: unavailable due to short file or error")
        Else
            'the QA5sig will contain the error code.
            'Let LioNet object test the Emotional Signature:
            lioNetResult = ""
            Dim outDist As Double = 0
            Dim outProb As Double = 0
            Dim outRisk As Double = 0
            Dim outOfRange As Double = 0
            Dim resAccuracy As Short
            NmsLioNet.nmslioNetGo(qa5Signature, lioNetResult, outDist, outProb, outRisk, outOfRange, resAccuracy)
            '"lioNetResult" is actually the final decision of the system in Offline, and should be added to your calls
            'database.
            If [String].IsNullOrEmpty(lioNetResult) Then
                ui.SetCallLioNetAnalysis("LioNet Analysis: " & "Not Available")
            Else
                ui.SetCallLioNetAnalysis("LioNet Analysis: " & lioNetResult)
            End If
            'few more bits of information are returned, such as:
            ' nStressL   = Number of high stress segments detected in the call
            ' nAVJstressD = Average Stress level in the call
            ' nAVJemoD = Average Emotional level in the call
            ' RepQA - the full QA report (textualized)
            nmsCallProfile.AppendLine(" QA5 data:")
            nmsCallProfile.AppendLine(RepQA)
        End If
        ui.SetCallEmotionalSignature(qa5Signature)
        'display the emotional Signature data
        ui.SetCallProfile(nmsCallProfile.ToString())
    End Sub

    Private Function CheckFileAndGetItsParameters(ByVal processingFileName As String, ByRef bufSize As Short, _
                                                   ByRef bps As Integer, ByRef SPS As Short) As Boolean
        bufSize = 0
        bps = 0
        SPS = 0
        fullDuplex.WaveOutFilename = processingFileName
        fullDuplex.WaveOutGetFileInfo()
        If fullDuplex.WaveOutFileLengthMilliseconds < 10000 Then
            ui.ShowMessage("File is too short")
            Return False
        End If

        processingFileChannels = fullDuplex.WaveOutChannels
        bps = fullDuplex.WaveOutBitsPerSample
        SPS = CShort(fullDuplex.WaveOutSamplesPerSecond)
        fullDuplex.WaveOutClose()

        If (bps <> 8) AndAlso (bps <> 16) Then
            ui.ShowMessage("Only files with 8 or 16 bit per sample are supported")
            Return False
        End If

        If (((SPS <> 8000) AndAlso (SPS <> 11025)) AndAlso (SPS <> 11000)) AndAlso (SPS <> 6000) Then
            ui.ShowMessage(String.Format("Files with {0}Hz sampling rate are not supported", SPS))
            Return False
        End If
        Select Case SPS
            Case 11000, 11025
                bufSize = 220
                Exit Select

            Case 6000
                bufSize = 120
                Exit Select

            Case 8000
                bufSize = 160
                Exit Select
        End Select

        If conversionBuffer <> IntPtr.Zero Then
            Marshal.FreeHGlobal(conversionBuffer)
        End If

        conversionBuffer = Marshal.AllocHGlobal(If(bps = 8, bufSize, bufSize * 2) * processingFileChannels)
        splitBuffer = New Short(bufSize * processingFileChannels) {}

        Return True
    End Function

    Private Function ConfigureCoreForItsDesignatedTasks() As Boolean
        Dim nmsConfigTestDataResult As Integer
        Dim lengthOfSegmentInSeconds As Short
        Dim calibrationType As Short = ui.GetCalibrationType()
        If ui.GetAnalysisType() = 1 Then
            calibrationType = CShort((calibrationType + 2))
        End If

        Dim backgroundLevel As Short = ui.GetBackgroundLevel()

        If ui.IsOneSecondBufferUsed() Then
            lengthOfSegmentInSeconds = 1
            nmsConfigTestDataResult = _
                nmsCOMcallee.nmsConfigTestData(waveOutSamplesPerSecond, backgroundLevel, lengthOfSegmentInSeconds, _
                                                calibrationType)
        Else
            lengthOfSegmentInSeconds = 2
            nmsConfigTestDataResult = _
                nmsCOMcallee.nmsConfigTestData(waveOutSamplesPerSecond, backgroundLevel, lengthOfSegmentInSeconds, _
                                                calibrationType)
        End If

        If nmsConfigTestDataResult <> 0 Then
            If nmsConfigTestDataResult = NMS_SDKERROR_WAVESMPRATEWRONG Then
                ui.ShowMessage("The wave format provided is not supported. The operation will now abort.")
                Marshal.ReleaseComObject(nmsCOMcallee)
                nmsCOMcallee = Nothing
                Return False
            End If

            ui.ShowMessage( _
                            String.Format( _
                                           "An error was detected while configuring the core. The operation will now abort. Error = {0}", _
                                           nmsConfigTestDataResult))
            Marshal.ReleaseComObject(nmsCOMcallee)
            nmsCOMcallee = Nothing
            Return False
        End If
        nmsCOMcallee.nmsQA_ConfigUse()
        nmsCOMcallee.nmsSD_ConfigStressCL()
        nmsCOMcallee.nmsBordersLoad(bordersFileName)
        Dim bActive As Boolean = True
        nmsCOMcallee.nmsBordersSetConfig(bActive)
        Return True
    End Function

    Private Shared Function CopyValuesFromEmoArrayIntoEmotionsStructure(ByVal EmoArr As Array, ByVal AIres As String) _
        As EmotionResults
        Dim emotionResults As New EmotionResults

        With emotionResults
            .AIres = AIres
            .angry = EmoArr.GetValue(0)
            .Atmos = EmoArr.GetValue(1)
            .concentration_level = EmoArr.GetValue(2)
            .embarrassment = EmoArr.GetValue(3)
            .excitement = EmoArr.GetValue(4)
            .hesitation = EmoArr.GetValue(5)
            .imagination_activity = EmoArr.GetValue(6)
            .intensive_thinking = EmoArr.GetValue(7)
            .content = EmoArr.GetValue(8)
            .saf = EmoArr.GetValue(9)
            .upset = EmoArr.GetValue(10)
            .extremeState = EmoArr.GetValue(11)
            .stress = EmoArr.GetValue(12)
            .uncertainty = EmoArr.GetValue(13)
            .Energy = EmoArr.GetValue(14)
            .BrainPower = EmoArr.GetValue(15)
            .EmoCogRatio = EmoArr.GetValue(16)
            .maxAmpVol = EmoArr.GetValue(17)
            .VoiceEnergy = EmoArr.GetValue(18)
        End With

        Return emotionResults
    End Function

    Private Sub CreateDataFolders()
        dataFolder = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) & " Data")

        If Not Directory.Exists(dataFolder) Then
            Directory.CreateDirectory(dataFolder)
        End If

        segmentsFolder = Path.Combine(dataFolder, "Segments")
        If Not Directory.Exists(segmentsFolder) Then
            Directory.CreateDirectory(segmentsFolder)
        End If
    End Sub

    Private Sub DrawSegmentWaveform(ByVal segmentFileName As String)
        SyncLock Me
            Using stream As WaveStream = New WaveStream(segmentFileName)
                Dim buffer() As Byte = New Byte(stream.Length - 1) {}
                stream.Read(buffer, 0, CInt(stream.Length))
                If stream.FormatEx.wBitsPerSample = 8 Then
                    ui.DrawSegment(buffer)
                Else
                    Dim zero As IntPtr = IntPtr.Zero
                    Try
                        Dim destination() As Short = New Short(buffer.Length \ 2 - 1) {}
                        zero = Marshal.AllocHGlobal(((Marshal.SizeOf(GetType(Short)) * buffer.Length) \ 2))
                        Marshal.Copy(buffer, 0, zero, buffer.Length)
                        Marshal.Copy(zero, destination, 0, buffer.Length \ 2)
                        ui.DrawSegment(destination)
                    Finally
                        If zero <> IntPtr.Zero Then
                            Marshal.FreeHGlobal(zero)
                        End If
                    End Try
                End If
            End Using
        End SyncLock
    End Sub

    Public Function Initialize() As Boolean
        lioDataEmoSigFileName = Path.Combine(ApplicationDirectoryPath, "QA5SigData.lio")
        LioDataSEGMENTfile = Path.Combine(ApplicationDirectoryPath, "EmoData.lio")
        bordersFileName = Path.Combine(ApplicationDirectoryPath, "QAbrdrs.dat")

        If Not InitQA5Core() Then
            Return False
        End If

        ui.SetBGLevel(1000)

        LoadUserDefinedEmoSegments()

        LoadLionetUserDefinedEmotions()

        If Not LoadLioNetKnowledgeAndEmotionalSignatures() Then
            Return False
        End If

        For i As Integer = 0 To trenders.Length - 1
            trenders(i) = New TrendAnalyzerClass()
        Next

        Return True
    End Function

    Private Function InitLicenseAndCallee() As Boolean
        Dim activationResult As Short
        nmsCOMcallee = New nmsQA5coreClass()
        Dim tProcessId As Integer = nmsCOMcallee.nmsSDKgetProcessId()
        Dim actCode As String = nmsQA5LicServer.nmsSRV_GetActivationLicense(tProcessId)
        If Not Short.TryParse(actCode.Substring(0, 4), activationResult) Then
            activationResult = 0
        End If

        If activationResult = NMS_SDKERROR_LICENSERENEWNEEDEDNOW Then
            ui.ShowMessage("The system requires re-licensing to operate!")
            Marshal.ReleaseComObject(nmsCOMcallee)
            nmsCOMcallee = Nothing
            Return False
        End If
        If activationResult = NMS_SDKERROR_PROTECTIONERROR Then
            ui.ShowMessage("The activation license could not be generated due to a protection error!")
            Marshal.ReleaseComObject(nmsCOMcallee)
            nmsCOMcallee = Nothing
            Return False
        End If
        If activationResult = NMS_SDKERROR_OPERATIONNOTALLOWED Then
            ui.ShowMessage( _
                            "The activation license could not be generated because too many processes are already running. Please wait until one of the other calls is ended, and obtain a license again")
            Marshal.ReleaseComObject(nmsCOMcallee)
            nmsCOMcallee = Nothing
            Return False
        End If
        Select Case nmsCOMcallee.nmsInitCore(actCode, LioDataSEGMENTfile, DEF_OWNER)
            Case NMS_OK
                Exit Select

            Case NMS_LIOERROR_FILENOTFOUND
                ui.ShowMessage("Error LioNet file missing - Deal here with Missing LioNet files")
                Marshal.ReleaseComObject(nmsCOMcallee)
                nmsCOMcallee = Nothing
                Return False

            Case NMS_LIONET_CREATED_OK
                ui.ShowMessage("New LioNet file created!")
                Exit Select
            Case Else

                If True Then
                    actCode = nmsQA5LicServer.nmsSRV_GetActivationLicense(tProcessId)
                    Dim nmsInitCoreResult As Integer = nmsCOMcallee.nmsInitCore(actCode, LioDataSEGMENTfile, DEF_OWNER)
                    Select Case nmsInitCoreResult
                        Case NMS_SDKERROR_UNSPECIFIED
                            ui.ShowMessage("An unspecified type of error occurred. The operation will now abort.")
                            Marshal.ReleaseComObject(nmsCOMcallee)
                            nmsCOMcallee = Nothing
                            Return False

                        Case NMS_SDKERROR_PROTECTIONERROR
                            ui.ShowMessage("The Activation License was not valid. The operation will now abort.")
                            Marshal.ReleaseComObject(nmsCOMcallee)
                            nmsCOMcallee = Nothing
                            Return False
                    End Select

                    ui.ShowMessage(String.Format("Error initializing QA5COM. Error: {0}.", nmsInitCoreResult))
                    Marshal.ReleaseComObject(nmsCOMcallee)
                    nmsCOMcallee = Nothing
                    Return False
                End If
        End Select
        Return True
    End Function

    Private Sub InitLioNetResultsCache()
        lioNetResultsCache.Clear()
        For i As Integer = 0 To 99
            lioNetResultsCache.Add(String.Empty)
        Next
    End Sub

    Private Function InitQA5Core() As Boolean
        Try
            nmsQA5LicServer = New nmsQA5coreClass()
            Dim nmsSRV_INITResult As Short = nmsQA5LicServer.nmsSRV_INIT(DEF_OWNER)
            If nmsSRV_INITResult <> 0 Then
                If nmsSRV_INITResult = NMS_SDKERROR_PROTECTIONERROR Then
                    ui.ShowMessage("(Plug Not found, or general security error.)")
                End If
                If nmsSRV_INITResult <> NMS_SDK_LICENSERENEWNEEDED Then
                    ui.ShowMessage("ERROR initializing SDK Server. Error Code:" & nmsSRV_INITResult)
                End If
                If nmsSRV_INITResult = NMS_SDKERROR_LICENSERENEWNEEDEDNOW Then
                    ui.ShowLicenseScreen()
                End If
                If nmsSRV_INITResult <> NMS_SDK_LICENSERENEWNEEDED Then
                    Return False
                End If
            End If

            NmsLioNet = New nmsLioNetV6
        Catch exception As Exception
            ui.ShowMessage(exception.Message)
            Return False
        End Try
        Return True
    End Function

    Private Sub InitTrenders()
        For Each analyzer As TrendAnalyzer In trenders
            analyzer.Init()
            Dim degree As Integer = 1
            analyzer.Degree = degree
        Next
    End Sub

    Private Function LoadLioNetKnowledgeAndEmotionalSignatures() As Boolean
        Dim outMsg As String = ""
        Dim doSleep As Boolean = True
        If NmsLioNet.nmslioLoadNet(lioDataEmoSigFileName, outMsg) <> 0 Then
            ui.ShowMessage( _
                            "Loading Lionet for Emotional Signatures failed: " & outMsg & _
                            ". If this is the first time you run this application, this is expected. Creating new LioNet data file.")
            Dim netInps As Integer = 116
            If NmsLioNet.nmslioNetCreate(lioDataEmoSigFileName, netInps, DEF_OWNER) <> 0 Then
                ui.ShowMessage("Unexpected Error occurred  while creating a new LioNet!")
                Return False
            End If
            doSleep = False
            NmsLioNet.nmslioSaveNet(lioDataEmoSigFileName, outMsg)
            ui.SetNetAccuracy("NEW NET")
        Else
            Dim sNetReport As String = ""
            ui.SetNetAccuracy( _
                               String.Format("Net accuracy: {0}%", _
                                              NmsLioNet.nmslioIsNetReady(lioDataEmoSigFileName, sNetReport).ToString( _
                                                                                                                       "F02", _
                                                                                                                       CultureInfo _
                                                                                                                          . _
                                                                                                                          InvariantCulture)))
        End If
        Return True
    End Function

    Private Sub LoadLionetUserDefinedEmotions()
        Dim emoUserDefFile As String = Path.Combine(ApplicationDirectoryPath, "EmoUserDef.ini")
        If File.Exists(emoUserDefFile) Then
            Try
                Using reader As TextReader = New StreamReader(emoUserDefFile)
                    Dim emotion As String = String.Empty
                    While (InlineAssignHelper(emotion, reader.ReadLine())) IsNot Nothing
                        callEmotions.Add(emotion)
                    End While
                End Using
            Catch exception As Exception
                ui.ShowMessage("Error reading EmoUserDef.ini: " & exception.Message)
            End Try
        End If

        If callEmotions.Count = 0 Then
            callEmotions.Add("Neutral")
        ElseIf callEmotions(0) = "" Then
            callEmotions(0) = "Neutral"
        End If
        ui.SetLionetUserDefinedEmotions(callEmotions.ToArray())
    End Sub

    Private Sub LoadUserDefinedEmoSegments()
        Dim emoSigsDefFile As String = Path.Combine(ApplicationDirectoryPath, "EmoSigsDef.ini")
        If File.Exists(emoSigsDefFile) Then
            Try
                Using reader As TextReader = New StreamReader(emoSigsDefFile)
                    Dim emotion As String = String.Empty
                    While (InlineAssignHelper(emotion, reader.ReadLine())) IsNot Nothing
                        segmentEmotions.Add(emotion)
                    End While
                End Using
            Catch exception As Exception
                ui.ShowMessage("Error reading EmoSigsDef.ini: " & exception.Message)
            End Try
        End If
        If segmentEmotions.Count = 0 Then
            segmentEmotions.Add("Neutral")
        ElseIf segmentEmotions(0) = "" Then
            segmentEmotions(0) = "Neutral"
        End If

        ui.SetUserDefinedSegmentEmotions(segmentEmotions.ToArray())
    End Sub

    Private Sub ui_FileSelected(ByVal sender As Object, ByVal e As FileSelectedArgs) Handles ui.FileSelected
        processingBatch = e.BatchProcess
        fileName = e.FileName
        ui.Reset()
        ui.FreezTables()

        If _
            CheckFileAndGetItsParameters(e.FileName, bufferSize, bitsPerSample, waveOutSamplesPerSecond) AndAlso _
            InitLicenseAndCallee() Then
            If ConfigureCoreForItsDesignatedTasks() Then
                InitTrenders()
                InitLioNetResultsCache()
                CreateDataFolders()
                CreateAndInitializeCsvFile()

                DoAnalysis()

                FinalizeCsvFile()
            End If
        End If
    End Sub

    Private Sub FinalizeCsvFile()
        cSvWriter.Close()
    End Sub

    Private Sub CreateAndInitializeCsvFile()
        cSvWriter = New StreamWriter(Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(fileName) + ".csv"))

        cSvWriter.WriteLine("Seg N.,Start Pos (Sec.),End Pos (Sec.),Energy,Content,Upset,Angry,Stressed,Uncertain,Excited,Concentrated,EmoCogRatio,Hesitation,BrainPower,Embar.,I. Think,Imagin,ExtremeEmotion,SAF,Atmos.,ONLINE Flag,LioNet analysis,MaxAmpVol.,Comment,LioNet Info")
    End Sub

    Private Sub PlaySegment(ByVal segCount As Integer)
        waitForPlaybackFinished.WaitOne()

        waitForPlaybackFinished.Reset()

        Dim _
            segmentFileName As String = _
                Path.Combine(segmentsFolder, String.Format("Segment_{0}.wav", segCount.ToString("D3")))
        DrawSegmentWaveform(segmentFileName)

        fullDuplex.WaveOutFilename = segmentFileName
        fullDuplex.WaveOutPlayFile(0, 0)
    End Sub

    Private Function ReadBuffer(ByVal waveStream As Stream, ByVal soundBuffer As Short()) As Integer
        Dim readBufferSize As Integer = (If((bitsPerSample = 8), bufferSize, (bufferSize * 2))) * processingFileChannels
        Dim buffer() As Byte = New Byte(readBufferSize - 1) {}
        Dim cb As Integer = waveStream.Read(buffer, 0, readBufferSize)
        If cb = 0 Then
            Return 0
        End If

        If bitsPerSample = 8 Then
            Dim j As Integer = 0
            Dim i As Integer = 0
            While i < cb
                soundBuffer(j) = CShort(((buffer(i) - 127) * 127))
                j += 1
                i += processingFileChannels
            End While

            Return cb
        End If

        If processingFileChannels <> 1 Then
            Marshal.Copy(buffer, 0, conversionBuffer, cb)
            Marshal.Copy(conversionBuffer, splitBuffer, 0, cb \ 2)

            Dim j As Integer = 0
            Dim i As Integer = ui.DataChannelNumber
            While i < cb \ 2
                soundBuffer(j) = splitBuffer(i)
                j += 1
                i += processingFileChannels
            End While
        Else
            Marshal.Copy(buffer, 0, conversionBuffer, cb)
            Marshal.Copy(conversionBuffer, soundBuffer, 0, cb \ 2)
        End If

        Return cb / processingFileChannels
    End Function

    Private Sub SaveSegment(ByVal segCount As Integer, ByVal testbuf As Array)
        Dim _
            writer As WaveWriter = _
                New WaveWriter( _
                                File.Create( _
                                             Path.Combine(segmentsFolder, _
                                                           String.Format("Segment_{0}.wav", segCount.ToString("D3")))), _
                                New WaveFormatEx(waveOutSamplesPerSecond, 16, 1))
        writer.Write(DirectCast(testbuf, Short()))
        writer.Close()
    End Sub

    Private Sub SegmentsList_ColumnClicked(ByVal sender As Object, ByVal e As ColumnClickArgs) Handles ui.SegmentsListColumnClicked
        If (e.Column >= 3) AndAlso (e.Column <= 20) Then
            Dim min As Double
            Dim max As Double
            Dim avg As Double
            Dim scaleMin As Integer
            Dim scaleMax As Integer
            Dim segmentsData As Double() = ui.GetSegemtsDataFromColumn(e.Column, min, max, avg)
            If e.Column = 3 Then
                scaleMin = 0
                scaleMax = 50
            ElseIf e.Column = 19 Then
                scaleMin = -80
                scaleMax = 80
            ElseIf e.Column = 13 Then
                scaleMin = 300
                scaleMax = 1500
            ElseIf e.Column = 11 Then
                scaleMin = 30
                scaleMax = 500
            Else
                scaleMin = 0
                scaleMax = 30
            End If
            ui.DrawData(segmentsData, min, max, avg, scaleMin, scaleMax, e.Column)
        End If
    End Sub

    Private Sub SegmentsList_SegmentClicked(ByVal sender As Object, ByVal e As SegmentClickedArgs) Handles ui.SegmentClicked
        PlaySegment(e.ClickedSegmentId)

        'Test the lionet to see whether or not LioNet would change its original analysis based on the last training sessions
        Dim lionetString As String = lioNetResultsCache(e.ClickedSegmentId)
        Dim lionetResponse As String = ""
        Dim Dist As Double = 0
        Dim risk As Double = 0
        Dim prob As Double = 0
        Dim OutR As Double = 0

        Dim CurRes As String = ui.GetCurrentAiResultForSegment(e.ClickedSegmentId)

        Dim resAccuracy As Short
        nmsCOMcallee.nmslioNetGo(lionetString, lionetResponse, Dist, risk, prob, OutR, resAccuracy)
        If lionetResponse <> CurRes AndAlso Not String.IsNullOrEmpty(lionetResponse) Then
            ' Yes - Lionet decided a new analysis is more appropriate...
            ui.SetAiResultForSegment(e.ClickedSegmentId, "R_" & lionetResponse)
        End If
    End Sub

    Private Sub UpdateProfiles(ByVal SegCount As Integer)
        Dim emoLevel As Short = 0
        Dim logicalLevel As Short = 0
        Dim hasitantLevel As Short = 0
        Dim stressLevel As Short = 0
        Dim energeticLevel As Short = 0
        Dim thinkingLevel As Short = 0
        If SegCount = 6 Then
            nmsCOMcallee.nmsQA_getProfilerData(emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, _
                                                thinkingLevel)
            ui.ShowProfile(emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel)
        ElseIf SegCount > 6 Then
            nmsCOMcallee.nmsQA_getChangesFromProfiler(emoLevel, logicalLevel, hasitantLevel, stressLevel, _
                                                       energeticLevel, thinkingLevel)
            ui.UpdateProfile(emoLevel, logicalLevel, hasitantLevel, stressLevel, energeticLevel, thinkingLevel)
        End If
    End Sub

    Private Sub UpdateSegmentsListAndCsv(ByVal segCount As Integer, ByVal bstring As String, ByVal sPos As Integer, _
                                          ByVal fPos As Integer, ByVal emotionResults As EmotionResults, _
                                          ByVal comment As String)
        ui.AddSegmentToList(segCount, bstring, sPos, fPos, EmotionResults, onlineFlag, _
                             comment)

        Dim stringBuilder As StringBuilder = New StringBuilder()

        stringBuilder.Append(segCount.ToString())
        Dim position As Double = (sPos) / 100
        stringBuilder.Append("," & position.ToString("F02"))
        position = (fPos) / 100
        stringBuilder.Append("," & position.ToString("F02"))
        stringBuilder.Append("," & emotionResults.Energy)
        stringBuilder.Append("," & emotionResults.content)
        stringBuilder.Append("," & emotionResults.upset)
        stringBuilder.Append("," & emotionResults.angry)
        stringBuilder.Append("," & emotionResults.stress)
        stringBuilder.Append("," & emotionResults.uncertainty)
        stringBuilder.Append("," & emotionResults.excitement)
        stringBuilder.Append("," & emotionResults.concentration_level)
        stringBuilder.Append("," & emotionResults.EmoCogRatio)
        stringBuilder.Append("," & emotionResults.hesitation)
        stringBuilder.Append("," & emotionResults.BrainPower)
        stringBuilder.Append("," & emotionResults.embarrassment)
        stringBuilder.Append("," & emotionResults.intensive_thinking)
        stringBuilder.Append("," & emotionResults.imagination_activity)
        stringBuilder.Append("," & emotionResults.extremeState)
        stringBuilder.Append("," & emotionResults.saf)
        stringBuilder.Append("," & emotionResults.Atmos)
        stringBuilder.Append("," + onlineFlag)
        stringBuilder.Append("," & emotionResults.AIres)
        stringBuilder.Append("," & emotionResults.maxAmpVol)
        stringBuilder.Append("," & comment)
        stringBuilder.Append("," & bstring)

        cSvWriter.WriteLine(stringBuilder.ToString())

    End Sub

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class