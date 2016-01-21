from naoqi import ALProxy
import math
import time

class BasicMotions:
    def __init__(self, ip = 'luke.local', port = 9559):
        self.NAOip = ip
        self.NAOport = port

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

        speechProxy.say(str(text))
        print("------> Said something: " + text)

    def naoSit(self):
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

    def naoStand(self):
        motionProxy = self.connectToProxy("ALMotion")
        postureProxy = self.connectToProxy("ALRobotPosture")

        motionProxy.wakeUp()
        self.StiffnessOn(motionProxy)
        standResult = postureProxy.goToPosture("Stand", 0.5)
        if (standResult):
            print("------> Stood Up")
        else:
            print("------> Did NOT Stand Up...")

    def naoWalk(self, xpos, ypos):
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

    def naoNodHead(self):
        motionProxy = self.connectToProxy("ALMotion")
        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]]

        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)
        for i in range(3):
            motionProxy.angleInterpolation(motionNames, [0.0, 1.0], times, True)
            motionProxy.angleInterpolation(motionNames, [0.0, -1.0], times, True)
        motionProxy.angleInterpolation(motionNames, [0.0,0.0], times, True)

        print("------> Nodded")

    def naoShadeHead(self):
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
        self.StiffnessOff(motionProxy)

    def naoShadeHeadSay(self, sayText = "Hi"):
        motionProxy = self.connectToProxy("ALMotion")
#        motionProxy.wakeUp()

        motionNames = ['HeadYaw', "HeadPitch"]
        times = [[0.7], [0.7]] # time to preform (s)

        # resets the angle of the motions (angle in radians)
        moveID = motionProxy.post.angleInterpolation(motionNames, [0.0,0.0], times, True)
        motionProxy.wait(moveID, 0)
        self.naoSay(sayText)

        # shakes the head 3 times, back and forths
        for i in range(3):
            moveID = motionProxy.post.angleInterpolation(motionNames, [1.0, 0.0], times, True)
            # motionProxy.wait(moveID, 0)
            moveID = motionProxy.post.angleInterpolation(motionNames, [-1.0, 0.0], times, True)
            motionProxy.wait(moveID, 0)
            print "move head"
        moveID = motionProxy.post.angleInterpolation(motionNames, [0.0,0.0], times, True)
        motionProxy.wait(moveID, 0)

        print("------> Nodded")
        self.StiffnessOff(motionProxy)

    def naoWaveRight(self, movePercent = 1.0, numWaves = 3):
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

    def naoWaveBoth(self):
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

    def naoWaveRightSay(self, sayText = "Hi", sayEmotion = -1, movePercent = 1.0, numWaves = 3):
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

    def stopNAOActions(self):
        motionProxy = self.connectToProxy("ALMotion")
        speechProxy = self.connectToProxy("ALTextToSpeech")

        motionProxy.killAll()
        speechProxy.stopAll()
        print("------> Stopped All Actions")
