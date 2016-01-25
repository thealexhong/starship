from NAO_Util.BasicMotions import BasicMotions
from naoqi import ALProxy

import sys
import time
import NAO_Util.fileUtilitiy
from DietFitnessFSM import DietFitnessFSM
from GenUtil import GenUtil
import FileUtilitiy
from ThreadedCheckers import ThreadedChecker
from UserAffectGenerator import UserAffectGenerator

def main(NAOip, NAOport):
    naoMotions = BasicMotions(NAOip, NAOport)
    robotName = "Luke"
    genUtil = GenUtil(naoMotions)
    genUtil.showFoodDB()

    thread1 = ThreadedChecker(1, "Main Checker #1", genUtil)
    thread2 = UserAffectGenerator(2, "User Affect Generator #1", 3, genUtil)

    # select your activity to run
    activityConsultant = "Consultant By Appointment"
    # Daily Companion
    activityDayCompMorning = "Daily Companion Morning"
    activityDayCompDayEnd = "Daily Companion End of Day"

    userName = "Test User"
    userNumber = "1"
    dateTime = genUtil.getDateTime()
    activityInteractionType = activityDayCompMorning
    userInfo = initiateUserInfo(userName, userNumber, activityInteractionType, dateTime)

    # naoMotions.naoSit()
    # naoMotions.naoStand()
    # naoMotions.naoWaveBoth()

    # ============================================================= Start Functionality
    print("State Machine Started")
    print
    print
    thread1.start()
    thread2.start()

    dietFitnessFSM = DietFitnessFSM(genUtil, robotName, userName, userNumber, activityInteractionType)
    [currentState, robotEmotionNum, obserExpresNum] = dietFitnessFSM.getFSMState()

    appraiseState = False
    user_input = "Start"
    while currentState != 0:
        print("=========================================================================================")
        print "FSM Info: ", [currentState, robotEmotionNum, obserExpresNum]
        if appraiseState:
            genUtil.expressEmotion(obserExpresNum)

        [currentState, robotEmotionNum, obserExpresNum, appraiseState] = dietFitnessFSM.activityFSM()
    thread1.quit()
    thread2.quit()

    naoMotions.naoSit()
    print
    [stateTimeStamp, stateDateTime, fsmStateHist,
     reHist, oeHist, driveStatHist, fsmStateNameHist] = dietFitnessFSM.getHistories()
    print "History of TimeStamps: ", stateTimeStamp
    print "History of DateTimes: ", stateDateTime
    print "History of FSM States: ", fsmStateHist
    print "History of FSM State Names: ", fsmStateNameHist
    print "History of Robot Emotions: ", reHist
    print "History of Observable Expressions: ", oeHist
    print "History of Drive Statuses: ", driveStatHist
    writeUserHistories(userName, userNumber, userInfo,
                       stateTimeStamp, stateDateTime, fsmStateHist, reHist, oeHist, driveStatHist, fsmStateNameHist)


    print
    print("State Machine Finished")

def initiateUserInfo(userName, userNumber, activityType, dateTime):
    fileName = "ProgramDataFiles\userInfo.csv"
    writeLine = userName + ", " + str(userNumber) + ", " + activityType + ", " +dateTime
    FileUtilitiy.writeTextLine(fileName, writeLine)
    return writeLine

def writeUserHistories(userName, userNumber, userInfo,
                       stateTimeStamp, stateDateTime, fsmStateHist, reHist, oeHist, driveStatHist, fsmStateNameHist):
    fileName = "ProgramDataFiles\\" + userNumber + "_" + userName + ".csv"
    FileUtilitiy.writeTextLine(fileName, userInfo + " \n")
    writeLine = "State Time Stamp, State Date Time, FSM State, FSM State Name, Robot Emotion, Observable Expression, Drive Statuses"
    FileUtilitiy.writeTextLine(fileName, writeLine + " \n")
    for i in range(len(stateTimeStamp)):
        writeLine = str(stateTimeStamp[i]) + ", " + stateDateTime[i] + ", " + str(fsmStateHist[i]) + ", " + fsmStateNameHist[i] + ", "
        writeLine += str(reHist[i]) + ", " + str(oeHist[i]) + ", " + str(driveStatHist[i]).replace(",", "")
        FileUtilitiy.writeTextLine(fileName, writeLine)

# def writeConsole(userName, userNumber, dateTime):
#     fileName = "ProgramDataFiles\ConsoleOutput\\" + userNumber + "_" + userName + "_" + dateTime + ".txt"
#     sys.stdout = open(fileName, 'w')

def testNaoConnection(NAOip, NAOport):
    worked = False
    try:
        naoBehavior = connectToProxy(NAOip, NAOport, "ALBehaviorManager")
        names = naoBehavior.getInstalledBehaviors()
        print names
    
        naoMotions = BasicMotions(NAOip, NAOport)
        naoMotions.naoSay("Connected!")
        # naoMotions.naoSit()
        # naoMotions.naoShadeHeadSay("Hello, the connection worked!")
        worked = True
    except:
        print "Connection Failed, maybe wrong IP and/or Port"
        
    print "Connection Test Finished"
    return worked
    

def connectToProxy(NAOip, NAOport, proxyName):
        try:
            proxy = ALProxy(proxyName, NAOip, NAOport)
        except Exception, e:
            print "Could not create Proxy to ", proxyName
            print "Error was: ", e
            proxy = ""

        return proxy

if __name__ == '__main__':
    simulated = False
    if simulated:
        #simulated NAO
        NAOIP = "127.0.0.1"
        NAOPort = 63899
    else:
        #real NAO
        NAOIP = "luke.local"
        NAOPort = 9559

    print("Initiated Values")

    connWorks = testNaoConnection(NAOIP, NAOPort)
    print connWorks
    if connWorks:
        main(NAOIP, NAOPort)




