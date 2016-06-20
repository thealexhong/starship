clear;

% Parameters
segmentLen = 2;          % in seconds

files = dir('training/*_happy.wav');

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

%         [lpc, lpcc] = feature_lpccs(clean);
%         lpc = feature_lpccs(clean);
%         disp(lpc);
%         disp(lpcc);
%         zcr = feature_zcr(clean)
cv = feature_chroma_vector(clean, Fs);
%         disp(strcat('ZCR: ',num2str(zcr)));
%         stFeatureExtraction(clean, 11025, 0.1, 0.1);
%         mfccParams = feature_mfccs_init(windowLength, fs);
%         mfcc = feature_mfccs(clean, mfccParams);
%         disp(mfcc);
    end
    
end

