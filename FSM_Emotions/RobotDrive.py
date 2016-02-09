# drives

import numpy as np
import math
from numpy import linalg as LA
import FileUtilitiy

class RobotDrive: # parent class of all drives
	
	def __init__(self, desireability, userName, userNumber):
		self.desireability = desireability
		self.userName = userName
		self.userNumber = userNumber
		self.likelihood = 0.5
		self.pastLikelihood = 0.5
		self.status = 0 # 0 = inactive, 1 = active, 2 = succeeded, 3 = failed
		self.statusChangeState = 0

		self.currentFSMState = 0
		
	def getLikelihoodSuccess(self):
		return self.likelihood
		
	def getLikelihoodFail(self):
		return 1 - self.likelihood
		
	def getLikelihoodSuccessChange(self):
		return self.likelihood - self.pastLikelihood
		
	def getLikelihoodFailChange(self):
		return (1 - self.likelihood) - (1 - self.pastLikelihood)
		
	def getDesireability(self):
		return self.desireability
		
	def getStatus(self):
		return self.status

	def showDriveAppraisal(self, appraisal, driveName = "", likelihood = ""):
		showText = driveName + ": " + str(likelihood) + " Appraised: " + str(appraisal)
		print showText

		fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Appraisals.txt"
		FileUtilitiy.writeTextLine(fileName, "currentFSMState: " + str(self.currentFSMState) + " " + showText)

	def setCurrentFSMState(self, state):
		self.currentFSMState = state

	# ============= Setters
	def setDriveStatus(self, newStatus):
		self.status = newStatus
		self.statusChangeState = self.currentFSMState

	def setDriveLikelihood(self, newLikelihood):
		self.setDriveStatus(1)

		self.pastLikelihood = self.likelihood
		self.likelihood = newLikelihood
		
	def appraiseEmotions(self):
		eHappy = 0.0
		eSad = 0.0
		eFear = 0.0
		eHope = 0.0
		eAnger = 0.0
		eScared1 = 0.0
		eScared2 = 0.0
		eScared3 = 0.0
		if self.getStatus() == 1:
			if self.currentFSMState == self.statusChangeState:
				eHappy = self.getDesireability() * self.getLikelihoodSuccessChange()
				eSad = self.getDesireability() * self.getLikelihoodFailChange()
			eFear = self.getDesireability() * self.getLikelihoodFail()
			eHope = self.getDesireability() * self.getLikelihoodSuccess()

		if self.getStatus() == 2 and self.currentFSMState == self.statusChangeState:
			eHappy = self.getDesireability() * self.getLikelihoodSuccessChange()
		# print "self.statusChangeState: ", self.statusChangeState
		if self.getStatus() == 3 and self.currentFSMState == self.statusChangeState:
			# print "BE ANGRY!!!!!"
			# print "self.currentFSMState: ", self.currentFSMState
			# print "self.statusChangeState: ", self.statusChangeState
			eSad = self.getDesireability() * self.getLikelihoodFailChange()
			eAnger = 1.0*self.getDesireability()
			# print "eAnger: ", eAnger

		eV = np.array([eHappy, eHope, eSad, eFear, eAnger, eScared1, eScared2, eScared3])
		# remove negative influences and set them to 0 chance
		for i in range(len(eV)):
			if eV[i] < 0 or self.getStatus() == 0: #
				eV[i] = 0

		# print "eV: ", eV
		return eV
		

# To be told of a healthy diet by the user
class DriveHealthyDiet(RobotDrive):
	
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 1, userName, userNumber)
		self.userTotalCAL = 0
	
	def determineLikelihood(self, userCal, regularMeal = True):
		np.set_printoptions(precision=4)
		if regularMeal:
			healthyCal = 2500.0/3 # healthy calories value (meal amount)
		else:
			healthyCal = 100
		self.userTotalCAL += userCal
		newlikelihood = 1 - 1.0*abs(userCal - healthyCal)/healthyCal
		self.setDriveLikelihood(newlikelihood)
		# print "DietLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "DietLikeli", str(newlikelihood))
		return self.getLikelihoodSuccess()

	def getUserTotalCAL(self):
		return self.userTotalCAL
		
# To be told of an active life style by the user
class DriveHealthyFitness(RobotDrive):
	
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 1, userName, userNumber)
		
	def determineLikelihood(self, userActiveMin):
		np.set_printoptions(precision=4)
		healthyActiveMin = 150.0/7 # healthy number of active minutes (daily amount)
		newlikelihood = 1 - 1.0*(healthyActiveMin - userActiveMin)/healthyActiveMin
		self.setDriveLikelihood(newlikelihood)
		# print "FitLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "FitLikeli", str(newlikelihood))
		return self.getLikelihoodSuccess()

