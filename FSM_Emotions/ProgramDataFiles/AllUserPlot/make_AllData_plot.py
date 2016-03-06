import csv
import matplotlib.pyplot as plt
import numpy as np
from matplotlib import gridspec

affectFileName = "_AffectData_End of Day.csv"
robotFileName = "_RobotData_End of Day.csv"

numUsers = 1

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


def makeGroupPlot(affectLog,robotLog):

    colours = ['blue', 'green', 'red', 'orange', 'yellow', '#614126', 'cyan', 'magenta']
    userNumbers = [17,18, 21,24,25,26,28,29]

    legend = []
    legendName = []

    fig = plt.figure(1,figsize=(9,7.1))
    gs = gridspec.GridSpec(3,2, height_ratios=[1.4,1.4,1], width_ratios=[1,100])
    r = 3
    c = 1
    plt.subplot(gs[1])
    # plt.title("User " + str(userNumber) + " " + interactionType + " Interaction Measurements", fontsize=16)
    plt.yticks(np.arange(-2.0, 3.0, 1.0), fontsize=16)
    plt.xticks(fontsize=14)
    plt.axis([0, 1, -2, 2])
    plt.grid(True)
    plt.ylabel("User Affect", fontsize=18)

    for u in range(len(userNumbers)):
        userNum = userNumbers[u]
        ts = []
        Vs = []
        As = []

        for i in range(len(affectLog)):
            row = affectLog[i]
            # print row
            [uNum, t, v, a] = row
            if uNum == str(userNum):
                ts.append(t)
                Vs.append(v)
                As.append(a)

        filterSize = 0
        # print 2*filterSize+1.0
        for i in range(filterSize, len(ts)-filterSize):
            v = 0
            a = 0
            for j in range(-1*filterSize, filterSize+1):
                # print j
                v += float(Vs[i+j])/(2*filterSize+1.0)
                a += float(As[i+j])/(2*filterSize+1.0)
            Vs[i] = v
            As[i] = a
        for i in range(filterSize):
            Vs[i] = None
            As[i] = None
            Vs[-1*i] = None
            As[-1*i] = None

        mvp, = plt.plot(ts, Vs, '-', color = colours[u])
        map, = plt.plot(ts, As, '--', color = colours[u])

    allUserV = allUserA = 0
    for row in affectLog:
        allUserV += float(row[2])/len(affectLog)
        allUserA += float(row[3])/len(affectLog)
    tav = np.arange(0,1.025,0.025)
    Va = [np.round(allUserV)] * len(tav)
    Aa = [np.round(allUserA)] * len(tav)
    print "Average V:",allUserV, " Average A:", allUserA
    vap, = plt.plot(tav, Va, '-', color = 'blACK', linewidth=3.0)
    aap, = plt.plot(tav, Aa, '--', color = 'blACK', linewidth=3.0)


    plt.subplot(gs[3])
    labels = ["Happy", "Interested", "Sad", "Worried", "Angry", "Scared P", "Scared T", "Scared L"]
    plt.ylabel("Robot\nEmotional State", fontsize=18)
    plt.xlabel("Normalized Interaction Time", fontsize=18)
    plt.yticks(np.arange(0.0, 8.0, 1.0), labels, fontsize=16)
    # plt.yticks(np.arange(0.0, 14.0, 1.0), minor=True)
    plt.xticks(fontsize=14)
    plt.axis([0, 1, -1, 8])
    plt.grid(True)
    for u in range(len(userNumbers)):
        userNum = userNumbers[u]
        ts2 = []
        Ls = []
        Hs = []
        S3s = []
        for row in robotLog:

            for r in range(len(row)):
                if row[r] == str(-1):
                    row[r] = None
            # print row
            [uNum, t, ls, hs, s3s] = row
            if uNum == str(userNum):
                ts2.append(t)
                Ls.append(ls)
                Hs.append(hs)
                S3s.append(s3s)
        rLsp, = plt.plot(ts2, Ls, '-', color = colours[u], linewidth=3.0)
        rHsp, = plt.plot(ts2, Hs, '--', color = colours[u], linewidth=3.0)
        rS3sp, = plt.plot(ts2, S3s, ':', color = colours[u], linewidth=3.0)

        # legend.append(rLsp)
        lgd, = plt.plot(-1, -1, '-', color = colours[u], linewidth=3.0)
        legend.append(lgd)
        legendName.append("User " + str(u+1))

    valp, = plt.plot(-1, -1, '-', color = 'black')
    arop, = plt.plot(-1, -1, '--', color = 'black')
    rlowp, = plt.plot(-1, -1, '-', color = 'black', linewidth=3.0)
    rhighp, = plt.plot(-1, -1, '--', color = 'black', linewidth=3.0)
    styleLegend = [valp, arop, rlowp, rhighp]
    styleLegendNames = ["Valence", "Arousal", "Low Intensity Expression", "High Intensity Expression"]

    blank, = plt.plot([0], [0], '-', color='none', label='')
    # fig.legend(legend,legendName,
    #             bbox_to_anchor=(-0.46, -0.81, 1, 1), ncol = 2, prop={'size':14})
    # fig.legend(styleLegend, styleLegendNames,
    #             bbox_to_anchor=(-0.09, -0.81, 1, 1), ncol = 1, prop={'size':14})
    fig.legend([blank] + legend[0:2] + [rlowp,blank,blank] + legend[2:4] + [rhighp,blank,blank] + legend[4:6] + [blank,blank,blank] + legend[6:8] + [blank,blank],
               [""] + legendName[0:2] + ["Average Valence\nAcross Users","",""] + legendName[2:4] + ["Average Arousal\nAcross Users","",""] + legendName[4:6] + ["","",""] + legendName[6:8] + ["",""],
                'lower center', ncol = 4, prop={'size':14})


    plt.show()

affectLog = importCSVFile(affectFileName)
robotLog = importCSVFile(robotFileName)
makeGroupPlot(affectLog,robotLog)




