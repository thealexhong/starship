% Function that puts a noisy signal through a moving-averge LPF
%
% noisySignal - signal with noise
function [amp_var, pitch_var, pitch_lvl, pitch_con, MFCCs] = feature_extraction(signal, Fs)

    % Amplitude variation
    [u, s] = normfit(signal);
    sigma = num2str(s);
    mu = num2str(u);
    amp_var = s^2;
    disp(['Sigma = ' sigma])
    disp(['Mu = ' mu])
    
    % Pitch level
    c = spCepstrum(signal, Fs, 'hamming', 'plot');
    f0 = spPitchCepstrum(c, Fs);
    pitch_lvl = f0;
    
    % Pitch variation
    [F0, T, C] = spPitchTrackCepstrum(signal, Fs, 30, 15, 'hamming', 'plot');
    pitch_var = var(F0);
    
    % Tempo
    %[tempo, meterNum, meterDenom] = musicMeterTempoInduction(signal,Fs,2,1,1,0.01,0.2,1);
    
    % Pitch contour [Up/neutral/down]
    pitch_con = 0;
    R = corrcoef(F0, T);
    coeff = R(1:2);
    if any(coeff == 0.4:1.0)
            pitch_con = 1;
    elseif any(coeff == -0.39:0.39)
            pitch_con = 0;
    elseif any(coeff == -1.0:-0.4)
            pitch_con = -1;
    end
    
    % MFCC
    Tw = 25;                % analysis frame duration (ms)
    Ts = 10;                % analysis frame shift (ms)
    alpha = 0.97;           % preemphasis coefficient
    M = 28;                 % number of filterbank channels 
    C = 16;                 % number of cepstral coefficients
    L = 22;                 % cepstral sine lifter parameter
    LF = 300;               % lower frequency limit (Hz)
    HF = 3700;              % upper frequency limit (Hz)
    
    [ MFCCs, FBEs, frames ] = ...
                    mfcc( signal, Fs, Tw, Ts, alpha, @hamming, [LF HF], M, C+1, L );
    
    
end