from naoqi import ALProxy
from naoqi import ALModule
from naoqi import ALBroker
import math
import time
import random
import NAOTouchChecker

class BasicMotions:
    def __init__(self, ip = 'luke.local', port = 9559):
        self.NAOip = ip
        self.NAOport = port
        self.logPrint = True

        self.createEyeGroup()
        self.eyeColor={'happy': 0x0000FF00,
                       'sad': 0x00600088,
                       'scared1': 0x00000060,
                       'scared2': 0x00000060,
                       'fear': 0x00000060,
                       'hope': 0x00FFB428,
                       'anger': 0x00FF0000}
        self.eyeShape={'happy': "EyeTop",
                       'sad': "EyeBottom",
                       'scared1': "EyeNone",
                       'scared2': "EyeNone",
                       'fear': "EyeBottom",
                       'hope': "EyeTop",
                       'anger': "EyeTopBottom"}
        self.bScared = False

    def preMotion(self):
        NAOTouchChecker.UnsubscribeAllTouchEvent()

    def postMotion(self):
        NAOTouchChecker.SubscribeAllTouchEvent()

    def StiffnessOn(self, proxy):
        pNames = "Body"
        pStiffnessLists = 1.0
        pTimeLists = 1.0
        proxy.stiffnessInterpolation(pNames, pStiffnessLists, pTimeLists)

    def StiffnessOff(self, proxy):
        pNames = "Body"
        pStiffnessLists = 0.0
        pTimeLists = 1.0
        proxy.stiffnessInterpolation(pNames, pStiffnessLists, pTimeLists)

    def connectToProxy(self, proxyName):
        # proxy = []
        try:
            proxy = ALProxy(proxyName, self.NAOip, self.NAOport)
        except Exception, e:
            print "Could not create Proxy to ", proxyName
            print "Error was: ", e

        return proxy

    def naoSay(self, text):
        speechProxy = self.connectToProxy("ALTextToSpeech")

        moveID = speechProxy.post.say(str(text))
        print("------> Said something: " + text)
        return moveID

    def naoBreathON(self):
        motionProxy = self.connectToProxy("ALMotion")
        motionProxy.setBreathEnabled('Body', True)

    def naoBreathOFF(self):
        motionProxy = self.connectToProxy("ALMotion")
        motionProxy.setBreathEnabled('Body', False)

    def naoSayWait(self, text, waitTime):
        speechProxy = self.connectToProxy("ALTextToSpeech")

        moveID = speechProxy.post.say(str(text))
        print("------> Said something: " + text)
        speechProxy.wait(moveID, 0)
        time.sleep(waitTime)

    def naoSit(self):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
        postureProxy = self.connectToProxy("ALRobotPosture")

        motionProxy.wakeUp()
        self.StiffnessOn(motionProxy)

        if self.NAOip != "127.0.0.1":
            motionProxy.setFallManagerEnabled(False)

        sitResult = postureProxy.goToPosture("Sit", 0.5)
        if (sitResult):
            print("------> Sat Down")
        else:
            print("------> Did NOT Sit Down...")

        if self.NAOip != "127.0.0.1":
            motionProxy.setFallManagerEnabled(True)
        self.StiffnessOff(motionProxy)
        self.postMotion()

    def naoStand(self):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
        postureProxy = self.connectToProxy("ALRobotPosture")

        motionProxy.wakeUp()
        self.StiffnessOn(motionProxy)
        standResult = postureProxy.goToPosture("Stand", 0.5)
        if (standResult):
            print("------> Stood Up")
        else:
            print("------> Did NOT Stand Up...")
        # self.StiffnessOff(motionProxy)
        self.postMotion()

    def naoWalk(self, xpos, ypos):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
        postureProxy = self.connectToProxy("ALRobotPosture")

        motionProxy.wakeUp()
        standResult = postureProxy.goToPosture("StandInit", 0.5)
        motionProxy.setMoveArmsEnabled(True, True)
        motionProxy.setMotionConfig([["ENABLE_FOOT_CONTACT_PROTECTION", True]])

        turnAngle = math.atan2(ypos,xpos)
        walkDist = math.sqrt(xpos*xpos + ypos*ypos)

        try:
            motionProxy.walkTo(0.0,0.0, turnAngle)
            motionProxy.walkTo(walkDist,0.0,0.0)
        except Exception, e:
            print "The Robot could not walk to ", xpos, ", ", ypos
            print "Error was: ", e

        standResult = postureProxy.goToPosture("Stand", 0.5)
        print("------> Walked Somewhere")
        self.postMotion()

    def naoNodHead(self):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]]

        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        for i in range(3):
            motionProxy.angleInterpolation(motionNames, [0.0, 1.0], times, True)
            motionProxy.angleInterpolation(motionNames, [0.0, -1.0], times, True)
        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)

        print("------> Nodded")
        self.postMotion()

    def naoShadeHead(self):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
        motionProxy.wakeUp()
        self.StiffnessOn(motionProxy)

        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]] # time to preform (s)

        # resets the angle of the motions (angle in radians)
        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        # shakes the head 3 times, back and forths
        for i in range(3):
            motionProxy.angleInterpolation(motionNames, [1.0, 0.0], times, True)
            motionProxy.angleInterpolation(motionNames, [-1.0, 0.0], times, True)
        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)

        print("------> Nodded")
        # self.StiffnessOff(motionProxy)
        self.postMotion()

    def naoShadeHeadSay(self, sayText = "Hi"):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")
