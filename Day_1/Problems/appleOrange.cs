public static void countApplesAndOranges(int s, int t, int a, int b, List<int> apples, List<int> oranges){
    int applesHouse=0;
    int orangesHouse=0;
    foreach(int apple in apples){
        int applePosition=a+apple;
        if(applePosition>=s && applePosition<=t){
            applesHouse++;
        }
    }
    foreach(int orange in oranges){
        int orangePosition=b+orange;
        if(orangePosition>=s && orangePosition<=t){
            orangesHouse++;
        }
    }
    Console.WriteLine(applesHouse);
    Console.WriteLine(orangesHouse);
}