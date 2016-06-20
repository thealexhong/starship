function audioRecorderTimerCallback(obj, event)
    %
    % function audioRecorderTimerCallback(obj, event)
    %
    % This is the callback used by the audiorecorder object
    %    
    
    MAX_SEGMENTS_TO_RECORD = 50;
    Fs           = get(obj, 'SampleRate');
    num_channels = get(obj, 'NumberOfChannels');
    num_bits     = get(obj, 'BitsPerSample');    
    TimerPeriod  = get(obj, 'TimerPeriod');
    
    if length(get(obj, 'Tag'))==0
        set(obj, 'Tag', '0')
    end
    set(obj, 'Tag', num2str(str2num(get(obj, 'Tag')) + 1));
    N = str2num(get(obj, 'Tag')); 
    disp(strcat(num2str(N),'th segment'));
    if N>=MAX_SEGMENTS_TO_RECORD
        stop(obj)
        close
        return
    end    
    try
        % Do stuff here
        stop(obj);
        data = getaudiodata(obj);  
        disp(strcat('Segment length: ', num2str(length(data))));
        filtered_signal = LPF(data(1:22025));
        rtFeatures = stFeatureExtraction(filtered_signal, Fs, 0.02, 0.02);
        rtFeatures = mean(rtFeatures');
        rtFeatures = normalize_ft_matrix(rtFeatures);
        rtFeatures = rtFeatures(:,1:end-1);
        createRealTimeARFF(rtFeatures);
        s = system('python classify.py');
        drawnow;
        record(obj);              
    catch
        % Stop the recorder and exit
        stop(obj)
        rethrow(lasterror)        
    end            
end