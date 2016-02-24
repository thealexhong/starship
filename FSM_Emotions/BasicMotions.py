from naoqi import ALProxy
from naoqi import ALModule
from naoqi import ALBroker
import math
import time
import random
import NAOReactionChecker

class BasicMotions:
    def __init__(self, ip = 'luke.local', port = 9559):
        self.NAOip = ip
        self.NAOport = port
        self.logPrint = True
        self.isBreathing = False
        self.motionProxy = self.connectToProxy("ALMotion")

        self.createEyeGroup()
        self.eyeColor={'happy': 0x0000FF00,
                       'sad': 0x00000060,
                       'scared1': 0x009F2C00,
                       'scared2': 0x009F2C00,
                       'scared3': 0x009F2C00,
                       'fear': 0x00600088,
                       'hope': 0x00FFB428,
                       'anger': 0x00FF0000}
        self.eyeShape={'happy': "EyeTop",
                       'sad': "EyeBottom",
                       'scared1': "EyeTopBottom",
                       'scared2': "EyeTopBottom",
                       'scared3': "EyeTopBottom",
                       'fear': "EyeBottom",
                       'hope': "EyeTop",
                       'anger': "EyeTopBottom"}
        #=========SETUP FOR VOICE================
        tts = self.connectToProxy("ALTextToSpeech")
        tts.setParameter("pitchShift", 0)
        tts.setParameter("doubleVoice", 0)
        tts.setParameter("doubleVoiceLevel", 0)
        try:
            audioProxy = self.connectToProxy("ALAudioDevice")
            audioProxy.setOutputVolume(0.9*100) #use 90%
        except Exception as e:
            print "No Audio device found"
            print e
        #Valid Value:50 to 200
        self.ttsPitch={      'default':   100, # \\vct=value\\
                             'happy':     120,
                             'sad':       50,
                             'scared':    150,
                             'fear':      65,
                             'hope':      100,
                             'anger':     55}
        #Valid Value: 50 to 400"\\
        self.ttsSpeed={      'default':   "\\rspd=100\\",
                             'happy':     "\\rspd=100\\",
                             'sad':       "\\rspd=70\\",
                             'scared':    "\\rspd=130\\",
                             'fear':      "\\rspd=75\\",
                             'hope':      "\\rspd=100\\",
                             'anger':     "\\rspd=110\\"}
        #Valid Value: 0 to 100
        default = 50#+15
        self.ttsVolume={     'default':   "\\vol=0" + str(default) + "\\",
                             'happy':     "\\vol=0" + str(default+10) + "\\",
                             'sad':       "\\vol=0" + str(default-15) + "\\",
                             'scared':    "\\vol=0" + str(default+20) + "\\",
                             'fear':      "\\vol=0" + str(default) + "\\",
                             'hope':      "\\vol=0" + str(default) + "\\",
                             'anger':     "\\vol=0" + str(default+10) + "\\"}
    def getConnectInfo(self):
        return [self.NAOip, self.NAOport]

    def preMotion(self):
        NAOReactionChecker.UnsubscribeAllEvents()

    def postMotion(self):
        NAOReactionChecker.SubscribeAllEvents()

    def StiffnessOn(self, proxy, pNames = "Body"):
        pStiffnessLists = 1.0
        pTimeLists = 1.0
        proxy.stiffnessInterpolation(pNames, pStiffnessLists, pTimeLists)

    def StiffnessOff(self, proxy, pNames = "Body"):
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

    def naoAliveON(self):
        print "Breathing and watching"
        self.naoBreathON()
        self.faceTrackingON()

    def naoAliveOff(self):
        print "No more breathing and watching"
        self.naoBreathOFF()
        self.faceTrackingOFF()

    def faceTrackingON(self):
        # faceProxy = self.connectToProxy("ALFaceDetection")
        self.StiffnessOn(self.motionProxy, "Head")
        # faceProxy.setTrackingEnabled(True)
        tracker = self.connectToProxy("ALTracker")
        targetName = "Face"
        faceWidth = 0.1
        tracker.registerTarget(targetName, faceWidth)
        tracker.track(targetName)

    def faceTrackingOFF(self):
        tracker = self.connectToProxy("ALTracker")
        tracker.stopTracker()
        tracker.unregisterAllTargets()

    def naoBreathON(self):
        if not self.isBreathing:
            self.motionProxy.setBreathEnabled('Body', True)
            print "Started Breathing"
            self.isBreathing = True

    def naoBreathOFF(self):
        if self.isBreathing:
            self.motionProxy.setBreathEnabled('Body', False)
            print "Stopped Breathing"
            self.isBreathing = False

    def getNaoBreathing(self):
        return self.motionProxy.getBreathEnabled('Body')

    def naoSayWait(self, text, waitTime):
        speechProxy = self.connectToProxy("ALTextToSpeech")

        moveID = speechProxy.post.say(str(text))
        print("------> Said something: " + text)
        speechProxy.wait(moveID, 0)
        time.sleep(waitTime)

    def getPostureFamily(self):
        postureProxy = self.connectToProxy("ALRobotPosture")
        return postureProxy.getPostureFamily()

    def naoSit(self):
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")

        self.motionProxy.wakeUp()
        self.StiffnessOn(self.motionProxy)

        if self.NAOip != "127.0.0.1":
            try:
                self.motionProxy.setFallManagerEnabled(False)
            except:
                print "Couldn't turn off Fall manager"

        sitResult = postureProxy.goToPosture("Sit", 0.5)
        if (sitResult):
            print("------> Sat Down")
        else:
            print("------> Did NOT Sit Down...")

        if self.NAOip != "127.0.0.1":
            self.motionProxy.setFallManagerEnabled(True)
        self.StiffnessOff(self.motionProxy)
        self.postMotion()

    def naoStand(self, speed = 0.5):
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")

        self.motionProxy.wakeUp()
        self.StiffnessOn(self.motionProxy)
        standResult = postureProxy.goToPosture("Stand", speed)
        if (standResult):
            print("------> Stood Up")
        else:
            print("------> Did NOT Stand Up...")
        # self.StiffnessOff(motionProxy)
        self.postMotion()

    def naoWalk(self, xpos, ypos):
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")

        self.motionProxy.wakeUp()
        standResult = postureProxy.goToPosture("StandInit", 0.5)
        self.motionProxy.setMoveArmsEnabled(True, True)
        self.motionProxy.setMotionConfig([["ENABLE_FOOT_CONTACT_PROTECTION", True]])

        turnAngle = math.atan2(ypos,xpos)
        walkDist = math.sqrt(xpos*xpos + ypos*ypos)

        try:
            self.motionProxy.walkTo(0.0,0.0, turnAngle)
            self.motionProxy.walkTo(walkDist,0.0,0.0)
        except Exception, e:
            print "The Robot could not walk to ", xpos, ", ", ypos
            print "Error was: ", e

        standResult = postureProxy.goToPosture("Stand", 0.5)
        print("------> Walked Somewhere")
        self.postMotion()

    def naoNodHead(self):
        self.preMotion()
        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]]

        self.motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        for i in range(3):
            self.motionProxy.angleInterpolation(motionNames, [0.0, 1.0], times, True)
            self.motionProxy.angleInterpolation(motionNames, [0.0, -1.0], times, True)
        self.motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)

        print("------> Nodded")
        self.postMotion()

    def naoShakeHead(self):
        self.preMotion()
        self.motionProxy.wakeUp()
        self.StiffnessOn(self.motionProxy)

        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]] # time to preform (s)

        # resets the angle of the motions (angle in radians)
        self.motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        # shakes the head 3 times, back and forths
        for i in range(3):
            self.motionProxy.angleInterpolation(motionNames, [1.0, 0.0], times, True)
            self.motionProxy.angleInterpolation(motionNames, [-1.0, 0.0], times, True)
        self.motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)

        print("------> Nodded")
        # self.StiffnessOff(self.motionProxy)
        self.postMotion()

    def naoShadeHeadSay(self, sayText = "Hi"):
        self.preMotion()
