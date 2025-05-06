public static List<int> breakingRecords(List<int> scores){
    int minCount=0,maxCount=0;
    int minScore=scores[0];
    int maxScore=scores[0];
    for(int i=1;i<scores.Count;i++){
        if(scores[i]>maxScore){
            maxScore=scores[i];
            maxCount++;
        }
        else if(scores[i]<minScore){
            minScore=scores[i];
            minCount++;
        }
    }
    return new List<int>{maxCount,minCount};
}