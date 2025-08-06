import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Login } from './login';
import { AuthService } from '../../../services/auth.service';
import { RouterTestingModule } from '@angular/router/testing';

describe('Login Component', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['login', 'getRole']);

    await TestBed.configureTestingModule({
      imports: [Login, ReactiveFormsModule, RouterTestingModule],
      providers: [{ provide: AuthService, useValue: authSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty values', () => {
    const form = component.form;
    expect(form).toBeTruthy();
    expect(form.value.email).toBe('');
    expect(form.value.password).toBe('');
  });

  it('should have invalid form when empty', () => {
    component.form.setValue({ email: '', password: '' });
    expect(component.form.invalid).toBeTrue();
  });

  it('should validate form with correct values', () => {
    component.form.setValue({ email: 'test@example.com', password: '123456' });
    expect(component.form.valid).toBeTrue();
  });

  it('should call AuthService.login on valid submit', () => {
  const mockCredentials = { email: 'test@example.com', password: '123456' };
  component.form.setValue(mockCredentials);

  authServiceSpy.login.and.returnValue(of({
    success: true,
    message: 'Login successful',
    data: {
      accessToken: 'mock-token',
      refreshToken: 'mock-refresh',
      userId: 'user123',
      role: 'User',
      expiresMinutes: '60',
      deviceId: 'device123'
    }
  }));

  authServiceSpy.getRole.and.returnValue('User');

  const navigateSpy = spyOn(router, 'navigate');

  component.submit();

  expect(authServiceSpy.login).toHaveBeenCalledWith(mockCredentials);
  expect(authServiceSpy.getRole).toHaveBeenCalled();
  expect(navigateSpy).toHaveBeenCalledWith(['/user/dashboard/overview']);
});


  it('should navigate to Admin dashboard if role is Admin', () => {
  component.form.setValue({ email: 'admin@example.com', password: 'admin123' });

  authServiceSpy.login.and.returnValue(of({
    success: true,
    message: 'Login successful',
    data: {
      accessToken: 'admin-token',
      refreshToken: 'admin-refresh',
      userId: 'admin001',
      role: 'Admin',
      expiresMinutes: '60',
      deviceId: 'admin-device'
    }
  }));

  authServiceSpy.getRole.and.returnValue('Admin');

  const navigateSpy = spyOn(router, 'navigate');

  component.submit();

  expect(authServiceSpy.login).toHaveBeenCalled();
  expect(authServiceSpy.getRole).toHaveBeenCalled();
  expect(navigateSpy).toHaveBeenCalledWith(['/admin/dashboard/overview']);
});


  it('should alert and log error on login failure', () => {
    spyOn(window, 'alert');
    spyOn(console, 'error');

    component.form.setValue({ email: 'fail@example.com', password: 'wrongpass' });

    const errorResponse = {
      error: { message: 'Invalid credentials' },
    };

    authServiceSpy.login.and.returnValue(throwError(() => errorResponse));

    component.submit();

    expect(window.alert).toHaveBeenCalledWith('Login failed: Invalid credentials');
    expect(console.error).toHaveBeenCalledWith('Login failed:', errorResponse);
  });
});
