import GenUtil
import random
import FileUtilitiy

class FSMEndDayStates:
    def __init__(self, genUtil, FSMBody, userName = "Human", userNumber = 1, robotName = "NAO"):
        self.genUtil = genUtil
        self.FSMBody = FSMBody

        self.robotName = robotName
        self.userName = userName
        self.userNumber = userNumber

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
            "meal2CheckinLunch",
            "meal2CheckinYesAte", "meal2CheckinYesAteGood", "meal2CheckinYesAteBad",
            "meal2CheckinNoAte", "meal2CheckinNoAteYesComp", "meal2CheckinNoAteYesCompYesResp",
            "meal2CheckinNoAteDontknowComp", "meal2CheckinNoAteNoComp",
            "meal3CheckinDinner", "meal3CheckinYesHad", "meal3CheckinYesAte", "meal3CheckinYesAteGood",
            "meal3CheckinYesAteBad", "meal3CheckinNoAte", "meal3CheckinNoAteYesComp", "meal3CheckinNoAteDontknowComp",
            "meal3CheckinNoAteNoComp", "meal3CheckinNoHad",
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
        fileName = "ProgramDataFiles\\" + str(self.userNumber) + self.userName + "_morningVars.txt"
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


    # ===================================================================
    # ========================== End of Day Methods =====================
    # ===================================================================

    def dayEndIntro(self):
        self.getUserFSMVariables()

        sayText = "Hello again " + self.userName + ", how was the rest of your day?"
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

    def dayEndGood(self):
        sayText = "Awesome, I knew it would be a good day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+2)
        appraiseState = False
        return appraiseState

    def dayEndBad(self):
        sayText = "Oh, I'm sorry to hear that. there is always tomorrow to look forward to."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def askWeekend(self):
        sayText = "Do you have any big plans you are looking forward to later in the week or over the weekend?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Are they looking forward to any plans? (1) Yes, (2) No/Don't know, ('') No Response"
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

    def askWeekendYes(self):
        sayText = "That's great, it sounds like fun."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+2)
        appraiseState = False
        return appraiseState

    def askWeekendNo(self):
        sayText = "It would be great if you could plan something active."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal1IfSuggested(self):
        # skips checking if they ate breakfast if it was not recommended at the beginning of the day

        # print "self.meal1Suggested: '", self.meal1Suggested, "'"
        if not self.meal1Suggested == "No Suggestion 1":
            self.FSMBody.setFSMState(self.FSMBody.state+1)
        else:
            self.FSMBody.setFSMState(self.FSMBody.state+1+7)

        appraiseState = False
        return appraiseState

    def meal1CheckinBreakfast(self):
        sayText = "Did you have the " + self.meal1Suggested + " for breakfast this morning?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested breakfast? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal1CheckinYesAte(self):
        sayText = "That's great, you should always start off your day right, "
        sayText += "and breakfast is the most important meal of the day after all."
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Do you think you could have this regularly for breakfast?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Could the have it regularly? (1) Yes, (2) No, ('') No Response"
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

    def meal1CheckinYesAteYesReg(self):
        sayText = "Wonderful, I will suggest this again to you. "
        sayText += "A consistent healthy breakfast can improve your energy and reduce hunger throughout the day."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        appraiseState = False
        return appraiseState

    def meal1CheckinYesAteNoReg(self):
        sayText = "I see, I will try to come up with other healthy options that you may prefer more. "
        sayText += "A consistent healthy breakfast is really the way to go, "
        sayText += "it can improve your energy and reduce hunger throughout the day. "
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +3)
        appraiseState = False
        return appraiseState

    def meal1CheckinNoAte(self):
        sayText = "Did you at least have any breakfast this morning?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have any breakfast? (1) Yes, (2) No, ('') No Response"
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

    def meal1CheckinNoAteYesHad(self):
        sayText = "That's good at least, but I would recommend you have the " + self.meal1Suggested + " "
        sayText += "whenever possible. That way I'll know for sure that you are having a balanced breakfast, "
        sayText += "and I can help keep track of your lifestyle."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +1)
        appraiseState = False
        return appraiseState

    def meal1CheckinNoAteNoHad(self):
        sayText = "Breakfast is the most important meal of the day; skipping breakfast often leads to unhealthy habits "
        sayText += "and overeating later in the day. Please try to have at least something for breakfast in the future, "
        sayText += "and even better if you have something like the " + self.meal1Suggested + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal2CheckinLunch(self):
        sayText = "Did you get a chance to have the " + self.meal2Suggested + " for lunch today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested lunch? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2CheckinYesAte(self):
        sayText = "Fantastic, you are well on your way to a healthier diet."
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

    def meal2CheckinYesAteGood(self):
        sayText = "Marvelous, I'll remember that."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +6)
        appraiseState = False
        return appraiseState

    def meal2CheckinYesAteBad(self):
        sayText = "I am sorry to hear you did not like it. It sounded good. "
        sayText += "I will try to come up with new meal options for you. "
        sayText += "The important thing is that you ate it and it was good for you."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +5)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAte(self):
        sayText = "Well did you at least have something comparably healthy instead?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have a comparably healthy lunch instead? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+3)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal2CheckinNoAteYesComp(self):
        sayText = "Works for me, you are well on your way to a healthier diet."
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

    def meal2CheckinNoAteYesCompYesResp(self):
        sayText = "Interesting, that sounds really delicious, I'll keep that in mind as a future recommendation."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +3)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAteDontknowComp(self):
        sayText = "When you are not aware of the health value of the foods you eat, "
        sayText += "you are much more likely to follow unhealthy habits."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal2CheckinNoAteNoComp(self):
        sayText = "You should try to have the " + self.meal2Suggested + " tomorrow, "
        sayText += "it will put you on the right track to a healthier diet."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def meal3CheckinDinner(self):
        sayText = "Have you had dinner yet today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have dinner yet? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                udsmLikeli = self.FSMBody.drives.checkinMeal(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                udsmLikeli = self.FSMBody.drives.checkinMeal(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 8)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinYesHad(self):
        sayText = "Did you have the " + self.meal3Suggested + "?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have the suggested dinner? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 + 3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinYesAte(self):
        sayText = "Great, small changes to your diet can do wonders for your health."
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

    def meal3CheckinYesAteGood(self):
        sayText = "That's great, I haven't had a chance to try it myself, so that's good to hear."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +7)
        appraiseState = False
        return appraiseState

    def meal3CheckinYesAteBad(self):
        sayText = "I'll look for a new meal to suggest to you tomorrow."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +6)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoAte(self):
        sayText = "Did you have something else for dinner that was similarly healthy?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they have a comparably healthy dinner instead? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+2)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+3)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def meal3CheckinNoAteYesComp(self):
        sayText = "That sounds good."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state +1 +3)

        appraiseState = False
        return appraiseState

    def meal3CheckinNoAteDontknowComp(self):
        sayText = "Unhealthy meals are okay once in a while on a cheat day, but please try not to make it a habit."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+2)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoAteNoComp(self):
        sayText = "Being knowledgeable of the food you are putting in your stomach is a very "
        sayText += "important aspect of healthy eating. If you are unsure about whether your meals are healthy or not,"
        sayText += " try looking into it."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1+1)
        appraiseState = False
        return appraiseState

    def meal3CheckinNoHad(self):
        sayText = "In that case, be sure to have the " + self.meal3Suggested + " when it comes time for dinner tomorrow."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def exerciseCheckin(self):
        sayText = "Did you get a chance to do the " + self.exerciseSuggested + " today?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they do the suggested exercise? (1) Yes, (2) No, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinYesDid(self):
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

    def exerciseCheckinYesDidGoodDiff(self):
        sayText = "That's just what I like to hear, tomorrow you should go for "
        self.exerciseSets += 1
        sayText += str(self.exerciseSets) + " of them."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +9)
        appraiseState = False
        return appraiseState

    def exerciseCheckinYesDidHardDiff(self):
        sayText = "I see, well with some steady progress it will become easier over time. "
        sayText += "Try going for the same number of sets again tomorrow."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +8)
        appraiseState = False
        return appraiseState

    def exerciseCheckinYesDidEasyDiff(self):
        sayText = "I see, sounds like I went to easy on you, tomorrow you should go "
        self.exerciseSets += 2
        sayText += self.exerciseSuggested + " " + str(self.exerciseSets) + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +7)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDid(self):
        sayText = "Did you at least partake in a comparable exercise?"
        self.FSMBody.sayWithEmotion(sayText)

        writeText = "Did they do something comparable? (1) Yes, (2) No, (3) Don't know, ('') No Response"
        textInput = self.FSMBody.getUserInput(writeText)

        upLikeli = self.FSMBody.sendUserEmotion()
        if textInput != "" and (textInput == "1" or textInput == "2" or textInput == "3"):
            urLikeli = self.FSMBody.drives.askedUser(True)
            if textInput.lower() == "1":
                upbLikeli = self.FSMBody.drives.gotNewBranch(True)
                self.FSMBody.setFSMState(self.FSMBody.state+1)
            elif textInput.lower() == "2":
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1+1)
            else:
                upbLikeli = self.FSMBody.drives.gotNewBranch(False)
                self.FSMBody.setFSMState(self.FSMBody.state+1 +2)
        else:
            urLikeli = self.FSMBody.drives.askedUser(False)
            self.FSMBody.setFSMState(self.FSMBody.state)

        appraiseState = True
        return appraiseState

    def exerciseCheckinNoDidYesComp(self):
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

    def exerciseCheckinNoDidYesCompGotResp(self):
        sayText = "Interesting, I'll keep that in mind. I'm glad to see you are being creative with your exercise routines."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +4)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDidNoComp(self):
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

    def exerciseCheckinNoDidNoCompCouldnt(self):
        sayText = "I see, you should try to get to it tomorrow then. "
        sayText += "Prioritizing exercise is a key component of a healthy lifestyle. "
        sayText += "Exercise can be benificial for your blood pressure, help with weight loss and even improve your mood."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +2)
        appraiseState = False
        return appraiseState

    def exerciseCheckinNoDidNoCompDidntWant(self):
        sayText = "I see, but prioritizing is a key component of a healthy lifestyle. "
        sayText += "You should really try to implement more exercise into your daily routine. "
        sayText += "It can be beneficial for your blood pressure, help with weight loss and even improve your mood."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1 +1)
        appraiseState = False
        return appraiseState

    def exerciseCheckinDontknowDid(self):
        sayText = "Its likely that if you did another exercises, it wasn't quite as beneficial. "
        sayText += "Tomorrow you should try to do the " + self.exerciseSuggested + "."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(self.FSMBody.state+1)
        appraiseState = False
        return appraiseState

    def dayEndEnd(self):
        sayText = "Thanks for interacting with me today. "
        lastInteraction = True
        if lastInteraction:
            sayText += "See you tomorrow. "
        else:
            sayText += "I wish you all the best with your diet and exercise plan. "
        self.FSMBody.sayWithEmotion(sayText)

        sayText = "Bye for now."
        self.FSMBody.sayWithEmotion(sayText)

        self.FSMBody.setFSMState(0)
        appraiseState = False
        return appraiseState