#        motionProxy.wakeUp()

        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]] # time to preform (s)

        # resets the angle of the motions (angle in radians)
        moveID = motionProxy.post.angleInterpolation(motionNames, [0.0,0.0], times, True)

        sayID = self.naoSay(sayText)
        motionProxy.wait(moveID, 0)

        # shakes the head 3 times, back and forth
        for i in range(2):
            motionProxy.angleInterpolation(motionNames, [1.0, 0.0], times, True)

            motionProxy.angleInterpolation(motionNames, [-1.0, 0.0], times, True)

        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        # motionProxy.wait(sayID, 0)
        if self.logPrint:
            print "Moved head, now waiting"
        time.sleep(0.5)

        print("------> Shook Head")
        # self.StiffnessOff(motionProxy)
        self.postMotion()

    def naoWaveRight(self, movePercent = 1.0, numWaves = 3):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")

        if movePercent > 1:
            movePercent = 1.0
        elif movePercent < 0:
            movePercent = 0.0
        if numWaves > 3:
            numWaves = 3
        elif numWaves < 0:
            numWaves = 0

        rArmNames = motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand"]
        waveTimes = [2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99]
        moveID = motionProxy.post.angleInterpolation(waveNames, waveAngles, waveTimes, True)


        for i in range(numWaves):
            moveID = motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(88.5)*movePercent, [1], True)
            moveID = motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(27)*movePercent, [1], True)

        moveID = motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)

        print("------> Waved Right")
        self.postMotion()

    def naoWaveBoth(self):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")

        rArmNames = motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        lArmNames = motionProxy.getBodyNames("LArm")
        lArmDefaultAngles = [math.radians(84.6), math.radians(10.7),
                             math.radians(-68.2), math.radians(-23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        motionProxy.angleInterpolation(rArmNames + lArmNames, rArmDefaultAngles + lArmDefaultAngles, defaultTimes + defaultTimes, True)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand", "LShoulderPitch", "LShoulderRoll", "LHand"]
        waveTimes = [2, 2, 2, 2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99, math.radians(-11), math.radians(40), 0.99]
        motionProxy.angleInterpolation(waveNames, waveAngles, waveTimes, True)

        for i in range(3):
            waveNames = ["RElbowRoll", "LElbowRoll"]
            waveAngles = [math.radians(88.5), math.radians(-88.5)]
            motionProxy.angleInterpolation(waveNames,  waveAngles, [1,1], True)
            waveAngles = [math.radians(27), math.radians(-27)]
            motionProxy.angleInterpolation(waveNames,  waveAngles, [1,1], True)

        motionProxy.angleInterpolation(rArmNames + lArmNames, rArmDefaultAngles + lArmDefaultAngles, defaultTimes + defaultTimes, True)

        print("------> Waved Both")
        self.postMotion()

    def naoWaveRightSay(self, sayText = "Hi", sayEmotion = -1, movePercent = 1.0, numWaves = 3):
        self.preMotion()
        motionProxy = self.connectToProxy("ALMotion")

        if movePercent > 1:
            movePercent = 1.0
        elif movePercent < 0:
            movePercent = 0.0
        if numWaves > 3:
            numWaves = 3
        elif numWaves < 0:
            numWaves = 0

        rArmNames = motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        motionProxy.wait(moveID, 0)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand"]
        waveTimes = [2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99]
        moveID = motionProxy.post.angleInterpolation(waveNames, waveAngles, waveTimes, True)
        self.naoSay(sayText)

        for i in range(numWaves):
            moveID = motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(88.5)*movePercent, [1], True)
            moveID = motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(27)*movePercent, [1], True)

        moveID = motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        motionProxy.wait(moveID, 0)

        print("------> Waved Right")
        self.postMotion()

    def stopNAOActions(self):
        motionProxy = self.connectToProxy("ALMotion")
        speechProxy = self.connectToProxy("ALTextToSpeech")

        motionProxy.killAll()
        speechProxy.stopAll()
        print("------> Stopped All Actions")








