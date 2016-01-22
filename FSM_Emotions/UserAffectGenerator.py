import threading
import time
from GenUtil import GenUtil
import random
import numpy as np
import FileUtilitiy
import json

class UserAffectGenerator(threading.Thread):
    def __init__(self, threadID, name, generateInterval, genUtil):
        threading.Thread.__init__(self)

        self.threadID = threadID
        self.name = name

        self.genInter = generateInterval
        self.continueLoop = True
        self.genUtil = genUtil

    def run(self):
        print "Starting " + self.name
        self.thread_main()
        print "Ending " + self.name

    def thread_main(self):

        numWriten = 0
        happyValance = 0.89
        happyArousal = 0.17
        while self.continueLoop:
            ranValance = self.getRandAffect(happyValance, 1)
            ranArousal = self.getRandAffect(happyArousal, 1)
            self.writeAffect(ranValance, ranArousal)
            numWriten += 1

            # print "numWriten: ", numWriten, "ranValance: ", ranValance, "ranArousal: ", ranArousal
            time.sleep(self.genInter)

    def getRandAffect(self, mean, std):
        randAffect = 10
        while -1 > randAffect or randAffect > 1:
            randAffect = np.random.normal(mean, std, 1)[0]
        return randAffect

    def writeAffect(self, valence, arousal):
        # fileName = "ProgramDataFiles\userEmotionTextDump.txt"
        fileName = "..\\Data_Files\\out_emotionmodelJSON_test.txt"

        ts = self.genUtil.getTimeStamp()
        jsonData = {"timeStamp":ts}
        jsonData["dateTime"] = self.genUtil.getDateTimeFromTime(ts)
        jsonData["valence"] = valence
        jsonData["arousal"] = arousal
        jsonDataStr = json.dumps(jsonData)

        FileUtilitiy.writeTextLine(fileName, jsonDataStr)

    def quit(self):
        self.continueLoop = False

# gu = GenUtil()
# temp = UserAffectGenerator(1, "UA1", 3, gu)
# temp.start()
# time.sleep(20)
# temp.quit()