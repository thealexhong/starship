function recObject = audioRecorderOnline

timerPeriod = 2; % Adjusted so capture 2 seconds of samples
recObject = audiorecorder(11025, 16, 1);

set(recObject, 'TimerFcn', @audioRecorderTimerCallback, 'TimerPeriod', timerPeriod);
record(recObject);

end