### ================================================================================== davids stuff

    def createEyeGroup(self):
        ledProxy = self.connectToProxy("ALLeds")
        name1 = ["FaceLedRight6","FaceLedRight2",
                 "FaceLedLeft6", "FaceLedLeft2"]
        ledProxy.createGroup("LedEyeCorner",name1)
        name2 = ["FaceLedRight7","FaceLedRight0","FaceLedRight1",
                 "FaceLedLeft7","FaceLedLeft0","FaceLedLeft1"]
        ledProxy.createGroup("LedEyeTop",name2)
        name3 = ["FaceLedRight5","FaceLedRight4","FaceLedRight3",
                 "FaceLedLeft5","FaceLedLeft4","FaceLedLeft3"]
        ledProxy.createGroup("LedEyeBottom",name3)
        ledProxy.createGroup("LedEyeTopBottom",name2+name3)
        ledProxy.createGroup("LedEye",name1+name2+name3)

    def blinkEyes(self, color, duration, configuration):
        bAnimation= True;
        accu=0;
        rgbList=[]
        timeList=[]
        #Reset eye color to black
        self.setEyeColor(0x00000000,"LedEye")
        self.setEyeColor(color,"LedEyeCorner")
        if configuration == "EyeTop":
            self.setEyeColor(color, "LedEyeTop")
        elif configuration == "EyeBottom":
            self.setEyeColor(color, "LedEyeBottom")
        elif configuration == "EyeTopBottom":
            self.setEyeColor(color, "LedEyeTopBottom")
        else:
            bAnimation = False
        if bAnimation == True:
            while(accu<duration):
                newTimeList=[0.05,random.uniform(0.2, 1.0),0.1,0.05]
                accu+=(0.2+newTimeList[1])
                rgbList.extend([color,color,0x00000000,color])
                timeList.extend(newTimeList)
            try:
                ledProxy = self.connectToProxy("ALLeds")
                if configuration == "EyeTop":
                    ledProxy.post.fadeListRGB("LedEyeTop", rgbList, timeList)

                elif configuration == "EyeBottom":
                    ledProxy.post.fadeListRGB("LedEyeBottom", rgbList, timeList)

                else:
                    ledProxy.post.fadeListRGB("LedEyeTopBottom", rgbList, timeList)
            except BaseException, err:
                print err

    def setEyeColor(self, color ,configuration):
        rgbList=[color]
        timeList=[0.01]
        try:
            ledProxy = self.connectToProxy("ALLeds")
            ledProxy.fadeListRGB(configuration, rgbList, timeList)
        except BaseException, err:
            print err

    def setEyeEmotion(self,emotion):
        configuration = self.eyeShape[emotion]
        color = self.eyeColor[emotion]
        self.setEyeColor(0x00000000,"LedEye")
        self.setEyeColor(color,"LedEyeCorner")
        if configuration == "EyeTop":
            self.setEyeColor(color, "LedEyeTop")
        elif configuration == "EyeBottom":
            self.setEyeColor(color, "LedEyeBottom")
        elif configuration == "EyeTopBottom":
            self.setEyeColor(color, "LedEyeTopBottom")

    def updateWithBlink(self, names, keys, times, color, configuration):
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")
        standResult = postureProxy.goToPosture("StandInit", 0.3)
        if (standResult):
            print("------> Stood Up")
            try:
                # uncomment the following line and modify the IP if you use this script outside Choregraphe.
                # print("Time duration is ")
                # print(max(max(times)))
                self.blinkEyes(color,max(max(times))*3, configuration)
                motionProxy = self.connectToProxy("ALMotion")
                motionProxy.angleInterpolation(names, keys, times, True)
                # print 'Tasklist: ', motionProxy.getTaskList();
                time.sleep(max(max(times))+0.5)
            except BaseException, err:
                print err
        else:
            print("------> Did NOT Stand Up...")
        self.postMotion()

    def happyEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.0138481, -0.0138481, -0.50933, 0.00762796])

        names.append("HeadYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.00924587, -0.00924587, -0.0138481, -0.0138481])

        names.append("LAnklePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.345192, -0.357464, -0.354396, -0.354396])

        names.append("LAnkleRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.00157595, 0.00157595, 0.00157595, 0.00157595])

        names.append("LElbowRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.99399, -0.99399, -0.983252, -0.99399])

        names.append("LElbowYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-1.37297, -1.37297, -1.37297, -1.37297])

        names.append("LHand")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.2572, 0.2572, 0.2572, 0.2572])

        names.append("LHipPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.447886, -0.447886, -0.447886, -0.447886])

        names.append("LHipRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([4.19617e-05, 4.19617e-05, 4.19617e-05, 4.19617e-05])

        names.append("LHipYawPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.0061779, -0.00455999, -0.00455999, -0.00455999])

        names.append("LKneePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.70253, 0.70253, 0.70253, 0.70253])

        names.append("LShoulderPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.4097, 1.4097, 1.42044, 1.4097])

        names.append("LShoulderRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.291418, 0.291418, 0.28068, 0.291418])

        names.append("LWristYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.0123138, -0.0123138, -0.0123138, -0.0123138])

        names.append("RAnklePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.34971, -0.34971, -0.34971, -0.34971])

        names.append("RAnkleRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.00609398, 0.00464392, 0.00464392, 0.00464392])

        names.append("RElbowRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.01555, 1.43893, 0.265424, 1.53251])

        names.append("RElbowYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.3913, 1.64287, 1.61679, 1.35755])

        names.append("RHand")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.2544, 0.2544, 0.9912, 0.0108])

        names.append("RHipPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.45564, -0.45564, -0.444902, -0.444902])

        names.append("RHipRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.00924587, -0.00149202, -0.00149202, -0.00149202])

        names.append("RHipYawPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.0061779, -0.00455999, -0.00455999, -0.00455999])

        names.append("RKneePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.70108, 0.70108, 0.70108, 0.70108])

        names.append("RShoulderPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.41132, 0.535408, -1.0216, 0.842208])

        names.append("RShoulderRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.259288, 0.032172, 0.0444441, 0.202446])

        names.append("RWristYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.026036, 1.63213, 1.63213, 1.63213])
        self.updateWithBlink(names, keys, times, self.eyeColor['happy'], self.eyeShape['happy'])
        #self.updateWithBlink(names, keys, times, 0x0000FF00, "EyeTop")

    def sadEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([0.8, 1.36])
        keys.append([-0.0107799, 0.500042])

        names.append("HeadYaw")
        times.append([0.8, 1.36])
        keys.append([-0.00617791, 0.0383082])

        names.append("LAnklePitch")
        times.append([0.8, 1.36])
        keys.append([-0.34826, -0.254685])

        names.append("LAnkleRoll")
        times.append([0.8, 1.36])
        keys.append([-0.00609397, -0.0260359])

        names.append("LElbowRoll")
        times.append([0.8, 1.36])
        keys.append([-0.977116, -0.0383082])

        names.append("LElbowYaw")
        times.append([0.8, 1.36])
        keys.append([-1.37144, -1.37757])

        names.append("LHand")
        times.append([0.8, 1.36])
        keys.append([0.2592, 0.2588])

        names.append("LHipPitch")
        times.append([0.8, 1.36])
        keys.append([-0.446352, -0.639635])

        names.append("LHipRoll")
        times.append([0.8, 1.36])
        keys.append([0.00157595, 4.19617e-05])

        names.append("LHipYawPitch")
        times.append([0.8, 1.36])
        keys.append([-0.00455999, 0.0874801])

        names.append("LKneePitch")
        times.append([0.8, 1.36])
        keys.append([0.70253, 0.699462])

        names.append("LShoulderPitch")
        times.append([0.8, 1.36])
        keys.append([1.44038, 1.01853])

        names.append("LShoulderRoll")
        times.append([0.8, 1.36])
        keys.append([0.27301, -0.153442])

        names.append("LWristYaw")
        times.append([0.8, 1.36])
        keys.append([0.0152981, 1.29772])

        names.append("RAnklePitch")
        times.append([0.8, 1.36])
        keys.append([-0.34971, -0.271475])

        names.append("RAnkleRoll")
        times.append([0.8, 1.36])
        keys.append([0.00310993, -0.00762796])

        names.append("RElbowRoll")
        times.append([0.8, 1.36])
        keys.append([0.978734, 0.038392])

        names.append("RElbowYaw")
        times.append([0.8, 1.36])
        keys.append([1.36982, 0.519984])

        names.append("RHand")
        times.append([0.8, 1.36])
        keys.append([0.2672, 0.2612])

        names.append("RHipPitch")
        times.append([0.8, 1.36])
        keys.append([-0.454105, -0.651992])

        names.append("RHipRoll")
        times.append([0.8, 1.36])
        keys.append([4.19617e-05, -0.00762796])

        names.append("RHipYawPitch")
        times.append([0.8, 1.36])
        keys.append([-0.00455999, 0.0874801])

        names.append("RKneePitch")
        times.append([0.8, 1.36])
        keys.append([0.704148, 0.737896])

        names.append("RShoulderPitch")
        times.append([0.8, 1.36])
        keys.append([1.42973, 1.04623])

        names.append("RShoulderRoll")
        times.append([0.8, 1.36])
        keys.append([-0.25622, 0.113474])

        names.append("RWristYaw")
        times.append([0.8, 1.36])
        keys.append([0.032172, -0.12583])
        self.updateWithBlink(names, keys, times, self.eyeColor['sad'], self.eyeShape['sad'])
        #self.updateWithBlink(names, keys, times, 0x00600088, "EyeBottom")

    def scaredEmotion1(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([0.8, 2.36])
        keys.append([-0.0537319, -0.498592])

        names.append("HeadYaw")
        times.append([0.8, 2.36])
        keys.append([0.0183661, -0.852946])

        names.append("LAnklePitch")
        times.append([0.8, 2.36])
        keys.append([-0.34826, 0.208583])

        names.append("LAnkleRoll")
        times.append([0.8, 2.36])
        keys.append([-0.00609397, -0.0873961])

        names.append("LElbowRoll")
        times.append([0.8, 2.36])
        keys.append([-0.989389, -1.46186])

        names.append("LElbowYaw")
        times.append([0.8, 2.36])
        keys.append([-1.3699, -0.895898])

        names.append("LHand")
        times.append([0.8, 2.36])
        keys.append([0.262, 0.9908])

        names.append("LHipPitch")
        times.append([0.8, 2.36])
        keys.append([-0.440216, -0.15796])

        names.append("LHipRoll")
        times.append([0.8, 2.36])
        keys.append([0.00157595, 0.113558])

        names.append("LHipYawPitch")
        times.append([0.8, 2.36])
        keys.append([0.00464392, -0.527655])

        names.append("LKneePitch")
        times.append([0.8, 2.36])
        keys.append([0.707132, 0.36505])

        names.append("LShoulderPitch")
        times.append([0.8, 2.36])
        keys.append([1.45419, 0.343573])

        names.append("LShoulderRoll")
        times.append([0.8, 2.36])
        keys.append([0.299088, -0.259288])

        names.append("LWristYaw")
        times.append([0.8, 2.36])
        keys.append([0.0137641, 1.26091])

        names.append("RAnklePitch")
        times.append([0.8, 2.36])
        keys.append([-0.352778, -0.542995])

        names.append("RAnkleRoll")
        times.append([0.8, 2.36])
        keys.append([0.00771189, 0.165714])

        names.append("RElbowRoll")
        times.append([0.8, 2.36])
        keys.append([0.981802, 1.33769])

        names.append("RElbowYaw")
        times.append([0.8, 2.36])
        keys.append([1.36829, 1.37135])

        names.append("RHand")
        times.append([0.8, 2.36])
        keys.append([0.2644, 0.9912])

        names.append("RHipPitch")
        times.append([0.8, 2.36])
        keys.append([-0.449504, 0.078192])

        names.append("RHipRoll")
        times.append([0.8, 2.36])
        keys.append([-0.00609397, -0.283749])

        names.append("RHipYawPitch")
        times.append([0.8, 2.36])
        keys.append([0.00464392, -0.527655])

        names.append("RKneePitch")
        times.append([0.8, 2.36])
        keys.append([0.70108, 0.859083])

        names.append("RShoulderPitch")
        times.append([0.8, 2.36])
        keys.append([1.45581, 0.33292])

        names.append("RShoulderRoll")
        times.append([0.8, 2.36])
        keys.append([-0.276162, 0.0628521])

        names.append("RWristYaw")
        times.append([0.8, 2.36])
        keys.append([0.0168321, -1.21037])
        if self.bScared == False:
            self.updateWithBlink(names, keys, times, self.eyeColor['scared1'], self.eyeShape['scared1'])
            self.bScared = True

        #self.updateWithBlink(names, keys, times, 0x00000060,"EyeNone")

    def scaredEmotion2(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.00310993, -0.019984, -0.019984, -0.270025, -0.0153821])

        names.append("HeadYaw")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.00617791, 0.0106959, -0.00157595, -0.495523, -0.605971])

        names.append("LAnklePitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.343659, -0.276162, -0.131966, 0.245399, 0.383458])

        names.append("LAnkleRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.00464392, 0.185656, 0.230143, 0.176453, 0.0951499])

        names.append("LElbowRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.981718, -0.992455, -0.981718, -0.774628, -1.50174])

        names.append("LElbowYaw")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-1.37144, -1.36837, -1.36837, -1.13827, -1.14287])

        names.append("LHand")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.2572, 0.246, 0.246, 0.264, 0.9864])

        names.append("LHipPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.450955, -0.444818, -0.616627, -0.791502, -0.521518])

        names.append("LHipRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.00464392, -0.141086, -0.199378, -0.15796, -0.13495])

        names.append("LHipYawPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.00157595, 0.0153821, -0.0735901, -0.164096, -0.162562])

        names.append("LKneePitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.696393, 0.651908, 0.659577, 0.438682, 0.440216])

        names.append("LShoulderPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([1.41737, 1.4097, 1.42198, 1.03541, 0.622761])

        names.append("LShoulderRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.27301, 0.277612, 0.266875, -0.0828778, -0.176453])

        names.append("LWristYaw")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.0152981, 0.0137641, 0.0137641, 0.061318, 1.80701])

        names.append("RAnklePitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.36505, -0.406468, -0.619695, -0.538392, -0.998592])

        names.append("RAnkleRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.00609397, 0.162646, 0.142704, 0.22554, -0.06592])

        names.append("RElbowRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.977199, 0.975665, 0.963394, 0.928112, 1.48035])

        names.append("RElbowYaw")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([1.38209, 1.36982, 1.36829, 1.48487, 1.23023])

        names.append("RHand")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.2476, 0.2332, 0.2464, 0.2488, 0.98])

        names.append("RHipPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.461776, -0.276162, -0.04913, 0.328234, 0.248467])

        names.append("RHipRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.00617791, -0.124212, -0.15029, -0.196309, -0.032172])

        names.append("RHipYawPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.00157595, 0.0153821, -0.0735901, -0.164096, -0.162562])

        names.append("RKneePitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([0.702614, 0.567621, 0.573758, 0.075208, 1.01862])

        names.append("RShoulderPitch")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([1.41286, 1.40212, 1.42513, 1.01555, 0.423426])

        names.append("RShoulderRoll")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.27923, -0.259288, -0.248551, 0.0689882, 0.231591])

        names.append("RWristYaw")
        times.append([0.8, 1.44, 2.24, 3.08, 3.84])
        keys.append([-0.0138481, -0.0138481, 0.0429101, -0.0107799, -1.56012])

        self.updateWithBlink(names, keys, times, self.eyeColor['scared2'], self.eyeShape['scared2'])



        #self.updateWithBlink(names, keys, times, 0x00000060, "EyeNone")

    def fearEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.0123138, 0.50311, 0.421347, 0.46597, 0.449239, 0.50311])

        names.append("HeadYaw")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0, -0.108956, -0.484786, 0.260738, -0.343659, -0.108956])

        names.append("LAnklePitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.354396, -0.744032, -0.744032, -0.744032, -0.744032, -0.744032])

        names.append("LAnkleRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.00455999, -0.12728, -0.12728, -0.12728, -0.12728, -0.12728])

        names.append("LElbowRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.984786, -1.50021, -1.48947, -1.48947, -1.48947, -1.50021])

        names.append("LElbowYaw")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-1.37144, -1.2932, -1.2932, -1.2932, -1.2932, -1.2932])

        names.append("LHand")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.2592, 0.2592, 0.2592, 0.2592, 0.2592, 0.2592])

        names.append("LHipPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.443284, -1.3192, -1.3192, -1.3192, -1.3192, -1.3192])

        names.append("LHipRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([4.19617e-05, 0.0353239, 0.0353239, 0.0353239, 0.0353239, 0.0353239])

        names.append("LHipYawPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.00762796, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586])

        names.append("LKneePitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.710201, 1.83769, 1.83769, 1.83769, 1.83769, 1.83769])

        names.append("LShoulderPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([1.44345, 0.243864, 0.243864, 0.243864, 0.243864, 0.243864])

        names.append("LShoulderRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.262272, -0.277696, -0.277696, -0.277696, -0.277696, -0.277696])

        names.append("LWristYaw")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.0183661, -0.47865, -0.47865, -0.47865, -0.47865, -0.47865])

        names.append("RAnklePitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.34971, -0.892746, -0.892746, -0.892746, -0.892746, -0.892746])

        names.append("RAnkleRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.00310993, 0.00310993, 0.00310993, 0.00310993, 0.00310993, 0.00310993])

        names.append("RElbowRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.977199, 1.45581, 1.45581, 1.45581, 1.45581, 1.45581])

        names.append("RElbowYaw")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([1.36982, 1.11978, 1.13052, 1.13052, 1.14586, 1.11978])

        names.append("RHand")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.2692, 0.2692, 0.2692, 0.2692, 0.2692, 0.2692])

        names.append("RHipPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.451038, -1.35456, -1.35456, -1.35456, -1.35456, -1.35456])

        names.append("RHipRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.00762796, -0.128814, -0.128814, -0.128814, -0.128814, -0.128814])

        names.append("RHipYawPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.00762796, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586])

        names.append("RKneePitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.704148, 1.95896, 1.95896, 1.95896, 1.95896, 1.95896])

        names.append("RShoulderPitch")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([1.4328, 0.124296, 0.124296, 0.124296, 0.124296, 0.124296])

        names.append("RShoulderRoll")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([-0.25622, 0.208583, 0.208583, 0.208583, 0.197844, 0.208583])

        names.append("RWristYaw")
        times.append([0.8, 2.2, 2.48, 2.72, 2.96, 3.2])
        keys.append([0.0413762, 0.697927, 0.697927, 0.697927, 0.697927, 0.697927])
        self.updateWithBlink(names, keys, times, self.eyeColor['fear'], self.eyeShape['fear'])
        #self.updateWithBlink(names, keys, times, 0x00000060,"EyeTopBottom")


    def hopeEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.00762796, -0.158044, -0.6704, -0.667332, -0.6704, -0.667332, -0.6704])

        names.append("HeadYaw")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.00464392, -0.00464392, -0.019984, -0.01845, -0.019984, -0.01845, -0.019984])

        names.append("LAnklePitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.357464, 0.0827939, 0.0873961, 0.0873961, 0.0873961, 0.0873961, 0.0873961])

        names.append("LAnkleRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.00762796, -0.121144, -0.110406, -0.118076, -0.110406, -0.118076, -0.110406])

        names.append("LElbowRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.974047, -0.776162, -1.49714, -1.21489, -1.49714, -1.21489, -1.49714])

        names.append("LElbowYaw")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-1.37144, -1.17509, -0.823801, -0.86215, -0.823801, -0.862151, -0.823801])

        names.append("LHand")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.2684, 0.2904, 0.9692, 0.9712, 0.9692, 0.9712, 0.9692])

        names.append("LHipPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.449421, 0.127364, 0.122762, 0.116626, 0.122762, 0.116626, 0.122762])

        names.append("LHipRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.00464392, 0.092082, 0.093616, 0.0997519, 0.093616, 0.099752, 0.093616])

        names.append("LHipYawPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.00762796, -0.170232, -0.167164, -0.162562, -0.167164, -0.162562, -0.167164])

        names.append("LKneePitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.700996, -0.090548, -0.0923279, -0.0923279, -0.0923279, -0.0923279, -0.0923279])

        names.append("LShoulderPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([1.44345, 1.01547, 0.671851, 0.857464, 0.671851, 0.857465, 0.671851])

        names.append("LShoulderRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.262272, -0.181053, 0.0674542, -0.0307219, 0.0674542, -0.030722, 0.0674542])

        names.append("LWristYaw")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.0183661, -0.0107799, 0.236194, -0.023052, 0.236194, -0.023052, 0.236194])

        names.append("RAnklePitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.358915, 0.0874801, 0.092082, 0.090548, 0.092082, 0.090548, 0.092082])

        names.append("RAnkleRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.00464392, 0.130432, 0.127364, 0.135034, 0.127364, 0.135034, 0.127364])

        names.append("RElbowRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.975665, 1.08458, 1.43433, 1.24105, 1.43433, 1.24105, 1.43433])

        names.append("RElbowYaw")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([1.36675, 1.26397, 0.897349, 0.9403, 0.897349, 0.9403, 0.897349])

        names.append("RHand")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.2696, 0.2852, 0.952, 0.9548, 0.952, 0.9548, 0.952])

        names.append("RHipPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.455641, 0.130348, 0.121144, 0.121144, 0.121144, 0.121144, 0.121144])

        names.append("RHipRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.00762796, -0.091998, -0.107338, -0.10427, -0.107338, -0.10427, -0.107338])

        names.append("RHipYawPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.00762796, -0.170232, -0.167164, -0.162562, -0.167164, -0.162562, -0.167164])

        names.append("RKneePitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.702614, -0.0843279, -0.0858622, -0.0923279, -0.0858622, -0.0923279, -0.0858622])

        names.append("RShoulderPitch")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([1.43587, 1.26099, 0.673468, 0.921976, 0.673468, 0.921975, 0.673468])

        names.append("RShoulderRoll")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([-0.257754, 0.032172, 0.115008, 0.0352399, 0.115008, 0.0352399, 0.115008])

        names.append("RWristYaw")
        times.append([0.8, 1.6, 2, 2.2, 2.4, 2.6, 2.8])
        keys.append([0.033706, 0.621227, -0.145772, -0.121228, -0.145772, -0.121228, -0.145772])
        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'])
        #self.updateWithBlink(names, keys, times, 0x00FFB428, "EyeTop")

    def angerEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.0153821, -0.073674, -0.046062, -0.046062, -0.046062, -0.046062])

        names.append("HeadYaw")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.00302602, -0.00617791, -0.395814, 0.283749, -0.563021, -0.10282])

        names.append("LAnklePitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.35593, -0.563021, -0.556884, -0.556884, -0.556884, -0.556884])

        names.append("LAnkleRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.0106959, -0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("LElbowRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.977116, -1.46953, -1.44499, -1.44499, -1.44499, -1.44499])

        names.append("LElbowYaw")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-1.37297, -0.227074, -0.260822, -0.260822, -0.260822, -0.260822])

        names.append("LHand")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.2608, 0.25, 0.25, 0.25, 0.25, 0.25])

        names.append("LHipPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.452487, -0.691793, -0.682588, -0.682588, -0.682588, -0.682588])

        names.append("LHipRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.00464392, 0.00464392, 0.00464392, 0.00464392, 0.00464392, 0.00464392])

        names.append("LHipYawPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00916195, -0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("LKneePitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.705598, 1.11364, 1.11211, 1.11211, 1.11211, 1.11211])

        names.append("LShoulderPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.44345, 1.33607, 1.32533, 1.32533, 1.32533, 1.32533])

        names.append("LShoulderRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.268407, 0.74088, 0.734743, 0.734743, 0.734743, 0.734743])

        names.append("LWristYaw")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.0152981, 0.06592, 0.05058, 0.05058, 0.05058, 0.05058])

        names.append("RAnklePitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.361981, -0.592082, -0.582879, -0.593616, -0.593616, -0.593616])

        names.append("RAnkleRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.00157595, 0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189])

        names.append("RElbowRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.971065, 1.42513, 1.39138, 1.39138, 1.39138, 1.39138])

        names.append("RElbowYaw")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.36982, -0.14117, -0.121228, -0.121228, -0.121228, -0.121228])

        names.append("RHand")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.2624, 0.2556, 0.2556, 0.2556, 0.2556, 0.2556])

        names.append("RHipPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.449504, -0.68574, -0.679603, -0.679603, -0.679603, -0.679603])

        names.append("RHipRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00455999, -0.0122299, -0.0122299, -0.0122299, -0.0122299, -0.0122299])

        names.append("RHipYawPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00916195, -0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("RKneePitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.691876, 1.12446, 1.12446, 1.12446, 1.12446, 1.12446])

        names.append("RShoulderPitch")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.4328, 1.06771, 1.05083, 1.05083, 1.05083, 1.05083])

        names.append("RShoulderRoll")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.260822, -0.713353, -0.693411, -0.693411, -0.693411, -0.693411])

        names.append("RWristYaw")
        times.append([0.8, 2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.030638, -4.19617e-05, 0.0106959, 0.0106959, 0.0106959, 0.0106959])
        self.updateWithBlink(names, keys, times, self.eyeColor['anger'], self.eyeShape['anger'])
        #self.updateWithBlink(names, keys, times, 0x00FF0000, "EyeTopBottom")


