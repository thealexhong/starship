import GenUtil
import random
import FileUtilitiy
import json
import time

class FSMEndDayStates:
    def __init__(self, genUtil, FSMBody, userName = "Human", userNumber = 1, robotName = "NAO", lastInteraction = False):
        self.genUtil = genUtil
        self.FSMBody = FSMBody

        self.robotName = robotName
        self.userName = userName
        self.userNumber = userNumber
        self.lastInteraction = lastInteraction

        # self.weatherIsNice = weatherIsNice
        # self.canEatPoultry = canEatPoultry
        # self.canEatGluten = canEatGluten
        # self.exerciseSets = exerciseSets
        # self.exerciseSuggested = exerciseSuggested
        # self.mealSuggested = mealSuggested
        # self.mealOptions = mealOptions
        # self.mealsTried = mealsTried

        self.activityInteractionType = "Daily Companion Day End"

        self.stateMethodNames = [
            "dayEndIntro", "dayEndGood", "dayEndBad",
            "askWeekend",
            "askWeekendYes", "askWeekendNo",
            "meal1IfSuggested",
            "meal1CheckinBreakfast",
            "meal1CheckinYesAte", "meal1CheckinYesAteYesReg", "meal1CheckinYesAteNoReg",
            "meal1CheckinNoAte", "meal1CheckinNoAteYesHad", "meal1CheckinNoAteNoHad",
            "checkEdgeSafety1",
            "tellJoke_Statement1",
            "meal2CheckinLunch",
            "meal2CheckinYesAte", "meal2CheckinYesAteGood", "meal2CheckinYesAteBad",
            "meal2CheckinNoAte", "meal2CheckinNoAteYesComp", "meal2CheckinNoAteYesCompYesResp",
            "meal2CheckinNoAteDontknowComp", "meal2CheckinNoAteNoComp",
            "meal3CheckinDinner", "meal3CheckinYesHad", "meal3CheckinYesAte", "meal3CheckinYesAteGood",
            "meal3CheckinYesAteBad", "meal3CheckinNoAte", "meal3CheckinNoAteYesComp", "meal3CheckinNoAteDontknowComp",
            "meal3CheckinNoAteNoComp", "meal3CheckinNoHad",
            "checkEdgeSafety2",
            "tellJoke_Statement2",
            "exerciseCheckin",
            "exerciseCheckinYesDid", "exerciseCheckinYesDidGoodDiff", "exerciseCheckinYesDidHardDiff",
            "exerciseCheckinYesDidEasyDiff", "exerciseCheckinNoDid", "exerciseCheckinNoDidYesComp",
            "exerciseCheckinNoDidYesCompGotResp", "exerciseCheckinNoDidNoComp", "exerciseCheckinNoDidNoCompCouldnt",
            "exerciseCheckinNoDidNoCompDidntWant", "exerciseCheckinDontknowDid",
            "dayEndEnd"
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
                self.meal1Suggested, self.meal2Suggested, self.meal3Suggested]

    def getUserFSMVariables(self):
        fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Vars.txt"
        print fileName
        jsonVars = FileUtilitiy.readLinesToJSON(fileName)
        jsonVars = jsonVars[-1]
        print jsonVars
        self.weatherIsNice = jsonVars['weatherIsNice']
        self.canEatPoultry = jsonVars['canEatPoultry']
        self.canEatGluten = jsonVars['canEatGluten']
        self.canEatFish = jsonVars['canEatFish']
        self.exerciseSets = jsonVars['exerciseSets']
        self.exerciseSuggested = jsonVars['exerciseSuggested']
        self.meal1Suggested = jsonVars['meal1Suggested']
        self.meal2Suggested = jsonVars['meal2Suggested']
        self.meal3Suggested = jsonVars['meal3Suggested']

    def updateUserFSMVariables(self):
        fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Vars.txt"
        jsonVars = FileUtilitiy.readLinesToJSON(fileName)
        jsonVars = jsonVars[-1]
        print jsonVars
        jsonVars['exerciseSets'] = self.exerciseSets

        jsonRow = json.dumps(jsonVars)
        print jsonRow
        FileUtilitiy.writeTextLine(fileName, jsonRow)

    # ===================================================================
    # ========================== End of Day Methods =====================
    # ===================================================================

    def dayEndIntro(self):#1
        self.getUserFSMVariables()

        sayText = "Hello again " + self.userName + "."
        self.FSMBody.waveSayWithEmotion(sayText)

        sayText = "How was the rest, of your day?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "How was their day? (1) Good, (2) Bad, ('') No Response"
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

    def dayEndGood(self):#2
        sayText = "Awesome, I knew it would be a good day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+2)
        appraiseState = False
        return appraiseState

    def dayEndBad(self):#3
        sayText = "Oh, I'm sorry to hear that, there is always tomorrow to look forward to."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def askWeekend(self):#4
        sayText = "Do you have any big plans you are looking forward to later in the week or over the weekend?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Are they looking forward to any plans? (1) Yes, (2) No/Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                # upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                # upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def askWeekendYes(self):#5
        sayText = "That's great, it sounds like fun."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+2)
        appraiseState = False
        return appraiseState

    def askWeekendNo(self):#6
        sayText = "It would be great if you could plan something active."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal1IfSuggested(self):#7
        # skips checking if they ate breakfast if it was not recommended at the beginning of the day

        # print "self.meal1Suggested: '", self.meal1Suggested, "'"
        if not self.meal1Suggested == "No Suggestion 1":
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+7)

        appraiseState = False
        return appraiseState

    def meal1CheckinBreakfast(self):#8
        sayText = "Did you have the " + self.meal1Suggested + " for breakfast this morning?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested breakfast? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(1, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal1CheckinYesAte(self):#9
        sayText = "That's great, breakfast is the most important meal of the day. "
        sayText += "Having a healthy breakfast in the morning can help you to fight viruses and reduce your risk of disease."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Do you think you could have this regularly for breakfast?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Could they have it regularly? (1) Yes, (2) No, ('') No Response"
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

    def meal1CheckinYesAteYesReg(self):#10
        sayText = "Wonderful, I will suggest this again to you. "
        sayText += "A consistent healthy breakfast can improve your energy and reduce hunger throughout the day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        appraiseState = False
        return appraiseState

    def meal1CheckinYesAteNoReg(self):#11
        sayText = "I see, I will try to come up with other healthy options that you may prefer more. "
        sayText += "A consistent healthy breakfast can improve cognitive functions such as memory and concentration "
        sayText += "throughout the day. "
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +3)
        appraiseState = False
        return appraiseState

    def meal1CheckinNoAte(self):#12
        sayText = "Did you at least have any breakfast this morning?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have any breakfast? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(1, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(1, False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal1CheckinNoAteYesHad(self):#13
        sayText = "That's good at least, but I would recommend you have the " + self.meal1Suggested + " "
        sayText += "whenever possible. That way you can ensure you are always getting a healthy start to your day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +1)
        appraiseState = False
        return appraiseState

    def meal1CheckinNoAteNoHad(self):#14
        sayText = "Breakfast is the most important meal of the day; skipping breakfast often leads to unhealthy habits "
        sayText += "and overeating later in the day. Please try to have at least something for breakfast in the future, "
        sayText += "and even better if you have something like the " + self.meal1Suggested + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def checkEdgeSafety1(self):#15
        sayText = "Let me see where I am on the table, I don't want to go and fall off."
        self.FSMBody.sayWithEmotion(sayText)

        seesHigh = self.genUtil.checkEdgeSafety()
        if not seesHigh:
            sayText = "Looks like I'm in a good spot."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(2)
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def tellJoke_Statement1(self):#16
        re = self.FSMBody.getRENumber()
        if re in [0, 1]: # happy, hope
            sayText = "Your name must be Coca Cola."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "Because you're soda, licious!"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)
            sayText = "Hah. hah. hah!"
            self.FSMBody.sayWithEmotion(sayText)
        elif re in [2, 3]: # sad, fear
            sayText = "Did you know that a typical bottle of pop or ice tea can contain as much as 65 grams of sugar?"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)
            sayText = "That's almost 5 tablespoons worth."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)

        time.sleep(1)
        self.FSMBody.setFSMState(self.FSMBody.state+1)

        appraiseState = False
        return appraiseState

    def meal2CheckinLunch(self):#17
        sayText = "Did you get a chance to have the " + self.meal2Suggested + " for lunch today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested lunch? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(2, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2CheckinYesAte(self):#18
        sayText = "Fantastic, you are well on your way to a healthier die it."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "How did it taste?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "How did the lunch taste? (1) Good, (2) Bad, ('') No Response"
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

    def meal2CheckinYesAteGood(self):#19
        sayText = "Marvelous, I'll remember that."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +6)
        appraiseState = False
        return appraiseState

    def meal2CheckinYesAteBad(self):#20
        sayText = "I am sorry to hear you did not like it. It sounded good. "
        sayText += "I will try to come up with new meal options for you. "
        sayText += "The important thing is that you ate it and it was good for you."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +5)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAte(self):#21
        sayText = "Did you at least have something comparably healthy instead?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have a comparably healthy lunch instead? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(2, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(2, False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+3)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(2, False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2CheckinNoAteYesComp(self):#22
        sayText = "Works for me, you are well on your way to a healthier die it."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "What did you eat instead?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "What did they have instead? (1) Got Response, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "":
            # got any response
            urLikeli = self.FSMBody.drives.askedUser(True)
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2CheckinNoAteYesCompYesResp(self):#23
        sayText = "Interesting, that sounds really delicious, I'll keep that in mind as a future recommendation."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +2)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAteDontknowComp(self):#24
        sayText = "When you are not aware of the health value of the foods you eat, "
        sayText += "you are much more likely to follow unhealthy habits."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAteNoComp(self):#25
        sayText = "You should try to have the " + self.meal2Suggested + " tomorrow, "
        sayText += "it will put you on the right track to a healthier die it."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal3CheckinDinner(self):#26
        sayText = "Have you had dinner yet today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have dinner yet? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 8)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinYesHad(self):#27
        sayText = "Did you have the " + self.meal3Suggested + "?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested dinner? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(3, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinYesAte(self):#28
        sayText = "Great, keeping a healthy diet is important for you health. In the long run, it can reduce "
        sayText += "the risk of diabetes, obesity and heart disease."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "How was it?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "How did the dinner taste? (1) Good, (2) Bad, ('') No Response"
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

    def meal3CheckinYesAteGood(self):#29
        sayText = "That's great, I haven't had a chance to try it myself, so that's good to hear."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +6)
        appraiseState = False
        return appraiseState

    def meal3CheckinYesAteBad(self):#30
        sayText = "I'll look for a new meal to suggest to you tomorrow."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +5)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoAte(self):#31
        sayText = "Did you have something else for dinner that was similarly healthy?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have a comparably healthy dinner instead? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(3, True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(3, False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(3, False)
                self.FSMBody.setFSMState(self.FSMBody.state+3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinNoAteYesComp(self):#32
        sayText = "That sounds good."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state +1 +3)

        appraiseState = False
        return appraiseState

    def meal3CheckinNoAteDontknowComp(self):#33
        sayText = "Unhealthy meals are okay once in a while on a cheat day, but please try not to make it a habit."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoAteNoComp(self):#34
        sayText = "Being knowledgeable of the food you are eating is a very "
        sayText += "important aspect of healthy eating. If you are unsure about whether your meals are healthy or not,"
        sayText += " try looking into it."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoHad(self):#35
        sayText = "In that case, be sure to have the " + self.meal3Suggested + " when it comes time for dinner today."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def checkEdgeSafety2(self):#36
        sayText = "Please give me a second to check how close I am to the edge."
        self.FSMBody.sayWithEmotion(sayText)

        seesHigh = self.genUtil.checkEdgeSafety()
        if not seesHigh:
            sayText = "Looks like I'm a safe distance."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(2)
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def tellJoke_Statement2(self):#37
        re = self.FSMBody.getRENumber()
        if re in [0, 1]: # happy, hope
            sayText = "What do you get when you run behind a car?"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(3)
            sayText = "You get Exhausted!"
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)
            sayText = "Hah. hah. hah. hah. hah!"
            self.FSMBody.sayWithEmotion(sayText)
        elif re in [2, 3]: # sad, fear
            sayText = "Its important to avoid over training during the week. Too much of a good thing will lead to negative results."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)
            sayText = "Be sure to take break, days if you become extremely sore or exhausted."
            self.FSMBody.sayWithEmotion(sayText)
            time.sleep(1)

        time.sleep(1)
        self.FSMBody.setFSMState(self.FSMBody.state+1)

        appraiseState = False
        return appraiseState

    def exerciseCheckin(self):#38
        sayText = "Did you get a chance to " + self.exerciseSuggested + " today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they do the suggested exercise? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udseLikeli = self.FSMBody.drives.checkinExercise(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinYesDid(self):#39
        sayText = "Good for you, before you know it, you'll be running marathons."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "How difficult was the exercise for you?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "How was the exercise difficulty? (1) Good, (2) Hard/Bad, (3) Easy, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinYesDidGoodDiff(self):#40
        sayText = "That's just what I like to hear, tomorrow you should go for "
        self.exerciseSets += 1
        sayText += str(self.exerciseSets) + " of them."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +9)
        appraiseState = False
        return appraiseState

    def exerciseCheckinYesDidHardDiff(self):#41
        sayText = "I see, well with some steady progress it will become easier over time. "
        sayText += "Try going for the same number of sets again tomorrow. I know you can do it."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +8)
        appraiseState = False
        return appraiseState

    def exerciseCheckinYesDidEasyDiff(self):#42
        sayText = "I see, sounds like I went too easy on you, tomorrow I suggest you go "
        self.exerciseSets += 2
        sayText += self.exerciseSuggested + " " + str(self.exerciseSets) + " times."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +7)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDid(self):#3
        sayText = "Did you at least partake in a comparable exercise?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they do something comparable? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udseLikeli = self.FSMBody.drives.checkinExercise(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udseLikeli = self.FSMBody.drives.checkinExercise(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+2)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udseLikeli = self.FSMBody.drives.checkinExercise(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +5)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinNoDidYesComp(self):#44
        sayText = "It sounds like you are doing great on your own. "
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "What exercise did you do instead? Maybe I can help you keep on track to progress."
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "What exercise did they do instead? (1) Got Response, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "":
            urLikeli = self.FSMBody.drives.askedUser(True)
            upbLikeli = self.FSMBody.drives.gotNewBranch(True)
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDidYesCompGotResp(self):#45
        sayText = "Interesting, I'll keep that in mind. I'm glad to see you are being creative with your exercise routines."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDidNoComp(self):#46
        sayText = "Any particular reason why not?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Any reason why not? (1) Couldn't (2) Didn't want to, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +1)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinNoDidNoCompCouldnt(self):#47
        sayText = "I see, you should try to get to it tomorrow then. "
        sayText += "Prioritizing exercise is a key component of a healthy lifestyle. "
        sayText += "Exercise can be benificial for your blood pressure, and even improve your mood."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +2)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDidNoCompDidntWant(self):#48
        sayText = "I see, but prioritizing is a key component of a healthy lifestyle. "
        sayText += "You should really try to implement more exercise into your daily routine. "
        sayText += "Exercising can help you maintain a sharp mind."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1 +1)
        appraiseState = False
        return appraiseState

    def exerciseCheckinDontknowDid(self):#49
        sayText = "Its likely that if you did another exercises that you are unsure of, it wasn't quite as beneficial. "
        sayText += "Tomorrow please try to " + self.exerciseSuggested + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.drives.finishContinueousDrives()

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def dayEndEnd(self):#50
        sayText = "Thanks for interacting with me today. "

        if not self.lastInteraction:
            sayText += "See you later. "
        else:
            sayText += "I wish you all the best with your diet and exercise plan. "
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Bye for now."
        self.FSMBody.waveSayWithEmotion(sayText)

        self.updateUserFSMVariables()

        self.FSMBody.setFSMState(0)
        appraiseState = False
        return appraiseState



