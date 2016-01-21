# main robot module

import numpy as np
from EmotionMM import EmotionMM
from ObservableExpression import ObservableExpression
import RobotDrive

nEmotions = 6
nExpressions = 11

#recieve input
#user_valance = raw_input('Enter a user valance [-1, 1]: ')
#user_arousal = raw_input('Enter a user arousal [-1, 1]: ')
UE = np.array([0.5, 0.5])

# create the input manipulator of A
impact_ev = np.array([3, 2, 2, 1, 1, 1])
U_in = 1.0*impact_ev/np.sum(impact_ev)
print(U_in)
print


vRE = np.array([0, 1, 0, 0, 0, 0])

reMM = EmotionMM()
oeHMM = ObservableExpression()

newA = reMM.applyInputInfluence(U_in)
print newA
print

totalRE = np.zeros(nEmotions)
totalOE = np.zeros(nExpressions)
# test the distribution
for i in range(100):
	vre = reMM.incrementRobotEmotion()
	RE = reMM.getRobotEmotionNumber()
	totalRE[RE] += 1
	
	voe = oeHMM.determineObservableExpression(vre)
	OE = oeHMM.getObservableExpressionNumber()
	totalOE[OE] += 1
	
print totalRE
print totalOE
print
	
print vre
print voe
print

drives = RobotDrive.RobotDriveCollection()
urLikeli = drives.askedUser(False)
urLikeli = drives.askedUser(False)
dietLikeli = drives.mealCalorieInput(2000)
fitLikeli = drives.dayActiveMinutesInput(15)
urLikeli = drives.askedUser(True)
upLikeli = drives.currentUserEmotionInput(0, 0)
print dietLikeli
print fitLikeli
print urLikeli
print upLikeli
U_in = drives.appraiseEmotions()
print "Overall: ", U_in
print

newA = reMM.applyInputInfluence(U_in)
print newA
print
