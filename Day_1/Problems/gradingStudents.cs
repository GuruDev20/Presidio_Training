public static List<int> gradingStudents(List<int> grades){
    List<int> result=new List<int>();
    foreach(int grade in grades){
        if(grade<38){
            result.Add(grade);
        }
        else{
            int next=(grade/5+1)*5;
            if(next-grade<3){
                result.Add(next);
            }
            else{
                result.Add(grade);
            }
        }
    }
    return result;
}