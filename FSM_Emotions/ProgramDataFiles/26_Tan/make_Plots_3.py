import csv
import time as t
import datetime as dt
import matplotlib.pyplot as plt
import numpy as np
from matplotlib import gridspec
import matplotlib


userNumber = 25
offsetHours = 5
affectFileName = "26 Tan End of Day_apostropied_UNapostropied.csv"
robotFileName = "26_Tan_Flow_2016-02-26_16-24-56.csv"
# affectFileName = "tan1 2016-02-11 11_38_09 AM.csv"
# robotFileName = "10_Tan_Flow.csv"

# font = {'family' : 'normal',
#     # 'weight' : 'bold',
#     'size'   : 14}
# matplotlib.rc('font', **font)

def importCSVFile(fileName):
    csvList = []
    try:
        with open(fileName, 'rb') as f:
            reader = csv.reader(f)
            csvList = list(reader)
    except Exception as e:
        print "Something went wrong reading the file: ", fileName
        print e

    return csvList

def printCSVList(csvList):
    for row in csvList:
        print row

def makeNewList(csvList, cols):
    newList = []
    colNums = getColNumsFromNames(cols)
    # print colNums
    for row in csvList:
        newRow = [row[i] for i in colNums]
        newList.append(newRow)
    return newList

def formatAffectList(csvList):
    affectLogTitles = {'feature1':0, 'feature2':1,'feature3':2,'feature4':3,'feature5':4,'feature6':5,'feature7':6,'feature8':7,
                       'BV':8, 'BA':9, 'vV':10, 'vA':11, 'MV':12, 'MA':13, 'absTime':14, 'usedV':15}
    affectLogTitles.update({'time':16, 'VV':17, 'VA':18})

    newRow = []
    timeStart = t.mktime(dt.datetime.strptime(csvList[0][affectLogTitles['absTime']], "%Y-%m-%d %H:%M:%S.%f").timetuple())
    for row in csvList:
        vV = row[affectLogTitles['vV']]
        vA = row[affectLogTitles['vA']]
        usedV = row[affectLogTitles['usedV']]
        VV = "_"
        VA = "_"
        if usedV == '1':
            VV = vV
            VA = vA
        absTime = row[affectLogTitles['absTime']]
        time = t.mktime(dt.datetime.strptime(absTime, "%Y-%m-%d %H:%M:%S.%f").timetuple()) - timeStart
        newRow.append(row + [time, VV, VA])

    return newRow, timeStart

def getColNumsFromNames(colNames):
    affectLogTitles = {'feature1':0, 'feature2':1,'feature3':2,'feature4':3,'feature5':4,'feature6':5,'feature7':6,'feature8':7,
                       'BV':8, 'BA':9, 'vV':10, 'vA':11, 'MV':12, 'MA':13, 'absTime':14, 'usedV':15,
                       'time':16, 'VV':17, 'VA':18}
    colNums = [affectLogTitles[i] for i in colNames]
    return colNums

def formatRobotLog(csvList, timeStart, offsetHours = 5):
    timeStart -= (dt.timedelta(hours = offsetHours)).total_seconds()
    print "timeStart: ", timeStart
    robotLogTitles = {'State TimeStamp': 0, 'State Date Time': 1, 'FSM State': 2,
                      'FSM State Name': 3,'Robot Emotion': 4, 'Observable Expression': 5, 'Drive Statuses': 6}
    csvData = csvList[5:]
    newRows = []
    for row in csvData:
        print row
        RE = row[robotLogTitles['Robot Emotion']]
        OE = row[robotLogTitles['Observable Expression']]
        state = row[robotLogTitles['FSM State']]
        stateN = row[robotLogTitles['FSM State Name']][1:]
        absTime = row[robotLogTitles['State Date Time']].replace(" ", "")
        time = t.mktime(dt.datetime.strptime(absTime, "%Y-%m-%d_%H-%M-%S").timetuple()) - timeStart
        newRows.append([time, state, RE, OE, stateN])

    interactionType = csvList[1][2][len(' Daily Companion '):]
    print '"' + interactionType + '"'
    return newRows, interactionType