# To receive responses from the user
class DriveUserResponses(RobotDrive):
	
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 1, userName, userNumber)
		
	def determineLikelihood(self, numResponses, numAsked):
		np.set_printoptions(precision=4)
		# stops being happy when the user always responds and doesn't change behaviour (no delta)
		newlikelihood = 1.0*numResponses/numAsked
		self.setDriveLikelihood(newlikelihood)
		# print "UResponsesLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "UResponsesLikeli", str(newlikelihood))
		return self.getLikelihoodSuccess()
		
# To main positive emotions from the user
class DriveUserPositive(RobotDrive):
	
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 2, userName, userNumber)
		
	def determineLikelihood(self, userValance, userArousal):
		np.set_printoptions(precision=4)
		########################## need to update here - 0.89, 0.17 - Seeing Stars of Valence and Arousal in Blog Posts
		happyVA = np.array([0.89, 0.17])
		# happyVA = np.array([0.85, 0.63]) # coordinates of happiness on the affective space
		newlikelihood = 1 - LA.norm(happyVA - np.array([userValance, userArousal])) / math.sqrt(4+4) # normalize by the longest distance in the affective space
		self.setDriveLikelihood(newlikelihood)
		# print "UPositiveLikeli", self.likelihood, " Appraised: ", self.appraiseEmotions()
		showText = str(newlikelihood) + " " + "V:" + str(userValance) + " A:" + str(userArousal)
		self.showDriveAppraisal(self.appraiseEmotions(), "UPositiveLikeli", showText)
		return self.getLikelihoodSuccess()

# To receive positive feelings upon providing recommendations
class DriveUserPositiveOnRecomendation(RobotDrive):
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 2, userName, userNumber)
			
	def determineLikelihood(self, userValance, userArousal):
		np.set_printoptions(precision=4)
		# happyVA = np.array([0.89, 0.17])
		# newlikelihood = 1 - LA.norm(happyVA - np.array([userValance, userArousal])) / math.sqrt(4+4)

		if userValance > 0:#newlikelihood > 0.8:
			succeeded = True
		else:
			succeeded = False
		return self.determineSucceeded(succeeded)


	def determineSucceeded(self, succeeded):
		if succeeded:
			newLikelihood = 1.0
		else:
			newLikelihood = 0.0
		self.setDriveLikelihood(newLikelihood)
		if succeeded:
			self.setDriveStatus(2)
		else:
			self.setDriveStatus(3)
		# print "UPonRecLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "UPonRecLikeli", str(newLikelihood))
		return self.getLikelihoodSuccess()

# To receive positive branches in the FSM
class DriveUserPositiveBranches(RobotDrive):
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 2, userName, userNumber)

	def determineLikelihood(self, numPosBranches, numBranches):
		np.set_printoptions(precision=4)

		newlikelihood = 1.0*numPosBranches/numBranches

		self.setDriveLikelihood(newlikelihood)
		# print "UPosBranchesLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "UPosBranchesLikeli", str(newlikelihood))
		return self.getLikelihoodSuccess()

class DriveUserDidSuggestion(RobotDrive):
	def __init__(self, userName, userNumber):
		RobotDrive.__init__(self, 3, userName, userNumber)

	def determineLikelihood(self, succeeded):
		if succeeded:
			newLikelihood = 1.0
		else:
			newLikelihood = 0.0
		self.setDriveLikelihood(newLikelihood)
		if succeeded:
			self.setDriveStatus(2)
		else:
			self.setDriveStatus(3)
		# print "UDidSuggLikeli: ", newLikelihood, " Appraised: ", self.appraiseEmotions()
		self.showDriveAppraisal(self.appraiseEmotions(), "UDidSuggLikeli", str(newLikelihood))

