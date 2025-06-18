import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { Router } from '@angular/router';
import { first } from 'rxjs';

@Component({
    selector: 'app-add-user',
    imports: [CommonModule,ReactiveFormsModule],
    templateUrl: './add-user.html',
    styleUrl: './add-user.css',
    standalone: true
})
export class AddUser {

    userForm:FormGroup;
    constructor(private fb:FormBuilder, private userService:UserService, private router:Router){
        this.userForm=this.fb.group({
            firstName:['',[Validators.required,Validators.minLength(2)]],
            lastName:['',[Validators.required,Validators.minLength(2)]],
            gender:['',[Validators.required]],
            role:['',[Validators.required]],
            age:['',[Validators.required,Validators.min(1),Validators.max(120)]],
            state:['',[Validators.required]],
        });
    }

    get f(){
        return this.userForm.controls;
    }

    onSubmit(){
        if(this.userForm.invalid){
            return;
        }

        const userPayload={
            ...this.userForm.value,
            address:{
                state: this.userForm.value.state
            },
        };

        this.userService.addUser(userPayload).subscribe({
            next:()=>{
                alert('User added successfully');
                this.userForm.reset();
                this.router.navigate(['/dashboard']);
            },
            error:(err)=>{
                alert('Error adding user: ' + err.message);
            }
        })
    }
}
