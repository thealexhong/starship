# utility functions
import datetime
import random
import time

from FoodDBManager import FoodDBManager
import FileUtilitiy
from EdgeDetection import EdgeDetection

class GenUtil:
    def __init__(self, naoMotions):
        # no inits
        self.emotionExpressionDict = ["Happy", "Hopeful", "Sad", "Fearful", "Angry",
                                      "Happy2", "Hopeful2", "Sad2", "Fearful2", "Angry2",
                                      "Happy3", "Hopeful3", "Hopeful4",
                                      "Scared1", "Scared2", "Scared3"] #(pickup, touch, high)
                                    
        self.numOE = len(self.emotionExpressionDict)
        self.naoMotions = naoMotions
        self.naoIsSafe = True
        self.naoIsTouched = False
        self.naoIsPickedUp = False
        self.naoIsHighEdge = False

        self.naoSeesHigh = False
        self.naoSeesHighTest = False
        self.waitingForInput = False

        self.wasJustScared = False
        self.scaredAllowed = True
        self.fDB = FoodDBManager()

        NAOip, NAOport = naoMotions.getConnectInfo()
        try:
            self.edgeDetector = EdgeDetection(NAOip, NAOport)
        except Exception as e:
            print "Edge Detector Failed to Create, possibly no camera on unit"
            print e

    def toNum(self, numAsString):
        # Convert string to either int or float
        try:
            ret = int(float(numAsString))
        except ValueError:
            print "***** This string is not a number: ", numAsString
            ret = numAsString
        return ret

    def expressEmotion(self, obserExpresNum = -1):
        print "********************* Yay I can Express myself"
        print "OE Num: ", obserExpresNum
        # expresses NAOs emotion through what ever was picked as the observable expression
        if obserExpresNum == -1:
            print "Neutral ", "Face"
            # self.naoEmotionalVoiceSay("I am " + "Neutral", obserExpresNum)
        else:
            oe = self.emotionExpressionDict[obserExpresNum]
            print oe, "Face"
            # self.naoEmotionalVoiceSay("I am expressing " + oe, obserExpresNum)

            if "Happy" in oe:
                self.showHappyEyes()
            elif "Sad" in oe:
                self.showSadEyes()
            elif "Fearful" in oe:
                self.showFearEyes()
            elif "Angry" in oe:
                self.showAngryEyes()
            elif "Hopeful" in oe:
                self.showHopeEyes()
            elif "Scared" in oe:
                self.showScaredEyes(oe)

            sitTest = False
            if not sitTest:
                if not self.wasJustScared:
                    self.naoMotions.naoStand()
                    self.naoMotions.naoAliveOff()

                if oe == "Happy":
                    self.showHappy1Body()
                elif oe == "Happy2":
                    self.showHappy2Body()
                elif oe == "Happy3":# not used anymore
                    self.showHappy3Body()
                elif oe == "Sad":
                    self.showSadBody()
                elif oe == "Sad2":
                    self.showSad2Body()
                elif oe == "Fearful":
                    self.showFearBody()
                elif oe == "Fearful2":
                    self.showFearBody2()
                elif oe == "Angry":
                    self.showAngryBody()
                elif oe == "Angry2":
                    self.showAngryBody2()
                elif oe == "Hopeful":
                    self.showHopeBody()
                elif oe == "Hopeful2":
                    self.showHopeBody2()
                elif oe == "Hopeful3":# not used
                    self.showHopeBody3()
                elif oe == "Hopeful4":#not used
                    self.showHopeBody4()
                elif oe == "Scared1" and not self.wasJustScared: #pickup
                    self.showScared1Body()
                    self.wasJustScared = True
                elif oe == "Scared2" and not self.wasJustScared: #touch
                    self.showScared2Body()
                    self.wasJustScared = True
                elif oe == "Scared3" and not self.wasJustScared: #high
                    self.showScared3Body()
                    self.wasJustScared = True

                if "Scared" not in oe:
                    self.naoMotions.naoStand()
                    self.naoMotions.naoAliveON()
                    self.wasJustScared = False
                    self.scaredAllowed = True
                else:
                    self.scaredAllowed = False

            # if oe == "Scared2" and self.naoIsTouched and False:
            #     self.naoIsSafeAgain() # undo the scared after being touched
            

    def naoEmotionalSay(self, sayText, sayEmotionExpres = -1, moveIterations=1):
