export interface User{
    id?: number;
    firstName: string;
    lastName: string;
    age: number;
    gender: 'male'|'female';
    role: string;
    address:{
        state: string;
    };
    createdDate?: string;
}