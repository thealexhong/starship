#Starship

An architecture for affective human-robot interaction (HRI) on the NAO robots (Luke and Leia). The architecture consist of a multi-modal affect recognition system to recognize arousal and valence levels from vocal intonation and body language, and an emotional response behaviour model for HRI.

##External Libraries and Requirements

* Windows OS
* Kinect SDK 1.8 (Uses Kinect 1)
* OpenCV
* Weka
* Java Runtime Environment
* Nemesysco QA5 SDK
* NAOqi SDK
* Python 2.X
* Visual Studio

##Folder Structure

* Data_Files

Output files of FSM_Emotions.

* David Stuff

Tianhao Hu's personal files. He was responsible for controlling the Nao robot's affect display (body language, eye color, and vocal intonation).

* FSM_Emotions

Nolan's Emotional model, Wizard of Oz, and graph generation programs. Raw results of experiments are located in 'FSM_Emotions/ProgramDataFiles/'. Run main.py to start Wizard of Oz.

* KinectDevSamples

Provided by Kinect library. Use this to test if the Kinect is properly installed on your machine, and that it works.

* NAO Questionnaire

End of experiment questionnaire developed by Nolan.

* Results

A few result files from the experiments.

* Voice Analysis

Vocal intonation affect recognition developed by Yuma. Contains 2 methods of measuring: (1) using 3rd party software Nemesysco based on peaks and plateaus of sound signals, and (2) using voice features found in literature via MATLAB Audio Library functions.

* Docs

Misc. documents for reference.

* starship

Alex's GUI for affect recognition. Running this VS solution will enable body language recognition. Run vocal intonation affect recognition in parallel for multimodal affect recognition output. Voice affect is read from 'startship/Voice Analysis/voiceOutput.txt'. Training data for body language is located inside 'starship/TrainingData/'. 'starship/batFiles' contains windows script for running Weka in the background for classification - these will change automatically.
