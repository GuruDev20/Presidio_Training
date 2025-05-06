public static int getTotalX(List<int> a, List<int> b){
    int lcm=a.Aggregate((x,y)=>LCM(x,y));
    int gcd=b.Aggregate((x,y)=>GCD(x,y));
    int count=0;
    for(int i=lcm;i<=gcd;i+=lcm){
        if(gcd%i==0){
            count++;
        }
    }
    return count;
}

private static int GCD(int a,int b){
    return b==0?a:GCD(b,a%b);
}

private static int LCM(int a,int b){
    return a*b/GCD(a,b);
}