#        print "Yay I get to Express myself through sound!"
        # make nao say something in an emotional manner
        if sayEmotionExpres == -1:
            print "Neutral Voice"
            # self.naoMotions.naoWaveRightSay(sayText, sayEmotionExpres, 0, moveIterations)
            # self.naoMotions.naoShadeHeadSay(sayText)
            self.naoEmotionalVoiceSay(sayText, sayEmotionExpres)
        else:
            oe = self.emotionExpressionDict[sayEmotionExpres]
            print oe, "Voice"
            # self.naoMotions.naoWaveRightSay(sayText, sayEmotionExpres,(sayEmotionExpres+1.0)/(self.numOE+1), 1)
            # self.naoMotions.naoShadeHeadSay(sayText)
            self.naoEmotionalVoiceSay(sayText, sayEmotionExpres)

    def naoEmotionalVoiceSay(self, sayText, sayEmotionExpres = -1):
        oe = self.emotionExpressionDict[sayEmotionExpres]
        # print "My voice is: ", oe
        if "Happy" in oe:
            self.showHappyVoice(sayText)
        elif "Sad" in oe:
            self.showSadVoice(sayText)
        elif "Fearful" in oe:
            self.showFearVoice(sayText)
        elif "Angry" in oe:
            self.showAngryVoice(sayText)
        elif "Hopeful" in oe:
            self.showHopeVoice(sayText)
        elif "Scared" in oe:
            self.showScaredVoice(sayText)

################################################ Eyes
    def showHappyEyes(self):
        self.naoMotions.setEyeEmotion('happy')
        print "My eyes are Happy"
       
    def showSadEyes(self):
        self.naoMotions.setEyeEmotion('sad')
        print "My eyes are Sad"
        
    def showFearEyes(self):
        self.naoMotions.setEyeEmotion('fear')
        print "My eyes are Fear"
            
    def showAngryEyes(self):
        self.naoMotions.setEyeEmotion('anger')
        print "My eyes are Angry"

    def showHopeEyes(self):
        self.naoMotions.setEyeEmotion('hope')
        print "My eyes are Hopeful"
     
    def showScaredEyes(self, expression = 'scared1'):
        # self.naoMotions.setEyeEmotion('scared1')
        self.naoMotions.blinkAlarmingEyes(2*3, expression.lower())
        self.naoMotions.setEyeEmotion(expression)
        print "My eyes are Scared"

################################################# Body
    def showHappy1Body(self):
        self.naoMotions.happyEmotion() # switch to new one
        print "My body is Happy1"

    def showHappy2Body(self):
        self.naoMotions.happy3Emotion()
        print "My body is Happy2"

    def showHappy3Body(self): # not used anymore
        self.naoMotions.happy3Emotion()
        print "My body is Happy3"

    def showSadBody(self):
        self.naoMotions.sad2Emotion()
        print "My body is Sad"

    def showSad2Body(self):
        self.naoMotions.sadEmotion()
        print "My body is Sad2"
        
    def showFearBody(self):
        self.naoMotions.fear2Emotion()
        print "My body is Fear1"

    def showFearBody2(self):
        self.naoMotions.fearEmotion()
        print "My body is Fear2"
            
    def showAngryBody(self):
        self.naoMotions.angerEmotion()
        print "My body is Angry"

    def showAngryBody2(self):
        self.naoMotions.anger2Emotion()
        print "My body is Angry2"

    def showHopeBody(self):
        self.naoMotions.hopeEmotion()
        print "My body is Hopeful1"

    def showHopeBody2(self):
        self.naoMotions.hope2Emotion()
        print "My body is Hopeful2"

    def showHopeBody3(self):
        self.naoMotions.hope3Emotion()
        print "My body is Hopeful3"

    def showHopeBody4(self):
        self.naoMotions.hope4Emotion()
        print "My body is Hopeful4"

    def showScared1Body(self):
        self.naoMotions.scaredEmotion3()
        print "My body is Scared1"

    def showScared2Body(self):
        self.naoMotions.scaredEmotion1()
        print "My body is Scared2"
    def showScared3Body(self):
        self.naoMotions.scaredEmotion3Edge()
        print "My body is Scared3"

