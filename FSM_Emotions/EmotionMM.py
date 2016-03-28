import numpy as np
import random

class EmotionMM:
	
	def __init__(self):
		# initial A
		d = 10
		self.Ainit = np.array([
				# happy, hope, sad, fear, anger, scared 1 2 3
				[5.0/d, 4.0/d, 1.0/d, 2.0/d, 0.0/d, 2.0/d, 2.0/d, 2.0/d],
				[3.0/d, 2.0/d, 2.0/d, 2.0/d, 2.0/d, 2.0/d, 2.0/d, 2.0/d],
				[1.0/d, 2.0/d, 2.0/d, 2.0/d, 3.0/d, 2.0/d, 2.0/d, 2.0/d],
				[1.0/d, 1.0/d, 2.0/d, 1.0/d, 2.0/d, 2.0/d, 2.0/d, 2.0/d],
				[0.0/d, 1.0/d, 3.0/d, 3.0/d, 3.0/d, 2.0/d, 2.0/d, 2.0/d],
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000],
				[0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000, 0.000]
				])
		print(self.Ainit)
		print

		self.A = self.Ainit

		#E0 = happy, E1 = Sad, E2 = Fear, E3 = Anger, E4 = Hope, e5 = Scared1 (Pickup), e6 = Scared2 (touch), e7 = Scared3 (high)
		[rows, self.NumE] = self.Ainit.shape
		
		# default initial emotion
		self.vRE = np.zeros(self.NumE)
		self.vRE[0] = 1
	
	def getNextEmotionDistribution(self):
		# calculate the distribution of the next emotional state
		vRE_t1 = self.A * self.vRE #RE means robot emotion #Equivalent to X in formula A1 in paper(final version)
		#NOTE: numpy do not add multiplication of matrix elements together, need to sum it up explicitly
		vRE_t1 = np.sum(vRE_t1, axis = 1)
		
		return vRE_t1

	def determineNextRobotEmotion(self, vRE_t1):
		# randomly pick the new emotion based on the distribution of RE_t+1
		randRE = random.random() #between 0 and 1
		print "Random Emotion Number: ", randRE
		cdf = 0 #cumulative distribution function
		newRE = 0 #new robot emotion
		for pdf in vRE_t1: #probability distribution function
			cdf = cdf + pdf
			if randRE <= cdf:
				break
			newRE += 1 #emotion index increment
		
		vRE = np.zeros(self.NumE)
		vRE[newRE] = 1
		#it is convenient to store the emotion index as vector
		self.vRE = vRE
		
	def applyInputInfluence(self, U_in = np.zeros(1), U_feed = np.zeros(1)):
		# if no influences are present, then normalize the whole effect to 1 (no change to A
		if np.sum(U_in) <= 0:
			U_in = np.ones(self.NumE)
		if np.sum(U_feed) <= 0:
			U_feed = np.ones(self.NumE)
		A = np.transpose(np.transpose(self.Ainit) * U_in * U_feed) #formula A6 in the final version of the paper

		A = self.normalizeA(A)
		# only use probabilities >10%
		probThres = True
		if probThres:
			print "Checking Threshold"
			thres = 0.10 #emperical value
			for i in range(self.NumE):
				for j in range(self.NumE):
					a = A[i, j]
					if a < thres and a != 0.0:
						print "Old Val: ", a, " i=", i, " j=", j
						A[i, j] = 0
		self.A = self.normalizeA(A)
		return self.A

	def normalizeA(self, A):
		#formula A7 in the paper (final version)
		colsum = np.sum(A, axis = 0)
		# print "colsum", colsum
		for i in range(len(colsum)):
			if colsum[i] <= 0:
				colsum[i] = 1
		A = A * np.transpose(1/colsum) # renormalize the columns of A to sum to 1
		return A
		
	def incrementRobotEmotion(self):
		vRE_t1 = self.getNextEmotionDistribution()
		self.determineNextRobotEmotion(vRE_t1)
		return self.getRobotEmotionVector()
		
		
	# reactive component bypasses the MM and directly sets the robots emotion
	def checkReactiveEmotionsDiet(self, userCal):
		surpriseCal = 4000 # number of calories that are surprising to have eaten
		if userCal > surpriseCal:
			self.setCurrentEmotionByNumber(4)
			
	def checkReactiveEmotionsFitness(self, userActiveMin):
		surpriseMin = 700 # number of active minutes that are surprising
		if userActiveMin > surpriseMin:
			self.setCurrentEmotionByNumber(4)
		
				
	# ======================= Get Methods		
	def getRobotEmotionVector(self):
		return self.vRE
	
	def getRobotEmotionNumber(self):
		#RE means robot emotion
		return np.argmax(self.vRE)
		
	def getTransitionMatrix(self):
		return self.A
	
	# ======================= Set Methods
	def setCurrentEmotionVector(self, vREnew):
		# current emotional state
		self.vRE = vREnew
		
	def setCurrentEmotionByNumber(self, REnew):
		if REnew < self.NumE:
			self.vRE = np.zeros(self.NumE)
			self.vRE[REnew] = 1
