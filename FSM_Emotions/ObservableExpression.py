# Observable component of the HMM

import numpy as np
import random

class ObservableExpression:
	
	def __init__(self):
		# initial B
		self.B = np.array([
				# happy, hope,  sad,  fear, anger, scared1,scared2, scared 3
				[1.0/8, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000], # happy
				[0.000, 3.0/5, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000], # hope
				[0.000, 0.000, 1.0/4, 0.000, 0.000, 0.000, 0.000, 0.000], # sad
				[0.000, 0.000, 0.000, 2.0/5, 0.000, 0.000, 0.000, 0.000], # fear
				[0.000, 0.000, 0.000, 0.000, 1.0/8, 0.000, 0.000, 0.000], # anger
				[7.0/8, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000], # happy 2
				[0.000, 2.0/5, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000], # hope 2
				[0.000, 0.000, 3.0/4, 0.000, 0.000, 0.000, 0.000, 0.000], # sad 2
				[0.000, 0.000, 0.000, 3.0/5, 0.000, 0.000, 0.000, 0.000], # fear 2
				[0.000, 0.000, 0.000, 0.000, 7.0/8, 0.000, 0.000, 0.000], # anger 2
				[0.000, 0.000, 0.000, 0.000, 0.000, 1.000, 0.000, 0.000], # scared 1
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 1.000, 0.000], # scared 2
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 1.000]  # scared 3
		])
		print self.B
		print

		#E0 = happy, E1 = hope, E2 = sad, E3 = fear, E4 = anger, E5 = Scared1, E6 = Scared2
		#OE0 = happy 1-2, OE1 = hope 1-2, OE2 = sad 1-2, OE3 = fear 1-2, OE4 = anger 1-2, OE5 = Scared1, OE6 = Scared2
		[self.NumOE, self.NumE] = self.B.shape
		
		# set the default expression
		self.vOE = np.zeros(self.NumOE)
		self.vOE[0] = 1

		self.EmotionExpressionAssociation = np.array([
				# ha, ho,  sad, fe, anger,sc1,sc2, sc 3
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0], # happy
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0], # hope
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0], # sad
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0], # fear
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0], # anger
				[2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 2.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 2.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 2.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0],
				[1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0]
		])

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
		# print "HERERHERHERE"
		# print self.getObservableExpressionVector()
		# print self.getObservableExpressionNumber()
		return self.EmotionExpressionAssociation[self.getObservableExpressionNumber(), :]
		