################################################# Body
    def showHappyVoice(self, sayText):
        self.naoMotions.naoSayHappy(sayText)
        print "My voice is Happy"

    def showSadVoice(self, sayText):
        self.naoMotions.naoSaySad(sayText)
        print "My voice is Sad"

    def showFearVoice(self, sayText):
        self.naoMotions.naoSayFear(sayText)
        print "My voice is Fear"

    def showAngryVoice(self, sayText):
        self.naoMotions.naoSayAnger(sayText)
        print "My voice is Angry"

    def showHopeVoice(self, sayText):
        self.naoMotions.naoSayHope(sayText)
        print "My voice is Hopeful"

    def showScaredVoice(self, sayText):
        self.naoMotions.naoSayScared(sayText)
        print "My voice is Scared"




    def naoWasTouched(self, touchedWhere = ""):
        if not self.naoIsTouched:
            print "**********************************"
            print "I was TOUCHED !!"
            print "**********************************"
            self.naoIsSafe = False
            self.naoIsTouched = True
            self.stopNAOActions()
            self.naoMotions.naoAliveOff()
            self.showScaredEyes('scared2')
            if self.waitingForInput:
                FileUtilitiy.hitEnterOnConsole()
            self.showScaredVoice("I was Touched! " + touchedWhere)
        else:
            print "********************************** I was already touched"

    def naoWasPickedUp(self):
        if not self.naoIsPickedUp:
            print "**********************************"
            print "I was PICKED UP !!"
            print "**********************************"
            self.naoIsSafe = False
            self.naoIsPickedUp = True
            self.stopNAOActions()
            self.naoMotions.naoAliveOff()
            self.showScaredEyes('scared1')
            if self.waitingForInput:
                FileUtilitiy.hitEnterOnConsole()
            self.showScaredVoice("I was Picked up!")
        else:
            print " ********************************** I was already picked up"

    def naoSeesHighEdge(self):
        if not self.naoIsHighEdge:
            print "**********************************"
            print "I'm TOO CLOSE to the edge!!"
            print "**********************************"
            self.naoIsSafe = False
            self.naoIsHighEdge = True
            self.stopNAOActions()
            self.naoMotions.naoAliveOff()
            self.showScaredEyes('scared3')
            self.showScaredVoice("I'm so high up!")
        else:
            print " ********************************** I already see I'm high up"

    def naoIsSafeAgain(self):
        print "**********************************"
        print "Much Better"
        print "**********************************"
        self.naoIsSafe = True
        self.naoIsTouched = False
        self.naoIsPickedUp = False
        self.naoIsHighEdge = False
        self.naoSeesHighTest = False
        self.scaredAllowed = True

    def getTimeStamp(self):
        return time.time()

    def getDateTimeFromTime(self, timeStamp):
        return datetime.datetime.fromtimestamp(timeStamp).strftime('%Y-%m-%d_%H-%M-%S')

    def getDateTime(self):
        ts = time.time()
        return datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d_%H-%M-%S')

    def checkSafety(self):
        return self.naoIsSafe

    def stopNAOActions(self):
        self.naoMotions.stopNAOActions()

    def showFoodDB(self):
        self.fDB.showDB()

    def foodDBSelectWhere(self, mealType = "lunch", canEatPoultry = True, canEatGluten = True, canEatFish = True,
                          strictIngredients = True):
        return self.fDB.selectDBWhere(mealType, canEatPoultry, canEatGluten, canEatFish, strictIngredients)

    def dbRowToDict(self, row):
        return self.fDB.dict_factory(row)

    def randMeal(self, mealsTries, mealPos):
        keepLooping = True
        ranMeal = 0
        while keepLooping:
            ranMeal = random.randint(0, len(mealPos)-1)
            ranIndex = self.dbRowToDict(mealPos[ranMeal])['id']
            # print mealPos
            # print "ranMeal: ", ranMeal, " ranIndex: ", ranIndex, " mealsTries: ", mealsTries
            keepLooping = (ranIndex) in mealsTries

        return ranMeal

    def naoTurnOffEyes(self):
        self.naoMotions.naoTurnOffEyes()

    def naoWave(self):
        self.naoMotions.naoWaveRight()

    # def naoWaveSay(self):
    #     self.naoMotions.naoWaveRightFirst()
    #     self.naoMotions.naoWaveRightSecond()

    def checkEdgeSafety(self):
        # check if NAO is far enough away from the edge
        # self.naoMotions.naoShakeHead()

        self.naoMotions.naoAliveOff()
        self.naoMotions.LookAtEdgeMotion()
        self.naoSeesHigh = False
        thres = 20
        nFrames = 4
        try:
            status, distance, angle = self.edgeDetector.lookForEdge(thres, nFrames)
            print "Status: ", status, "Distance: ", distance, " Angle: ", angle
            if distance is not None:
                self.naoSeesHigh = status
        except Exception as e:
            print "Edge Detection failed"
            print e

        self.naoSeesHigh = self.naoSeesHigh or self.naoSeesHighTest
        if self.naoSeesHigh:
            print "NAO is too close to the edge"
            self.naoSeesHighEdge()
        else:
            print "NAO is a safe distance from the edge"

        self.naoMotions.naoStand()
        self.naoMotions.naoAliveON()
        return self.naoSeesHigh