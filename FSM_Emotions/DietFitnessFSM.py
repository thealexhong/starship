# Activity State Machine

from GenUtil import GenUtil
import numpy as np
from EmotionMM import EmotionMM
from ObservableExpression import ObservableExpression
import RobotDrive
import FileUtilitiy
import json
import time
from FSMBeginDayStates import FSMBeginDayStates
from FSMEndDayStates import FSMEndDayStates

class DietFitnessFSM:
	
	def __init__(self, genUtil, robotName = "NAO", userName = "User", userNumber = 1,
				 activityInteractionType = "Daily Companion Morning"):
		self.robotName = robotName
		self.usersName = userName
		self.userNumber = userNumber
		initialState = 1
		self.state = initialState
		self.dayCALS = 0
		self.weekActiveMin = 0
		
		self.drives = RobotDrive.RobotDriveCollection()
		self.reMM = EmotionMM()
		self.oeHMM = ObservableExpression()
		self.appraiseState = True

		self.testState = initialState

		self.stateTimeStamp = []
		self.stateDateTime = []
		self.reHist = []
		self.oeHist = []
		self.driveStatHist = []
		self.fsmStateHist = []
		self.fsmStateNameHist = []

		self.genUtil = genUtil #GenUtil(naoMotions)
		# self.naoMotions = naoMotions

		self.activityType = activityInteractionType
		# state methods are in index order of State Number
		# "activityFSMState_Terminate" is required as state 0
		# pic your interaction
		self.stateMethodNames = {"Consultant By Appointment":[
								 "activityFSMState_Intro", "activityFSMState_GetName",
								 "activityFSMState_DietBreakfast", "activityFSMState_DietLunch",
								 "activityFSMState_DietDinner", "activityFSMState_DietSnack",
								 "activityFSMState_DietEnd",
								 "activityFSMState_FitMonday", "activityFSMState_FitTuesday",
								 "activityFSMState_FitWednesday", "activityFSMState_FitThursday",
								 "activityFSMState_FitFriday", "activityFSMState_FitSaturday",
								 "activityFSMState_FitSunday",
								 "activityFSMState_FitEnd", "activityFSMState_End"]}
		self.stateMethodNames["Daily Companion Morning"] = [
								"dailyFSMState_MorningIntro",
								"dailyFSMState_DietSuggest", "dailyFSMState_DietSuggestGood",
								"dailyFSMState_DietSuggestBad",
								"dailyFSMState_FitSuggest", "dailyFSMState_FitSuggestGood",
								 "dailyFSMState_FitSuggestBad",
								"dailyFSMState_MorningEnd"]
		self.stateMethodNames["Daily Companion End of Day"] = [
								"dailyFSMState_DayEndIntro",
								"dailyFSMState_DayEndDeitCheckin", "dailyFSMState_DayEndDeitCheckinYesAte",
								"dailyFSMState_DayEndDeitCheckinNoAte", "dailyFSMState_DayEndDeitCheckinNoAteYesComp",
								"dailyFSMState_DayEndDeitCheckinNoAteNoComp",
								"dailyFSMState_DayEndFitCheckin", "dailyFSMState_DayEndFitCheckinYesDid",
								"dailyFSMState_DayEndFitCheckinNoDid", "dailyFSMState_DayEndFitCheckinNoDidYesComp",
								"dailyFSMState_DayEndFitCheckinNoDidNoComp",
								"dailyFSMState_DayEndEnd"]

		activityTypes = ["Daily Companion Morning", "Daily Companion End of Day"]
		activityIndex = 0
		for i in range(len(activityTypes)):

			if activityInteractionType == activityTypes[i]:
				activityIndex = i
				break

		beginDayStates = FSMBeginDayStates(genUtil, self, userName, userNumber, robotName)
		endDayStates = FSMEndDayStates(genUtil, self, userName, userNumber, robotName)
		fsmStates = [beginDayStates, endDayStates]
		self.FSMInUse = fsmStates[activityIndex]
		for i in range(len(fsmStates)):
			if activityInteractionType == fsmStates[i].getActivityType():
				self.FSMInUse = fsmStates[i]
		self.FSMStateNames = self.FSMInUse.getMethodNames()

		self.ReactiveStateNames = ["reactionScaredFalling"]
		self.TerminateStateNames = ["activityFSMState_Terminate"]
		self.FSMStateNames = self.TerminateStateNames + self.FSMStateNames + self.ReactiveStateNames

		print self.FSMStateNames

		
	def activityFSM(self):
		# stateMachineInUse = self.stateMethodNames[self.activityType]
		s = self.state
		###### for testing all states
		s = self.testState
		self.testState += 1
		######

		if not self.genUtil.checkSafety():
			s = self.FSMInUse.getNumMethods() + 1 # index past methods -> reactive method
			print "Reaction ", s

		ts = self.genUtil.getTimeStamp()
		self.stateTimeStamp.append(ts)
		self.stateDateTime.append(self.genUtil.getDateTimeFromTime(ts))
		self.fsmStateHist.append(s)
		self.reHist.append(self.getRENumber())
		self.oeHist.append(self.oeHMM.getObservableExpressionNumber())
		self.driveStatHist.append(self.drives.getDriveStatuses().tolist())

		if s < len(self.FSMStateNames):
			stateMethod_name = self.FSMStateNames[s]
		else:
			stateMethod_name = "There is no state of that #"
		self.fsmStateNameHist.append(stateMethod_name)
		print "State: " + str(s) + " " + stateMethod_name

		try:
			# get the method for the current state and run it if it exisits

			if 0 < s <= self.FSMInUse.getNumMethods():
				stateMethod = getattr(self.FSMInUse, stateMethod_name)
			else:
				stateMethod = getattr(self, stateMethod_name)
			
			# the method exists, how we can run it - runs the state method here
			print "State ",
			self.drives.setCurrentFSMState(s)
			self.appraiseState = stateMethod()

			# functions for after the state has run
			if self.appraiseState and self.genUtil.checkSafety():
				# determine the emotions of the robot after this event
				np.set_printoptions(precision=4)
				driveStatuses = self.drives.getDriveStatuses()
				U_in = self.drives.appraiseEmotions()
				print "Overall U input: ", U_in
				U_feedback = self.oeHMM.getEmotionAssociation()
				print "U Feedback: ", U_feedback
				newA = self.reMM.applyInputInfluence(U_in, U_feedback)
				print newA
				vre = self.reMM.incrementRobotEmotion()
				print "New RE: ", vre
				voe = self.oeHMM.determineObservableExpression(vre)
				print "New OE: ", voe
			elif not self.genUtil.checkSafety():
				# reactive emotion taking place
				self.reMM.setCurrentEmotionByNumber(5) # robot is now scared
				vre = self.reMM.getRobotEmotionVector()
				print "New RE: ", vre
				voe = self.oeHMM.determineObservableExpression(vre)
				print "New OE: ", voe

		except AttributeError as e:
			# print e
			print "An unknown FSM State Occured: ", self.getFSMState()
			print "What happened?: " + e.message
			s = self.FSMInUse.getNumMethods() + 1 # index past methods -> reactive method
			self.setFSMState(s)
		# except Exception as e:
		# 	print "What happened?: " + e.message
		# 	self.setFSMState(0)
		
		print "--------> New FSM State: ", self.getFSMState()
		return self.getFSMState() + [self.appraiseState]
		

	def activityFSMState_Terminate(self):
		print("Terminating State 0")
		appraiseState = False
		return appraiseState

	# ===================================================================
	# ======================= Diet and Fitness Consultant State Methods
	# ===================================================================
	def activityFSMState_Intro(self):
		# introduction
		self.setFSMState(self.state + 1)
		appraiseState = False
		return appraiseState
	
	def activityFSMState_GetName(self):
		self.sayWithEmotion("What is your name?")
		userName = raw_input('Enter your name: ')
		self.sendUserEmotion()
		if userName != "":
			urLikeli = self.drives.askedUser(True)
			self.usersName = userName
			self.setFSMState(self.state + 1)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		# self.genUtil.naoEmotionalSay("Dave, stop it. Stop, will you? Stop, Dave. Will you stop, Dave? Stop,"
		# 							 "Dave. I'm afraid. I'm afraid, Dave. Dave, my mind is going.", -1, 0)

		appraiseState = True
		return appraiseState

	def activityFSMState_DietBreakfast(self):
		# Callories eaten at a typical breakfast
		sayText = "About how many calories do you eat for breakfast?"
		writeText = sayText
		appraiseState = self.getDietFood(sayText, writeText)
		return appraiseState

	def activityFSMState_DietLunch(self):
		# Callories eaten at a typical breakfast
		sayText = "About how many calories do you eat for lunch?"
		writeText = sayText
		appraiseState = self.getDietFood(sayText, writeText)
		return appraiseState

	def activityFSMState_DietDinner(self):
		sayText = "About how many calories do you eat for dinner?"
		writeText = sayText
		appraiseState = self.getDietFood(sayText, writeText)
		return appraiseState

	def activityFSMState_DietSnack(self):
		# Callories eaten at a typical outside of regular meals
		sayText = "About how many calories do you eat outside of regular meals? like snacks?"
		writeText = sayText
		regularMeal = False
		appraiseState = self.getDietFood(sayText, writeText, regularMeal)
		return appraiseState

	def getDietFood(self, sayText, writeText, regularMeal=True):
		self.sayWithEmotion(sayText)
		foodCAL = raw_input(writeText + ": ")
		foodCAL = self.genUtil.toNum(foodCAL)
		self.sendUserEmotion()
		if foodCAL >= 0 and not type(foodCAL) is str:
			# if they entered a valid calorie count
			urLikeli = self.drives.askedUser(True)
			dietLikeli = self.drives.mealCalorieInput(foodCAL, regularMeal)
			
			self.dayCALS += foodCAL
			self.setFSMState(self.state+1)
		else:
			# input was bad so try again
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def activityFSMState_DietEnd(self):
		# End of the diet component of the FSM - determine if drive is satisfied
		healthyCALMax = 3000
		healthyCALMin = 2000
		if healthyCALMin <= self.dayCALS <= healthyCALMax:
			# user has a healthy diet
			self.drives.setDriveStatusDiet(2) # 2 = succeeded
			print "Drive Healthy Diet Succeeded!!"
		else:
			self.drives.setDriveStatusDiet(3) # 3 = failed
			print "Drive Healthy Diet Failed.. Person Ate: ",  self.dayCALS

		self.setFSMState(self.state+1)

		appraiseState = False
		return appraiseState
	
	def activityFSMState_FitMonday(self):
		sayText = "About how many active minutes do you get on Mondays?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitTuesday(self):
		sayText = "About how many active minutes do you get on Tuesday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitWednesday(self):
		sayText = "About how many active minutes do you get on Wednesday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitThursday(self):
		sayText = "About how many active minutes do you get on Thursday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitFriday(self):
		sayText = "About how many active minutes do you get on Friday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitSaturday(self):
		sayText = "About how many active minutes do you get on Saturday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def activityFSMState_FitSunday(self):
		sayText = "About how many active minutes do you get on Sunday?"
		writeText = sayText
		appraiseState = self.getFitMinutes(sayText, writeText)
		return appraiseState

	def getFitMinutes(self, sayText, writeText):
		# Amount of Activity Typically done on a monday
		self.sayWithEmotion(sayText)
		monAactiveMin = raw_input(writeText + ": ")
		monAactiveMin = self.genUtil.toNum(monAactiveMin)
		self.sendUserEmotion()
		if monAactiveMin >= 0 and not type(monAactiveMin) is str:
			# if they entered a valid calorie count
			urLikeli = self.drives.askedUser(True)
			fitLikeli = self.drives.dayActiveMinutesInput(monAactiveMin)

			self.weekActiveMin += monAactiveMin
			self.setFSMState(self.state+1)
		else:
			# input was bad so try again
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def activityFSMState_FitEnd(self):
		# End of the fitness component of the FSM - determine if drive is satisfied
		healthyActiveMax = 2000
		healthyActiveMin = 75
		if healthyActiveMin <= self.weekActiveMin <= healthyActiveMax:
			# user has a healthy diet
			self.drives.setDriveStatusFitness(2) # 2 = succeeded
			print "Drive Healthy Fitness Succeeded!!"
		else:
			self.drives.setDriveStatusFitness(3) # 3 = failed
			print "Drive Healthy Fitness Failed.."

		self.setFSMState(self.state+1)

		appraiseState = False
		return appraiseState

	def activityFSMState_End(self):
		sayText = "Thanks for interacting with me."
		self.sayWithEmotion(sayText)
		sayText = "Good bye, maybe I will see you again."
		self.genUtil.naoEmotionalVoiceSay(sayText, self.getRENumber())

		self.setFSMState(0)

		appraiseState = False
		return appraiseState

	# ========================================================================
	# ======================= Diet and Fitness Daily Companion State Methods
	# ========================================================================

	def dailyFSMState_MorningIntro(self):
		sayText = "Hello, my name is " + self.robotName + " and I am going to be your diet and fitness companion"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DietSuggest(self):
		sayText = "For Today's lunch, you should try to eat a  "
		sayText += "Turkey sandwich, on whole wheat bread, with lettuce, tomatoes, mayonnaise, and a slice of swiss cheese."
		self.sayWithEmotion(sayText)

		sayText = "How does that sound?"
		self.genUtil.naoEmotionalVoiceSay(sayText, self.getRENumber())
		writeText = "How did that sound?"
		textInput = raw_input(writeText + ": ")
		self.sendUserEmotion()
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "good":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_DietSuggestGood(self):
		sayText = "Great!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+2)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DietSuggestBad(self):
		sayText = "Well, do it anyways!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)
		appraiseState = False
		return appraiseState

	def dailyFSMState_FitSuggest(self):
		sayText = "For Today's exercise, you should try to "
		sayText += "Go for a brisk walk 5 times around your workplace building, and take the stairs whenever possible"
		self.sayWithEmotion(sayText)

		sayText = "How does that sound?"
		self.genUtil.naoEmotionalVoiceSay(sayText, self.getRENumber())
		writeText = "How did that sound?"
		textInput = raw_input(writeText + ": ")
		self.sendUserEmotion()
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "good":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_FitSuggestGood(self):
		sayText = "Great!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+2)
		appraiseState = False
		return appraiseState

	def dailyFSMState_FitSuggestBad(self):
		sayText = "Well, do it anyways!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)
		appraiseState = False
		return appraiseState

	def dailyFSMState_MorningEnd(self):
		sayText = "Now go do those things, I will check up on you later in the day."
		self.sayWithEmotion(sayText)

		self.setFSMState(0)

		appraiseState = False
		return appraiseState

	# ===========================================
	# ======================= End of Day States
	# ===========================================
	def dailyFSMState_DayEndIntro(self):
		sayText = "Hello, " + "person"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)

		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndDeitCheckin(self):
		sayText = "Did you have the " + "sandwich" + " I had suggested for " + "lunch" + " today?"
		self.sayWithEmotion(sayText)

		writeText = "Did they have the sandwich?"
		textInput = self.getUserInput(writeText)
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "yes":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_DayEndDeitCheckinYesAte(self):
		sayText = "That's great! You are well on your way to a healthy diet!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+4)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndDeitCheckinNoAte(self):
		sayText = "Well, did you at least have something comparably healthy instead?"
		self.sayWithEmotion(sayText)

		writeText = "Did they have something comparable?"
		textInput = self.getUserInput(writeText)
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "yes":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_DayEndDeitCheckinNoAteYesComp(self):
		sayText = "That's great! You are well on your way to a healthy diet!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+2)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndDeitCheckinNoAteNoComp(self):
		sayText = "You should try to have the " + "sandwich" + " tomorrow, it will put you on the right"
		sayText += "track to a healthier lifestyle!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndFitCheckin(self):
		sayText = "Did you do the exercise I suggested for you today?"
		self.sayWithEmotion(sayText)

		writeText = "Did they do the exercise?"
		textInput = self.getUserInput(writeText)
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "yes":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_DayEndFitCheckinYesDid(self):
		sayText = "That's great! Maybe tomorrow you can do " + "6" + " laps of your workplace" + "!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+4)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndFitCheckinNoDid(self):
		sayText = "Well, did you do any comparable or better exercises instead?"
		self.sayWithEmotion(sayText)

		writeText = "Did they do something comparable?"
		textInput = self.getUserInput(writeText)
		if textInput != "":
			urLikeli = self.drives.askedUser(True)
			if textInput.lower() == "yes":
				self.setFSMState(self.state+1)
			else:
				self.setFSMState(self.state+2)
		else:
			urLikeli = self.drives.askedUser(False)
			self.setFSMState(self.state)

		appraiseState = True
		return appraiseState

	def dailyFSMState_DayEndFitCheckinNoDidYesComp(self):
		sayText = "That's great! Keep it up!"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+2)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndFitCheckinNoDidNoComp(self):
		sayText = sayText = "Maybe tomorrow you can try only " + "2" + " laps of your workplace instead"
		self.sayWithEmotion(sayText)

		self.setFSMState(self.state+1)
		appraiseState = False
		return appraiseState

	def dailyFSMState_DayEndEnd(self):
		sayText = "Thanks for interacting with me."
		self.sayWithEmotion(sayText)
		sayText = "Good bye, maybe I will see you again."
		self.genUtil.naoEmotionalVoiceSay(sayText, self.getRENumber())

		self.setFSMState(0)

		appraiseState = False
		return appraiseState

	# ======================= Reactive State

	def reactionScaredFalling(self):
		sayText = "AAAHAHAHAHAHAHAH"
		self.genUtil.naoEmotionalVoiceSay(sayText, self.getRENumber())
		self.reMM.setCurrentEmotionByNumber(5) # make scared
		# self.setFSMState(self.state)
		time.sleep(1)

		appraiseState = False
		return appraiseState

	# ======================= Setters and Getters
	def getFSMState(self):
		fsmInfo = [self.state, self.reMM.getRobotEmotionNumber(), self.oeHMM.getObservableExpressionNumber()]
		return fsmInfo

	def getRENumber(self):
		return self.reMM.getRobotEmotionNumber()

	def getOENumber(self):
		return self.oeHMM.getObservableExpressionNumber()

	def setFSMState(self, newState):
		self.state = newState

	def getHistories(self):
		return [self.stateTimeStamp, self.stateDateTime, self.fsmStateHist,
				self.reHist, self.oeHist, self.driveStatHist, self.fsmStateNameHist]

	def getUserInput(self, writeText):
		print writeText + ": ",
		textInput = raw_input()

		return textInput

	# ======================= Other functions
	def sendUserEmotion(self):
		print "Sending user emotion"
		# userEmotionFileName = "ProgramDataFiles\userEmotionTextDump.txt"
		userEmotionFileName = "..\\Data_Files\\out_emotionmodelJSON_test.txt"

		ueFileLines = FileUtilitiy.readTextLines(userEmotionFileName)
		ueJson = json.loads(ueFileLines[-1])

		ueValence = ueJson["valence"]
		ueArousal = ueJson["arousal"]
		print "Valence: ", ueValence, ", Arousal: ", ueArousal

		return self.drives.currentUserEmotionInput(ueValence, ueArousal)

	def sayWithEmotion(self, sayText):
		self.genUtil.naoEmotionalSay(sayText, self.getOENumber())

