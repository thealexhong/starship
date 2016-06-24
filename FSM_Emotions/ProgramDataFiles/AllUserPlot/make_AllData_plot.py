import csv
import matplotlib.pyplot as plt
import numpy as np
from matplotlib import gridspec

# name of the files containing the data from all users (8 users used in plots)
if(0):
    affectFileName = "_AffectData_Morning.csv"
    robotFileName = "_RobotData_Morning.csv"
else:
    affectFileName = "_AffectData_End of Day.csv"
    robotFileName = "_RobotData_End of Day.csv"




# reads in the CSV file into a list
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

# makes the plot of all user data for the end of day 
def makeGroupPlot(affectLog,robotLog):

    # select the colors for each of the 8 users plot lines
    colours = ['blue', 'green', 'red', 'orange', 'yellow', '#614126', 'cyan', 'magenta']
    # the user # of the 8 users we want to plot
    userNumbers = [17,18, 21,24,25,26,28,29]

    legend = []
    legendName = []

    # create the figure size and the ratios between the subplot sizes
    fig = plt.figure(1,figsize=(9,7.1))
    gs = gridspec.GridSpec(3,2, height_ratios=[1.4,1.4,1], width_ratios=[1,100])
    plt.subplot(gs[1])


    # format the first subplot - plotting the Valence and Arousal
    plt.yticks(np.arange(-2.0, 3.0, 1.0), fontsize=16)
    plt.xticks(fontsize=14)
    plt.axis([0, 1, -2, 2])
    plt.grid(True)
    plt.ylabel("User Affect", fontsize=18)

    for u in range(len(userNumbers)):
        userNum = userNumbers[u]
        ###################################################################
        # keep separate lists of the time, valance and arousal for each users data
        ts = []
        Vs = []
        As = []

        # loop through all the user data, and select the data relevant to user 'u'
        for i in range(len(affectLog)):
            row = affectLog[i]
            # print row
            [uNum, t, v, a] = row
            if uNum == str(userNum):
                ts.append(t)
                Vs.append(v)
                As.append(a)


        filteredVs = [0]*len(ts)
        filteredAs = [0]*len(ts)

        # apply an averaging mask of size 1+filterSize if we want to smooth the curve
        filterSize = 2
        # print 2*filterSize+1.0
        for i in range(filterSize, len(ts)-filterSize):
            for j in range(-1*filterSize, filterSize+1):
                index = i+j
                #if(index<0):
                #    index=0
                #elif(index>=len(ts)-1):
                #    index=len(ts)-1
                filteredVs[i] += float(Vs[index])/(2*filterSize+1.0)
                filteredAs[i] += float(As[index])/(2*filterSize+1.0)

        for i in range(filterSize):
            filteredVs[i] = filteredVs[filterSize]
            filteredAs[i] = filteredAs[filterSize]
            filteredVs[-1*i] = filteredVs[-filterSize]
            filteredAs[-1*i] = filteredVs[-filterSize]

        # plot this users data to the first subplot
        mvp, = plt.plot(ts, filteredVs, '-', color = colours[u])
        map, = plt.plot(ts, filteredAs, '--', color = colours[u])

    # get ther average Valence and Arousal for all the users
    allUserV = allUserA = 0
    for row in affectLog:
        allUserV += float(row[2])/len(affectLog)
        allUserA += float(row[3])/len(affectLog)
    tav = np.arange(0,1.025,0.025)
    Va = [np.round(allUserV)] * len(tav)
    Aa = [np.round(allUserA)] * len(tav)
    print "Average V:",allUserV, " Average A:", allUserA
    # plot the averages on the first subplot
    vap, = plt.plot(tav, Va, '-', color = 'blACK', linewidth=3.0)
    aap, = plt.plot(tav, Aa, '--', color = 'blACK', linewidth=3.0)

    # create and format the second subplot - Plotting the robots states during the interaction
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
        # for each user store the time, low intensity and high intensity expressions
        ts2 = []
        Ls = []
        Hs = []
        S3s = []
        # loop through all the robot data for each user and select the data relevant to user 'u'
        for row in robotLog:
            [uNum, t, ls, hs, s3s] = row
            if uNum == str(userNum):
                for r in range(len(row)):
                # set data that had no expression to None (will need to read the code that formats the data files to understand this)
                    if row[r] == str(-1):
                        row[r] = None
                # print row
                [uNum, t, ls, hs, s3s] = row
                ts2.append(t)
                Ls.append(ls)
                Hs.append(hs)
                S3s.append(s3s)
        # plot the robot data for each users interaction

        rLsp, = plt.plot(ts2, Ls, '-', color = colours[u], linewidth=3.0)
        rHsp, = plt.plot(ts2, Hs, '--', color = colours[u], linewidth=3.0)
        rS3sp, = plt.plot(ts2, S3s, '--', color = colours[u], linewidth=3.0)

        # legend.append(rLsp)
        lgd, = plt.plot(-1, -1, '-', color = colours[u], linewidth=3.0)
        legend.append(lgd)
        legendName.append("User " + str(u+1))

    # create and format the legend
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


# the part of the code that actually runs, imports the files then runs the method
affectLog = importCSVFile(affectFileName)
robotLog = importCSVFile(robotFileName)
makeGroupPlot(affectLog,robotLog)




