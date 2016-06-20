function Features = stFeatureExtraction(signal, fs, win, step)

% function Features = stFeatureExtraction(signal, fs, win, step)
%
% This function computes basic audio feature sequencies for an audio
% signal, on a short-term basis.
%
% ARGUMENTS:
%  - signal:    the audio signal
%  - fs:        the sampling frequency
%  - win:       short-term window size (in seconds)
%  - step:      short-term step (in seconds)
%
% RETURNS:
%  - Features: a [MxN] matrix, where M is the number of features and N is
%  the total number of short-term windows. Each line of the matrix
%  corresponds to a seperate feature sequence
%
% (c) 2014 T. Giannakopoulos, A. Pikrakis

% if STEREO ...
if (size(signal,2)>1)
    signal = (sum(signal,2)/2); % convert to MONO
end

% convert window length and step from seconds to samples:
windowLength = round(win * fs);
step = round(step * fs);

curPos = 1;
L = length(signal);

% compute the total number of frames:
numOfFrames = floor((L-windowLength)/step) + 1;
% disp(strcat('Number of Frames', num2str(numOfFrames)));
% number of features to be computed:
numOfFeatures = 52;
Features = zeros(numOfFeatures, numOfFrames);
Ham = window(@hamming, windowLength);
mfccParams = feature_mfccs_init(windowLength, fs);

for i=1:numOfFrames % for each frame
    % get current frame:
    frame  = signal(curPos:curPos+windowLength-1);
    frame  = frame .* Ham;
    frameFFT = getDFT(frame, fs);
    
    if (sum(abs(frame))>eps)
        
        % compute time-domain features:
        Features(1,i) = feature_zcr(frame);
        Features(2,i) = feature_energy(frame);

        % compute freq-domain features: 
        if (i==1) frameFFTPrev = frameFFT; end;
        [SpecCentroid, SpecCentroidSpread] = ...
            feature_spectral_centroid(frameFFT, fs);
        Features(3,i) = SpecCentroidSpread;
        Features(4,i) = feature_spectral_entropy(frameFFT, 10);
        Features(5,i) = feature_spectral_flux(frameFFT, frameFFTPrev);
        Features(6,i) = feature_spectral_rolloff(frameFFT, 0.90);
        MFCCs = feature_mfccs(frameFFT, mfccParams);
        Features(7:24,i)  = MFCCs(1:18);
        
        [HR, F0] = feature_harmonic(frame, fs);
        Features(25, i) = F0;
        ChromaVector = feature_chroma_vector(frame, fs);
        Features(26:37, i) = ChromaVector;
       
        LPC = feature_lpccs(frame);
        Features(38:52, i) = LPC(2:16);
%         % compute fundamental features
%         Features(1,i) = feature_zcr(frame);
% 
%         if (i==1) frameFFTPrev = frameFFT; end;
%         [SpecCentroid, SpecCentroidSpread] = ...
%             feature_spectral_centroid(frameFFT, fs);
%         Features(2,i) = SpecCentroidSpread;
%         Features(3,i) = feature_spectral_flux(frameFFT, frameFFTPrev);
%         Features(4,i) = feature_spectral_rolloff(frameFFT, 0.90);
%         
%         [HR, F0] = feature_harmonic(frame, fs);
%         Features(5, i) = F0;
%         
%         ChromaVector = feature_chroma_vector(frame, fs);
%         Features(6:17, i) = ChromaVector;
%         MFCCs = feature_mfccs(frameFFT, mfccParams);
%         Features(18:35,i)  = MFCCs(1:18);
%         
%         % compute optimization features: 
%         Features(36,i) = feature_energy(frame);
%         Features(37,i) = feature_spectral_entropy(frameFFT, 10);
%         LPC = feature_lpccs(frame);
%         Features(38:52, i) = LPC(2:16);
        
        
    else
        Features(:,i) = zeros(numOfFeatures, 1);
    end    
    curPos = curPos + step;
    frameFFTPrev = frameFFT;
end
Features(numOfFeatures, :) = medfilt1(Features(numOfFeatures, :), 3);