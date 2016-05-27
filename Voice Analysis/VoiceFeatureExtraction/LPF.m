% Function that puts a noisy signal through a moving-averge LPF
%
% noisySignal - signal with noise
function cleanSignal = LPF(noisySignal)

    windowSize = 30;
    b = (1/windowSize)*ones(1,windowSize);
    a = 1;
    cleanSignal = filter(b, a, noisySignal);
    
end