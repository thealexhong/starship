from naoqi import ALBroker
from naoqi import ALProxy

import FileUtilitiy
from BasicMotions import BasicMotions
from DietFitnessFSM import DietFitnessFSM
from GenUtil import GenUtil
import NAOReactionChecker
from ThreadedCheckers import ThreadedChecker
from UserAffectGenerator import UserAffectGenerator


def main(NAOip, NAOport, name):
    naoMotions = BasicMotions(NAOip, NAOport)
    robotName = name
    genUtil = GenUtil(naoMotions)
    genUtil.showFoodDB()

    myBroker = ALBroker("myBroker", "0.0.0.0", 0, NAOip, NAOport)
    global naoReactionChecker
    naoReactionChecker = NAOReactionChecker.NAOReactionChecker(genUtil, NAOip, NAOport)

    thread1 = ThreadedChecker(1, "Main Checker #1", genUtil)
    thread2 = UserAffectGenerator(2, "User Affect Generator #1", 3, genUtil)

    # select your activity to run
    # activityConsultant = "Consultant By Appointment"
    # Daily Companion
    activityDayCompMorning = "Daily Companion Morning"
    activityDayCompDayEnd = "Daily Companion End of Day"

    userName = "Test User"
    userNumber = "1"
    dateTime = genUtil.getDateTime()
    activityInteractionType = activityDayCompDayEnd
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
    # thread2.quit()
    genUtil.showHappyEyes()
    naoMotions.naoAliveOff()
    # naoMotions.naoSit()
    NAOReactionChecker.UnsubscribeAllEvents()
    myBroker.shutdown()

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
    FileUtilitiy.makeFolder("ProgramDataFiles\\" + str(userNumber) + "_" + userName)

    return writeLine

def writeUserHistories(userName, userNumber, userInfo,
                       stateTimeStamp, stateDateTime, fsmStateHist, reHist, oeHist, driveStatHist, fsmStateNameHist):
    fileName = "ProgramDataFiles\\" + str(userNumber) + "_" + userName + "\\" + str(userNumber)  + "_" + userName +"_Flow.csv"
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
        naoBehavior = connectToProxy(NAOip, NAOport, "ALBehaviorManager")
        names = naoBehavior.getInstalledBehaviors()
        print "Names: ", names
    
        naoMotions = BasicMotions(NAOip, NAOport)
        print "Made Basic Motions"
        naoMotions.naoSay("Connected!")
        # naoMotions.naoSit()
        # naoMotions.naoShadeHeadSay("Hello, the connection worked!")
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

if __name__ == '__main__':
    simulated = False
    name = "NAO"
    if simulated:
        #simulated NAO
        NAOIP = "127.0.0.1"
        NAOPort = 52030
    else:
        useLuke = False
        #real NAO
        if useLuke:
            NAOIP = "luke.local"
            NAOIP = "192.168.1.135"
            name = "Luke"
        else:
            NAOIP = "leia.local"
            name = "Leia"
        NAOPort = 9559

    print("Initiated Values")

    connWorks = testNaoConnection(NAOIP, NAOPort)
    print connWorks
    if connWorks:
        main(NAOIP, NAOPort, name)




