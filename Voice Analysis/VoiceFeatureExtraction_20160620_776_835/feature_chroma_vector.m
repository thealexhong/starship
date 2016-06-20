function [C, y, c] = feature_chroma_vector(x_in, Fs)

% [C, y, c]=feature_chroma_vector(x_in,Fs,winlength,step)

% ARGUMENTS:
%  - x_in:          input window (1D vector) 
%  - Fs:            sampling frequency

% RETURNS:
%  - C:             y ./ c (see below)
%  - y:             sequence of chroma vectors. Each bin of each chroma 
%                   vector is a sum of FFT amplitudes
%  - c:             sequence vectors that indicates, for each chroma 
%                   vector, the number of  Fourier coefs that take part 
%                   in the respective bin. This is useful when it comes to 
%                   calculating the man value at each bin
%
% (c) 2014 T. Giannakopoulos, A. Pikrakis


x_in = x_in / max(abs(x_in));
tone_analysis=12;
num_of_bins=12;

[mm,nn]=size(x_in);
if nn>1
    x_in=x_in';
end

l=1;
y=[];
c=[];
lengthx=length(x_in);
winlength = lengthx;
freqs=0:Fs/winlength:(floor(winlength/2)-1)*(Fs/winlength);
f0=55;
i=0;
while (1) % define the chromatic scale on the frequency axis
    f(i+1)=f0*2^(i/tone_analysis);
    if f(i+1)>freqs(length(freqs))
        f(i+1)=[];
        break
    end
    i=i+1;
end

time_vector=[];
    
    x = x_in;      
    fftMag=abs(fft(x))';
    fftMag=fftMag(1:floor(winlength/2));
    
    the_max=max(fftMag); %checking for very low-energ frames
    if the_max<=eps
        ytemp=zeros(num_of_bins,1);
        y=[ytemp];
    end
        
    dfind=find(freqs<f(1) | freqs>2000);
    fftMag(dfind)=zeros(1,length(dfind));    

    %Keep spectral PEAKS ONLY (can be omitted)
    c1=fftMag-[0 fftMag(1:length(fftMag)-1)];
    c2=[fftMag]-[fftMag(2:length(fftMag)) 0];
    dfind=find(~(c1>0 & c2>0));
    fftMag(dfind)=zeros(1,length(dfind));          
  
    nonzero=find(fftMag>0);
    if isempty(nonzero)
        y=zeros(num_of_bins,1);
    end
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    

    ytemp=zeros(num_of_bins,1);
    ctemp=zeros(num_of_bins,1);
    for k=1:length(nonzero)
        temp=freqs(nonzero(k));
        %N=hist(temp,f);                
        %pitch_class=find(N==1);
        [MIN, IMIN] = min(abs(temp-f));
        pitch_class = IMIN;
        h=rem(pitch_class,num_of_bins);
        if h==0
            h=num_of_bins;
        end
        ytemp(h)=ytemp(h)+fftMag(nonzero(k));
        ctemp(h)=ctemp(h)+1;
    end
    
    y=ytemp;
    c=ctemp;

[K1,L1] = size(y);
[K2,L2] = size(c);
if (L1~=L2)
    c = imresize(c,size(y));
end
C = y./(c+1);
% disp('before');
% disp(C);
% C(1) = normalize(C(1), 0.2, 4.5);
% C(2) = normalize(C(2), 0.07, 1);
% C(3) = normalize(C(3), 0.58, 10);
% C(4) = normalize(C(4), 0.01, 0.3);
% C(5) = normalize(C(5), 0.1, 6.5);
% C(6) = normalize(C(6), 0.5, 14);
% C(7) = normalize(C(7), 0.02, 0.5);
% C(8) = normalize(C(8), 0.1, 3.2);
% C(9) = normalize(C(9), 0.05, 3.4);
% C(10) = normalize(C(10), 0.03, 0.7);
% C(11) = normalize(C(11), 0.1, 16);
% C(12) = normalize(C(12), 0.05, 1.8);
% disp('after');
% disp(C);

% C = C * 100;