class RobotDriveCollection:
	
	def __init__(self, userName, userNumber):
		self.currentFSMState = 0
		self.userName = userName
		self.userNumber = userNumber

		#### not needed
		self.driveHealthyDiet = DriveHealthyDiet(userName, userNumber)
		self.driveHealthyFitness = DriveHealthyFitness(userName, userNumber)
		####

		self.driveUserResponses = DriveUserResponses(userName, userNumber)
		self.numResponses = 0
		self.numAsked = 0
		self.driveUserPositive = DriveUserPositive(userName, userNumber)
		self.sumVal = 0.0
		self.sumArou = 0.0
		self.numAffect = 0
		self.driveUserPositiveBranches = DriveUserPositiveBranches(userName, userNumber)
		self.numPosBranches = 0
		self.numBranches = 0

		self.driveUserPositiveOnRecomendationMeal1 = DriveUserPositiveOnRecomendation(userName, userNumber)
		self.driveUserPositiveOnRecomendationMeal2 = DriveUserPositiveOnRecomendation(userName, userNumber)
		self.driveUserPositiveOnRecomendationMeal3 = DriveUserPositiveOnRecomendation(userName, userNumber)
		self.driveUserPositiveOnRecomendationExcercise = DriveUserPositiveOnRecomendation(userName, userNumber)

		self.driveUserDidSuggestionMeal1 = DriveUserDidSuggestion(userName, userNumber)
		self.driveUserDidSuggestionMeal2 = DriveUserDidSuggestion(userName, userNumber)
		self.driveUserDidSuggestionMeal3 = DriveUserDidSuggestion(userName, userNumber)
		self.driveUserDidSuggestionExercise = DriveUserDidSuggestion(userName, userNumber)

		self.drivesCollection = [#self.driveHealthyDiet, self.driveHealthyFitness,
								 self.driveUserResponses, self.driveUserPositive, self.driveUserPositiveBranches,
								 self.driveUserPositiveOnRecomendationMeal1, self.driveUserPositiveOnRecomendationMeal2,
								 self.driveUserPositiveOnRecomendationMeal3, self.driveUserPositiveOnRecomendationExcercise,
								 self.driveUserDidSuggestionMeal1, self.driveUserDidSuggestionMeal2,
								 self.driveUserDidSuggestionMeal3, self.driveUserDidSuggestionExercise]

		self.showAppraisal("")
		self.showAppraisal("")
		self.showAppraisal("")
				
	def mealCalorieInput(self, userCal, regularMeal=True):
		self.driveHealthyDiet.determineLikelihood(userCal, regularMeal)
		return self.driveHealthyDiet.appraiseEmotions()

	def setDriveStatusDiet(self, newStatus):
		self.driveHealthyDiet.setDriveStatus(newStatus)
		
	def dayActiveMinutesInput(self, userActiveMin):
		self.driveHealthyFitness.determineLikelihood(userActiveMin)
		return self.driveHealthyFitness.appraiseEmotions()

	def setDriveStatusFitness(self, newStatus):
		self.driveHealthyFitness.setDriveStatus(newStatus)

	def askedUser(self, gotResponse):
		self.numAsked += 1
		if gotResponse:
			self.numResponses += 1
		self.driveUserResponses.determineLikelihood(self.numResponses, self.numAsked)
		return self.driveUserResponses.appraiseEmotions()

	def finishedUserResponses(self):
		if self.numAsked/self.numResponses < 0.8:
			# user responded to all questions
			self.driveUserResponses.setDriveStatus(3)
		else:
			self.driveUserResponses.setDriveStatus(2)
		return self.driveUserResponses.appraiseEmotions()
		
	def currentUserEmotionInput(self, userValance, userArousal):
		userValance /= 2
		userArousal /= 2
		self.sumArou += userArousal
		self.sumVal += userValance
		self.numAffect += 1
		self.driveUserPositive.determineLikelihood(userValance, userArousal)
		return self.driveUserPositive.appraiseEmotions()

	def finishUserPositive(self):
		wantAffect = np.array([0.89, 0.17])
		userAffect = np.array([self.sumVal/self.numAffect, self.sumArou/self.numAffect])
		avgDist = LA.norm(wantAffect - userAffect)
		print avgDist
		if avgDist > 0.2:
			self.driveUserPositive.setDriveStatus(3)
		else:
			self.driveUserPositive.setDriveStatus(2)
		return self.driveUserPositive.appraiseEmotions()

	def userEmotionAfterRecomendationMeal(self, userValance, userArousal, mealNum):
		userValance /= 2
		userArousal /= 2
		if mealNum == 1:
			self.driveUserPositiveOnRecomendationMeal1.determineLikelihood(userValance, userArousal)
			return self.driveUserPositiveOnRecomendationMeal1.appraiseEmotions()
		if mealNum == 2:
			self.driveUserPositiveOnRecomendationMeal2.determineLikelihood(userValance, userArousal)
			return self.driveUserPositiveOnRecomendationMeal2.appraiseEmotions()
		else:
			self.driveUserPositiveOnRecomendationMeal3.determineLikelihood(userValance, userArousal)
			return self.driveUserPositiveOnRecomendationMeal3.appraiseEmotions()

	def userEmotionAfterRecomendationExercise(self, userValance, userArousal):
		userValance /= 2
		userArousal /= 2
		self.driveUserPositiveOnRecomendationExcercise.determineLikelihood(userValance, userArousal)
		return self.driveUserPositiveOnRecomendationExcercise.appraiseEmotions()

	def checkinMeal(self, mealNum, userAteSuggestion):
		if mealNum == 1:
			self.driveUserDidSuggestionMeal1.determineLikelihood(userAteSuggestion)
			return self.driveUserDidSuggestionMeal1.appraiseEmotions()
		elif mealNum == 2:
			self.driveUserDidSuggestionMeal2.determineLikelihood(userAteSuggestion)
			return self.driveUserDidSuggestionMeal2.appraiseEmotions()
		else:
			self.driveUserDidSuggestionMeal3.determineLikelihood(userAteSuggestion)
			return self.driveUserDidSuggestionMeal3.appraiseEmotions()

	def checkinExercise(self, userDidSuggExercise):
		self.driveUserDidSuggestionExercise.determineLikelihood(userDidSuggExercise)
		return self.driveUserDidSuggestionExercise.appraiseEmotions()

	def gotNewBranch(self, positiveBranch):
		if positiveBranch:
			self.numPosBranches += 1
		self.numBranches += 1
		self.driveUserPositiveBranches.determineLikelihood(self.numPosBranches, self.numBranches)
		return self.driveUserPositiveBranches.appraiseEmotions()

	def finishedBranches(self):
		if self.numPosBranches/self.numBranches < 0.8:
			self.driveUserPositiveBranches.setDriveStatus(3)
		else:
			self.driveUserPositiveBranches.setDriveStatus(2)
		return self.driveUserPositiveBranches.appraiseEmotions()

	def finishContinueousDrives(self):
		self.finishedBranches()
		self.finishedUserResponses()
		self.finishUserPositive()
		
	def appraiseEmotions(self):
		# healthyDietEV = self.driveHealthyDiet.appraiseEmotions()
		# healthyFitnessEV = self.driveHealthyFitness.appraiseEmotions()
		userResponsesEV = self.driveUserResponses.appraiseEmotions()
		userPositiveEV = self.driveUserPositive.appraiseEmotions()
		userPosBranchEV = self.driveUserPositiveBranches.appraiseEmotions()

		userPositiveOnRecM1EV = self.driveUserPositiveOnRecomendationMeal1.appraiseEmotions()
		userPositiveOnRecM2EV = self.driveUserPositiveOnRecomendationMeal2.appraiseEmotions()
		userPositiveOnRecM3EV = self.driveUserPositiveOnRecomendationMeal3.appraiseEmotions()
		userPositiveOnRecEEV = self.driveUserPositiveOnRecomendationExcercise.appraiseEmotions()

		userDidSugMeal1EV = self.driveUserDidSuggestionMeal1.appraiseEmotions()
		userDidSugMeal2EV = self.driveUserDidSuggestionMeal2.appraiseEmotions()
		userDidSugMeal3EV = self.driveUserDidSuggestionMeal3.appraiseEmotions()
		userDidSugExerEV = self.driveUserDidSuggestionExercise.appraiseEmotions()

		print "Component EVs"
		# print healthyDietEV
		# print healthyFitnessEV
		self.showAppraisal(userResponsesEV)
		self.showAppraisal(userPositiveEV)
		self.showAppraisal(userPosBranchEV)

		self.showAppraisal(userPositiveOnRecM1EV)
		self.showAppraisal(userPositiveOnRecM2EV)
		self.showAppraisal(userPositiveOnRecM3EV)
		self.showAppraisal(userPositiveOnRecEEV)

		self.showAppraisal(userDidSugMeal1EV)
		self.showAppraisal(userDidSugMeal2EV)
		self.showAppraisal(userDidSugMeal3EV)
		self.showAppraisal(userDidSugExerEV)

		overallEV = 0.0 #+ healthyDietEV + healthyFitnessEV
		overallEV += userPositiveOnRecM1EV + userPositiveOnRecM2EV + userPositiveOnRecM3EV + userPositiveOnRecEEV
		overallEV += userPositiveEV + userResponsesEV + userPosBranchEV
		overallEV += userDidSugMeal1EV + userDidSugMeal2EV + userDidSugMeal3EV
		overallEV += userDidSugExerEV
		self.showAppraisal(overallEV, "OverallEV")
		# self.showAppraisal("")

		return overallEV

	# ======== getting drive values
	def getUserDailyCAL(self):
		return self.driveHealthyDiet.getUserTotalCAL()

	def getDriveStatuses(self):
		# print self.drivesCollection
		driveStatuses = np.zeros(len(self.drivesCollection))
		for i in range(len(self.drivesCollection)):
			driveStatuses[i] = self.drivesCollection[i].getStatus()
		print "driveStatuses: ", driveStatuses
		return driveStatuses

	def setCurrentFSMState(self, state):
		self.currentFSMState = state
		for i in range (len(self.drivesCollection)):
			self.drivesCollection[i].setCurrentFSMState(state)

	def showAppraisal(self, EV, name = ""):
		showText = str(EV)
		if showText != "":
			print showText
		fileName = "ProgramDataFiles\\" + str(self.userNumber) + "_" + self.userName + "\\" + str(self.userNumber)  + "_" + self.userName +"_Appraisals.txt"
		FileUtilitiy.writeTextLine(fileName, "currentFSMState: " + str(self.currentFSMState) + " " + name + " " + showText)






