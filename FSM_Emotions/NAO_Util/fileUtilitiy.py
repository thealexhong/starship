# fileUtility.py



# fileLines = fileUtilitiy.readTextLines('inputText.txt')
def readTextLines(textFile):
    f = open(textFile, 'r')

    fileLines = []

    readLine = f.readline()
    while (readLine != ''):
        # readLine.replace("\\n", "")
        fileLines.append(readLine)
        readLine = f.readline()

    f.close()
    return fileLines