public static string timeConversion(string s){
    string period=s.Substring(s.Length-2);
    int hour=int.Parse(s.Substring(0,2));
    string minSec=s.Substring(2,6);
    if(period=="AM"){
        if(hour==12){
            hour=0;
        }
    }
    else if(period=="PM"){
        if(hour!=12){
            hour+=12;
        }
    }
    return $"{hour:D2}{minSec}";
}