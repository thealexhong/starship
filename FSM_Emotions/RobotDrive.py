# drives

import numpy as np
import math
from numpy import linalg as LA

class RobotDrive: # parent class of all drives
	
	def __init__(self, desireability):
		self.desireability = desireability
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

	def setCurrentFSMState(self, state):
		self.currentFSMState = state

	# ============= Setters
	def setDriveStatus(self, newStatus):
		self.status = newStatus
		self.statusChangeState = self.currentFSMState

	def setDriveLikelihood(self, newLikelihood):
		self.status = 1

		self.pastLikelihood = self.likelihood
		self.likelihood = newLikelihood
		
	def appraiseEmotions(self):
		if self.getStatus() == 1:
			eHappy = self.getDesireability() * self.getLikelihoodSuccessChange()
			eSad = self.getDesireability() * self.getLikelihoodFailChange()
			eFear = self.getDesireability() * self.getLikelihoodFail()

			eHope = self.getDesireability() * self.getLikelihoodSuccess()
		else:
			eHappy = 0.0
			eSad = 0.0
			eFear = 0.0
			eHope = 0.0

		# print "self.statusChangeState: ", self.statusChangeState
		if self.getStatus() == 3 and self.currentFSMState == self.statusChangeState:
			# print "BE ANGRY!!!!!"
			# print "self.currentFSMState: ", self.currentFSMState
			# print "self.statusChangeState: ", self.statusChangeState
			eAnger = 1.0*self.getDesireability()
			# print "eAnger: ", eAnger
		else:
			eAnger = 0.0

		eSurprise = 0.0
		eScared = 0.0
		
		eV = np.array([eHappy, eSad, eFear, eAnger, eSurprise, eScared, eHope])
		# remove negative influences and set them to 0 chance
		for i in range(len(eV)):
			if eV[i] < 0 or self.getStatus() == 0: #
				eV[i] = 0

		# print "eV: ", eV
		return eV
		

# To be told of a healthy diet by the user
class DriveHealthyDiet(RobotDrive):
	
	def __init__(self):
		RobotDrive.__init__(self, 1)
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
		print "DietLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()

	def getUserTotalCAL(self):
		return self.userTotalCAL
		
# To be told of an active life style by the user
class DriveHealthyFitness(RobotDrive):
	
	def __init__(self):
		RobotDrive.__init__(self, 1)
		
	def determineLikelihood(self, userActiveMin):
		np.set_printoptions(precision=4)
		healthyActiveMin = 150.0/7 # healthy number of active minutes (daily amount)
		newlikelihood = 1 - 1.0*(healthyActiveMin - userActiveMin)/healthyActiveMin
		self.setDriveLikelihood(newlikelihood)
		print "FitLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()

# To receive responses from the user
class DriveUserResponses(RobotDrive):
	
	def __init__(self):
		RobotDrive.__init__(self, 1)
		
	def determineLikelihood(self, numResponses, numAsked):
		np.set_printoptions(precision=4)
		# stops being happy when the user always responds and doesn't change behaviour (no delta)
		newlikelihood = 1.0*numResponses/numAsked
		self.setDriveLikelihood(newlikelihood)
		print "UResponsesLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()
		
# To main positive emotions from the user
class DriveUserPositive(RobotDrive):
	
	def __init__(self):
		RobotDrive.__init__(self, 2)
		
	def determineLikelihood(self, userValance, userArousal):
		np.set_printoptions(precision=4)
		########################## need to update here - 0.89, 0.17 - Seeing Stars of Valence and Arousal in Blog Posts
		happyVA = np.array([0.89, 0.17])
		# happyVA = np.array([0.85, 0.63]) # coordinates of happiness on the affective space
		newlikelihood = 1 - LA.norm(happyVA - np.array([userValance, userArousal])) / math.sqrt(4+4) # normalize by the longest distance in the affective space
		self.setDriveLikelihood(newlikelihood)
		print "UPositiveLikeli", self.likelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()

# To receive positive feelings upon providing recommendations
class DriveUserPositiveOnRecomendation(RobotDrive):
	def __init__(self):
		RobotDrive.__init__(self, 2)
			
	def determineLikelihood(self, userValance, userArousal):
		np.set_printoptions(precision=4)
		if userValance > 0:
			newlikelihood = 1
		else:
			newlikelihood = 0
		self.setDriveLikelihood(newlikelihood)
		print "UPonRecLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()

