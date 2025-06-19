import { AbstractControl,ValidationErrors,ValidatorFn,FormGroup } from "@angular/forms";

export const bannedWords=['admin', 'root'];
export function bannedUsernameValidator():ValidatorFn{
    return (control:AbstractControl):ValidationErrors|null=>{
        const username = control.value?.toLowerCase();
        return bannedWords.some(word=>username?.includes(word))
            ? { bannedWords: true }
            : null;
    };
}

export function passwordStrengthValidator(control:AbstractControl):ValidationErrors|null{
    const value=control.value;
    if(!value){
        return null;
    }
    const hasNumber = /\d/.test(value);
    const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    const valid = value.length >= 8 && hasNumber && hasSymbol;
    return valid ? null : { weakPassword: true };
}

export function matchPasswordsValidator(group:FormGroup):ValidationErrors|null{
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password===confirmPassword?null:{passwordMisMatch: true};
}