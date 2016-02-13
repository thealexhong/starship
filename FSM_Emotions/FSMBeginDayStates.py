import GenUtil
import random
import FileUtilitiy
import json
import time

class FSMBeginDayStates:
    def __init__(self, genUtil, FSMBody, userName = "Human", userNumber = 1, robotName = "NAO"):
        self.genUtil = genUtil
        self.FSMBody = FSMBody

        self.robotName = robotName
        self.userName = userName
        self.userNumber = userNumber

        self.weatherIsNice = True
        self.canEatPoultry = False
        self.canEatGluten = False
        self.canEatFish = False
        self.exerciseSets = 10
        self.exerciseSuggested = "Laps around your Workplace Building"
        self.meal1Suggested = "No Suggestion 1"
        self.meal2Suggested = "No Suggestion 2"
        self.meal3Suggested = "No Suggestion 3"
        self.hasAskedBreakfast = False
        self.hasTalkedJapan = False
        self.hasTalkedParis = False

        self.hasAskedGluten = False
        self.meal2Options = 1
        self.meal2Tried = []
        self.meal3Options = 1
        self.meal3Tried = []

        self.activityInteractionType = "Daily Companion Morning"

        self.stateMethodNames = [
            "morningIntro", "morningGood", "morningBad",
            "askWeather",
            "askWeatherGood", "askWeatherGoodJapan", "askWeatherGoodYesTravel", "askWeatherGoodNoTravel",
            "askWeatherBad", "askWeatherBadParis", "askWeatherBadSameHome", "askWeatherBadDiffHome",
            "askWeatherBadDiffHomeYesTake", "askWeatherBadDiffHomeNoTake",
            "checkEdgeSafety1",
            "tellJoke_Statement1",
            "askBreakfast", "askBreakfastAte",
            "askDietGluten",
            "askDietGlutenYesEat", "askDietGlutenNoEat",
            "meal1DecideBreakfast",
            "askDietPoultry",
            "askDietPoultryYesEat", "askDietPoultryNoEat",
            "meal2DecideLunch",
            "mealFeedbackLunch", "meal2FeedbackYesDelici", "meal2FeedbackNoDelici", "meal2FeedbackNoOptions",
            "askDietFish", "askDietFishYesEat", "askDietFishNoEat",
            "checkEdgeSafety2",
            "tellJoke_Statement2",
            "meal3DecideDinner",
            "meal3FeedbackDinner", "meal3FeedbackYesGood", "meal3FeedbackNoGood", "meal3FeedbackNoOptions",
            "exerciseDecide",
            "exerciseWeatherGood", "exerciseWeatherBad", "exerciseFeedback",
            "exerciseFeedbackGood", "exerciseFeedbackBad", "exerciseFeedbackEasy",
            "morningEnd"
        ]

    def getMethodNames(self):
        return self.stateMethodNames #self.stateMethodNames[self.activityInteractionType]

    def getActivityType(self):
        return self.activityInteractionType

    def getNumMethods(self):
        return len(self.getMethodNames())

    def getFSMVariables(self):
        return [self.weatherIsNice, self.canEatPoultry, self.canEatGluten, self.canEatFish,
                self.exerciseSets, self.exerciseSuggested,
                self.meal1Suggested, self.meal2Suggested, self.meal3Suggested,
                self.hasAskedBreakfast, self.hasTalkedJapan, self.hasTalkedParis]

    def writeUserFSMVariables(self):
        fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Vars.txt"
        varNames = ["weatherIsNice", "canEatPoultry", "canEatGluten", "canEatFish",
                    "exerciseSets", "exerciseSuggested",
                    "meal1Suggested", "meal2Suggested", "meal3Suggested",
                    "hasAskedBreakfast", "hasTalkedJapan", "hasTalkedParis",
                    "dateTime", "activityInteractionType"]

        varVals = self.getFSMVariables()
        varVals.append(self.genUtil.getDateTime())
        varVals.append(self.activityInteractionType)
        jsonRow = {varNames[0]:varVals[0]}
        for i in range(1, len(varNames)):
            jsonRow[varNames[i]] = varVals[i]

        jsonRow = json.dumps(jsonRow)
        print jsonRow
        FileUtilitiy.writeTextLine(fileName, jsonRow)
        print "Wrote users VARS to file"

    def getUserFSMVars(self):
        fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Vars.txt"
        if FileUtilitiy.checkFileExists(fileName):
            jsonVars = FileUtilitiy.readLinesToJSON(fileName)
            jsonVars = jsonVars[-1]
            
            print "Loaded old VARS"
            print jsonVars
            print
            
            self.canEatPoultry = jsonVars['canEatPoultry']
            self.canEatGluten = jsonVars['canEatGluten']
            self.canEatFish = jsonVars['canEatFish']
            self.hasTalkedJapan = jsonVars['hasTalkedJapan']
            self.hasTalkedParis = jsonVars['hasTalkedParis']
            self.exerciseSets = jsonVars['exerciseSets']
        else:
            # keep own files
            print "Didn't need to grab VARS"
            pass


    # ===================================================================
    # ========================== Beginning of Day Methods ===============
    # ===================================================================

    def morningIntro(self):
        self.getUserFSMVars()

        sayText = "Hello " + self.userName + ". "
        self.FSMBody.waveSayWithEmotion(sayText)

        if not (self.hasTalkedJapan or self.hasTalkedParis):
            sayText = "My name is " + self.robotName + ", and I am going to be your diet and fitness companion"
            self.FSMBody.sayWithEmotion(sayText)

        sayText = "How has your morning been so far?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "How was their morning? (1) Good, (2) Bad, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def morningGood(self):
        sayText = "That's great! I hope the rest of your day will go just as well."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+2)
        appraiseState = False
        return appraiseState

    def morningBad(self):
        sayText = "I am sorry to hear that. Hopefully I can cheer you up so you can have a great day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def askWeather(self):
        sayText = "How's the weather outside today?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "How's the weather? (1) Good/Nice, (2) Bad/Not Nice, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
                self.weatherIsNice = True
            # elif textInput == "dono" or textInput == "don't know":

            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1+4)
                self.weatherIsNice = False
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askWeatherGood(self):
        sayText = "Awesome, maybe you should spend some time outside today."
        self.FSMBody.sayWithEmotion(sayText)

        if not self.hasTalkedJapan:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+9)
        appraiseState = False
        return appraiseState

    def askWeatherGoodJapan(self):
        self.hasTalkedJapan = True
        sayText = "That reminds me of Tokyo Japan. It is my favourite place to visit. "
        sayText += "They have the best sushi restaurants in the world! Yummy!"
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Have you been anywhere interesting recently?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Have they been anywhere interesting? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askWeatherGoodYesTravel(self):
        sayText = "Amazing, I love traveling. I am a bit fragile though so I require my own foam-bed "
        sayText += "during transportation. Lucky for me I am not big, so my bed can easily fit on a plane."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+7)
        appraiseState = False
        return appraiseState

    def askWeatherGoodNoTravel(self):
        sayText = "I highly recommend that you go to Japan, I hear it's beautiful around this time of year. "
        sayText += "Cherry blossoms are blooming. Mount Fuji is amazing too!"
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+6)
        appraiseState = False
        return appraiseState

    def askWeatherBad(self):
        sayText = "Not to worry, there are a number of fun activities to do indoors too."
        self.FSMBody.sayWithEmotion(sayText)

        if not self.hasTalkedParis:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+5)
        appraiseState = False
        return appraiseState

    def askWeatherBadParis(self):
        self.hasTalkedParis = True
        sayText = "That reminds me of Paris France where I'm from. It's often rainy there. "
        sayText += "On days like these, I like to go to the Louvre. It has some of the best collections of historic "
        sayText += "items in the world."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Where are you from, what's your hometown?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Was their hometown the same? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askWeatherBadSameHome(self):
        sayText = "Wow, what are the chances, seems like we have a lot in common"
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+3)
        appraiseState = False
        return appraiseState

    def askWeatherBadDiffHome(self):
        sayText = "Wow cool, I've always wanted to visit. One of these days, I'll have to go see it."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Will you take me with you?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Will they take you? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askWeatherBadDiffHomeYesTake(self):
        sayText = "I will definitely hold you to that"
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def askWeatherBadDiffHomeNoTake(self):
        sayText = "I'll just have to go on my own sometime then; I've been meaning to visit there."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def checkEdgeSafety1(self):
        seesHigh = self.genUtil.checkEdgeSafety()
        if not seesHigh:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def tellJoke_Statement1(self):
        re = self.FSMBody.getRENumber()
        if re in [0, 1]: # happy, hope
            sayText = "What does a wolf call a rabbit?"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "Fast Food!"
            self.FSMBody.sayWithEmotion(sayText)
            sayText = "Hah. hah. hah!"
            self.FSMBody.sayWithEmotion(sayText)
        elif re in [2, 3, 4]: # sad, fear, anger
            sayText = "What happened to the Italian chef?"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "He pastaa-away."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)

        time.sleep(1)
        self.FSMBody.setFSMState(self.FSMBody.state+1)

        appraiseState = False
        return appraiseState

    def askBreakfast(self):
        sayText = "Let's start planning your die it for today."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Have you had breakfast yet this morning?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Did they have breakfast? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                self.hasAskedBreakfast = True
                if not self.hasAskedGluten:
                    self.FSMBody.setFSMState(self.FSMBody.state+1+1)
                else:
                    self.FSMBody.setFSMState(self.FSMBody.state+1+3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askBreakfastAte(self):
        sayText = "That's good to hear. Far too often people skip breakfast, which is important to start your day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+4)
        appraiseState = False
        return appraiseState

    def askDietGluten(self):
        sayText = "Are you able to eat products containing gluten?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Can they eat Gluten? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                self.FSMBody.setFSMState(self.FSMBody.state+1)
                self.canEatGluten = True
            else:
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
                self.canEatGluten = False
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        self.hasAskedGluten = True

        appraiseState = True
        return appraiseState

    def askDietGlutenYesEat(self):
        sayText = "Gluten products are high in iron."
        self.FSMBody.sayWithEmotion(sayText)

        if self.hasAskedBreakfast:
            self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+5)
        appraiseState = False
        return appraiseState

    def askDietGlutenNoEat(self):
        sayText = "I can make suggestions that do not include Gluten."
        self.FSMBody.sayWithEmotion(sayText)

        if self.hasAskedBreakfast:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+4)
        appraiseState = False
        return appraiseState

    def meal1DecideBreakfast(self):
        self.genUtil.showFoodDB()
        possibleMeals = self.genUtil.foodDBSelectWhere("breakfast", self.canEatPoultry, self.canEatGluten,
                                                       self.canEatFish, True)
        randMeal = random.randint(0, len(possibleMeals)-1)
        # print "Random select: ", randMeal, " ", possibleMeals
        useMeal = self.genUtil.dbRowToDict(possibleMeals[randMeal])
        self.meal1Suggested = useMeal['name']
        print useMeal

        sayText = "For breakfast, I suggest "
        sayText += useMeal['name'] + "."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "This meal should have about  "
        sayText += useMeal['calories'] + " calories."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def askDietPoultry(self):
        sayText = "Now for lunch today."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Can you eat poultry foods like chicken or turkey?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Can they eat poultry? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                self.FSMBody.setFSMState(self.FSMBody.state+1)
                self.canEatPoultry = True
            else:
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
                self.canEatPoultry = False
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askDietPoultryYesEat(self):
        sayText = "Poultry is a great source of protein."
        self.FSMBody.sayWithEmotion(sayText)

        if self.hasAskedBreakfast:
            self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state -5)
        appraiseState = False
        return appraiseState

    def askDietPoultryNoEat(self):
        sayText = "Thanks for letting me know, I'll remember that"
        self.FSMBody.sayWithEmotion(sayText)

        if self.hasAskedBreakfast:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state -6)
        appraiseState = False
        return appraiseState

    def meal2DecideLunch(self):
        # self.genUtil.showFoodDB()
        strictIngredients = True
        if len(self.meal2Tried) > 0:
            strictIngredients = False
        possibleMeals = self.genUtil.foodDBSelectWhere("lunch", self.canEatPoultry, self.canEatGluten,
                                                       self.canEatFish, strictIngredients)
        randMeal = self.genUtil.randMeal(self.meal2Tried, possibleMeals)#random.randint(0, len(possibleMeals)-1)
        # print "Random select: ", randMeal, " ", possibleMeals
        useMeal = self.genUtil.dbRowToDict(possibleMeals[randMeal])
        if len(self.meal2Tried) <= 0:
            possibleMeals = self.genUtil.foodDBSelectWhere("lunch", self.canEatPoultry, self.canEatGluten,
                                                       self.canEatFish, False)
            self.meal2Options = len(possibleMeals)
        self.meal2Options -= 1
        self.meal2Tried.append(useMeal['id'])
        self.meal2Suggested = useMeal['name']
        # print useMeal

        sayText = "Now for your lunch today, I suggest you eat "
        sayText += useMeal['name'] + "."
        # print sayText
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "This meal should have about  "
        sayText += useMeal['calories'] + " calories."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "You can buy something like this from "
        sayText += useMeal['buyFrom'] + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def mealFeedbackLunch(self):
        sayText = "Doesn't that sound delicious?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Did it sound delicious? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                uporLikeli = self.FSMBody.sendUserEmotionOnRecomend(2)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2FeedbackYesDelici(self):
        sayText = "Perfect, I wish I could eat it too, but I'm a robot."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        appraiseState = False
        return appraiseState

    def meal2FeedbackNoDelici(self):
        sayText = "Let me see if I can suggest an alternative."
        self.FSMBody.sayWithEmotion(sayText)

        if self.meal2Options > 0 and len(self.meal2Tried) < 5:
            self.FSMBody.setFSMState(self.FSMBody.state -3) # suggest again
        else:
            self.FSMBody.setFSMState(self.FSMBody.state +1) # dont suggest again

        appraiseState = False
        return appraiseState

    def meal2FeedbackNoOptions(self):
        sayText = "I don't have anything else to suggest for you. "
        sayText += "Please try to have the " + self.meal2Suggested + ". Maybe you'll like it."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def askDietFish(self):
        sayText = "Are you able to eat fish?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Can they eat fish? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                self.FSMBody.setFSMState(self.FSMBody.state+1)
                self.canEatFish = True
            else:
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
                self.canEatFish = False
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askDietFishYesEat(self):
        sayText = "That's good, fish has a lot of Omega-3 fatty acids. These are essential nutrients "
        sayText += "to keeping your heart and brain healthy."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def askDietFishNoEat(self):
        sayText = "I'll make a note to avoid fish in your diet."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def checkEdgeSafety2(self):
        seesHigh = self.genUtil.checkEdgeSafety()
        if not seesHigh:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def tellJoke_Statement2(self):
        re = self.FSMBody.getRENumber()
        if re in [0, 1]: # happy, hope
            sayText = "Do you have raisins?"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "No? How about a date!"
            self.FSMBody.sayWithEmotion(sayText)
            sayText = "Hah. hah. hah. hah. hah!"
            self.FSMBody.sayWithEmotion(sayText)
        elif re in [2, 3, 4]: # sad, fear, anger
            sayText = "I'm like ice-cream right now."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "I'm feeling really cold right now."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)

        time.sleep(1)
        self.FSMBody.setFSMState(self.FSMBody.state+1)

        appraiseState = False
        return appraiseState

    def meal3DecideDinner(self):
        # self.genUtil.showFoodDB()
        strictIngredients = True
        if len(self.meal3Tried) > 0:
            strictIngredients = False
        print "strictIngredients: ", 
        possibleMeals = self.genUtil.foodDBSelectWhere("dinner", self.canEatPoultry, self.canEatGluten,
                                                       self.canEatFish, strictIngredients)
        print possibleMeals
        randMeal = self.genUtil.randMeal(self.meal3Tried, possibleMeals)#random.randint(0, len(possibleMeals)-1)
        # print "Random select: ", randMeal, " ", possibleMeals
        useMeal = self.genUtil.dbRowToDict(possibleMeals[randMeal])
        if len(self.meal3Tried) <= 0:
            possibleMeals = self.genUtil.foodDBSelectWhere("dinner", self.canEatPoultry, self.canEatGluten,
                                                       self.canEatFish, False)
            self.meal3Options = len(possibleMeals)
        self.meal3Options -= 1
        self.meal3Tried.append(useMeal['id'])
        self.meal3Suggested = useMeal['name']
        # print useMeal

        sayText = "For dinner, I suggest you eat "
        sayText += useMeal['name'] + "."
        # print sayText
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "This meal should have about  "
        sayText += useMeal['calories'] + " calories."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "You can get something like this from "
        sayText += useMeal['buyFrom'] + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal3FeedbackDinner(self):
        sayText = "Does that sound like a good dinner to you?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "Did it sound good? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                uporLikeli = self.FSMBody.sendUserEmotionOnRecomend(3)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3FeedbackYesGood(self):
        sayText = "Great, I look forward to hearing how it was."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        appraiseState = False
        return appraiseState

    def meal3FeedbackNoGood(self):
        sayText = "Let's see if there is anything else I can suggest."
        self.FSMBody.sayWithEmotion(sayText)

        if self.meal3Options > 0 and len(self.meal3Tried) < 5:
            self.FSMBody.setFSMState(self.FSMBody.state -3) # suggest again
        else:
            self.FSMBody.setFSMState(self.FSMBody.state +1)

        appraiseState = False
        return appraiseState

    def meal3FeedbackNoOptions(self):
        sayText = "I'm all out of suggestions. "
        sayText += "Please try the " + self.meal3Suggested + ". It may turn out to be better than you expect."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def exerciseDecide(self):
        sayText = "As exercise for today."
        self.FSMBody.sayWithEmotion(sayText)

        if self.weatherIsNice:
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+1)

        appraiseState = False
        return appraiseState

    def exerciseWeatherGood(self):
        sayText = "Since the weather is nice outside, you should "
        self.exerciseSuggested = "walk a number of blocks one way, and back again"
        sayText += self.exerciseSuggested + "."
        self.FSMBody.sayWithEmotion(sayText)

