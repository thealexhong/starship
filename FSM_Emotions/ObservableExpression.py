# Observable component of the HMM

import numpy as np
import random

class ObservableExpression:
	
	def __init__(self):
		# initial B
		self.B = np.array([
				[1.0/2, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 1.0/2, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 1.0/2, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 1.0/2, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 1.0/2, 0.000, 0.000],
                [0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 1.0/2],
				[1.0/2, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 1.0/2, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 1.0/2, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 1.0/2, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 1.0/2, 0.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 0.000, 1.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 1.0/2]])
		print self.B
		print

		#E1 = happy, E2 = Sad, E3 = Fear, E4 = Anger, E5 = Surprise, E6 = Hope
		#OE1 = happy 1-2, OE2 = Sad 1-2, OE3 = Fear 1-2, OE4 = Anger 1-2, OE5 = Surprise 1-2, OE6 = Hope 1-2
		[self.NumOE, self.NumE] = self.B.shape
		
		# set the default expression
		self.vOE = np.zeros(self.NumOE)
		self.vOE[0] = 1

		self.EmotionExpressionAssociation = np.array([
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
                [1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 2.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 2.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 2.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 2.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 2.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0]])

	def determineObservableExpression(self, robotEmotionVector):
		vOE_t1 = self.getExpressionDistribution(robotEmotionVector)
		self.determineNextExpression(vOE_t1)
		return self.getObservableExpressionVector()
		
		
	def getExpressionDistribution(self, vRE):
		# calculate the distribution of the observable expressions
		vOE_t1 = self.B * vRE
		vOE_t1 = np.sum(vOE_t1, axis = 1)
		return vOE_t1
		
	def determineNextExpression(self, vOE_t1):
		# randomly pick the expression based on the distribution of OE_t+1
		randOE = random.random()
		print "Random Expression Number: ", randOE
		cdf = 0
		newOE = 0
		for pdf in vOE_t1:
			cdf = cdf + pdf
			if randOE <= cdf:
				break
			newOE += 1
		
		vOE = np.zeros(self.NumOE)
		vOE[newOE] = 1

		self.vOE = vOE
		
		
		
		
	# ======================= Get Methods		
	def getObservableExpressionVector(self):
		return self.vOE
	
	def getObservableExpressionNumber(self):
		return np.argmax(self.vOE)
		
	def getTransitionMatrix(self):
		return self.B	

	def getEmotionAssociation(self):
		return self.EmotionExpressionAssociation[self.getObservableExpressionNumber(), :]
		