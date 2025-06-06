public static string dayOfProgrammer(int year){
    string date="";
    if (year==1918){
        date=$"26.09.{year}";
    }
    else if(year>=1700 && year<=1917){
        if(year%4==0){
            date = $"12.09.{year}";
        }
        else{
            date=$"13.09.{year}";
        }
    }
    else{
        if((year%400==0) || (year%4==0 && year%100!=0)){
            date=$"12.09.{year}";
        }
        else{
            date=$"13.09.{year}";
        }
    }

    return date;
}