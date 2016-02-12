import csv
import time as t
import datetime as dt
import matplotlib.pyplot as plt
import numpy as np


userNumber = 10
affectFileName = "tan1 endofday 2016-02-11 4_33_30 PM.csv"
robotFileName = "10_Tan_Flow_endofday.csv"
offsetHours = 5

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
    csvData = csvList[4:]
    newRows = []
    for row in csvData:
        RE = row[robotLogTitles['Robot Emotion']]
        OE = row[robotLogTitles['Observable Expression']]
        state = row[robotLogTitles['FSM State']]
        absTime = row[robotLogTitles['State Date Time']].replace(" ", "")
        time = t.mktime(dt.datetime.strptime(absTime, "%Y-%m-%d_%H-%M-%S").timetuple()) - timeStart
        newRows.append([time, state, RE, OE])
    return newRows

def plotAffect(affectCSVList, robotCSVList, userNumber = 1):
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
    plt.title("User " + str(userNumber) + " Affect Measurements")
    plt.ylabel("Valence")
    plt.yticks(np.arange(-2.0, 3.0, 1.0))
    plt.axis([-10, maxTime + 10, -2.1, 2.1])
    plt.grid(True)
    bvp, = plt.plot(times, BVs, 'ro', label = "Body Language")
    vvp, = plt.plot(timesV, VVs, 'bo', label = "Vocal Intonation")
    mvp, = plt.plot(times, MVs, 'g', label = "Multi-modal Fusion")
    avgvp, = plt.plot(times, AvgV, 'k--', label = "Average")
    # plt.legend(handles=[bvp, vvp, mvp, avgvp])#, bbox_to_anchor=(1.05,1))

    plt.subplot(r,c,3)
    plt.ylabel("Arousal")
    plt.yticks(np.arange(-2.0, 3.0, 1.0))
    plt.axis([-10, maxTime + 10, -2.1, 2.1])
    plt.grid(True)
    plt.plot(times, BAs, 'ro', timesV, VAs, 'bo', times, MAs, 'g', times, AvgA, 'k--')
    # plt.show()

    REs = []
    OEs = []
    times = []
    for row in robotCSVList:
        [ts, st, re, oe] = row
        REs += [int(re)]
        OEs += [int(oe)]
        times += [float(ts)]

    plt.subplot(r,c,4)
    # plt.title("Robot States")
    plt.ylabel("Emotion/Expression #")
    plt.xlabel("Time (s)")
    plt.yticks(np.arange(0.0, 14.0, 2.0))
    plt.axis([-10, maxTime + 10, -0.1, 14.1])
    rep, = plt.plot(times, REs, 'mo-', label = "Robot Emotion")
    oep, = plt.plot(times, OEs, 'co-', label = "Robot Expression")
    plt.grid(True)
    plt.subplots_adjust(hspace=0.5)

    fig.legend((bvp, vvp, mvp, avgvp, rep, oep),
                ('Body Language', 'Vocal Intonation', 'Multi-modal Fusion', 'Average', "Robot Emotion", "Robot Expression"),
                'upper left', ncol = 2)


    plt.show()

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
robotLog = formatRobotLog(robotLog, startTime, offsetHours)
printCSVList(robotLog)
print
plotAffect(affectLogAV, robotLog, userNumber)


print "Done"