def plotAffect(affectCSVList, robotCSVList, userNumber = 1, interactionType = "Morning"):
    BVs = []
    BAs = []
    VVs = []
    VAs = []
    MVs = []
    MAs = []
    times = []
    timesV = []
    for row in affectCSVList:
        [bv, ba, vv, va, mv, ma, ts] = row[0:7]
        BVs += [int(bv)]
        BAs += [int(ba)]
        if vv != "_":
            VVs += [int(vv)]
            VAs += [int(va)]
            timesV += [float(ts)]
        MVs += [int(mv)]
        MAs += [int(ma)]
        times += [float(ts)]
    # print times
    AvgV = [np.mean(MVs)] * len(MVs)
    AvgA = [np.mean(MAs)] * len(MAs)
    maxTime = max(times)

    fig = plt.figure(1)
    r = 4
    c = 1
    plt.subplot(r,c,2)
    plt.title("User " + str(userNumber) + " " + interactionType + " Interaction Measurements")
    plt.ylabel("User\nValence")
    plt.yticks(np.arange(-2.0, 3.0, 1.0))
    plt.axis([-10, maxTime + 10, -2.1, 2.1])
    plt.grid(True)
    bvp, = plt.plot(times, BVs, 'ro', label = "Body Language")
    vvp, = plt.plot(timesV, VVs, 'bo', label = "Vocal Intonation")
    mvp, = plt.plot(times, MVs, 'g', label = "Multi-modal Fusion")
    avgvp, = plt.plot(times, AvgV, 'k--', label = "Average")
    # plt.legend(handles=[bvp, vvp, mvp, avgvp])#, bbox_to_anchor=(1.05,1))

    plt.subplot(r,c,3)
    plt.ylabel("User\nArousal")
    plt.yticks(np.arange(-2.0, 3.0, 1.0))
    plt.axis([-10, maxTime + 10, -2.1, 2.1])
    plt.grid(True)
    plt.plot(times, BAs, 'ro', timesV, VAs, 'bo', times, MAs, 'g', times, AvgA, 'k--')
    # plt.show()

    morningAppraisals = [2,3,5,7,8,9,11,12,13,14,18,19,20,21,24,25,28,29,32,33,37,38,39,45,46,47]
    morningAppraisals = ["morningGood","morningBad","askWeatherGood","askWeatherGoodYesTravel","askWeatherGoodNoTravel",
                         "askWeatherBad","askWeatherBadSameHome","askWeatherBadDiffHome","askWeatherBadDiffHomeYesTake",
                         "askWeatherBadDiffHomeNoTake","askBreakfastAte","askDietGluten","askDietGlutenYesEat",
                         "askDietGlutenNoEat","askDietPoultryYesEat","askDietPoultryNoEat","meal2FeedbackYesDelici",
                         "meal2FeedbackNoDelici","askDietFishYesEat","askDietFishNoEat","meal3FeedbackDinner",
                         "meal3FeedbackYesGood","meal3FeedbackNoGood","exerciseFeedbackGood","exerciseFeedbackBad",
                         "exerciseFeedbackEasy"]
    endDayAppraisals = [2,3,5,6,9,10,11,12,13,14,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,39,40,41,42,43,44,45,46,47,48,49]
    endDayAppraisals = ["dayEndGood","dayEndBad","askWeekendYes","askWeekendNo","meal1CheckinYesAte",
                        "meal1CheckinYesAteYesReg","meal1CheckinYesAteNoReg","meal1CheckinNoAte",
                        "meal1CheckinNoAteYesHad","meal1CheckinNoAteNoHad","meal2CheckinYesAte","meal2CheckinYesAteGood",
                        "meal2CheckinYesAteBad","meal2CheckinNoAte","meal2CheckinNoAteYesComp",
                        "meal2CheckinNoAteYesCompYesResp","meal2CheckinNoAteDontknowComp","meal2CheckinNoAteNoComp",
                        "meal3CheckinYesHad","meal3CheckinYesAte","meal3CheckinYesAteGood","meal3CheckinYesAteBad",
                        "meal3CheckinNoAte","meal3CheckinNoAteYesComp","meal3CheckinNoAteDontknowComp",
                        "meal3CheckinNoAteNoComp","meal3CheckinNoHad","exerciseCheckinYesDid",
                        "exerciseCheckinYesDidGoodDiff","exerciseCheckinYesDidHardDiff","exerciseCheckinYesDidEasyDiff",
                        "exerciseCheckinNoDid","exerciseCheckinNoDidYesComp","exerciseCheckinNoDidYesCompGotResp",
                        "exerciseCheckinNoDidNoComp","exerciseCheckinNoDidNoCompCouldnt",
                        "exerciseCheckinNoDidNoCompDidntWant","exerciseCheckinDontknowDid"]

    REs = []
    OEs = []
    times = []
    aREs = []
    aOEs = []
    atimes = []
    last_stn = ""
    last_re = 0
    last_oe = 0
    for row in robotCSVList:
        [ts, st, re, oe, stn] = row
        if (interactionType == "Morning" and stn in morningAppraisals) or (
            interactionType == "End of Day" and stn in endDayAppraisals) or (
            stn == last_stn):
            aREs += [int(re)]
            aOEs += [int(oe)]
            atimes += [float(ts)]
            REs += [int(last_re)]
            OEs += [int(last_oe)]
            times += [float(ts)-0.1]
        REs += [int(re)]
        OEs += [int(oe)]
        times += [float(ts)]
        last_stn = stn
        last_re = re
        last_oe = oe

    plt.subplot(r,c,4)
    # plt.title("Robot States")
    plt.ylabel("Robot\nState #")
    plt.xlabel("Time (s)")
    plt.yticks(np.arange(0.0, 14.0, 1.0))
    plt.axis([-10, maxTime + 10, -0.1, 14.1])
    rep, = plt.plot(times, REs, 'm-', label = "Robot Emotion")
    oep, = plt.plot(times, OEs, 'c-', label = "Robot Expression")
    plt.plot(atimes, aREs, 'm|', markersize=10)
    plt.plot(atimes, aOEs, 'c|', markersize=10)
    plt.grid(True)
    plt.subplots_adjust(hspace=0.5)

    fig.legend((bvp, vvp, mvp, avgvp, rep, oep),
                ('Body Language', 'Vocal Intonation', 'Multi-modal Fusion', 'Average', "Robot Emotion", "Robot Expression"),
                'upper left', ncol = 2)


    # plt.show()

    ###### make new plots

    fig2 = plt.figure(2,figsize=(8.5,5.6))
    timeStart = times[0]
    timeEnd = times[-1]
    print timeStart, ' ', timeEnd
    want_Square = False
    MVs = []
    MAs = []
    last_v = 0
    last_a = 0
    aff_times = []
    for row in affectCSVList:
        [bv, ba, vv, va, mv, ma, ts] = row[0:7]
        if float(ts) >= timeStart and float(ts) <= timeEnd:
            if want_Square:
                MVs += [last_v]
                MAs += [last_a]
                aff_times += [float(ts) - timeStart - 0.1]
            MVs += [int(mv)]
            MAs += [int(ma)]
            aff_times += [float(ts) - timeStart]
            last_v = int(mv)
            last_a = int(ma)
    AvgV = [np.mean(MVs)] * len(MVs)
    AvgA = [np.mean(MAs)] * len(MAs)

    AvgV = [0.216666666667] * len(MVs)
    AvgA = [-0.283333333333] * len(MVs)


    print "average V: ", np.mean(MVs), "Average A: ",np.mean(MAs)

    gs = gridspec.GridSpec(3,2, height_ratios=[2,2,1], width_ratios=[1,100])
    r = 3
    c = 1
    plt.subplot(gs[1])
    # plt.title("User " + str(userNumber) + " " + interactionType + " Interaction Measurements", fontsize=16)
    plt.yticks(np.arange(-2.0, 3.0, 1.0), fontsize=14)
    plt.xticks(fontsize=10)
    plt.axis([0, timeEnd-timeStart, -2.1, 2.1])
    plt.grid(True)
    plt.ylabel("User Affect", fontsize=14)
    # plt.xlabel("Time (s)")
    mvp, = plt.plot(aff_times, MVs, 'g', label = "User's Valence")
    map, = plt.plot(aff_times, MAs, 'b', label = "User's Arousal")
    avgvp, = plt.plot(aff_times, AvgV, 'g--', label = "Average Valence")
    avgap, = plt.plot(aff_times, AvgA, 'b--', label = "Average Arousal")

    for t in range(len(times)):
        times[t] -= timeStart
    for t in range(len(atimes)):
        atimes[t] -= timeStart
    print times
    print REs
    print OEs
    REOEs, ts = colourREplotData(times, REs, OEs)
    print
    print REOEs
    # print ts
    h, i, s, w, a, h2, i2, s2, w2, a2, sc1, sc2, sc3 = REOEs
    # h_t, i_t, s_t, w_t, a_t, h2_t, i2_t, s2_t, w2_t, a2_t, sc1_t, sc2_t, sc3_t = ts

    print h
    print ts
    print len(h)
    print len(ts)

    plt.subplot(gs[3])
    labels = ["Happy", "Interested", "Sad", "Worried", "Angry", "Scared P", "Scared T", "Scared L"]
    plt.ylabel("Robot\nEmotional State", fontsize=14)
    plt.xlabel("Time (s)", fontsize=12)
    plt.yticks(np.arange(0.0, 8.0, 1.0), labels, fontsize=14)
    # plt.yticks(np.arange(0.0, 14.0, 1.0), minor=True)
    plt.xticks(fontsize=10)
    plt.axis([0, timeEnd-timeStart, -1, 8])
    # rep, = plt.plot(times, REs, 'm-', label = "Robot Emotion")
    hp, = plt.plot(ts, h, '-', label = "Happy", color="yellowgreen", linewidth=3)
    ip, = plt.plot(ts, i, '-', label = "Interested", color="#ffff00", linewidth=3)
    sp, = plt.plot(ts, s, '-', label = "Sad", color="cyan", linewidth=3)
    wp, = plt.plot(ts, w, '-', label = "Worried", color="magenta", linewidth=3)
    ap, = plt.plot(ts, a, '-', label = "Angry", color="#ff0000", linewidth=3)

    h2p, = plt.plot(ts, h2, '-', label = "Happy 2", color="darkgreen", linewidth=3)
    i2p, = plt.plot(ts, i2, '-', label = "Interested 2", color="#e5c100", linewidth=3)
    s2p, = plt.plot(ts, s2, '-', label = "Sad 2", color="darkblue", linewidth=3)
    w2p, = plt.plot(ts, w2, '-', label = "Worried 2", color="darkorchid", linewidth=3)
    a2p, = plt.plot(ts, a2, '-', label = "Angry 2", color="#bb0000", linewidth=3)

    sc1p, = plt.plot(ts, sc1, '-', label = "Scared P", color="#ffa500", linewidth=3)
    sc2p, = plt.plot(ts, sc2, '-', label = "Scared T", color="orangered", linewidth=3)
    sc3p, = plt.plot(ts, sc3, '-', label = "Scared L", color="#996300", linewidth=3)
    # oep, = plt.plot(times, OEs, 'c-', label = "Robot Expression")
    # plt.plot(atimes, aREs, 'm|', markersize=10)
    # plt.plot(atimes, aOEs, 'c|', markersize=10)
    plt.grid(True)

    blank, = plt.plot([0], [0], '-', color='none', label='')
    fig2.legend((mvp, map, avgvp, avgap, blank,
                 hp, ip, sp, wp, ap, h2p, i2p, s2p, w2p, a2p,
                 sc1p, sc2p, sc3p, blank, blank
                 ),
                ('User Valence', 'User Arousal', "Valence Average", "Arousal Average", ""
                    ,"Low Degree Happy", "Low Degree Interested", "Low Degree Sad", "Low Degree Worried", "Low Degree Angry",
                    "High Degree Happy", "High Degree Interested", "High Degree Sad", "High Degree Worried", "High Degree Angry",
                    "Scared P (Picked Up)", "Scared T (Touched)", "Scared L (Ledges)", "", ""
                 ),
                'lower center', ncol = 4, prop={'size':10})

    plt.show()

    last_t = max(aff_times)
    aff_times2 = []
    for t in range(len(aff_times)):
        aff_times2.append(1.0 * aff_times[t] / last_t)
    affData = [aff_times2, MVs, MAs]
    robtData = groupColourData(times, REs, OEs)
    makeFormatDataset(affData, robtData, str(userNumber), interactionType)


