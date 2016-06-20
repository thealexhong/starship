function ft_matrix_normalized = normalize_ft_matrix(F)

% % Zcr
% ft_matrix_normalized(1) = normalize(F(1), 0.02, 0.08);
% % Spectral centroid spread
% ft_matrix_normalized(2) = normalize(F(2), 0.08, 0.16);
% % Spectral flux
% ft_matrix_normalized(3) = normalize(F(3), 0.01, 0.055);
% % Spectral rolloff
% ft_matrix_normalized(4) = normalize(F(4), 0.02, 0.11);
% % F0
% ft_matrix_normalized(5) = normalize(F(5), 95, 320);
% % CV
% ft_matrix_normalized(6) = normalize(F(6), 0.2, 4.5);
% ft_matrix_normalized(7) = normalize(F(7), 0.07, 1.0);
% ft_matrix_normalized(8) = normalize(F(8), 0.58, 10);
% ft_matrix_normalized(9) = normalize(F(9), 0.01, 0.3);
% ft_matrix_normalized(10) = normalize(F(10), 0.1, 6.5);
% ft_matrix_normalized(11) = normalize(F(11), 0.5, 14);
% ft_matrix_normalized(12) = normalize(F(12), 0.02, 0.5);
% ft_matrix_normalized(13) = normalize(F(13), 0.1, 3.2);
% ft_matrix_normalized(14) = normalize(F(14), 0.05, 3.4);
% ft_matrix_normalized(15) = normalize(F(15), 0.03, 0.7);
% ft_matrix_normalized(16) = normalize(F(16), 1.1, 16);
% ft_matrix_normalized(17) = normalize(F(17), 0.05, 1.8);
% % MFCC
% ft_matrix_normalized(18) = normalize(F(18), -50, -34);
% ft_matrix_normalized(19) = normalize(F(19), 3, 6);
% ft_matrix_normalized(20) = normalize(F(20), -0.5, 1.6);
% ft_matrix_normalized(21) = normalize(F(21), -0.6, 1.16);
% ft_matrix_normalized(22) = normalize(F(22), -0.7, 0.9);
% ft_matrix_normalized(23) = normalize(F(23), 0.10, 1.1);
% ft_matrix_normalized(24) = normalize(F(24), 0.15, 1.1);
% ft_matrix_normalized(25) = normalize(F(25), -0.2, 0.52);
% ft_matrix_normalized(26) = normalize(F(26), -0.7, 0.13);
% ft_matrix_normalized(27) = normalize(F(27), -1.1, -0.05);
% ft_matrix_normalized(28) = normalize(F(28), -1.2, 0.11);
% ft_matrix_normalized(29) = normalize(F(29), -0.6, 0.6);
% ft_matrix_normalized(30) = normalize(F(30), -0.4, 0.5);
% ft_matrix_normalized(31) = normalize(F(31), -0.6, 0.5);
% ft_matrix_normalized(32) = normalize(F(32), -0.6, 0.5);
% ft_matrix_normalized(33) = normalize(F(33), -0.4, 0.6);
% ft_matrix_normalized(34) = normalize(F(34), -0.5, 0.4);
% ft_matrix_normalized(35) = normalize(F(35), -0.8, 0.07);
% 
% % Energy
% ft_matrix_normalized(36) = normalize(F(36), 0.000035, 0.0022);
% % Spectral entropy
% ft_matrix_normalized(37) = normalize(F(37), 0.01, 0.7);
% % LPC
% ft_matrix_normalized(38) = normalize(F(38), -2.3, -1.3);
% ft_matrix_normalized(39) = normalize(F(39), 0.15, 1.8);
% ft_matrix_normalized(40) = normalize(F(40), -1, 0.28);
% ft_matrix_normalized(41) = normalize(F(41), -0.02, 0.8);
% ft_matrix_normalized(42) = normalize(F(42), -0.5, 0.1);
% ft_matrix_normalized(43) = normalize(F(43), -0.15, 0.5);
% ft_matrix_normalized(44) = normalize(F(44), -0.3, 0.2);
% ft_matrix_normalized(45) = normalize(F(45), -0.15, 0.3);
% ft_matrix_normalized(46) = normalize(F(46), -0.35, 0.6);
% ft_matrix_normalized(47) = normalize(F(47), -0.015, 0.3);
% ft_matrix_normalized(48) = normalize(F(48), -0.35, 0.0);
% ft_matrix_normalized(49) = normalize(F(49), -0.11, 0.25);
% ft_matrix_normalized(50) = normalize(F(50), -0.2, 0.15);
% ft_matrix_normalized(51) = normalize(F(51), -0.12, 0.16);
% ft_matrix_normalized(52) = normalize(F(52), -0.17, 0.8);

