public static void miniMaxSum(List<int> arr){
    long totalSum=arr.Sum(a=>(long)a);
    int min=arr.Min();
    int max=arr.Max();
    long minSum=totalSum-max;
    long maxSum=totalSum-min;
    Console.WriteLine($"{minSum} {maxSum}");
}