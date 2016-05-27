function plp = feature_plps(windowFFT, Fs)

% This function computes the mfccs using the provided DFT.
% The parameters (DCT, filter banks, etc) need to have been 
% computed using the feature_mfccs_init function.
%
% ARGUMENTS:
% - windowFFT:       	the abs(FFT) of an audio frame
%                    	(computed by getDFT() function)
%
% RETURNS:
% - lpc:	
% - lpcc:

% Calculate Feacalc-style PLPs

plp = melfcc(windowFFT, Fs, 'wintime', 0.05, 'hoptime', 0.05, 'lifterexp', 0.6, 'nbands', 21, ...
      'dcttype', 1, 'maxfreq', 8000, 'fbtype', 'bark', 'preemph', 0, ...
      'numcep', 13, 'modelorder', 12, 'usecmp', 1);
  
disp(plp);