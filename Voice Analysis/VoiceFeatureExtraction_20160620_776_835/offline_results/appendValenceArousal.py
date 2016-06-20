config = "old_optimized_20ms_window_20ms_step"

with open('data_' + config + '.csv') as f:
    lines = f.read().splitlines()

del lines[0]

words = [None]*len(lines)
appendedlines = [None]*len(lines)

fv_v = open("data_" + config + "_valence.arff", "wb")
fv_a = open("data_" + config + "_arousal.arff", "wb")

# Appending valence values

fv_v.write("""@relation valence

@attribute Zcr numeric
@attribute Energy numeric
@attribute EnergyEntropy numeric
@attribute SpectralCentroid numeric
@attribute SpectralCentroidSpread numeric
@attribute SpectralEntropy numeric
@attribute SpectralFlux numeric
@attribute SpectralRollOff numeric
@attribute MFCC1 numeric
@attribute MFCC2 numeric
@attribute MFCC3 numeric
@attribute MFCC4 numeric
@attribute MFCC5 numeric
@attribute MFCC6 numeric
@attribute MFCC7 numeric
@attribute MFCC8 numeric
@attribute MFCC9 numeric
@attribute MFCC10 numeric
@attribute MFCC11 numeric
@attribute MFCC12 numeric
@attribute MFCC13 numeric
@attribute MFCC14 numeric
@attribute MFCC15 numeric
@attribute MFCC16 numeric
@attribute MFCC17 numeric
@attribute MFCC18 numeric
@attribute MFCC19 numeric
@attribute MFCC20 numeric
@attribute HarmonicRatio numeric
@attribute F0 numeric
@attribute ChromaVector1 numeric
@attribute ChromaVector2 numeric
@attribute ChromaVector3 numeric
@attribute ChromaVector4 numeric
@attribute ChromaVector5 numeric
@attribute ChromaVector6 numeric
@attribute ChromaVector7 numeric
@attribute ChromaVector8 numeric
@attribute ChromaVector9 numeric
@attribute ChromaVector10 numeric
@attribute ChromaVector11 numeric
@attribute ChromaVector12 numeric
@attribute LPC1 numeric
@attribute LPC2 numeric
@attribute LPC3 numeric
@attribute LPC4 numeric
@attribute LPC5 numeric
@attribute LPC6 numeric
@attribute LPC7 numeric
@attribute LPC8 numeric
@attribute LPC9 numeric
@attribute LPC10 numeric
@attribute LPC11 numeric
@attribute LPC12 numeric
@attribute LPC13 numeric
@attribute LPC14 numeric
@attribute LPC15 numeric
@attribute LPC16 numeric
@attribute valence {-2,-1,0,1,2}

@data

""")
for i in range(len(lines)):
    words[i] = lines[i].split(",")
    #
    #if len(words[i]) > 18:
    #    del words[i][-1]
    if "angry" in words[i][0]:
        words[i].append("-1")
    elif "disgust" in words[i][0]:
        words[i].append("-2")
    elif "sad" in words[i][0]:
        words[i].append("-2")
    elif "happy" in words[i][0]:
        words[i].append("2")
    elif "excited" in words[i][0]:
        words[i].append("2")
    elif "boredom" in words[i][0]:
        words[i].append("-1")
    elif "interest" in words[i][0]:
        words[i].append("1")
    elif "surprise" in words[i][0]:
        words[i].append("0")
    elif "fear" in words[i][0]:
        words[i].append("0")
    words[i].pop(0)
    appendedlines[i] = ','.join(map(str, words[i])) 
    fv_v.write(appendedlines[i] + "\n")

# Appending arousal values

fv_a.write("""@relation arousal

@attribute Zcr numeric
@attribute Energy numeric
@attribute EnergyEntropy numeric
@attribute SpectralCentroid numeric
@attribute SpectralCentroidSpread numeric
@attribute SpectralEntropy numeric
@attribute SpectralFlux numeric
@attribute SpectralRollOff numeric
@attribute MFCC1 numeric
@attribute MFCC2 numeric
@attribute MFCC3 numeric
@attribute MFCC4 numeric
@attribute MFCC5 numeric
@attribute MFCC6 numeric
@attribute MFCC7 numeric
@attribute MFCC8 numeric
@attribute MFCC9 numeric
@attribute MFCC10 numeric
@attribute MFCC11 numeric
@attribute MFCC12 numeric
@attribute MFCC13 numeric
@attribute MFCC14 numeric
@attribute MFCC15 numeric
@attribute MFCC16 numeric
@attribute MFCC17 numeric
@attribute MFCC18 numeric
@attribute MFCC19 numeric
@attribute MFCC20 numeric
@attribute HarmonicRatio numeric
@attribute F0 numeric
@attribute ChromaVector1 numeric
@attribute ChromaVector2 numeric
@attribute ChromaVector3 numeric
@attribute ChromaVector4 numeric
@attribute ChromaVector5 numeric
@attribute ChromaVector6 numeric
@attribute ChromaVector7 numeric
@attribute ChromaVector8 numeric
@attribute ChromaVector9 numeric
@attribute ChromaVector10 numeric
@attribute ChromaVector11 numeric
@attribute ChromaVector12 numeric
@attribute LPC1 numeric
@attribute LPC2 numeric
@attribute LPC3 numeric
@attribute LPC4 numeric
@attribute LPC5 numeric
@attribute LPC6 numeric
@attribute LPC7 numeric
@attribute LPC8 numeric
@attribute LPC9 numeric
@attribute LPC10 numeric
@attribute LPC11 numeric
@attribute LPC12 numeric
@attribute LPC13 numeric
@attribute LPC14 numeric
@attribute LPC15 numeric
@attribute LPC16 numeric
@attribute arousal {-2,-1,0,1,2}

@data

""")
for i in range(len(lines)):
    words[i] = lines[i].split(",")
    if "angry" in words[i][0]:
        words[i].append("2")
    elif "disgust" in words[i][0]:
        words[i].append("1")
    elif "sad" in words[i][0]:
        words[i].append("-1")
    elif "happy" in words[i][0]:
        words[i].append("0")
    elif "excited" in words[i][0]:
        words[i].append("2")
    elif "boredom" in words[i][0]:
        words[i].append("-2")
    elif "interest" in words[i][0]:
        words[i].append("0")
    elif "surprise" in words[i][0]:
        words[i].append("2")
    elif "fear" in words[i][0]:
        words[i].append("2")
    words[i].pop(0)
    appendedlines[i] = ','.join(map(str, words[i])) 
    fv_a.write(appendedlines[i] + "\n")

fv_v.close()
fv_a.close()