def colourREplotData(times, REs, OEs):
    # h = i = s = w = a = h2 = i2 = s2 = w2 = a2 = sc1 = sc2 = sc3 = []
    # h_t = i_t = s_t = w_t = a_t = h2_t = i2_t = s2_t = w2_t = a2_t = sc1_t = sc2_t = sc3_t = []
    # REOEs = [h, i, s, w, a, h2, i2, s2, w2, a2, sc1, sc2, sc3]
    REOEs = [[],[],[],[],[],[],[],[],[],[],[],[],[]]
    # ts  = [h_t, i_t, s_t, w_t, a_t, h2_t, i2_t, s2_t, w2_t, a2_t, sc1_t, sc2_t, sc3_t]
    ts = []

    last_re = 0
    for t in range(len(times)):
        re = REs[t]
        oe = OEs[t]
        REOEs[oe].append(last_re)
        REOEs[oe].append(re)
        for r in range(13):
            if r != oe:
                REOEs[r].append(None)
                REOEs[r].append(None)
        ts += [times[t]-0.1,times[t]]
        last_re = re

    return REOEs, ts


def groupColourData(times, REs, OEs):

    REOEs = [[], [], []] # low degree, high degree, scared3
    ts = []

    last_re = 0
    for t in range(len(times)):
        re = REs[t]
        oe = OEs[t]
        style = 2
        if oe < 5 or oe == 10:
            style = 0
        elif oe < 12:
            style = 1
        REOEs[style].append(last_re)
        REOEs[style].append(re)

        for s in range(3):
            if s != style:
                REOEs[s].append(-1)
                REOEs[s].append(-1)
        ts += [times[t]-0.1,times[t]]
        last_re = re

    last_time = max(ts)
    print last_time
    for t in range(len(ts)):
        ts[t] /= 1.0*last_time

    return REOEs, ts


