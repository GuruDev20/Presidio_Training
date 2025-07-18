export interface LoginRequest{
    email:string;
    password:string;
}

export interface RegisterRequest{
    username:string;
    email:string;
    password:string;
    profilePictureUrl?:string;
}

export interface AuthResponse{
    accessToken:string;
    refreshToken:string;
    userId:string;
    role:'Admin' | 'User' | 'Agent';
    expiresMinutes:string;
    deviceId:string;
}

export interface UserProfile{
    id:string;
    fullName:string;
    email:string;
    u?: string;
    role:'Admin' | 'User' | 'Agent';
    profilePictureUrl?: string;
}

export interface UserDevice {
    deviceId: string;
    browser: string;
    deviceType: string;
    os: string;
    lastLogin: string;
}
