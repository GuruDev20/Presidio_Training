public static void staircase(int n){
    for(int i=1;i<=n;i++){
        for(int j=1;j<=n-i;j++){
            Console.Write(" ");
        }
        for(int k=1;k<=i;k++){
            Console.Write("#");
        }
        Console.WriteLine();
    }
}
