function v_normalized = normalize(v, min, max)

% This function calculates the zero crossing rate of an audio frame.
% 
% ARGUMENTS:
% - window: 	an array that contains the audio samples of the input frame
% 
% RETURN:
% - Z:		the computed zero crossing rate value
%

    v_normalized = (v - min)/(max - min);
end