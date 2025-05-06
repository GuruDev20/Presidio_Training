public static int migratoryBirds(List<int> arr){
    int[] freq=new int[6];
    foreach(int bird in arr){
        freq[bird]++;
    }
    int maxCount=0;
    int result=0;
    for(int i=1;i<=5;i++){
        if(freq[i]>maxCount){
            maxCount=freq[i];
            result=i;
        } 
        else if(freq[i]==maxCount && i<result){
            result=i;
        }
    }
    return result;
}