% Zcr
ft_matrix_normalized(1) = normalize(F(1), 0.02, 0.08);
% Energy
ft_matrix_normalized(2) = normalize(F(2), 0.000035, 0.0022);
% Spectral centroid spread
ft_matrix_normalized(3) = normalize(F(3), 0.08, 0.16);
% Spectral entropy
ft_matrix_normalized(4) = normalize(F(4), 0.01, 0.7);
% Spectral flux
ft_matrix_normalized(5) = normalize(F(5), 0.01, 0.055);
% Spectral rolloff
ft_matrix_normalized(6) = normalize(F(6), 0.02, 0.11);
% MFCC
ft_matrix_normalized(7) = normalize(F(7), -50, -34);
ft_matrix_normalized(8) = normalize(F(8), 3, 6);
ft_matrix_normalized(9) = normalize(F(9), -0.5, 1.6);
ft_matrix_normalized(10) = normalize(F(10), -0.6, 1.16);
ft_matrix_normalized(11) = normalize(F(11), -0.7, 0.9);
ft_matrix_normalized(12) = normalize(F(12), 0.10, 1.1);
ft_matrix_normalized(13) = normalize(F(13), 0.15, 1.1);
ft_matrix_normalized(14) = normalize(F(14), -0.2, 0.52);
ft_matrix_normalized(15) = normalize(F(15), -0.7, 0.13);
ft_matrix_normalized(16) = normalize(F(16), -1.1, -0.05);
ft_matrix_normalized(17) = normalize(F(17), -1.2, 0.11);
ft_matrix_normalized(18) = normalize(F(18), -0.6, 0.6);
ft_matrix_normalized(19) = normalize(F(19), -0.4, 0.5);
ft_matrix_normalized(20) = normalize(F(20), -0.6, 0.5);
ft_matrix_normalized(21) = normalize(F(21), -0.6, 0.5);
ft_matrix_normalized(22) = normalize(F(22), -0.4, 0.6);
ft_matrix_normalized(23) = normalize(F(23), -0.5, 0.4);
ft_matrix_normalized(24) = normalize(F(24), -0.8, 0.07);
% F0
ft_matrix_normalized(25) = normalize(F(25), 95, 320);
% CV
ft_matrix_normalized(26) = normalize(F(26), 0.2, 4.5);
ft_matrix_normalized(27) = normalize(F(27), 0.07, 1.0);
ft_matrix_normalized(28) = normalize(F(28), 0.58, 10);
ft_matrix_normalized(29) = normalize(F(29), 0.01, 0.3);
ft_matrix_normalized(30) = normalize(F(30), 0.1, 6.5);
ft_matrix_normalized(31) = normalize(F(31), 0.5, 14);
ft_matrix_normalized(32) = normalize(F(32), 0.02, 0.5);
ft_matrix_normalized(33) = normalize(F(33), 0.1, 3.2);
ft_matrix_normalized(34) = normalize(F(34), 0.05, 3.4);
ft_matrix_normalized(35) = normalize(F(35), 0.03, 0.7);
ft_matrix_normalized(36) = normalize(F(36), 1.1, 16);
ft_matrix_normalized(37) = normalize(F(37), 0.05, 1.8);
% LPC
ft_matrix_normalized(38) = normalize(F(38), -2.3, -1.3);
ft_matrix_normalized(39) = normalize(F(39), 0.15, 1.8);
ft_matrix_normalized(40) = normalize(F(40), -1, 0.28);
ft_matrix_normalized(41) = normalize(F(41), -0.02, 0.8);
ft_matrix_normalized(42) = normalize(F(42), -0.5, 0.1);
ft_matrix_normalized(43) = normalize(F(43), -0.15, 0.5);
ft_matrix_normalized(44) = normalize(F(44), -0.3, 0.2);
ft_matrix_normalized(45) = normalize(F(45), -0.15, 0.3);
ft_matrix_normalized(46) = normalize(F(46), -0.35, 0.6);
ft_matrix_normalized(47) = normalize(F(47), -0.015, 0.3);
ft_matrix_normalized(48) = normalize(F(48), -0.35, 0.0);
ft_matrix_normalized(49) = normalize(F(49), -0.11, 0.25);
ft_matrix_normalized(50) = normalize(F(50), -0.2, 0.15);
ft_matrix_normalized(51) = normalize(F(51), -0.12, 0.16);
ft_matrix_normalized(52) = normalize(F(52), -0.17, 0.8);
end