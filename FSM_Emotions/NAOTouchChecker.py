from naoqi import ALModule
from naoqi import ALProxy
from naoqi import ALBroker
from GenUtil import GenUtil
import time
import sys


class NAOTouchChecker(ALModule):
    def __init__(self, genUtil, ip = "luke.local", port = 9559):
        self.name ="naoTouchChecker"
        ALModule.__init__(self, self.name)
        
        self.genUtil = genUtil
        
        global memory
        memory = ALProxy("ALMemory")
        SubscribeAllTouchEvent()
        
    def __del__(self):
        UnsubscribeAllTouchEvent()
        self.exit()
        
    def onTouched(self, strVarName, value):
        UnsubscribeAllTouchEvent()
        s = "Touch event detected: " + strVarName
        print s
        self.genUtil.naoEmotionalVoiceSay(s)
        SubscribeAllTouchEvent()
        
def SubscribeAllTouchEvent():
    print "Subcribe Touch Events"
    name = "naoTouchChecker"
    onTouch = "onTouched"
    sensors = ["RightBumperPressed", "LeftBumperPressed", "FrontTactilTouched", "MiddleTactilTouched",
               "RearTactilTouched", "HandRightBackTouched", "HandRightLeftTouched", "HandRightRightTouched",
               "HandLeftBackTouched", "HandLeftLeftTouched", "HandLeftRightTouched"]
    for sensor in sensors:
        memory.subscribeToEvent(sensor, name, onTouch)

def UnsubscribeAllTouchEvent():
    print "Unsubcribe Touch Events"
    name = "naoTouchChecker"
    sensors = ["RightBumperPressed", "LeftBumperPressed", "FrontTactilTouched", "MiddleTactilTouched",
               "RearTactilTouched", "HandRightBackTouched", "HandRightLeftTouched", "HandRightRightTouched",
               "HandLeftBackTouched", "HandLeftLeftTouched", "HandLeftRightTouched"]
    for sensor in sensors:
        memory.unsubscribeToEvent(sensor, name)


# NAOip = "luke.local"
# NAOport = 9559
# myBroker = ALBroker("myBroker", "0.0.0.0", 0, NAOip, NAOport)
# global naoTouchChecker
# naoTouchChecker = NAOTouchChecker()
#
# print "Is it present?: ", myBroker.isModulePresent("NAOTouchChecker")
#
# try:
#     while True:
#         print "Waiting..."
#         time.sleep(10)
# except KeyboardInterrupt:
#     print
#     print "Ending"
#     myBroker.shutdown()
#     sys.exit(0)


