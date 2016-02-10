from EdgeDetection import EdgeDetection
from BasicMotions import BasicMotions
from naoqi import ALBroker
from naoqi import ALModule
import cv2
import random
import sys
import time

# import fileUtilitiy

def main(NAOip, NAOport):
    print "OpenCV Version:", cv2.__version__
    myBroker = ALBroker("myBroker",
                        "0.0.0.0",   # listen to anyone
                        0,           # find a free port and use it
                        NAOip,          # parent broker IP
                        NAOport)        # parent broker port
    global BasicMotions
    BasicMotions = BasicMotions(NAOip, NAOport, myBroker)
    EdgeDetector = EdgeDetection(NAOip, NAOport)
    user_input = "Start"
    while user_input != "end":
        user_input = raw_input("Enter a command: ")
        user_input = user_input.lower()
        print(user_input)
        if user_input == "speak":
            BasicMotions.naoSay("Oh My Gawd, I can Speak!")
        elif user_input == "sit":
            BasicMotions.naoSit()
        elif user_input == "stand":
            BasicMotions.naoStand()
        elif user_input == "standinit":
            BasicMotions.naoStandInit()
        elif user_input == "walk":
            BasicMotions.naoWalk(0.5, 0.4)
        elif user_input == "nod":
            BasicMotions.naoNodHead()
        elif user_input == "shake head":
            BasicMotions.naoShadeHead()
        elif user_input == "wave right":
            BasicMotions.naoWaveRight()
        elif user_input == "wave both":
            BasicMotions.naoWaveBoth()
        elif user_input == "rest":
            BasicMotions.naoRest()
        #=======EMOTION DISPLAY=============
        elif user_input == "happymotion":
            time.sleep(4)
            BasicMotions.happyEmotion()
        elif user_input == "sadmotion":
            time.sleep(4)
            BasicMotions.sadEmotion()
        elif user_input == "scared1motion":
            time.sleep(4)
            BasicMotions.scaredEmotion1()
        elif user_input == "scared2motion":
            time.sleep(4)
            BasicMotions.scaredEmotion2()
        elif user_input == "fear1motion":
            time.sleep(4)
            BasicMotions.fear1Emotion()
        elif user_input == "fear2motion":
            time.sleep(4)
            BasicMotions.fear2Emotion()
        elif user_input == "hope1motion":
            time.sleep(4)
            BasicMotions.hope1Emotion()
        elif user_input == "hope2motion":
            time.sleep(4)
            BasicMotions.hope2Emotion()
        elif user_input == "angermotion":
            time.sleep(4)
            BasicMotions.angerEmotion()
        elif user_input == "lookmotion":
            time.sleep(4)
            BasicMotions.LookAtEdgeEmotion()
        elif user_input == "edgemotion":
            time.sleep(4)
            BasicMotions.scaredEmotion3Edge()

        #=======VOICE EFFECT=============
        elif user_input == "happy":
            BasicMotions.naoSayHappy(BasicMotions.HriDialogEOD['1good'])
            #BasicMotions.happyEmotion()
        elif user_input == "sad":
            BasicMotions.naoSaySad(BasicMotions.HriDialogEOD['1bad'])
            #BasicMotions.sadEmotion()
        elif user_input == "scared":
            BasicMotions.naoSayScared(BasicMotions.HriDialogEOD['31nono'])
            #BasicMotions.scaredEmotion1()
        elif user_input == "fear":
            BasicMotions.naoSayFear(BasicMotions.HriDialogEOD['31nono'])
            #BasicMotions.fearEmotion()
        elif user_input == "hope":
            BasicMotions.naoSayHope(BasicMotions.HriDialogEOD['2yes'])
            #BasicMotions.hopeEmotion()
        elif user_input == "anger":
           BasicMotions.naoSayAnger(BasicMotions.HriDialogEOD['31nono'])
           #BasicMotions.angerEmotion()
        #=======EYE DISPLAY=============
        elif user_input == "happyeye":
            BasicMotions.setEyeEmotion('happy')
        elif user_input == "sadeye":
            BasicMotions.setEyeEmotion('sad')
        elif user_input == "scared1eye":
            BasicMotions.setEyeEmotion('scared1')
        elif user_input == "scared2eye":
            BasicMotions.setEyeEmotion('scared2')
        elif user_input == "feareye":
            BasicMotions.setEyeEmotion('fear')
        elif user_input == "hopeeye":
            BasicMotions.setEyeEmotion('hope')
        elif user_input == "angereye":
            BasicMotions.setEyeEmotion('anger')
        #=======TESTING INTERNAL FUNCTIONS=======
        elif user_input == "blinktop":
            BasicMotions.blinkEyes(random.randint(0x00000000, 0x00FFFFFF), 5.0, "EyeTop")
        elif user_input == "blinkbottom":
            BasicMotions.blinkEyes(random.randint(0x00000000, 0x00FFFFFF), 5.0, "EyeBottom")
        elif user_input == "blinkfull":
            BasicMotions.blinkEyes(random.randint(0x00000000, 0x00FFFFFF), 5.0, "EyeFull")
        elif user_input == "eyecolor":
            BasicMotions.setEyeColor(0x00FFFFFF,"LedEyeTop")
            BasicMotions.setEyeColor(0x00FF00FF,"LedEyeCorner")
            BasicMotions.setEyeColor(0x00FF00FF,"LedEyeBottom")
        #==========TESTING CAMERA============
        elif user_input == "img":
            EdgeDetector.SetDebugMode(True)
            i=0
            BasicMotions.LookAtEdgeEmotion()
            while True:
                status, distance, angle = EdgeDetector.lookForEdge(20,4)
                print "Edge: ", status,distance,angle
                if status==True:
                    BasicMotions. scaredEmotion3Edge()
                    break
                i+=1
                if i>100:
                    print "Not close enough"
                    break


        #============================================
        elif user_input != "end":
            print("That is not an availiable command")

    print("=== Main Program Finished Running ===")

if __name__ == '__main__':
    #NAOIP = "127.0.0.1"
    #NAOPort = 58357

    #NAOIP = "192.168.1.135"

    NAOIP = "Luke.local"
    NAOPort = 9559



    print("Initiated Values")
    main(NAOIP, NAOPort)