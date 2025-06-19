import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { selectUsers } from '../../store/user.selectors';
import { bannedUsernameValidator, matchPasswordsValidator, passwordStrengthValidator } from '../../validator/validator';
import { combineLatest, debounceTime, distinctUntilChanged, fromEvent, map, Observable, of, switchMap } from 'rxjs';
import { addUser } from '../../store/user.actions';
import { User } from '../../models/user.model';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-user-dashboard',
    imports: [FormsModule,CommonModule,ReactiveFormsModule],
    templateUrl: './user-dashboard.html',
    styleUrl: './user-dashboard.css',
    standalone: true
})
export class UserDashboard implements OnInit{
    
    userForm!:FormGroup;
    roles=['Admin', 'User', 'Guest'];
    filteredUsers$:Observable<User[]>=of([]);
    searchTerm='';
    selectedRole='All';
    showToast=false;

    @ViewChild('searchInput',{static:true}) searchInput!:ElementRef;

    constructor(private fb:FormBuilder,private store:Store){}

    ngOnInit(): void {
        this.userForm=this.fb.group({
            username:['',[Validators.required,bannedUsernameValidator()]],
            email:['',[Validators.required, Validators.email]],
            password:['',[Validators.required,passwordStrengthValidator]],
            confirmPassword:['',Validators.required],
            role:['User',Validators.required]
        },{validators:matchPasswordsValidator});

        this.filteredUsers$=this.store.select(selectUsers);
        this.setupSearchFilter();
    }

    setupSearchFilter(){
        const search$=fromEvent(this.searchInput.nativeElement,'input')
            .pipe(
                debounceTime(300),
                distinctUntilChanged(),
                switchMap(()=>this.store.select(selectUsers)),
            );
        
        const roles$=this.store.select(selectUsers)
        this.filteredUsers$=combineLatest([search$,roles$]).pipe(
            map(([users])=>{
                return users.filter(user=>{
                    const matchSearch=user.username.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                                        user.role.toLowerCase().includes(this.searchTerm.toLowerCase());

                    const matchRole=this.selectedRole==='All' || user.role===this.selectedRole;
                    return matchSearch && matchRole;
                });
            })
        );
    }

    onSubmit(){
        if(this.userForm.valid){
            const {confirmPassword,...user}= this.userForm.value;
            this.store.dispatch(addUser({user}));
            this.userForm.reset({role: 'User'});
            this.showToast=true;
            setTimeout(() => this.showToast = false, 3000);
        }
    }
}