def makeFormatDataset(affData, robtData,  userNumber, interactionType):
    fileNameAff = "AffectData_" + userNumber + "_" + interactionType + ".csv"
    [aff_times, MVs, MAs] = affData

    for t in range(len(aff_times)):
        writeText = userNumber + "," + str(aff_times[t]) + "," + str(MVs[t]) + "," + str(MAs[t])
        writeTextLine(fileNameAff, writeText)

    fileNameRobt = "RobotData_" + userNumber + "_" + interactionType + ".csv"
    [REOEs, ts] = robtData
    print REOEs
    print len(REOEs[2])
    print len((ts))
    for t in range(len(ts)):
        print t
        writeText = userNumber + "," + str(ts[t]) + "," + str(REOEs[0][t]) + "," + str(REOEs[1][t]) + "," + str(REOEs[2][t])
        writeTextLine(fileNameRobt, writeText)

def writeTextLine(textFile, lineText):
    try:
        import os.path
        if os.path.isfile(textFile):
            f = open(textFile, 'a')
        else:
            f = open(textFile.replace('\\', '/'), 'a')
        s = "\n" + str(lineText)
        f.write(s)
        f.close()
    except Exception, e:
        print "Writing to file '" + textFile + "' Failed with error:"
        print e

















affectLog = importCSVFile(affectFileName)
# printCSVList(affectLog)

colsWant = ['BV', 'BA', 'vV', 'vA', 'MV', 'MA', 'absTime', 'usedV']
affectLogAV = makeNewList(affectLog, colsWant)
printCSVList(affectLogAV)

print
colsWant = ['BV', 'BA', 'VV', 'VA', 'MV', 'MA', 'time', 'usedV']
affectLog, startTime = formatAffectList(affectLog)
affectLogAV = makeNewList(affectLog, colsWant)
# printCSVList(affectLogAV)

print
# plt, fig, maxTime = plotAffect(affectLogAV, userNumber)


robotLog = importCSVFile(robotFileName)
printCSVList(robotLog)
print
robotLog, interactionType = formatRobotLog(robotLog, startTime, offsetHours)
printCSVList(robotLog)
print
plotAffect(affectLogAV, robotLog, userNumber, interactionType)


print "Done"