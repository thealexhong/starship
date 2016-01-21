# utility functions
import time
import datetime
from NAO_Util.BasicMotions import BasicMotions
from FoodDBManager import FoodDBManager
import random

class GenUtil:
    def __init__(self, naoMotions = BasicMotions()):
        # no inits
        self.emotionExpressionDict = ["Happy", "Sad", "Fearful", "Angry", "Surprised",
                                      "Happy2", "Sad2", "Fearful2", "Angry2", "Surprised2",
                                      "Scared", "Hopeful"]
        self.numOE = len(self.emotionExpressionDict)
        self.naoMotions = naoMotions
        self.naoIsSafe = True
        self.fDB = FoodDBManager()
		
    def toNum(self, numAsString):
        # Convert string to either int or float
        try:
            ret = int(float(numAsString))
        except ValueError:
            print "***** This string is not a number: ", numAsString
            ret = numAsString
        return ret

    def expressEmotion(self, obserExpresNum):
        # expresses NAOs emotion through what ever was picked as the observable expression
        if obserExpresNum == 0:
            print self.emotionExpressionDict[obserExpresNum], "Face"
            self.naoEmotionalVoiceSay("I am " + self.emotionExpressionDict[obserExpresNum], obserExpresNum)
        else:
            print self.emotionExpressionDict[obserExpresNum], "Face"
            self.naoEmotionalVoiceSay("I am " + self.emotionExpressionDict[obserExpresNum], obserExpresNum)

    def naoEmotionalSay(self, sayText, sayEmotionExpres = -1, moveIterations=1):
        # make nao say something in an emotional manner
        if sayEmotionExpres == -1:
            print "Neutral Voice"
            self.naoMotions.naoWaveRightSay(sayText, sayEmotionExpres, 0, moveIterations)
            # self.naoMotions.naoSay(sayText)
        else:
            print self.emotionExpressionDict[sayEmotionExpres], "Voice"
            self.naoMotions.naoWaveRightSay(sayText, sayEmotionExpres,(sayEmotionExpres+1.0)/(self.numOE+1), 1)
            # self.naoMotions.naoSay(sayText)

    def naoEmotionalVoiceSay(self, sayText, sayEmotionExpres = -1):
        self.naoMotions.naoSay(sayText)

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