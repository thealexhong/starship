import subprocess, sys
from time import localtime, strftime

output = subprocess.Popen(["classify.bat"], stdout=subprocess.PIPE).communicate()[0]

output2 = output.split(":")

predict_valence = output2[3].split(" ")[0]
predict_arousal = output2[6].split(" ")[0]

f = open('rt_valence.arff', 'r')
lines = f.read().splitlines();
featurevector = lines[-1].replace("?", "");

f = open('..\\voiceOutput.txt', 'w')
f.write(predict_valence + "," + predict_arousal)
print predict_valence + "," + predict_arousal

f = open('log.csv', 'a')
time = strftime("%Y-%m-%d %H:%M:%S", localtime())
entry = time + "," + featurevector + predict_valence + "," + predict_arousal + "\n"
f.write(entry)
f.close()

sys.exit(0)