from naoqi import ALBroker
from naoqi import ALProxy

import FileUtilitiy
from BasicMotions import BasicMotions
from DietFitnessFSM import DietFitnessFSM
from GenUtil import GenUtil
import NAOReactionChecker
from ThreadedCheckers import ThreadedChecker
from UserAffectGenerator import UserAffectGenerator
import json
import atexit
import time

def main(NAOip, NAOport, name):

    print "***************************************************************************"
    print "***************************************************************************"
    print "Have You updated the _FSM_INPUT File for this interaction? (1) yes, (2) no: ",
    textInput = str(raw_input()).lower()
    print "***************************************************************************"

    if textInput == "1":
        naoMotions = BasicMotions(NAOip, NAOport)
        robotName = name
        genUtil = GenUtil(naoMotions)
        genUtil.showFoodDB()

        myBroker = ALBroker("myBroker", "0.0.0.0", 0, NAOip, NAOport)
        global naoReactionChecker
        naoReactionChecker = NAOReactionChecker.NAOReactionChecker(genUtil, NAOip, NAOport)

        thread1 = ThreadedChecker(1, "Main Checker #1", genUtil)
        thread2 = UserAffectGenerator(2, "User Affect Generator #1", 3, genUtil)

        dateTime = genUtil.getDateTime()
        [userName, userNumber, activityInteractionType, lastInteraction] = getFSMInputVars()

        userInfo = initiateUserInfo(userName, userNumber, activityInteractionType, dateTime)

        # runSomeTest(genUtil)

        genUtil.showHappyEyes()
        # naoMotions.naoSit()
        print "NAO is currently: ", naoMotions.getPostureFamily()
        speed = 0.2
        if naoMotions.getPostureFamily() == "Sitting":
            speed = 0.7

        global sitTest
        sitTest = False
        if not sitTest:
            naoMotions.naoStand(speed)
        # naoMotions.naoWaveBoth()
        naoMotions.naoAliveON()

        # ============================================================= Start Functionality
        print("State Machine Started")
        print
        print
        thread1.start()
        # thread2.start()

        dietFitnessFSM = DietFitnessFSM(genUtil, robotName, userName, userNumber, activityInteractionType, lastInteraction)
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
        # thread2.quit()
        genUtil.naoTurnOffEyes()
        # naoMotions.naoSit()
        NAOReactionChecker.ActuallyUnsubscribeAllEvents()
        myBroker.shutdown()
        naoMotions.naoAliveOff()

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
        writeUserHistories(userName, userNumber, userInfo, dateTime,
                           stateTimeStamp, stateDateTime, fsmStateHist, reHist, oeHist, driveStatHist, fsmStateNameHist)

    else:
        print "Go updated the input file for this interaction"
        print "It can be found at: ProgramDataFiles\_FSM_INPUT.json"

    print
    print("State Machine Finished")

def getFSMInputVars():
    fileName = "ProgramDataFiles\_FSM_INPUT.json"
    jsInput = FileUtilitiy.readFileToJSON(fileName)
    # print jsInput
    userName = jsInput['userName']
    userNumber = jsInput['userNumber']
    interactionType = jsInput['interactionType']
    lastInteraction = jsInput['lastInteraction']
    print
    print "userName: ", userName, " | userNumber: ", userNumber, " | interactionType: ", interactionType, " | lastInteraction: ", lastInteraction
    print
    # select your activity to run
    # activityConsultant = "Consultant By Appointment"
    if 'morning' in interactionType.lower():
        activityInteractionType = "Daily Companion Morning"
    else:
        activityInteractionType = "Daily Companion End of Day"

    return [userName, userNumber, activityInteractionType, lastInteraction]


def initiateUserInfo(userName, userNumber, activityType, dateTime):
    fileName = "ProgramDataFiles\userInfo.csv"
    writeLine = userName + ", " + str(userNumber) + ", " + activityType + ", " +dateTime
    FileUtilitiy.writeTextLine(fileName, writeLine)
    FileUtilitiy.makeFolder("ProgramDataFiles\\" + str(userNumber) + "_" + userName)

    return writeLine

def writeUserHistories(userName, userNumber, userInfo, dateTime,
                       stateTimeStamp, stateDateTime, fsmStateHist, reHist, oeHist, driveStatHist, fsmStateNameHist):
    fileName = "ProgramDataFiles\\" + str(userNumber) + "_" + userName + "\\" + str(userNumber)  + "_" + userName +"_Flow_" + str(dateTime) + ".csv"
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

def runSomeTest(genUtil):
    s = "Did you have the An apple and two slices of toast with strawberry jam for breakfast this morning?"
    genUtil.showFearVoice(s)

def testNaoConnection(NAOip, NAOport):
    worked = False
    try:
        print NAOip, NAOport
        # naoBehavior = connectToProxy(NAOip, NAOport, "ALBehaviorManager")
        # names = naoBehavior.getInstalledBehaviors()
        # print "Names: ", names
    
        naoMotions = BasicMotions(NAOip, NAOport)
        print "Made Basic Motions"
        naoMotions.naoSay("Connected!")
        # naoMotions.naoSit()
        # naoMotions.naoShadeHeadSay("Hello, the connection worked!")

        testExpressions = False
        if testExpressions:
            genUtil = GenUtil(naoMotions)
            genUtil.testExpressions()

        worked = True
    except Exception as e:
        print "Connection Failed, maybe wrong IP and/or Port"
        print e
        
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

def getNAOIP():
    fileName = "ProgramDataFiles\_FSM_INPUT.json"
    jsInput = FileUtilitiy.readFileToJSON(fileName)
    naoIP = str(jsInput['naoIP'])
    return naoIP

def exitingProgram():
    print "Program Exiting..."

if __name__ == '__main__':
    simulated = False
    name = "NAO"
    if simulated:
        #simulated NAO
        NAOIP = "127.0.0.1"
        NAOPort = 51962
    else:
        useLuke = True
        #real NAO
        if useLuke:
            NAOIP = "luke.local"
            NAOIP = "192.168.1.135"
            name = "Luke"
        else:
            NAOIP = "leia.local"
            name = "Leia"
        NAOIP = getNAOIP()
        name = NAOIP[0:4]
        print "Robot Name: ", name
        NAOPort = 9559

    print("Initiated Values")

    connWorks = testNaoConnection(NAOIP, NAOPort)
    print "Connection Worked: ",connWorks
    atexit.register(exitingProgram)
    if connWorks:
        main(NAOIP, NAOPort, name)