#        self.exerciseSets = 5
        sayText = "Lets say do " + str(self.exerciseSets) + " blocks."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def exerciseWeatherBad(self):
        sayText = "Since the weather is not that great outside, how about you  "
        self.exerciseSuggested = "Climb some flights of stairs"
        sayText += self.exerciseSuggested + "."
        self.FSMBody.sayWithEmotion(sayText)

#        self.exerciseSets = 5
        sayText = "Let's do " + str(self.exerciseSets) + " sets of climbing a floor up and down."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def exerciseFeedback(self):
        sayText = "How does all this sound to you?"
        self.FSMBody.sayWithEmotion(sayText)
        writeText = "How does the exercise sound? (1) Good, (2) Bad/Hard, (3) Easy, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText).lower()

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            uporLikeli = self.FSMBody.sendUserEmotionOnRecomend(4)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False) # not really bad?
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
            else: # easy
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseFeedbackGood(self):
        sayText = "Great, I hope you enjoy it. It will put you well on your way to a healthy lifestyle."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        appraiseState = False
        return appraiseState

    def exerciseFeedbackBad(self):
        sayText = "Okay. Just for today I'll reduce your exercise, how about you "
        self.exerciseSets -= 1
        sayText += self.exerciseSuggested + ", " + str(self.exerciseSets) + " times instead."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def exerciseFeedbackEasy(self):
        sayText = "I see, in that case, for today, you should try to "
        self.exerciseSets += 1
        sayText +=  self.exerciseSuggested + " " + str(self.exerciseSets) + " times instead."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def morningEnd(self):
        sayText = "I'm looking forward to hearing about how everything goes today, "
        sayText += "I'll check in with you later on."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Have a nice day"
        self.FSMBody.waveSayWithEmotion(sayText)

        self.writeUserFSMVariables()

        self.FSMBody.setFSMState(0)

        appraiseState = False
        return appraiseState