#        self.motionProxy.wakeUp()

        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]] # time to preform (s)

        # resets the angle of the motions (angle in radians)
        moveID = self.motionProxy.post.angleInterpolation(motionNames, [0.0,0.0], times, True)

        sayID = self.naoSay(sayText)
        self.motionProxy.wait(moveID, 0)

        # shakes the head 3 times, back and forth
        for i in range(2):
            self.motionProxy.angleInterpolation(motionNames, [1.0, 0.0], times, True)

            self.motionProxy.angleInterpolation(motionNames, [-1.0, 0.0], times, True)

        self.motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        # self.motionProxy.wait(sayID, 0)
        if self.logPrint:
            print "Moved head, now waiting"
        time.sleep(0.5)

        print("------> Shook Head")
        # self.StiffnessOff(self.motionProxy)
        self.postMotion()

    def naoWaveRightFirst(self, movePercent = 1.0, numWaves = 3):
        self.preMotion()
        self.naoAliveOff()

        if movePercent > 1:
            movePercent = 1.0
        elif movePercent < 0:
            movePercent = 0.0
        if numWaves > 3:
            numWaves = 3
        elif numWaves < 0:
            numWaves = 0

        rArmNames = self.motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        self.motionProxy.wait(moveID, 0)
        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand"]
        waveTimes = [2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99]
        moveID = self.motionProxy.post.angleInterpolation(waveNames, waveAngles, waveTimes, True)
        self.motionProxy.wait(moveID, 0)

    def naoWaveRightSecond(self, movePercent = 1.0, numWaves = 3):
        for i in range(numWaves):

            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(88.5)*movePercent, [1], True)
            # self.motionProxy.wait(moveID, 0)
            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(27)*movePercent, [1], True)
            # self.motionProxy.wait(moveID, 0)

        rArmNames = self.motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        # self.motionProxy.wait(moveID, 0)

        print("------> Waved Right")
        self.postMotion()
        self.naoAliveON()

    def naoWaveRight(self, movePercent = 1.0, numWaves = 3):
        self.preMotion()

        if movePercent > 1:
            movePercent = 1.0
        elif movePercent < 0:
            movePercent = 0.0
        if numWaves > 3:
            numWaves = 3
        elif numWaves < 0:
            numWaves = 0

        rArmNames = self.motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        self.motionProxy.wait(moveID, 0)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand"]
        waveTimes = [2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99]
        moveID = self.motionProxy.post.angleInterpolation(waveNames, waveAngles, waveTimes, True)
        self.motionProxy.wait(moveID, 0)

        for i in range(numWaves):

            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(88.5)*movePercent, [1], True)
            self.motionProxy.wait(moveID, 0)
            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(27)*movePercent, [1], True)
            self.motionProxy.wait(moveID, 0)

        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        self.motionProxy.wait(moveID, 0)

        print("------> Waved Right")
        self.postMotion()

    def naoWaveBoth(self):
        self.preMotion()

        rArmNames = self.motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        lArmNames = self.motionProxy.getBodyNames("LArm")
        lArmDefaultAngles = [math.radians(84.6), math.radians(10.7),
                             math.radians(-68.2), math.radians(-23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        self.motionProxy.angleInterpolation(rArmNames + lArmNames, rArmDefaultAngles + lArmDefaultAngles, defaultTimes + defaultTimes, True)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand", "LShoulderPitch", "LShoulderRoll", "LHand"]
        waveTimes = [2, 2, 2, 2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99, math.radians(-11), math.radians(40), 0.99]
        self.motionProxy.angleInterpolation(waveNames, waveAngles, waveTimes, True)

        for i in range(3):
            waveNames = ["RElbowRoll", "LElbowRoll"]
            waveAngles = [math.radians(88.5), math.radians(-88.5)]
            self.motionProxy.angleInterpolation(waveNames,  waveAngles, [1,1], True)
            waveAngles = [math.radians(27), math.radians(-27)]
            self.motionProxy.angleInterpolation(waveNames,  waveAngles, [1,1], True)

        self.motionProxy.angleInterpolation(rArmNames + lArmNames, rArmDefaultAngles + lArmDefaultAngles, defaultTimes + defaultTimes, True)

        print("------> Waved Both")
        self.postMotion()

    def naoWaveRightSay(self, sayText = "Hi", sayEmotion = -1, movePercent = 1.0, numWaves = 3):
        self.preMotion()

        if movePercent > 1:
            movePercent = 1.0
        elif movePercent < 0:
            movePercent = 0.0
        if numWaves > 3:
            numWaves = 3
        elif numWaves < 0:
            numWaves = 0

        rArmNames = self.motionProxy.getBodyNames("RArm")
        # set arm to the initial position
        rArmDefaultAngles = [math.radians(84.6), math.radians(-10.7),
                             math.radians(68.2), math.radians(23.3),
                             math.radians(5.8), 0.3]
        defaultTimes = [2,2,2,2,2,2]
        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        self.motionProxy.wait(moveID, 0)

        # wave setup
        waveNames = ["RShoulderPitch", "RShoulderRoll", "RHand"]
        waveTimes = [2, 2, 2]
        waveAngles = [math.radians(-11), math.radians(-40), 0.99]
        moveID = self.motionProxy.post.angleInterpolation(waveNames, waveAngles, waveTimes, True)
        self.motionProxy.wait(moveID, 0)
        self.naoSay(sayText)

        for i in range(numWaves):
            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(88.5)*movePercent, [1], True)
            self.motionProxy.wait(moveID, 0)
            moveID = self.motionProxy.post.angleInterpolation(["RElbowRoll"],  math.radians(27)*movePercent, [1], True)
            self.motionProxy.wait(moveID, 0)

        moveID = self.motionProxy.post.angleInterpolation(rArmNames, rArmDefaultAngles, defaultTimes, True)
        self.motionProxy.wait(moveID, 0)

        print("------> Waved Right")
        self.postMotion()

    def stopNAOActions(self):
        speechProxy = self.connectToProxy("ALTextToSpeech")

        self.motionProxy.killAll()
        speechProxy.stopAll()
        print("------> Stopped All Actions")

    def naoTurnOffEyes(self):
        self.initEyes("sad")






### ================================================================================== davids stuff
    def naoSayHappy(self,sayText):
        emotion = 'happy'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        sentence+=sayText
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def naoSaySad(self,sayText):
        emotion = 'sad'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        sentence+=sayText
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def naoSayScared(self,sayText):
        emotion = 'scared'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        sentence+= str(sayText).replace(" ", "! ")
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def naoSayFear(self,sayText):
        emotion = 'fear'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        # newText = self.contourSpeechSlope(sayText, self.ttsPitch[emotion], 20)
        sentence+= str(sayText).replace(" ", ". ")
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def naoSayHope(self,sayText):
        emotion = 'hope'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        sentence+=sayText
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def naoSayAnger(self,sayText):
        emotion = 'anger'
        sentence= "\\vct=" + str(self.ttsPitch[emotion]) + "\\"
        sentence += self.ttsVolume[emotion] +  self.ttsSpeed[emotion]
        sentence+=sayText
        # self.tts.say(sentence)
        self.naoSayWait(sentence, 0.5)

    def contourSpeechSlope(self, sayText, mainPitch, slopeGain):
        print sayText
        sayText = " " + sayText
        spaces = [pos for pos, char in enumerate(sayText) if char == ' ']
        print "spaces: ", spaces
        nSpace = len(spaces)
        pitchStart = mainPitch - 1.0*slopeGain/2
        print mainPitch
        print pitchStart
        print slopeGain
        print nSpace
        newText = ""
        ns = 0
        for c in sayText:
            newText += c
            if c == " ":
                newPitch = self.getNewPitch(pitchStart, ns, slopeGain, nSpace)
                newText += "\\vct=" + str(newPitch) + "\\ "
                ns += 1
        return newText

    def getNewPitch(self, pitchStart, numSpace, slope, nSpace):
        newPitch = int(pitchStart + 1.0*numSpace*slope/nSpace)
        if newPitch < 50:
            newPitch = 50
        if newPitch > 200:
            newPitch = 200
        return newPitch

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
        bAnimation= True
        accu=0
        rgbList=[]
        timeList=[]
        #Reset eye color to white
        self.initEyes(color)
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

    def blinkAlarmingEyes(self,duration, expression):
        bAnimation= True
        accu=0
        rgbList=[]
        timeList=[]
        color = 0x00FF7F00
        #Reset eye color to black
        self.setEyeColor(0x00000000,"LedEye")
        while(accu<duration):
            newTimeList=[0.01,0.75,0.01]
            accu+=(newTimeList[1])
            rgbList.extend([0x002F1F00,color,0x00000000])
            timeList.extend(newTimeList)
        try:
            ledProxy = self.connectToProxy("ALLeds")
            shape = "Led" + self.eyeShape[expression]
            # ledProxy.post.fadeListRGB("LedEyeCorner", rgbList, timeList)
            ledProxy.post.fadeListRGB(shape, rgbList, timeList)
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

    def initEyes(self, emotion = "Happy"):
        ledProxy = self.connectToProxy("ALLeds")
        if ("sad" != emotion and emotion != 0x00600088 and "fear" != emotion and
            "scared1" != emotion and emotion != "scared2" and emotion != 0x00000060 and
            emotion != 0x009F2C00 and emotion != "scared3"):
            ledProxy.fade("FaceLeds", 1, 0.1)
        else:
            ledProxy.fade("FaceLeds", 0, 0.1)

    def setEyeEmotion(self, emotion):
        emotion = emotion.lower()
        configuration = self.eyeShape[emotion]
        color = self.eyeColor[emotion]
        self.initEyes(emotion)
        self.setEyeColor(color,"LedEyeCorner")
        if configuration == "EyeTop":
            self.setEyeColor(color, "LedEyeTop")
        elif configuration == "EyeBottom":
            self.setEyeColor(color, "LedEyeBottom")
        elif configuration == "EyeTopBottom":
            self.setEyeColor(color, "LedEyeTopBottom")

    def shortenFirstFrameBy(self,times,offSetSecond):
        timeFirstFrame=0
        for t in times:
            if t[0]>timeFirstFrame:
                timeFirstFrame=t[0]
        if timeFirstFrame>offSetSecond:
            for tArray in times:
                tArray[:]=[x - offSetSecond for x in  tArray]
            return times
        else:
            return None

    def updateWithBlink(self, names, keys, times, color, configuration, bStand=None, bDisableEye=None,bDisableInit=None):
        times = self.shortenFirstFrameBy(times,0.8)
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")
        standResult = False
        if bDisableInit is None:
            if bStand is None:
                standResult = postureProxy.goToPosture("StandInit", 0.3)
            elif bStand==True:
                standResult = postureProxy.goToPosture("Stand", 0.3)
        else:
            standResult=True
        if (standResult):
            print("------> Stood Up")
            try:
                # uncomment the following line and modify the IP if you use this script outside Choregraphe.
                # print("Time duration is ")
                # print(max(max(times)))
                if bDisableEye is None:
                    self.blinkEyes(color,max(max(times))*3, configuration)
                self.motionProxy.angleInterpolation(names, keys, times, True)
                # print 'Tasklist: ', self.motionProxy.getTaskList();

            except BaseException, err:
                print "***********************Did not show Emotion"
                print err
        else:
            print("------> Did NOT Stand Up...")
        self.postMotion()

    def happy1Emotion(self): # praise the sun --------- Not Used
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1, 1.64, 2.76])
        keys.append([0.00916195, -0.00310993, -0.151908])

        names.append("HeadYaw")
        times.append([1, 1.64, 2.76])
        keys.append([-0.0061779, -0.0061779, -0.0061779])

        names.append("LAnklePitch")
        times.append([1, 1.64, 2.76])
        keys.append([-0.357464, -0.3636, 0.0904641])

        names.append("LAnkleRoll")
        times.append([1, 1.64, 2.76])
        keys.append([-0.00916195, -0.00455999, -0.125746])

        names.append("LElbowRoll")
        times.append([1, 1.64, 2.76])
        keys.append([-0.98632, -0.811444, -0.0352399])

        names.append("LElbowYaw")
        times.append([1, 1.64, 2.76])
        keys.append([-1.37297, -0.251618, -0.889762])

        names.append("LHand")
        times.append([1, 1.64, 2.76])
        keys.append([0.2612, 0.974, 0.9824])

        names.append("LHipPitch")
        times.append([1, 1.64, 2.76])
        keys.append([-0.450954, -0.454022, 0.130348])

        names.append("LHipRoll")
        times.append([1, 1.64, 2.76])
        keys.append([-0.00609398, 4.19617e-05, 0.090548])

        names.append("LHipYawPitch")
        times.append([1, 1.64, 2.76])
        keys.append([4.19617e-05, -0.00609398, -0.164096])

        names.append("LKneePitch")
        times.append([1, 1.64, 2.76])
        keys.append([0.70253, 0.7102, -0.090548])

        names.append("LShoulderPitch")
        times.append([1, 1.64, 2.76])
        keys.append([1.42811, 0.799172, -1.04163])

        names.append("LShoulderRoll")
        times.append([1, 1.64, 2.76])
        keys.append([0.27301, 0.049046, 0.662646])

        names.append("LWristYaw")
        times.append([1, 1.64, 2.76])
        keys.append([0.00916195, -0.809994, 0.289884])

        names.append("RAnklePitch")
        times.append([1, 1.64, 2.76])
        keys.append([-0.358914, -0.363516, 0.0904641])

        names.append("RAnkleRoll")
        times.append([1, 1.64, 2.76])
        keys.append([-0.00149202, 0.00310993, 0.125746])

        names.append("RElbowRoll")
        times.append([1, 1.64, 2.76])
        keys.append([0.971064, 0.813062, 0.0349066])

        names.append("RElbowYaw")
        times.append([1, 1.64, 2.76])
        keys.append([1.37289, 0.260738, 0.869736])

        names.append("RHand")
        times.append([1, 1.64, 2.76])
        keys.append([0.264, 0.9832, 0.9912])

        names.append("RHipPitch")
        times.append([1, 1.64, 2.76])
        keys.append([-0.454106, -0.449504, 0.130348])

        names.append("RHipRoll")
        times.append([1, 1.64, 2.76])
        keys.append([0.0107799, 4.19617e-05, -0.090548])

        names.append("RHipYawPitch")
        times.append([1, 1.64, 2.76])
        keys.append([4.19617e-05, -0.00609398, -0.164096])

        names.append("RKneePitch")
        times.append([1, 1.64, 2.76])
        keys.append([0.70108, 0.699546, -0.090548])

        names.append("RShoulderPitch")
        times.append([1, 1.64, 2.76])
        keys.append([1.42513, 0.797722, -1.09523])

        names.append("RShoulderRoll")
        times.append([1, 1.64, 2.76])
        keys.append([-0.26389, -0.067538, -0.664264])

        names.append("RWristYaw")
        times.append([1, 1.64, 2.76])
        keys.append([0.032172, 0.814512, -0.30991])

        self.updateWithBlink(names, keys, times, self.eyeColor['happy'], self.eyeShape['happy'])

    def happyEmotion(self): # fist
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.0138481, -0.509331, 0.00762796])

        names.append("HeadYaw")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.00924586, -0.0138481, -0.0138481])

        names.append("LAnklePitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.357464, -0.354396, -0.354396])

        names.append("LAnkleRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.00157595, 0.00157595, 0.00157595])

        names.append("LElbowRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.993989, -0.983252, -0.993989])

        names.append("LElbowYaw")
        times.append([1.56, 2.12, 2.72])
        keys.append([-1.37297, -1.37297, -1.37297])

        names.append("LHand")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.2572, 0.2572, 0.2572])

        names.append("LHipPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.447886, -0.447886, -0.447886])

        names.append("LHipRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([4.19617e-05, 4.19617e-05, 4.19617e-05])

        names.append("LHipYawPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.00455999, -0.00455999, -0.00455999])

        names.append("LKneePitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.70253, 0.70253, 0.70253])

        names.append("LShoulderPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([1.4097, 1.42044, 1.4097])

        names.append("LShoulderRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.291418, 0.28068, 0.291418])

        names.append("LWristYaw")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.0123138, -0.0123138, -0.0123138])

        names.append("RAnklePitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.34971, -0.34971, -0.34971])

        names.append("RAnkleRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.00464392, 0.00464392, 0.00464392])

        names.append("RElbowRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([1.43893, 0.265424, 1.53251])

        names.append("RElbowYaw")
        times.append([1.56, 2.12, 2.72])
        keys.append([1.64287, 1.61679, 1.35755])

        names.append("RHand")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.2544, 0.9912, 0.0108])

        names.append("RHipPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.455641, -0.444902, -0.444902])

        names.append("RHipRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.00149202, -0.00149202, -0.00149202])

        names.append("RHipYawPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([-0.00455999, -0.00455999, -0.00455999])

        names.append("RKneePitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.70108, 0.70108, 0.70108])

        names.append("RShoulderPitch")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.535408, -1.0216, 0.842209])

        names.append("RShoulderRoll")
        times.append([1.56, 2.12, 2.72])
        keys.append([0.032172, 0.0444441, 0.202446])

        names.append("RWristYaw")
        times.append([1.56, 2.12, 2.72])
        keys.append([1.63213, 1.63213, 1.63213])

        self.updateWithBlink(names, keys, times, self.eyeColor['happy'], self.eyeShape['happy'])

    def happy3Emotion(self): # little dance
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05, -4.19617e-05])

        names.append("HeadYaw")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791])

        names.append("LAnklePitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.383541, -0.414222, -0.342125, -0.351328, -0.342125, -0.351328, -0.342125, -0.339056])

        names.append("LAnkleRoll")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.141086, 0.101286, -0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("LElbowRoll")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-1.46186, -1.04921, -1.46186, -1.04921, -1.37596, -0.992455, -1.37596, -0.992455, -0.906552, -1.47413, -0.906552, -1.47413, -0.906552, -1.09063])

        names.append("LElbowYaw")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-2.06787, -2.07555, -2.06787, -2.07555, -1.34689, -1.33002, -1.34689, -1.33002, -1.5233, -1.54171, -1.5233, -1.54171, -1.5233, -1.53404])

        names.append("LHand")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676, 0.2676])

        names.append("LHipPitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.452487, -0.463226, -0.450955, -0.450955, -0.450955, -0.450955, -0.450955, -0.450955])

        names.append("LHipRoll")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.14884, -0.0827939, 0.00157595, 0.00157595, 0.00157595, 0.00157595, 0.00157595, 0.00157595])

        names.append("LHipYawPitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.00310993, -0.0137641, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586])

        names.append("LKneePitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.734743, 0.808375, 0.699462, 0.699462, 0.699462, 0.699462, 0.699462, 0.699462])

        names.append("LShoulderPitch")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([1.1658, 1.2425, 1.1658, 1.2425, 1.13358, 1.16733, 1.13358, 1.16733, 0.960242, 1.60912, 0.960242, 1.60912, 0.960242, 1.34528])

        names.append("LShoulderRoll")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.0537319, -0.107422, -0.0537319, -0.107422, -0.0583338, -0.115092, -0.0583338, -0.115092, -0.0353239, 0.021434, -0.0353239, 0.021434, -0.0353239, -0.0153821])

        names.append("LWristYaw")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.223922, 0.263807, 0.223922, 0.263807, 0.176367, 0.262272, 0.176367, 0.262272, -0.0153821, -0.032256, -0.0153821, -0.032256, -0.0153821, -0.0767419])

        names.append("RAnklePitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.41107, -0.366584, -0.351244, -0.346642, -0.351244, -0.346642, -0.351244, -0.348176])

        names.append("RAnkleRoll")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.130348, 0.0767419, 0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189])

        names.append("RElbowRoll")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([1.1214, 0.779314, 1.1214, 0.779314, 1.35456, 0.871354, 1.35456, 0.871354, 1.49262, 0.837606, 1.49262, 0.837606, 1.49262, 1.1306])

        names.append("RElbowYaw")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([1.16273, 1.11211, 1.16273, 1.11211, 1.78554, 1.89445, 1.78554, 1.89445, 1.60299, 1.59072, 1.60299, 1.59072, 1.60299, 1.67815])

        names.append("RHand")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696, 0.2696])

        names.append("RHipPitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([-0.452573, -0.449504, -0.449504, -0.449504, -0.449504, -0.449504, -0.449504, -0.449504])

        names.append("RHipRoll")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.145772, -0.0551819, 0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189])

        names.append("RHipYawPitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.00310993, -0.0137641, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586, 0.00924586])

        names.append("RKneePitch")
        times.append([2.6, 3.56, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.734827, 0.728692, 0.694945, 0.688808, 0.694945, 0.688808, 0.694945, 0.694945])

        names.append("RShoulderPitch")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.960325, 1.07538, 0.960325, 1.07538, 1.15514, 1.04776, 1.15514, 1.04776, 1.84698, 1.05697, 1.84698, 1.05697, 1.84698, 1.2932])

        names.append("RShoulderRoll")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.199378, 0.249999, 0.199378, 0.249999, 0.075124, 0.024502, 0.075124, 0.024502, 0.00609397, 0.059784, 0.00609397, 0.059784, 0.00609397, 0.116542])

        names.append("RWristYaw")
        times.append([1.64, 1.96, 2.28, 2.6, 2.92, 3.24, 3.56, 3.88, 4.2, 4.52, 4.88, 5.24, 5.6, 5.8])
        keys.append([0.263807, 0.251533, 0.263807, 0.251533, -0.030722, -0.06447, -0.030722, -0.06447, 0.0199001, -0.036858, 0.0199001, -0.036858, 0.0199001, -0.0552659])

        self.updateWithBlink(names, keys, times, self.eyeColor['happy'], self.eyeShape['happy'])

    def sadEmotion(self): # arms in front head down
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.36])
        keys.append([0.500042])

        names.append("HeadYaw")
        times.append([1.36])
        keys.append([0.0383082])

        names.append("LAnklePitch")
        times.append([1.36])
        keys.append([-0.254685])

        names.append("LAnkleRoll")
        times.append([1.36])
        keys.append([-0.0260359])

        names.append("LElbowRoll")
        times.append([1.36])
        keys.append([-0.0383082])

        names.append("LElbowYaw")
        times.append([1.36])
        keys.append([-1.37757])

        names.append("LHand")
        times.append([1.36])
        keys.append([0.2588])

        names.append("LHipPitch")
        times.append([1.36])
        keys.append([-0.639635])

        names.append("LHipRoll")
        times.append([1.36])
        keys.append([4.19617e-05])

        names.append("LHipYawPitch")
        times.append([1.36])
        keys.append([0.0874801])

        names.append("LKneePitch")
        times.append([1.36])
        keys.append([0.699462])

        names.append("LShoulderPitch")
        times.append([1.36])
        keys.append([1.01853])

        names.append("LShoulderRoll")
        times.append([1.36])
        keys.append([-0.153442])

        names.append("LWristYaw")
        times.append([1.36])
        keys.append([1.29772])

        names.append("RAnklePitch")
        times.append([1.36])
        keys.append([-0.271475])

        names.append("RAnkleRoll")
        times.append([1.36])
        keys.append([-0.00762796])

        names.append("RElbowRoll")
        times.append([1.36])
        keys.append([0.038392])

        names.append("RElbowYaw")
        times.append([1.36])
        keys.append([0.519984])

        names.append("RHand")
        times.append([1.36])
        keys.append([0.2612])

        names.append("RHipPitch")
        times.append([1.36])
        keys.append([-0.651992])

        names.append("RHipRoll")
        times.append([1.36])
        keys.append([-0.00762796])

        names.append("RHipYawPitch")
        times.append([1.36])
        keys.append([0.0874801])

        names.append("RKneePitch")
        times.append([1.36])
        keys.append([0.737896])

        names.append("RShoulderPitch")
        times.append([1.36])
        keys.append([1.04623])

        names.append("RShoulderRoll")
        times.append([1.36])
        keys.append([0.113474])

        names.append("RWristYaw")
        times.append([1.36])
        keys.append([-0.12583])

        self.updateWithBlink(names, keys, times, self.eyeColor['sad'], self.eyeShape['sad'])

    def sad2Emotion(self): # and to face, crying
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2, 2.72, 3.4, 4.12, 4.8])
        keys.append([0.455555, 0.283749, 0.472429, 0.283749, 0.472429])

        names.append("HeadYaw")
        times.append([2, 2.72, 3.4, 4.12, 4.8])
        keys.append([0.0367741, 0.0260359, 0.0260359, 0.0260359, 0.0260359])

        names.append("LElbowRoll")
        times.append([2])
        keys.append([-0.423342])

        names.append("LElbowYaw")
        times.append([2])
        keys.append([-1.20883])

        names.append("LHand")
        times.append([2])
        keys.append([0.2856])

        names.append("LShoulderPitch")
        times.append([2])
        keys.append([1.46493])

        names.append("LShoulderRoll")
        times.append([2])
        keys.append([0.167164])

        names.append("LWristYaw")
        times.append([2])
        keys.append([0.0873961])

        names.append("RElbowRoll")
        times.append([2])
        keys.append([1.19963])

        names.append("RElbowYaw")
        times.append([2])
        keys.append([-0.181053])

        names.append("RHand")
        times.append([2])
        keys.append([0.5556])

        names.append("RShoulderPitch")
        times.append([2])
        keys.append([-0.4034])

        names.append("RShoulderRoll")
        times.append([2])
        keys.append([0.314159])

        names.append("RWristYaw")
        times.append([2])
        keys.append([-0.460242])

        self.updateWithBlink(names, keys, times, self.eyeColor['sad'], self.eyeShape['sad'],True)

    def scaredEmotion1(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.36])
        keys.append([0.279146])

        names.append("HeadYaw")
        times.append([2.36])
        keys.append([-0.770109])

        names.append("LAnklePitch")
        times.append([2.36])
        keys.append([0.211651])

        names.append("LAnkleRoll")
        times.append([2.36])
        keys.append([-0.093532])

        names.append("LElbowRoll")
        times.append([2.36])
        keys.append([-1.43118])

        names.append("LElbowYaw")
        times.append([2.36])
        keys.append([-0.90817])

        names.append("LHand")
        times.append([2.36])
        keys.append([0.9844])

        names.append("LHipPitch")
        times.append([2.36])
        keys.append([-0.154892])

        names.append("LHipRoll")
        times.append([2.36])
        keys.append([0.108956])

        names.append("LHipYawPitch")
        times.append([2.36])
        keys.append([-0.519984])

        names.append("LKneePitch")
        times.append([2.36])
        keys.append([0.37272])

        names.append("LShoulderPitch")
        times.append([2.36])
        keys.append([0.395731])

        names.append("LShoulderRoll")
        times.append([2.36])
        keys.append([-0.214803])

        names.append("LWristYaw")
        times.append([2.36])
        keys.append([1.2517])

        names.append("RAnklePitch")
        times.append([2.36])
        keys.append([-0.5568])

        names.append("RAnkleRoll")
        times.append([2.36])
        keys.append([0.158044])

        names.append("RElbowRoll")
        times.append([2.36])
        keys.append([1.34536])

        names.append("RElbowYaw")
        times.append([2.36])
        keys.append([1.36062])

        names.append("RHand")
        times.append([2.36])
        keys.append([0.9844])

        names.append("RHipPitch")
        times.append([2.36])
        keys.append([0.0812599])

        names.append("RHipRoll")
        times.append([2.36])
        keys.append([-0.282215])

        names.append("RHipYawPitch")
        times.append([2.36])
        keys.append([-0.519984])

        names.append("RKneePitch")
        times.append([2.36])
        keys.append([0.865217])

        names.append("RShoulderPitch")
        times.append([2.36])
        keys.append([0.417291])

        names.append("RShoulderRoll")
        times.append([2.36])
        keys.append([0.06592])

        names.append("RWristYaw")
        times.append([2.36])
        keys.append([-1.17815])

        self.blinkAlarmingEyes(max(max(times))*3, 'scared1')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared1'], self.eyeShape['scared1'],bDisableEye=True)
        self.setEyeEmotion('scared1')

    def scaredEmotion2(self): #
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.11961, 0.15029, 0.102736, 0.331302])

        names.append("HeadYaw")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.00302602, 0.00302602, -0.489389, -0.716419])

        names.append("LAnklePitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.277696, -0.12583, 0.246933, 0.375789])

        names.append("LAnkleRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.194861, 0.224006, 0.182588, 0.099752])

        names.append("LElbowRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.977116, -0.966378, -0.76389, -1.47873])

        names.append("LElbowYaw")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-1.34689, -1.35763, -1.14747, -1.14747])

        names.append("LHand")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.2596, 0.2596, 0.2596, 0.9844])

        names.append("LHipPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.449421, -0.618161, -0.789967, -0.523053])

        names.append("LHipRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.171766, -0.197844, -0.167164, -0.14262])

        names.append("LHipYawPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.0138481, -0.0720561, -0.162562, -0.162562])

        names.append("LKneePitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.654977, 0.670316, 0.44175, 0.44175])

        names.append("LShoulderPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([1.43271, 1.43271, 1.05382, 0.685656])

        names.append("LShoulderRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.271475, 0.260738, -0.0567998, -0.138102])

        names.append("LWristYaw")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.022968, 0.022968, 0.0444441, 1.7932])

        names.append("RAnklePitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.404934, -0.619695, -0.539926, -1.0124])

        names.append("RAnkleRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.158044, 0.147306, 0.228608, -0.0628521])

        names.append("RElbowRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.989473, 0.977199, 0.935783, 1.45734])

        names.append("RElbowYaw")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([1.36062, 1.36062, 1.46953, 1.23943])

        names.append("RHand")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.2472, 0.2472, 0.2472, 0.9768])

        names.append("RHipPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.282298, -0.0567998, 0.331302, 0.249999])

        names.append("RHipRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.116542, -0.15796, -0.191709, -0.0383082])

        names.append("RHipYawPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.0138481, -0.0720561, -0.162562, -0.162562])

        names.append("RKneePitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([0.564555, 0.575292, 0.0782759, 1.02322])

        names.append("RShoulderPitch")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([1.4374, 1.4374, 1.0631, 0.483252])

        names.append("RShoulderRoll")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.24088, -0.24088, 0.0383082, 0.18864])

        names.append("RWristYaw")
        times.append([1.44, 2.24, 3.08, 3.84])
        keys.append([-0.00924586, 0.0260359, 0.0152981, -1.54785])

        self.blinkAlarmingEyes(max(max(times))*3, 'scared2')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared2'], self.eyeShape['scared2'],bDisableEye=True)
        self.setEyeEmotion('scared2')

    def scaredEmotion3(self):
        self.blinkAlarmingEyes(4*3, 'scared1')
        postureProxy = self.connectToProxy("ALRobotPosture")
        postureProxy.goToPosture("StandInit", 0.5)
        self.setEyeEmotion('scared1')

    def fearEmotion(self): # crouch down
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.48, 2.72, 2.96, 3.2, 3.44])
        keys.append([0.496974, 0.421347, 0.46597, 0.449239, 0.50311])

        names.append("HeadYaw")
        times.append([2.48, 2.72, 2.96, 3.2, 3.44])
        keys.append([-0.0890141, -0.484786, 0.260738, -0.343659, -0.108956])

        names.append("LAnklePitch")
        times.append([2.48])
        keys.append([-0.744032])

        names.append("LAnkleRoll")
        times.append([2.48])
        keys.append([0.067538])

        names.append("LElbowRoll")
        times.append([2.48])
        keys.append([-1.44038])

        names.append("LElbowYaw")
        times.append([2.48])
        keys.append([-1.31161])

        names.append("LHand")
        times.append([2.48])
        keys.append([0.2768])

        names.append("LHipPitch")
        times.append([2.48])
        keys.append([-1.07836])

        names.append("LHipRoll")
        times.append([2.48])
        keys.append([-0.053648])

        names.append("LHipYawPitch")
        times.append([2.48])
        keys.append([-0.11961])

        names.append("LKneePitch")
        times.append([2.48])
        keys.append([1.83002])

        names.append("LShoulderPitch")
        times.append([2.48])
        keys.append([0.36505])

        names.append("LShoulderRoll")
        times.append([2.48])
        keys.append([-0.190258])

        names.append("LWristYaw")
        times.append([2.48])
        keys.append([-0.457174])

        names.append("RAnklePitch")
        times.append([2.48])
        keys.append([-0.747016])

        names.append("RAnkleRoll")
        times.append([2.48])
        keys.append([-0.055182])

        names.append("RElbowRoll")
        times.append([2.48])
        keys.append([1.3607])

        names.append("RElbowYaw")
        times.append([2.48])
        keys.append([1.13512])

        names.append("RHand")
        times.append([2.48])
        keys.append([0.2892])

        names.append("RHipPitch")
        times.append([2.48])
        keys.append([-1.1214])

        names.append("RHipRoll")
        times.append([2.48])
        keys.append([0.0828779])

        names.append("RHipYawPitch")
        times.append([2.48])
        keys.append([-0.11961])

        names.append("RKneePitch")
        times.append([2.48])
        keys.append([1.82857])

        names.append("RShoulderPitch")
        times.append([2.48])
        keys.append([0.26389])

        names.append("RShoulderRoll")
        times.append([2.48])
        keys.append([0.107338])

        names.append("RWristYaw")
        times.append([2.48])
        keys.append([0.676452])

        self.updateWithBlink(names, keys, times, self.eyeColor['fear'], self.eyeShape['fear'],True)

    def fear2Emotion(self): # hand to face
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.28, 3.12, 4.04, 4.88, 5.76, 6.36])
        keys.append([-0.194861, -0.194861, -0.194861, -0.194861, -0.194861, -0.194861])

        names.append("HeadYaw")
        times.append([2.28, 3.12, 4.04, 4.88, 5.76, 6.36])
        keys.append([-0.613642, 0.406468, -0.613642, 0.406468, -0.613642, 0.00762796])

        names.append("LElbowRoll")
        times.append([1.8])
        keys.append([-0.401866])

        names.append("LElbowYaw")
        times.append([1.8])
        keys.append([-1.2073])

        names.append("LHand")
        times.append([1.8])
        keys.append([0.2952])

        names.append("LShoulderPitch")
        times.append([1.8])
        keys.append([1.47413])

        names.append("LShoulderRoll")
        times.append([1.8])
        keys.append([0.177901])

        names.append("LWristYaw")
        times.append([1.8])
        keys.append([0.0873961])

        names.append("RElbowRoll")
        times.append([1.8])
        keys.append([1.07998])

        names.append("RElbowYaw")
        times.append([1.8])
        keys.append([0.391128])

        names.append("RHand")
        times.append([1.8])
        keys.append([0.9916])

        names.append("RShoulderPitch")
        times.append([1.8])
        keys.append([-0.826783])

        names.append("RShoulderRoll")
        times.append([1.8])
        keys.append([0.131882])

        names.append("RWristYaw")
        times.append([1.8])
        keys.append([-1.26559])

        self.updateWithBlink(names, keys, times, self.eyeColor['fear'], self.eyeShape['fear'],True)

    def hopeEmotion(self): # hands to chest and head back
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.24])
        keys.append([-0.589097])

        names.append("HeadYaw")
        times.append([2.24])
        keys.append([0.0137641])

        names.append("LAnklePitch")
        times.append([2.24])
        keys.append([0.182504])

        names.append("LAnkleRoll")
        times.append([2.24])
        keys.append([-0.133416])

        names.append("LElbowRoll")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([-1.27318, -1.24557, -1.27318, -1.24557, -1.27318, -1.24557, -1.27318])

        names.append("LElbowYaw")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([-0.949588, -1.33309, -0.949588, -1.33309, -0.949588, -1.33309, -0.949588])

        names.append("LHand")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([0.9904, 0.9784, 0.9904, 0.9784, 0.9904, 0.9784, 0.9904])

        names.append("LHipPitch")
        times.append([2.24])
        keys.append([-0.236194])

        names.append("LHipRoll")
        times.append([2.24])
        keys.append([0.0614019])

        names.append("LHipYawPitch")
        times.append([2.24])
        keys.append([-0.259204])

        names.append("LKneePitch")
        times.append([2.24])
        keys.append([-0.0859461])

        names.append("LShoulderPitch")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([1.03234, 1.16887, 1.03234, 1.16887, 1.03234, 1.16887, 1.03234])

        names.append("LShoulderRoll")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([-0.0153821, 0.033706, -0.0153821, 0.033706, -0.0153821, 0.033706, -0.0153821])

        names.append("LWristYaw")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([-0.039926, -0.067538, -0.039926, -0.067538, -0.039926, -0.067538, -0.039926])

        names.append("RAnklePitch")
        times.append([2.24])
        keys.append([0.237812])

        names.append("RAnkleRoll")
        times.append([2.24])
        keys.append([0.145772])

        names.append("RElbowRoll")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([1.27786, 1.23645, 1.27786, 1.23645, 1.27786, 1.23645, 1.27786])

        names.append("RElbowYaw")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([0.954107, 1.35908, 0.954107, 1.35908, 0.954107, 1.35908, 0.954107])

        names.append("RHand")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([0.9908, 0.9776, 0.9908, 0.9776, 0.9908, 0.9776, 0.9908])

        names.append("RHipPitch")
        times.append([2.24])
        keys.append([-0.312978])

        names.append("RHipRoll")
        times.append([2.24])
        keys.append([-0.093532])

        names.append("RHipYawPitch")
        times.append([2.24])
        keys.append([-0.259204])

        names.append("RKneePitch")
        times.append([2.24])
        keys.append([-0.091998])

        names.append("RShoulderPitch")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([1.01708, 1.14134, 1.01708, 1.14134, 1.01708, 1.14134, 1.01708])

        names.append("RShoulderRoll")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([0.0199001, 0.0122299, 0.0199001, 0.0122299, 0.0199001, 0.0122299, 0.0199001])

        names.append("RWristYaw")
        times.append([2.24, 2.64, 3.04, 3.44, 3.84, 4.24, 4.64])
        keys.append([0.061318, 0.0858622, 0.061318, 0.0858622, 0.061318, 0.0858622, 0.061318])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'], True)

    def hope2Emotion(self): # head back ------------- Not Used
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.84, 2.32, 2.76, 3.2, 3.6, 4.04])
        keys.append([-0.385075, -0.145772, -0.613642, -0.145772, -0.613642, -0.385075])

        names.append("HeadYaw")
        times.append([1.84, 2.32, 2.76, 3.2, 3.6, 4.04])
        keys.append([-0.00924586, 0.0106959, -0.00771189, 0.0106959, -0.00771189, -0.00924586])

        names.append("LAnklePitch")
        times.append([1.84])
        keys.append([0.170232])

        names.append("LAnkleRoll")
        times.append([1.84])
        keys.append([-0.131882])

        names.append("LElbowRoll")
        times.append([1.84])
        keys.append([-0.0889301])

        names.append("LElbowYaw")
        times.append([1.84])
        keys.append([0.0183661])

        names.append("LHand")
        times.append([1.84])
        keys.append([0.582])

        names.append("LHipPitch")
        times.append([1.84])
        keys.append([-0.154892])

        names.append("LHipRoll")
        times.append([1.84])
        keys.append([0.0982179])

        names.append("LHipYawPitch")
        times.append([1.84])
        keys.append([-0.21932])

        names.append("LKneePitch")
        times.append([1.84])
        keys.append([-0.0859461])

        names.append("LShoulderPitch")
        times.append([1.84])
        keys.append([0.905018])

        names.append("LShoulderRoll")
        times.append([1.84])
        keys.append([-0.21787])

        names.append("LWristYaw")
        times.append([1.84])
        keys.append([-0.115092])

        names.append("RAnklePitch")
        times.append([1.84])
        keys.append([0.2102])

        names.append("RAnkleRoll")
        times.append([1.84])
        keys.append([0.0951499])

        names.append("RElbowRoll")
        times.append([1.84])
        keys.append([0.150374])

        names.append("RElbowYaw")
        times.append([1.84])
        keys.append([0.317496])

        names.append("RHand")
        times.append([1.84])
        keys.append([0.4776])

        names.append("RHipPitch")
        times.append([1.84])
        keys.append([-0.181053])

        names.append("RHipRoll")
        times.append([1.84])
        keys.append([-0.05058])

        names.append("RHipYawPitch")
        times.append([1.84])
        keys.append([-0.21932])

        names.append("RKneePitch")
        times.append([1.84])
        keys.append([-0.0827939])

        names.append("RShoulderPitch")
        times.append([1.84])
        keys.append([0.912772])

        names.append("RShoulderRoll")
        times.append([1.84])
        keys.append([0.185572])

        names.append("RWristYaw")
        times.append([1.84])
        keys.append([-0.113558])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'], True)

    def hope3Emotion(self): # mr burns ----------- Not Used
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        # Choregraphe simplified export in Python.

        names.append("HeadPitch")
        times.append([2.28, 2.68, 3.08])
        keys.append([-0.671952, -0.671952, -0.671952])

        names.append("HeadYaw")
        times.append([1, 2.28, 2.68, 3.08])
        keys.append([-0.0061779, -0.0061779, -0.00617791, -0.00617791])

        names.append("LAnklePitch")
        times.append([1])
        keys.append([0.0919981])

        names.append("LAnkleRoll")
        times.append([1])
        keys.append([-0.128814])

        names.append("LElbowRoll")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([-0.421808, -0.940384, -1.27318, -1.26704, -1.27318, -1.26704, -1.27318])

        names.append("LElbowYaw")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([-1.17048, -1.48794, -0.949588, -1.10299, -0.949588, -1.10299, -0.949588])

        names.append("LHand")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.296, 0.2936, 0.9904, 0.9904, 0.9904, 0.9904, 0.9904])

        names.append("LHipPitch")
        times.append([1])
        keys.append([0.127364])

        names.append("LHipRoll")
        times.append([1])
        keys.append([0.107422])

        names.append("LHipYawPitch")
        times.append([1])
        keys.append([-0.168698])

        names.append("LKneePitch")
        times.append([1])
        keys.append([-0.0890141])

        names.append("LShoulderPitch")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([1.48027, 1.01402, 1.03234, 1.04768, 1.03234, 1.04768, 1.03234])

        names.append("LShoulderRoll")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.159494, 0.0506639, -0.0153821, 0.024502, -0.0153821, 0.024502, -0.0153821])

        names.append("LWristYaw")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.0889301, -0.12728, -0.0399261, -0.019984, -0.039926, -0.019984, -0.039926])

        names.append("RAnklePitch")
        times.append([1])
        keys.append([0.090548])

        names.append("RAnkleRoll")
        times.append([1])
        keys.append([0.127364])

        names.append("RElbowRoll")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.43263, 0.940384, 1.27786, 1.2748, 1.27786, 1.2748, 1.27786])

        names.append("RElbowYaw")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([1.20415, 1.48794, 0.954106, 1.10444, 0.954107, 1.10444, 0.954107])

        names.append("RHand")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.2936, 0.2936, 0.9908, 0.9908, 0.9908, 0.9908, 0.9908])

        names.append("RHipPitch")
        times.append([1])
        keys.append([0.12728])

        names.append("RHipRoll")
        times.append([1])
        keys.append([-0.10427])

        names.append("RHipYawPitch")
        times.append([1])
        keys.append([-0.168698])

        names.append("RKneePitch")
        times.append([1])
        keys.append([-0.0858622])

        names.append("RShoulderPitch")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([1.47575, 1.01402, 1.01708, 1.04623, 1.01708, 1.04623, 1.01708])

        names.append("RShoulderRoll")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([-0.14884, -0.0506639, 0.0199001, -0.0138481, 0.0199001, -0.0138481, 0.0199001])

        names.append("RWristYaw")
        times.append([1, 1.72, 2.28, 2.48, 2.68, 2.88, 3.08])
        keys.append([0.138018, 0.12728, 0.0613179, 0.026036, 0.061318, 0.0260359, 0.061318])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'],True, bDisableInit=True)

    def hope4Emotion(self): # mr burns fingers --------- Not Used
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        # Choregraphe simplified export in Python.

        names.append("HeadPitch")
        times.append([2.04, 2.28, 2.52, 2.76, 3])
        keys.append([-0.671952, -0.671952, -0.671952, -0.671952, -0.671952])

        names.append("HeadYaw")
        times.append([1, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([-0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791, -0.00617791])

        names.append("LAnklePitch")
        times.append([1])
        keys.append([0.091998])

        names.append("LAnkleRoll")
        times.append([1])
        keys.append([-0.128814])

        names.append("LElbowRoll")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([-0.421808, -0.940383, -1.27318, -1.22716, -1.27318, -1.22716, -1.27318])

        names.append("LElbowYaw")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([-1.17048, -1.48794, -0.949588, -0.983336, -0.949588, -0.983336, -0.949588])

        names.append("LHand")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.296, 0.2936, 0.9904, 0.4532, 0.9904, 0.4532, 0.9904])

        names.append("LHipPitch")
        times.append([1])
        keys.append([0.127364])

        names.append("LHipRoll")
        times.append([1])
        keys.append([0.107422])

        names.append("LHipYawPitch")
        times.append([1])
        keys.append([-0.168698])

        names.append("LKneePitch")
        times.append([1])
        keys.append([-0.0890141])

        names.append("LShoulderPitch")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([1.48027, 1.01402, 1.03234, 1.06455, 1.03234, 1.06455, 1.03234])

        names.append("LShoulderRoll")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.159494, 0.0506639, -0.0153821, 0.0367742, -0.0153821, 0.0367741, -0.0153821])

        names.append("LWristYaw")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.0889301, -0.12728, -0.039926, -0.047596, -0.039926, -0.047596, -0.039926])

        names.append("RAnklePitch")
        times.append([1])
        keys.append([0.090548])

        names.append("RAnkleRoll")
        times.append([1])
        keys.append([0.127364])

        names.append("RElbowRoll")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.43263, 0.940383, 1.27786, 1.23951, 1.27786, 1.23951, 1.27786])

        names.append("RElbowYaw")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([1.20415, 1.48794, 0.954107, 0.972514, 0.954107, 0.972515, 0.954107])

        names.append("RHand")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.2936, 0.2936, 0.9908, 0.4452, 0.9908, 0.4452, 0.9908])

        names.append("RHipPitch")
        times.append([1])
        keys.append([0.12728])

        names.append("RHipRoll")
        times.append([1])
        keys.append([-0.10427])

        names.append("RHipYawPitch")
        times.append([1])
        keys.append([-0.168698])

        names.append("RKneePitch")
        times.append([1])
        keys.append([-0.0858622])

        names.append("RShoulderPitch")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([1.47575, 1.01402, 1.01708, 1.0493, 1.01708, 1.0493, 1.01708])

        names.append("RShoulderRoll")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([-0.14884, -0.0506639, 0.0199001, -0.019984, 0.0199001, -0.019984, 0.0199001])

        names.append("RWristYaw")
        times.append([1, 1.72, 2.04, 2.28, 2.52, 2.76, 3])
        keys.append([0.138018, 0.12728, 0.061318, 0.05825, 0.061318, 0.05825, 0.061318])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'],True, bDisableInit=True)

    def angerEmotion(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.073674, -0.046062, -0.046062, -0.046062, -0.046062])

        names.append("HeadYaw")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00617791, -0.395814, 0.283749, -0.563021, -0.10282])

        names.append("LAnklePitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.563021, -0.556884, -0.556884, -0.556884, -0.556884])

        names.append("LAnkleRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("LElbowRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-1.46953, -1.44499, -1.44499, -1.44499, -1.44499])

        names.append("LElbowYaw")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.227074, -0.260822, -0.260822, -0.260822, -0.260822])

        names.append("LHand")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.25, 0.25, 0.25, 0.25, 0.25])

        names.append("LHipPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.691793, -0.682588, -0.682588, -0.682588, -0.682588])

        names.append("LHipRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.00464392, 0.00464392, 0.00464392, 0.00464392, 0.00464392])

        names.append("LHipYawPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("LKneePitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.11364, 1.11211, 1.11211, 1.11211, 1.11211])

        names.append("LShoulderPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.33607, 1.32533, 1.32533, 1.32533, 1.32533])

        names.append("LShoulderRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.74088, 0.734743, 0.734743, 0.734743, 0.734743])

        names.append("LWristYaw")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.06592, 0.05058, 0.05058, 0.05058, 0.05058])

        names.append("RAnklePitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.592082, -0.582879, -0.593616, -0.593616, -0.593616])

        names.append("RAnkleRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.00771189, 0.00771189, 0.00771189, 0.00771189, 0.00771189])

        names.append("RElbowRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.42513, 1.39138, 1.39138, 1.39138, 1.39138])

        names.append("RElbowYaw")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.14117, -0.121228, -0.121228, -0.121228, -0.121228])

        names.append("RHand")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([0.2556, 0.2556, 0.2556, 0.2556, 0.2556])

        names.append("RHipPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.68574, -0.679603, -0.679603, -0.679603, -0.679603])

        names.append("RHipRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.0122299, -0.0122299, -0.0122299, -0.0122299, -0.0122299])

        names.append("RHipYawPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.00455999, -0.00455999, -0.00455999, -0.00455999, -0.00455999])

        names.append("RKneePitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.12446, 1.12446, 1.12446, 1.12446, 1.12446])

        names.append("RShoulderPitch")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([1.06771, 1.05083, 1.05083, 1.05083, 1.05083])

        names.append("RShoulderRoll")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-0.713353, -0.693411, -0.693411, -0.693411, -0.693411])

        names.append("RWristYaw")
        times.append([2.12, 2.36, 2.56, 2.76, 2.96])
        keys.append([-4.19617e-05, 0.0106959, 0.0106959, 0.0106959, 0.0106959])

        self.updateWithBlink(names, keys, times, self.eyeColor['anger'], self.eyeShape['anger'])
        #self.updateWithBlink(names, keys, times, 0x00FF0000, "EyeTopBottom")

    def anger2Emotion(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([2.68])
        keys.append([0.00762796])

        names.append("HeadYaw")
        times.append([2.68])
        keys.append([-0.00771189])

        names.append("LElbowRoll")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([-0.888144, -1.30693, -1.29772, -0.384992, -1.3959, -0.607422, -0.61049, -0.61049, -0.61049])

        names.append("LElbowYaw")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([-1.27633, -1.29167, -0.935783, -0.879025, -1.04163, -1.33155, -1.31468, -1.31468, -1.31468])

        names.append("LHand")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([0.2904, 0.2904, 0.3012, 0.2936, 0.3012, 0.2936, 0.2952, 0.2952, 0.2952])

        names.append("LShoulderPitch")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([0.414139, -0.438765, -0.351328, -0.694945, -0.517, 1.49714, 1.47873, 1.47873, 1.47873])

        names.append("LShoulderRoll")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([0.220854, 0.710201, -0.0567998, 0.90962, -0.0107799, 0.329768, 0.308291, 0.308291, 0.308291])

        names.append("LWristYaw")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([-0.895898, -1.00174, -0.93118, -0.983336, -0.949588, -1.70432, -1.67824, -1.67824, -1.67824])

        names.append("RElbowRoll")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([1.01862, 1.00481, 1.40825, 0.805393, 1.54018, 0.661195, 0.68574, 0.68574, 0.68574])

        names.append("RElbowYaw")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([1.05228, 0.67952, 0.67952, 0.754686, 0.742414, 0.579811, 0.484702, 0.489305, 0.484702])

        names.append("RHand")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([0.2892, 0.2892, 0.3016, 0.288, 0.3016, 0.288, 0.018, 0.0272, 0.018])

        names.append("RShoulderPitch")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([-0.059784, 0.300706, -0.555266, 0.16418, -0.736278, -1.42965, -0.665714, -1.44959, -0.665714])

        names.append("RShoulderRoll")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([-0.50166, -0.280764, -0.179519, -0.894364, -0.343659, -0.247016, -0.266959, -0.30224, -0.266959])

        names.append("RWristYaw")
        times.append([2, 2.36, 2.68, 3, 3.32, 4.04, 4.44, 4.84, 5.2])
        keys.append([1.08143, 0.87127, 0.911154, 1.19494, 1.18267, 1.35448, 1.31613, 1.31613, 1.31613])

        self.updateWithBlink(names, keys, times, self.eyeColor['anger'], self.eyeShape['anger'])

    def LookAtEdgeMotion(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.88])
        keys.append([0.248467])

        names.append("HeadYaw")
        times.append([1.88])
        keys.append([-0.00771189])

        names.append("LAnklePitch")
        times.append([1.88])
        keys.append([0.0873961])

        names.append("LAnkleRoll")
        times.append([1.88])
        keys.append([-0.13495])

        names.append("LElbowRoll")
        times.append([1.88])
        keys.append([-0.404934])

        names.append("LElbowYaw")
        times.append([1.88])
        keys.append([-1.17509])

        names.append("LHand")
        times.append([1.88])
        keys.append([0.29])

        names.append("LHipPitch")
        times.append([1.88])
        keys.append([0.12583])

        names.append("LHipRoll")
        times.append([1.88])
        keys.append([0.105888])

        names.append("LHipYawPitch")
        times.append([1.88])
        keys.append([-0.16563])

        names.append("LKneePitch")
        times.append([1.88])
        keys.append([-0.090548])

        names.append("LShoulderPitch")
        times.append([1.88])
        keys.append([1.48027])

        names.append("LShoulderRoll")
        times.append([1.88])
        keys.append([0.16563])

        names.append("LWristYaw")
        times.append([1.88])
        keys.append([0.0812599])

        names.append("RAnklePitch")
        times.append([1.88])
        keys.append([0.0951499])

        names.append("RAnkleRoll")
        times.append([1.88])
        keys.append([0.135034])

        names.append("RElbowRoll")
        times.append([1.88])
        keys.append([0.431096])

        names.append("RElbowYaw")
        times.append([1.88])
        keys.append([1.17807])

        names.append("RHand")
        times.append([1.88])
        keys.append([0.2868])

        names.append("RHipPitch")
        times.append([1.88])
        keys.append([0.12728])

        names.append("RHipRoll")
        times.append([1.88])
        keys.append([-0.105804])

        names.append("RHipYawPitch")
        times.append([1.88])
        keys.append([-0.16563])

        names.append("RKneePitch")
        times.append([1.88])
        keys.append([-0.0873961])

        names.append("RShoulderPitch")
        times.append([1.88])
        keys.append([1.48035])

        names.append("RShoulderRoll")
        times.append([1.88])
        keys.append([-0.15651])

        names.append("RWristYaw")
        times.append([1.88])
        keys.append([0.078192])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'],True, True)

    def scaredEmotion3Edge(self):
        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1.72, 2.28])
        keys.append([0.113474, 0.260738])

        names.append("HeadYaw")
        times.append([1.72, 2.28])
        keys.append([-0.0123138, 0.10427])

        names.append("LAnklePitch")
        times.append([1.72, 2.28])
        keys.append([0.093532, 0.093532])

        names.append("LAnkleRoll")
        times.append([1.72, 2.28])
        keys.append([-0.13495, -0.13495])

        names.append("LElbowRoll")
        times.append([1.72, 2.28])
        keys.append([-1.38823, -1.49868])

        names.append("LElbowYaw")
        times.append([1.72, 2.28])
        keys.append([-1.37144, -1.11679])

        names.append("LHand")
        times.append([1.72, 2.28])
        keys.append([0.524, 0.9896])

        names.append("LHipPitch")
        times.append([1.72, 2.28])
        keys.append([0.12583, 0.12583])

        names.append("LHipRoll")
        times.append([1.72, 2.28])
        keys.append([0.108956, 0.108956])

        names.append("LHipYawPitch")
        times.append([1.72, 2.28])
        keys.append([-0.176367, -0.16563])

        names.append("LKneePitch")
        times.append([1.72, 2.28])
        keys.append([-0.090548, -0.090548])

        names.append("LShoulderPitch")
        times.append([1.72, 2.28])
        keys.append([1.07836, 0.417205])

        names.append("LShoulderRoll")
        times.append([1.72, 2.28])
        keys.append([0.033706, -0.127364])

        names.append("LWristYaw")
        times.append([1.72, 2.28])
        keys.append([-1.6675, -1.20423])

        names.append("RAnklePitch")
        times.append([1.72, 2.28])
        keys.append([0.0828778, 0.093616])

        names.append("RAnkleRoll")
        times.append([1.72, 2.28])
        keys.append([0.124296, 0.135034])

        names.append("RElbowRoll")
        times.append([1.72, 2.28])
        keys.append([1.40825, 1.5049])

        names.append("RElbowYaw")
        times.append([1.72, 2.28])
        keys.append([1.59532, 1.15046])

        names.append("RHand")
        times.append([1.72, 2.28])
        keys.append([0.546, 0.7084])

        names.append("RHipPitch")
        times.append([1.72, 2.28])
        keys.append([0.122678, 0.122678])

        names.append("RHipRoll")
        times.append([1.72, 2.28])
        keys.append([-0.0981341, -0.108872])

        names.append("RHipYawPitch")
        times.append([1.72, 2.28])
        keys.append([-0.176367, -0.16563])

        names.append("RKneePitch")
        times.append([1.72, 2.28])
        keys.append([-0.0873961, -0.0873961])

        names.append("RShoulderPitch")
        times.append([1.72, 2.28])
        keys.append([1.11219, 0.464844])

        names.append("RShoulderRoll")
        times.append([1.72, 2.28])
        keys.append([0.147222, 0.185572])

        names.append("RWristYaw")
        times.append([1.72, 2.28])
        keys.append([1.27931, 1.27931])

        self.blinkAlarmingEyes(max(max(times))*3, 'scared3')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared3'], self.eyeShape['scared3'],True)
        self.setEyeEmotion('scared3')
