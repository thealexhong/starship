from naoqi import ALProxy

NAOIP = "leia.local"
NAOPort = 9559

tts = ALProxy("ALTextToSpeech", NAOIP, NAOPort)
audioProxy = ALProxy("ALAudioDevice", NAOIP, NAOPort)
audioProxy.setOutputVolume(100/2)

t = "Hello, I am speaking in some kind of voice right now."

fear = ""
fear += "\\rspd=100\\"
fear += "\\vol=050\\"
fear += "\\vct=60\\"
tts.setParameter("pitchShift", 0)
tts.say(fear + t)

tts.setParameter("doubleVoice", 1)
tts.setParameter("doubleVoiceLevel", 1)
tts.setParameter("doubleVoiceTimeShift", 0.1)
tts.setParameter("pitchShift", 1.1)
tts.say(fear + t)

