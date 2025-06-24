export interface LoginRequest{
    email:string;
    password:string;
}

export interface RegisterRequest{
    username:string;
    email:string;
    password:string;
}

export interface AuthResponse{
    accessToken:string;
    refreshToken:string;
    userId:string;
    role:'Admin' | 'User' |'Agent';
    expiresMinutes:string;
}

export interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
    errors: any;
}