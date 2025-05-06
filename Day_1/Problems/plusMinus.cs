public static void plusMinus(List<int> arr){
    int pCount=0,nCount=0,zCount=0;
    int n=arr.Count;
    foreach(int num in arr){
        if(num>0){
            pCount++;
        }
        else if(num<0){
            nCount++;
        }
        else{
            zCount++;
        }
    }
    Console.WriteLine($"{(float)pCount/n:F6}");
    Console.WriteLine($"{(float)nCount/n:F6}");
    Console.WriteLine($"{(float)zCount/n:F6}");
}