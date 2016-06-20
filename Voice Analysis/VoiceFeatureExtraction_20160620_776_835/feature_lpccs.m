function LPC = feature_lpccs(windowFFT)

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

LPC = lpc(windowFFT, 16);