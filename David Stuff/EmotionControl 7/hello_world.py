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
    delay = 1
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
        elif user_input == "happy1motion":
            time.sleep(delay)
            BasicMotions.happy1Emotion()
        elif user_input == "happy2motion":
            time.sleep(delay)
            BasicMotions.happy2Emotion()
        elif user_input == "happy3motion":
            time.sleep(delay)
            BasicMotions.happy3Emotion()
        elif user_input == "sadmotion":
            time.sleep(delay)
            BasicMotions.sadEmotion()
        elif user_input == "scared1motion":
            time.sleep(delay)
            BasicMotions.scared1Emotion()
        elif user_input == "scared2motion":
            time.sleep(delay)
            BasicMotions.scared2Emotion()
        elif user_input == "fear1motion":
            time.sleep(delay)
            BasicMotions.fear1Emotion()
        elif user_input == "fear2motion":
            time.sleep(delay)
            BasicMotions.fear2Emotion()
        elif user_input == "hope1motion":
            time.sleep(delay)
            BasicMotions.hope1Emotion()
        elif user_input == "hope2motion":
            time.sleep(delay)
            BasicMotions.hope2Emotion()
        elif user_input == "hope3motion":
            time.sleep(delay)
            BasicMotions.hope3Emotion()
        elif user_input == "hope4motion":
            time.sleep(delay)
            BasicMotions.hope4Emotion()
        elif user_input == "angermotion":
            time.sleep(delay)
            BasicMotions.angerEmotion()
        elif user_input == "lookmotion":
            time.sleep(delay)
            BasicMotions.LookAtEdgeEmotion()
        elif user_input == "edgemotion":
            time.sleep(delay)
            BasicMotions.FoundEdgeEmotion()

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
        elif user_input == "scaredeye":
            BasicMotions.setEyeEmotion('scared')
        elif user_input == "feareye":
            BasicMotions.setEyeEmotion('fear')
        elif user_input == "hopeeye":
            BasicMotions.setEyeEmotion('hope')
        elif user_input == "angereye":
            BasicMotions.setEyeEmotion('anger')
        elif user_input == "alarmingeye":
            BasicMotions.blinkAlarmingEyes(5,0x00FF7F00,"LedEye")
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
                status, distance, angle = EdgeDetector.lookForEdge(20,8)
                print "Edge: ", status,distance,angle
                if status==True:
                    BasicMotions. FoundEdgeEmotion()
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
    NAOIP = "127.0.0.1"
    NAOPort = 49869

    # NAOIP = "Luke.local"

    #NAOIP = "Leia.local"
    #NAOIP = "127.0.0.1"
    # NAOPort = 9559
    #NAOPort = 57089


    print("Initiated Values")
    main(NAOIP, NAOPort)