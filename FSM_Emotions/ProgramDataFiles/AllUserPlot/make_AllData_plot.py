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

    colours = ['blue', 'green', 'red', 'orange', 'yellow', 'black', 'cyan', 'magenta']
    userNumbers = [17,18, 21,24,25,26,28,29]

    legend = []
    legendName = []

    fig = plt.figure(1,figsize=(8.5,5.6))
    gs = gridspec.GridSpec(3,2, height_ratios=[2,2,1], width_ratios=[1,100])
    r = 3
    c = 1
    plt.subplot(gs[1])
    # plt.title("User " + str(userNumber) + " " + interactionType + " Interaction Measurements", fontsize=16)
    plt.yticks(np.arange(-2.0, 3.0, 1.0), fontsize=14)
    plt.xticks(fontsize=10)
    plt.axis([0, 1, -2, 2])
    plt.grid(True)
    plt.ylabel("User Affect", fontsize=14)

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
    plt.ylabel("Robot\nEmotional State", fontsize=14)
    plt.xlabel("Normalized Interaction Time", fontsize=12)
    plt.yticks(np.arange(0.0, 8.0, 1.0), labels, fontsize=14)
    # plt.yticks(np.arange(0.0, 14.0, 1.0), minor=True)
    plt.xticks(fontsize=10)
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
        legendName.append("User " + str(userNum))


    blank, = plt.plot([0], [0], '-', color='none', label='')
    fig.legend(legend,legendName,
                'lower center', ncol = 2, prop={'size':10})

    plt.show()

affectLog = importCSVFile(affectFileName)
robotLog = importCSVFile(robotFileName)
makeGroupPlot(affectLog,robotLog)