# To receive positive branches in the FSM
class DriveUserPositiveBranches(RobotDrive):
	def __init__(self):
		RobotDrive.__init__(self, 2)

	def determineLikelihood(self, numPosBranches, numBranches):
		np.set_printoptions(precision=4)

		newlikelihood = 1.0*numPosBranches/numBranches

		self.setDriveLikelihood(newlikelihood)
		print "UPosBranchesLikeli: ", newlikelihood, " Appraised: ", self.appraiseEmotions()
		return self.getLikelihoodSuccess()

class DriveUserDidSuggestionMeal(RobotDrive):
	def __init__(self):
		RobotDrive.__init__(self, 3)

	def determineLikelihood(self, succeded):
		if succeded:
			newLikelihood = 1.0
		else:
			newLikelihood = 0.0
		self.setDriveLikelihood(newLikelihood)
		if succeded:
			self.setDriveStatus(2)
		else:
			self.setDriveStatus(3)
		print "UDidSMealLikeli: ", newLikelihood, " Appraised: ", self.appraiseEmotions()

class RobotDriveCollection:
	
	def __init__(self):
		self.currentFSMState = 0

		self.driveHealthyDiet = DriveHealthyDiet()
		self.driveHealthyFitness = DriveHealthyFitness()
		self.driveUserResponses = DriveUserResponses()
		self.numResponses = 0
		self.numAsked = 0
		self.driveUserPositive = DriveUserPositive()
		self.driveUserPositiveOnRecomendation = DriveUserPositiveOnRecomendation()
		self.driveUserPositiveBranches = DriveUserPositiveBranches()
		self.numPosBranches = 0
		self.numBranches = 0
		self.driveUserDidSuggestionMeal1 = DriveUserDidSuggestionMeal()
		self.driveUserDidSuggestionMeal2 = DriveUserDidSuggestionMeal()
		self.driveUserDidSuggestionMeal3 = DriveUserDidSuggestionMeal()

		self.drivesCollection = [self.driveHealthyDiet, self.driveHealthyFitness, self.driveUserResponses,
								 self.driveUserPositive, self.driveUserPositiveOnRecomendation,
								 self.driveUserPositiveBranches, self.driveUserDidSuggestionMeal1,
								  self.driveUserDidSuggestionMeal2,  self.driveUserDidSuggestionMeal3]
				
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
		
	def currentUserEmotionInput(self, userValance, userArousal):
		self.driveUserPositive.determineLikelihood(userValance, userArousal)
		return self.driveUserPositive.appraiseEmotions()
		
	def userEmotionAfterRecomendation(self, userValance, userArousal):
		self.driveUserPositiveOnRecomendation.determineLikelihood(userValance, userArousal)
		return self.driveUserPositiveOnRecomendation.appraiseEmotions()

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

	def gotNewBranch(self, positiveBranch):
		if positiveBranch:
			self.numPosBranches += 1
		self.numBranches += 1
		self.driveUserPositiveBranches.determineLikelihood(self.numPosBranches, self.numBranches)
		return self.driveUserPositiveBranches.appraiseEmotions()
		
	def appraiseEmotions(self, ):
		healthyDietEV = self.driveHealthyDiet.appraiseEmotions()
		healthyFitnessEV = self.driveHealthyFitness.appraiseEmotions()
		userResponsesEV = self.driveUserResponses.appraiseEmotions()
		userPositiveEV = self.driveUserPositive.appraiseEmotions()
		userPositiveOnRecEV = self.driveUserPositiveOnRecomendation.appraiseEmotions()
		userPosBranchEV = self.driveUserPositiveBranches.appraiseEmotions()
		userDidSugMeal1EV =  self.driveUserDidSuggestionMeal1.appraiseEmotions()
		userDidSugMeal2EV =  self.driveUserDidSuggestionMeal2.appraiseEmotions()
		userDidSugMeal3EV =  self.driveUserDidSuggestionMeal3.appraiseEmotions()

		# print "Component EVs"
		# print healthyDietEV
		# print healthyFitnessEV
		# print userResponsesEV
		# print userPositiveEV
		# print userPositiveOnRecEV
		# print userPosBranchEV
		# print userDidSugMeal1EV
		# print userDidSugMeal2EV
		# print userDidSugMeal3EV

		overallEV = healthyDietEV + healthyFitnessEV + userResponsesEV
		overallEV += userPositiveEV + userPositiveOnRecEV + userPosBranchEV
		overallEV += userDidSugMeal1EV + userDidSugMeal2EV + userDidSugMeal3EV
		# sumOverallEV = sum(overallEV)
		# if sumOverallEV <= 0:
		# 	sumOverallEV = 1
		# overallEV = overallEV / sumOverallEV
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





