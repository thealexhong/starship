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

    colours = ['blue', 'green', 'red', 'orange', 'yellow', 'grey', 'cyan', 'magenta']
    userNumbers = [17,18, 21,24,25,26,28,29]

    legend = []
    legendName = []

    fig = plt.figure(1,figsize=(9,7))
    gs = gridspec.GridSpec(3,2, height_ratios=[2.5,2.5,1], width_ratios=[1,100])
    r = 3
    c = 1
    plt.subplot(gs[1])
    # plt.title("User " + str(userNumber) + " " + interactionType + " Interaction Measurements", fontsize=16)
    plt.yticks(np.arange(-2.0, 3.0, 1.0), fontsize=16)
    plt.xticks(fontsize=14)
    plt.axis([0, 1, -2, 2])
    plt.grid(True)
    plt.ylabel("User Affect", fontsize=22)

    for u in range(len(userNumbers)):
        userNum = userNumbers[u]
        ts = []
        Vs = []
        As = []
        for row in affectLog:
            print row
            [uNum, t, v, a] = row
            if uNum == str(userNum):
                ts.append(t)
                Vs.append(v)
                As.append(a)
        mvp, = plt.plot(ts, Vs, '-', color = colours[u])
        map, = plt.plot(ts, As, ':', color = colours[u])


    plt.subplot(gs[3])
    labels = ["Happy", "Interested", "Sad", "Worried", "Angry", "Scared P", "Scared T", "Scared L"]
    plt.ylabel("Robot Emotional State", fontsize=22)
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
            print row
            [uNum, t, ls, hs, s3s] = row
            if uNum == str(userNum):
                ts2.append(t)
                Ls.append(ls)
                Hs.append(hs)
                S3s.append(s3s)
        rLsp, = plt.plot(ts2, Ls, '-', color = colours[u], linewidth=3.0)
        rHsp, = plt.plot(ts2, Hs, '--', color = colours[u], linewidth=3.0)
        rS3sp, = plt.plot(ts2, S3s, ':', color = colours[u], linewidth=3.0)

        legend.append(rLsp)
        legendName.append("User " + str(u+1))

    valp, = plt.plot(-1, -1, '-', color = 'black')
    arop, = plt.plot(-1, -1, ':', color = 'black')
    rlowp, = plt.plot(-1, -1, '-', color = 'black', linewidth=3.0)
    rhighp, = plt.plot(-1, -1, '--', color = 'black', linewidth=3.0)
    styleLegend = [valp, arop, rlowp, rhighp]
    styleLegendNames = ["Valence", "Arousal", "Low Degree Expression", "High Degree Expression"]

    blank, = plt.plot([0], [0], '-', color='none', label='')
    fig.legend(legend,legendName,
                bbox_to_anchor=(-0.44, -0.81, 1, 1), ncol = 2, prop={'size':14})
    fig.legend(styleLegend, styleLegendNames,
                bbox_to_anchor=(-0.09, -0.81, 1, 1), ncol = 1, prop={'size':14})

    plt.show()

affectLog = importCSVFile(affectFileName)
robotLog = importCSVFile(robotFileName)
makeGroupPlot(affectLog,robotLog)




