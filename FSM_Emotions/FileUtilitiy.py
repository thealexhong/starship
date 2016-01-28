# FileUtility.py
import os.path
import json
import subprocess

# fileLines = fileUtilitiy.readTextLines('inputText.txt')
def readTextLines(textFile):
	fileLines = []
	try:
		if os.path.isfile(textFile):
			f = open(textFile, 'r')
		else:
			f = open(textFile.replace('\\', '/'), 'r')

		readLine = f.readline()
		while (readLine != ''):
	
			# readLine.replace("\\n", ""
			fileLines.append(readLine)
			readLine = f.readline()

		f.close()
	except Exception as e:
		print "Reading from file '" + textFile + "' Failed with error:"
		print e

	return fileLines

def writeTextLine(textFile, lineText):
	try:
		if os.path.isfile(textFile):
			f = open(textFile, 'a')
		else:
			f = open(textFile.replace('\\', '/'), 'a')
		s = "\n" + str(lineText)
		f.write(s)
		f.close()
	except Exception, e:
		print "Writing to file '" + textFile + "' Failed with error:"
		print e

def readLinesToJSON(textFile):
	fileLines = readTextLines(textFile)

	linesJson = []
	for line in fileLines:
		# print "Line: ", line
		if not (line == "" or line == "\n"):
			jsLine = json.loads(line)
			linesJson.append(jsLine)

	return linesJson

def checkFileExists(fileName):
	fWin = fileName
	funi = fileName.replace('\\', '/')
	return (os.path.isfile(fWin) or os.path.isfile(funi))

def makeFolder(folderName):
	if not checkFileExists(folderName):
		try:
			os.makedirs(folderName)
		except:
			pass
			# file exisits?


def hitEnterOnConsole():
	fileName = "ScriptFiles\HitEnter.vbs"
	if checkFileExists(fileName):
		print "Hitting Enter"
		subprocess.call("cscript " + fileName)
	else:
		print "Did not hit enter"
