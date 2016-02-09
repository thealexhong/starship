::java -cp weka.jar weka.classifiers.trees.RandomForest -T FeatureVectorValence.arff -l v_2s_type5_randomForest.model -p 0 > results.txt
::java -cp weka.jar weka.classifiers.meta.AdaBoostM1 -T FeatureVectorValence.arff -l v_2s_type5_ADABoostRandomForest.model -p 0 > results.txt
::java -cp weka.jar weka.classifiers.meta.ClassificationViaRegression -T FeatureVectorValence.arff -l v_2s_type5_classificationViaRegression.model -p 0 > results.txt
::java -cp weka.jar weka.classifiers.meta.AdaBoostM1 -T FeatureVectorArousal.arff -l a_2s_type5_ADABoostRandomForest.model -p 0 > results.txt
java -cp weka.jar weka.classifiers.functions.Logistic -T FeatureVectorArousal.arff -l a_2s_type5_logistic.model -p 0 > results.txt