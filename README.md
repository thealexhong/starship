# Starship

An architecture for affective human-robot interaction (HRI) on the NAO robots (Luke and Leia). The architecture consist of a multi-modal affect recognition system to recognize arousal and valence levels from vocal intonation and body language, and an emotional response behaviour model for HRI.

## External Libraries and Requirements

* Windows OS
* Kinect SDK 1.8 (Uses Kinect 1)
* OpenCV
* Weka
* Java Runtime Environment
* Nemesysco QA5 SDK
* NAOqi SDK
* Python 2.X
* Visual Studio

## Running Starship

1. Connect the Kinect, Microphone, and router to the computer. The router is used to communicate with the Nao robot.

2. Open `starship > starship.sln`. This should work with VS Community (free to download). Build and run the solution. If everything is working, you should see a 2D and 3D camera view of the kinect on the starship GUI. You can walk in front of the Kinect to test affective dynamic body language recognition. All log files are written to `starship/logs/` directory for analysis.

3. For multimodal recognition, starship expects input from a seperate voice program. Open `Voice Analysis/Real-time Recognition/nmsVoiceAnalysisRun/nmsVoiceAnalysisRun.sln` in VS 2008. The current state of the program only works in VS 2008. Run the solution. If everything is working, you should see voice affect outputs from starship. starship will also calculate the multimodal output, based on a previous trained model (`starship/TrainingData/`).

4. For human-robot interaction, starship outputs a log for another program to read to control the robot. Run `starship/FSM_Emotions/main.py` in PyCharm. The robot should move, and interact with a human. The robot uses Wizard of Oz for speech recognition to interact.

5. To save results, copy log files from starship, and `starship/FSM_Emotions/ProgramDataFiles/ConsoleOutput` to `starship/FSM_Emotions/ProgramDataFiles/<new user>`. The data needs to be processed before a graph is generated. Data should be aligned with when the robot reads the emotion from human so there's no continuous fluctuations in emotion. Copy a make_plots.py from another user and change variables to produce graphs.

```
affectFileName = "13 Jacob End of day.csv"
robotFileName = "13_Jacob_Flow_2016-02-18_16-32-26.csv"
```

## Folder Structure

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

Important Files to note inside starship folder:

1. BLFeatureCalculator: Calculates body language features based off Derek's previous dynamic body language work.

2. NuiStreamViewer: Creates .bat files for running Weka, parameter ML files for weka to use, specify which ML algorithm to use, draws skeleton onto the body, and the main controller to write the final results to both GUI and csv file.
