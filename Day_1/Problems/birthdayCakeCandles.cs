public static int birthdayCakeCandles(List<int> candles){
    int max=candles.Max();
    int count=candles.Count(c=>c==max);
    return count;
}