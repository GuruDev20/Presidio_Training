import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Register } from './register';
import { AuthService } from '../../../services/auth.service';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';

describe('Register Component', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['register']);

    await TestBed.configureTestingModule({
      imports: [Register, ReactiveFormsModule, RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: authSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty values', () => {
    const form = component.form;
    expect(form).toBeTruthy();
    expect(form.value.username).toBe('');
    expect(form.value.email).toBe('');
    expect(form.value.password).toBe('');
    expect(form.value.confirmPassword).toBe('');
    expect(form.value.profilePictureUrl).toBe('');
  });

  it('should mark form as invalid when empty', () => {
    component.form.setValue({
      username: '',
      email: '',
      password: '',
      confirmPassword: '',
      profilePictureUrl: ''
    });
    expect(component.form.invalid).toBeTrue();
  });

  it('should submit the form and navigate on success', () => {
    const mockForm = {
      username: 'devtester',
      email: 'dev@example.com',
      password: 'secret123',
      confirmPassword: 'secret123',
      profilePictureUrl: ''
    };

    component.form.setValue(mockForm);

    // valid form -> mock register
    authServiceSpy.register.and.returnValue(of({
      success: true,
      message: 'Registered!',
      data: {}
    }));

    const navSpy = spyOn(router, 'navigate');

    component.submit();

    expect(authServiceSpy.register).toHaveBeenCalledWith(mockForm);
    expect(navSpy).toHaveBeenCalledWith(['/user/dashboard/overview']);
  });

  it('should handle error on failed registration', () => {
    const mockForm = {
      username: 'failUser',
      email: 'fail@example.com',
      password: '123456',
      confirmPassword: '123456',
      profilePictureUrl: ''
    };

    component.form.setValue(mockForm);

    const error = { error: { message: 'Registration failed' } };
    authServiceSpy.register.and.returnValue(throwError(() => error));
    spyOn(console, 'error');

    component.submit();

    expect(authServiceSpy.register).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Login failed', error);
  });

  it('should return passwordMismatch true if mismatch exists', () => {
    component.form.setValue({
      username: 'testuser',
      email: 'test@example.com',
      password: 'pass123',
      confirmPassword: 'wrong123',
      profilePictureUrl: ''
    });

    // trigger touched for confirmPassword
    const confirmCtrl = component.form.get('confirmPassword');
    confirmCtrl?.markAsTouched();

    expect(component.passwordMismatch).toBeTrue();
  });
});
