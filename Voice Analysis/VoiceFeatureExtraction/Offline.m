clear;

% Parameters
segmentLen = 2;          % in seconds

% Ouput settings
f = fopen('offline_results/data_resized_20ms_window_20ms_step.csv','w');
labelCol = ['Name,Zcr,Energy,EnergyEntropy,SpecCentro,' ...
    'SpecCentroSpread,SpecEntropy,SpecFlux,SpecRolloff,' ... 
    'MFCC1,MFCC2,MFCC3,MFCC4,MFCC5,MFCC6,MFCC7,MFCC8,' ... 
    'MFCC9,MFCC10,MFCC11,MFCC12,MFCC13,MFCC14,MFCC15,MFCC16,' ...
    'MFCC17,MFCC18,MFCC19,MFCC20,HarmonicRatio,F0,' ...
    'CV1,CV2,CV3,CV4,CV5,CV6,CV7,CV8,CV9,CV10,CV11,CV12,' ...
    'LPC1,LPC2,LPC3,LPC4,LPC5,LPC6,LPC7,LPC8,LPC9,LPC10,' ...
    'LPC11,LPC12,LPC13,LPC14,LPC15,LPC16\n'];
fprintf(f,labelCol);

files = dir('training/*.wav');
for file = files'
    disp('Working on:');
    disp(file.name);
    [y,Fs] = audioread(strcat('training/',file.name));
    samplesLeft = length(y);

    segmentNum = 0;
    
    while samplesLeft > Fs*segmentLen
        segment = y(segmentNum*Fs*segmentLen+1:(segmentNum+1)*Fs*segmentLen);    
        segmentNum = segmentNum+1;
        samplesLeft = samplesLeft-Fs*segmentLen;
        clean = LPF(segment);
        
%         figure;
%         t=0:1/Fs:(length(segment)-1)/Fs;
%         plot(t, segment, t, clean);

        features = stFeatureExtraction(clean, Fs, 0.02, 0.02);
        features = mean(features');
        allOneString = sprintf('%.0f,' , features);
        s = strcat(file.name, ',',allOneString,'\n');
        fprintf(f,s);
    end
    
end

fclose(f);


