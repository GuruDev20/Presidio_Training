import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function validUsername():ValidatorFn{
    return (control:AbstractControl):ValidationErrors | null =>{
        const value= control.value || '';
        const isValid = /^[a-zA-Z\s]+$/.test(value);
        return isValid ? null : { invalidUsername: true };
    }
}