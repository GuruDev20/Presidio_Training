import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PaymentForm } from './payment-form';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PaymentService } from '../../services/paymentService.service';
import { of, throwError } from 'rxjs';
import { PaymentResponse } from '../../models/payment.model';
import { CommonModule } from '@angular/common';

describe('PaymentForm', () => {
    let component: PaymentForm;
    let fixture: ComponentFixture<PaymentForm>;
    let mockPaymentService: jasmine.SpyObj<PaymentService>;

    const mockResponse: PaymentResponse = {
        paymentId: '12345',
        message: 'Payment successful',
        status: 'success'
    };

    beforeEach(async () => {
        const spy = jasmine.createSpyObj('PaymentService', ['initiatePayment']);

        await TestBed.configureTestingModule({
            imports: [ReactiveFormsModule, FormsModule, CommonModule, HttpClientTestingModule, PaymentForm],
            providers: [{ provide: PaymentService, useValue: spy }]
        }).compileComponents();

        fixture = TestBed.createComponent(PaymentForm);
        component = fixture.componentInstance;
        mockPaymentService = TestBed.inject(PaymentService) as jasmine.SpyObj<PaymentService>;
        fixture.detectChanges();
    });

    it('should create the component', () => {
        expect(component).toBeTruthy();
    });

    it('form should be invalid when empty', () => {
        expect(component.paymentForm.valid).toBeFalsy();
    });

    it('form should be valid with proper data', () => {
        component.paymentForm.setValue({
            amount: 100,
            customerName: 'Dev Kumar',
            email: 'dev@example.com',
            contactNumber: '9876543210'
        });
        expect(component.paymentForm.valid).toBeTrue();
    });

    it('should call initiatePayment on valid form submit (success case)', fakeAsync(() => {
        component.paymentForm.setValue({
            amount: 100,
            customerName: 'Dev Kumar',
            email: 'dev@example.com',
            contactNumber: '9876543210'
        });

        mockPaymentService.initiatePayment.and.returnValue(of(mockResponse));

        component.onSubmit();
        tick();

        expect(component.isLoading).toBeFalse();
        expect(component.paymentResponse).toEqual(mockResponse);
        expect(mockPaymentService.initiatePayment).toHaveBeenCalled();
    }));

    it('should handle payment failure response', fakeAsync(() => {
        component.paymentForm.setValue({
            amount: 100,
            customerName: 'Dev Kumar',
            email: 'dev@example.com',
            contactNumber: '9876543210'
        });

        mockPaymentService.initiatePayment.and.returnValue(throwError(() => new Error('Network error')));

        component.onSubmit();
        tick();

        expect(component.isLoading).toBeFalse();
        expect(component.paymentResponse?.status).toBe('failure');
    }));
});
