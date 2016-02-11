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
                       'scared3': "EyeNone",
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
            audioProxy.setOutputVolume(0.7*100)
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

    def updateWithBlink(self, names, keys, times, color, configuration, bStand=None, bDisableEye=None):
        self.preMotion()
        postureProxy = self.connectToProxy("ALRobotPosture")
        standResult = False
        if bStand is None:
            standResult = postureProxy.goToPosture("StandInit", 0.3)
        elif bStand==True:
            standResult = postureProxy.goToPosture("Stand", 0.3)
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
                time.sleep(max(max(times))+0.5)
            except BaseException, err:
                print "***********************Did not show Emotion"
                print err
        else:
            print("------> Did NOT Stand Up...")
        self.postMotion()

    def happyEmotion(self):
        names = list()
        times = list()
        keys = list()
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
        keys.append([-0.0537319, 0.279146])

        names.append("HeadYaw")
        times.append([0.8, 2.36])
        keys.append([0.0183661, -0.77011])

        names.append("LAnklePitch")
        times.append([0.8, 2.36])
        keys.append([-0.34826, 0.21165])

        names.append("LAnkleRoll")
        times.append([0.8, 2.36])
        keys.append([-0.00609397, -0.0935321])

        names.append("LElbowRoll")
        times.append([0.8, 2.36])
        keys.append([-0.989389, -1.43118])

        names.append("LElbowYaw")
        times.append([0.8, 2.36])
        keys.append([-1.3699, -0.90817])

        names.append("LHand")
        times.append([0.8, 2.36])
        keys.append([0.262, 0.9844])

        names.append("LHipPitch")
        times.append([0.8, 2.36])
        keys.append([-0.440216, -0.154892])

        names.append("LHipRoll")
        times.append([0.8, 2.36])
        keys.append([0.00157595, 0.108956])

        names.append("LHipYawPitch")
        times.append([0.8, 2.36])
        keys.append([0.00464392, -0.519984])

        names.append("LKneePitch")
        times.append([0.8, 2.36])
        keys.append([0.707132, 0.37272])

        names.append("LShoulderPitch")
        times.append([0.8, 2.36])
        keys.append([1.45419, 0.39573])

        names.append("LShoulderRoll")
        times.append([0.8, 2.36])
        keys.append([0.299088, -0.214802])

        names.append("LWristYaw")
        times.append([0.8, 2.36])
        keys.append([0.0137641, 1.2517])

        names.append("RAnklePitch")
        times.append([0.8, 2.36])
        keys.append([-0.352778, -0.5568])

        names.append("RAnkleRoll")
        times.append([0.8, 2.36])
        keys.append([0.00771189, 0.158044])

        names.append("RElbowRoll")
        times.append([0.8, 2.36])
        keys.append([0.981802, 1.34536])

        names.append("RElbowYaw")
        times.append([0.8, 2.36])
        keys.append([1.36829, 1.36062])

        names.append("RHand")
        times.append([0.8, 2.36])
        keys.append([0.2644, 0.9844])

        names.append("RHipPitch")
        times.append([0.8, 2.36])
        keys.append([-0.449504, 0.08126])

        names.append("RHipRoll")
        times.append([0.8, 2.36])
        keys.append([-0.00609397, -0.282214])

        names.append("RHipYawPitch")
        times.append([0.8, 2.36])
        keys.append([0.00464392, -0.519984])

        names.append("RKneePitch")
        times.append([0.8, 2.36])
        keys.append([0.70108, 0.865218])

        names.append("RShoulderPitch")
        times.append([0.8, 2.36])
        keys.append([1.45581, 0.41729])

        names.append("RShoulderRoll")
        times.append([0.8, 2.36])
        keys.append([-0.276162, 0.0659201])

        names.append("RWristYaw")
        times.append([0.8, 2.36])
        keys.append([0.0168321, -1.17815])

        self.blinkAlarmingEyes(max(max(times))*3, 'scared1')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared1'], self.eyeShape['scared1'],bDisableEye=True)
        self.setEyeEmotion('scared1')

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

        self.blinkAlarmingEyes(max(max(times))*3, 'scared2')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared2'], self.eyeShape['scared2'],bDisableEye=True)
        self.setEyeEmotion('scared2')
        #self.updateWithBlink(names, keys, times, 0x00000060, "EyeNone")

    def scaredEmotion3(self):
        self.blinkAlarmingEyes(4*3, 'scared1')
        postureProxy = self.connectToProxy("ALRobotPosture")
        postureProxy.goToPosture("StandInit", 0.5)
        self.setEyeEmotion('scared1')

    def fearEmotion(self):
        names = list()
        times = list()
        keys = list()
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

    def fear2Emotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        # Choregraphe simplified export in Python.

        names.append("HeadPitch")
        times.append([0.8, 1.6])
        keys.append([-0.15651, 0.477418])

        names.append("HeadYaw")
        times.append([0.8, 1.6])
        keys.append([-0.00924587, -0.196394])

        names.append("LAnklePitch")
        times.append([0.8, 1.6])
        keys.append([0.0889301, 0.0843279])

        names.append("LAnkleRoll")
        times.append([0.8, 1.6])
        keys.append([-0.124212, -0.124212])

        names.append("LElbowRoll")
        times.append([0.8, 1.6])
        keys.append([-0.427944, -0.417206])

        names.append("LElbowYaw")
        times.append([0.8, 1.6])
        keys.append([-1.20577, -1.20577])

        names.append("LHand")
        times.append([0.8, 1.6])
        keys.append([0.2944, 0.2944])

        names.append("LHipPitch")
        times.append([0.8, 1.6])
        keys.append([0.135034, 0.1335])

        names.append("LHipRoll")
        times.append([0.8, 1.6])
        keys.append([0.098218, 0.0997519])

        names.append("LHipYawPitch")
        times.append([0.8, 1.6])
        keys.append([-0.164096, -0.164096])

        names.append("LKneePitch")
        times.append([0.8, 1.6])
        keys.append([-0.0828779, -0.0828779])

        names.append("LShoulderPitch")
        times.append([0.8, 1.6])
        keys.append([1.45879, 1.4726])

        names.append("LShoulderRoll")
        times.append([0.8, 1.6])
        keys.append([0.1733, 0.1733])

        names.append("LWristYaw")
        times.append([0.8, 1.6])
        keys.append([0.0873961, 0.0873961])

        names.append("RAnklePitch")
        times.append([0.8, 1.6])
        keys.append([0.0890141, 0.090548])

        names.append("RAnkleRoll")
        times.append([0.8, 1.6])
        keys.append([0.12583, 0.136568])

        names.append("RElbowRoll")
        times.append([0.8, 1.6])
        keys.append([0.438766, 1.04009])

        names.append("RElbowYaw")
        times.append([0.8, 1.6])
        keys.append([1.20261, -0.128898])

        names.append("RHand")
        times.append([0.8, 1.6])
        keys.append([0.2888, 0.2888])

        names.append("RHipPitch")
        times.append([0.8, 1.6])
        keys.append([0.133416, 0.133416])

        names.append("RHipRoll")
        times.append([0.8, 1.6])
        keys.append([-0.0919981, -0.102736])

        names.append("RHipYawPitch")
        times.append([0.8, 1.6])
        keys.append([-0.164096, -0.164096])

        names.append("RKneePitch")
        times.append([0.8, 1.6])
        keys.append([-0.0889301, -0.0889301])

        names.append("RShoulderPitch")
        times.append([0.8, 1.6])
        keys.append([1.47268, -0.454022])

        names.append("RShoulderRoll")
        times.append([0.8, 1.6])
        keys.append([-0.168782, 0.222388])

        names.append("RWristYaw")
        times.append([0.8, 1.6])
        keys.append([0.0904641, -1.2748])

        self.updateWithBlink(names, keys, times, self.eyeColor['fear'], self.eyeShape['fear'],True)

    def hopeEmotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False

        names.append("HeadPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.158044, -0.158044, -0.6704, -0.667332, -0.6704, -0.667332, -0.6704])

        names.append("HeadYaw")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.00924587, -0.00464392, -0.019984, -0.01845, -0.019984, -0.01845, -0.019984])

        names.append("LAnklePitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.0966001, 0.0827939, 0.0873961, 0.0873961, 0.0873961, 0.0873961, 0.0873961])

        names.append("LAnkleRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.122678, -0.121144, -0.110406, -0.118076, -0.110406, -0.118076, -0.110406])

        names.append("LElbowRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.42641, -0.776162, -1.49714, -1.21489, -1.49714, -1.21489, -1.49714])

        names.append("LElbowYaw")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-1.2073, -1.17509, -0.823801, -0.862151, -0.823801, -0.862151, -0.823801])

        names.append("LHand")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.292, 0.2904, 0.9692, 0.9712, 0.9692, 0.9712, 0.9692])

        names.append("LHipPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.122762, 0.127364, 0.122762, 0.116626, 0.122762, 0.116626, 0.122762])

        names.append("LHipRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.0997519, 0.092082, 0.093616, 0.099752, 0.093616, 0.099752, 0.093616])

        names.append("LHipYawPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.170232, -0.170232, -0.167164, -0.162562, -0.167164, -0.162562, -0.167164])

        names.append("LKneePitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.0859461, -0.090548, -0.0923279, -0.0923279, -0.0923279, -0.0923279, -0.0923279])

        names.append("LShoulderPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([1.46646, 1.01547, 0.671851, 0.857465, 0.671851, 0.857465, 0.671851])

        names.append("LShoulderRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.167164, -0.181053, 0.0674542, -0.030722, 0.0674542, -0.030722, 0.0674542])

        names.append("LWristYaw")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.0858622, -0.0107799, 0.236194, -0.023052, 0.236194, -0.023052, 0.236194])

        names.append("RAnklePitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.09515, 0.0874801, 0.092082, 0.090548, 0.092082, 0.090548, 0.092082])

        names.append("RAnkleRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.122762, 0.130432, 0.127364, 0.135034, 0.127364, 0.135034, 0.127364])

        names.append("RElbowRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.4403, 1.08458, 1.43433, 1.24105, 1.43433, 1.24105, 1.43433])

        names.append("RElbowYaw")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([1.21182, 1.26397, 0.897349, 0.9403, 0.897349, 0.9403, 0.897349])

        names.append("RHand")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.282, 0.2852, 0.952, 0.9548, 0.952, 0.9548, 0.952])

        names.append("RHipPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.138018, 0.130348, 0.121144, 0.121144, 0.121144, 0.121144, 0.121144])

        names.append("RHipRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.099668, -0.091998, -0.107338, -0.10427, -0.107338, -0.10427, -0.107338])

        names.append("RHipYawPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.170232, -0.170232, -0.167164, -0.162562, -0.167164, -0.162562, -0.167164])

        names.append("RKneePitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.0904641, -0.0843279, -0.0858622, -0.0923279, -0.0858622, -0.0923279, -0.0858622])

        names.append("RShoulderPitch")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([1.46808, 1.26099, 0.673468, 0.921975, 0.673468, 0.921975, 0.673468])

        names.append("RShoulderRoll")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([-0.168782, 0.032172, 0.115008, 0.0352399, 0.115008, 0.0352399, 0.115008])

        names.append("RWristYaw")
        times.append([0.8, 1.8, 2.44, 3, 3.52, 4.08, 4.6])
        keys.append([0.0889301, 0.621227, -0.145772, -0.121228, -0.145772, -0.121228, -0.145772])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'], True)
        #self.updateWithBlink(names, keys, times, 0x00FFB428, "EyeTop")

    def hope2Emotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        # Choregraphe simplified export in Python.

        names.append("HeadPitch")
        times.append([1, 1.44, 1.84, 2.24, 2.64])
        keys.append([-0.170316, -0.6704, -0.391212, -0.6704, -0.170316])

        names.append("HeadYaw")
        times.append([1, 1.44, 1.84, 2.24, 2.64])
        keys.append([-0.00310993, -0.00771189, -0.019984, -0.00771189, -0.00310993])

        names.append("LAnklePitch")
        times.append([1, 2.64])
        keys.append([0.0858622, 0.0858622])

        names.append("LAnkleRoll")
        times.append([1, 2.64])
        keys.append([-0.124212, -0.124212])

        names.append("LElbowRoll")
        times.append([1, 2.64])
        keys.append([-0.421808, -0.421808])

        names.append("LElbowYaw")
        times.append([1, 2.64])
        keys.append([-1.20577, -1.20577])

        names.append("LHand")
        times.append([1, 2.64])
        keys.append([0.294, 0.294])

        names.append("LHipPitch")
        times.append([1, 2.64])
        keys.append([0.138102, 0.138102])

        names.append("LHipRoll")
        times.append([1, 2.64])
        keys.append([0.09515, 0.0951499])

        names.append("LHipYawPitch")
        times.append([1, 2.64])
        keys.append([-0.170232, -0.170232])

        names.append("LKneePitch")
        times.append([1, 2.64])
        keys.append([-0.0828779, -0.0828778])

        names.append("LShoulderPitch")
        times.append([1, 2.64])
        keys.append([1.46186, 1.46186])

        names.append("LShoulderRoll")
        times.append([1, 2.64])
        keys.append([0.164096, 0.164096])

        names.append("LWristYaw")
        times.append([1, 2.64])
        keys.append([0.0873961, 0.0873961])

        names.append("RAnklePitch")
        times.append([1, 2.64])
        keys.append([0.096684, 0.0966839])

        names.append("RAnkleRoll")
        times.append([1, 2.64])
        keys.append([0.122762, 0.122762])

        names.append("RElbowRoll")
        times.append([1, 2.64])
        keys.append([0.444902, 0.444902])

        names.append("RElbowYaw")
        times.append([1, 2.64])
        keys.append([1.21028, 1.21028])

        names.append("RHand")
        times.append([1, 2.64])
        keys.append([0.2888, 0.2888])

        names.append("RHipPitch")
        times.append([1, 2.64])
        keys.append([0.133416, 0.133416])

        names.append("RHipRoll")
        times.append([1, 2.64])
        keys.append([-0.0966001, -0.0966001])

        names.append("RHipYawPitch")
        times.append([1, 2.64])
        keys.append([-0.170232, -0.170232])

        names.append("RKneePitch")
        times.append([1, 2.64])
        keys.append([-0.0923279, -0.0923279])

        names.append("RShoulderPitch")
        times.append([1, 2.64])
        keys.append([1.46961, 1.46961])

        names.append("RShoulderRoll")
        times.append([1, 2.64])
        keys.append([-0.16418, -0.16418])

        names.append("RWristYaw")
        times.append([1, 2.64])
        keys.append([0.0843279, 0.0843279])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'],True)

    def angerEmotion(self):
        names = list()
        times = list()
        keys = list()

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

    def LookAtEdgeMotion(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False
        names.append("HeadPitch")
        times.append([1, 1.88])
        keys.append([-0.145772, 0.248466])

        names.append("HeadYaw")
        times.append([1, 1.88])
        keys.append([0.0106959, -0.00771189])

        names.append("LAnklePitch")
        times.append([1, 1.88])
        keys.append([0.0873961, 0.0873961])

        names.append("LAnkleRoll")
        times.append([1, 1.88])
        keys.append([-0.13495, -0.13495])

        names.append("LElbowRoll")
        times.append([1, 1.88])
        keys.append([-0.404934, -0.404934])

        names.append("LElbowYaw")
        times.append([1, 1.88])
        keys.append([-1.17509, -1.17509])

        names.append("LHand")
        times.append([1, 1.88])
        keys.append([0.29, 0.29])

        names.append("LHipPitch")
        times.append([1, 1.88])
        keys.append([0.12583, 0.12583])

        names.append("LHipRoll")
        times.append([1, 1.88])
        keys.append([0.105888, 0.105888])

        names.append("LHipYawPitch")
        times.append([1, 1.88])
        keys.append([-0.16563, -0.16563])

        names.append("LKneePitch")
        times.append([1, 1.88])
        keys.append([-0.090548, -0.090548])

        names.append("LShoulderPitch")
        times.append([1, 1.88])
        keys.append([1.48027, 1.48027])

        names.append("LShoulderRoll")
        times.append([1, 1.88])
        keys.append([0.16563, 0.16563])

        names.append("LWristYaw")
        times.append([1, 1.88])
        keys.append([0.08126, 0.08126])

        names.append("RAnklePitch")
        times.append([1, 1.88])
        keys.append([0.09515, 0.09515])

        names.append("RAnkleRoll")
        times.append([1, 1.88])
        keys.append([0.135034, 0.135034])

        names.append("RElbowRoll")
        times.append([1, 1.88])
        keys.append([0.431096, 0.431096])

        names.append("RElbowYaw")
        times.append([1, 1.88])
        keys.append([1.17807, 1.17807])

        names.append("RHand")
        times.append([1, 1.88])
        keys.append([0.2868, 0.2868])

        names.append("RHipPitch")
        times.append([1, 1.88])
        keys.append([0.12728, 0.12728])

        names.append("RHipRoll")
        times.append([1, 1.88])
        keys.append([-0.105804, -0.105804])

        names.append("RHipYawPitch")
        times.append([1, 1.88])
        keys.append([-0.16563, -0.16563])

        names.append("RKneePitch")
        times.append([1, 1.88])
        keys.append([-0.0873961, -0.0873961])

        names.append("RShoulderPitch")
        times.append([1, 1.88])
        keys.append([1.48035, 1.48035])

        names.append("RShoulderRoll")
        times.append([1, 1.88])
        keys.append([-0.15651, -0.15651])

        names.append("RWristYaw")
        times.append([1, 1.88])
        keys.append([0.078192, 0.078192])

        self.updateWithBlink(names, keys, times, self.eyeColor['hope'], self.eyeShape['hope'],True, True)

    def scaredEmotion3Edge(self):
        names = list()
        times = list()
        keys = list()
        self.bScared = False

        names = list()
        times = list()
        keys = list()

        names.append("HeadPitch")
        times.append([1, 1.72, 2.28])
        keys.append([-0.165714, 0.113474, 0.260738])

        names.append("HeadYaw")
        times.append([1, 1.72, 2.28])
        keys.append([0.00302601, -0.0123138, 0.10427])

        names.append("LAnklePitch")
        times.append([1, 1.72, 2.28])
        keys.append([0.0873961, 0.0935321, 0.0935321])

        names.append("LAnkleRoll")
        times.append([1, 1.72, 2.28])
        keys.append([-0.13495, -0.13495, -0.13495])

        names.append("LElbowRoll")
        times.append([1, 1.72, 2.28])
        keys.append([-0.404934, -1.38823, -1.49868])

        names.append("LElbowYaw")
        times.append([1, 1.72, 2.28])
        keys.append([-1.17509, -1.37144, -1.11679])

        names.append("LHand")
        times.append([1, 1.72, 2.28])
        keys.append([0.29, 0.524, 0.9896])

        names.append("LHipPitch")
        times.append([1, 1.72, 2.28])
        keys.append([0.12583, 0.12583, 0.12583])

        names.append("LHipRoll")
        times.append([1, 1.72, 2.28])
        keys.append([0.105888, 0.108956, 0.108956])

        names.append("LHipYawPitch")
        times.append([1, 1.72, 2.28])
        keys.append([-0.16563, -0.176368, -0.16563])

        names.append("LKneePitch")
        times.append([1, 1.72, 2.28])
        keys.append([-0.090548, -0.090548, -0.090548])

        names.append("LShoulderPitch")
        times.append([1, 1.72, 2.28])
        keys.append([1.48334, 1.07836, 0.417206])

        names.append("LShoulderRoll")
        times.append([1, 1.72, 2.28])
        keys.append([0.1733, 0.0337059, -0.127364])

        names.append("LWristYaw")
        times.append([1, 1.72, 2.28])
        keys.append([0.08126, -1.6675, -1.20423])

        names.append("RAnklePitch")
        times.append([1, 1.72, 2.28])
        keys.append([0.090548, 0.0828779, 0.093616])

        names.append("RAnkleRoll")
        times.append([1, 1.72, 2.28])
        keys.append([0.122762, 0.124296, 0.135034])

        names.append("RElbowRoll")
        times.append([1, 1.72, 2.28])
        keys.append([0.431096, 1.40825, 1.5049])

        names.append("RElbowYaw")
        times.append([1, 1.72, 2.28])
        keys.append([1.17807, 1.59532, 1.15046])

        names.append("RHand")
        times.append([1, 1.72, 2.28])
        keys.append([0.2868, 0.546, 0.7084])

        names.append("RHipPitch")
        times.append([1, 1.72, 2.28])
        keys.append([0.12728, 0.122678, 0.122678])

        names.append("RHipRoll")
        times.append([1, 1.72, 2.28])
        keys.append([-0.0935321, -0.098134, -0.108872])

        names.append("RHipYawPitch")
        times.append([1, 1.72, 2.28])
        keys.append([-0.16563, -0.176368, -0.16563])

        names.append("RKneePitch")
        times.append([1, 1.72, 2.28])
        keys.append([-0.0873961, -0.0873961, -0.0873961])

        names.append("RShoulderPitch")
        times.append([1, 1.72, 2.28])
        keys.append([1.48035, 1.11219, 0.464844])

        names.append("RShoulderRoll")
        times.append([1, 1.72, 2.28])
        keys.append([-0.168782, 0.147222, 0.185572])

        names.append("RWristYaw")
        times.append([1, 1.72, 2.28])
        keys.append([0.078192, 1.27931, 1.27931])

        self.blinkAlarmingEyes(max(max(times))*3, 'scared3')
        self.updateWithBlink(names, keys, times, self.eyeColor['scared3'], self.eyeShape['scared3'],True)
        self.setEyeEmotion('scared3')
