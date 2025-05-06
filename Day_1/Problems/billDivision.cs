public static void bonAppetit(List<int> bill, int k, int b){
    int total=0;
    for(int i=0;i<bill.Count;i++){
        if(i!=k){            
            total+=bill[i];
        }
    }
    int annaShare=total/2;
    if(b==annaShare){
        Console.WriteLine("Bon Appetit");
    }
    else{
        Console.WriteLine(b-annaShare);
    }
}