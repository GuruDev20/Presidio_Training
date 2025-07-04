import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentResult } from './payment-result';
import { CommonModule } from '@angular/common';
import { PaymentResponse } from '../../models/payment.model';


describe('PaymentResult', () => {
    let component: PaymentResult;
    let fixture: ComponentFixture<PaymentResult>;

    const mockResponse: PaymentResponse = {
        paymentId: 'abc123',
        message: 'Payment processed successfully',
        status: 'success'
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CommonModule, PaymentResult],
        }).compileComponents();

        fixture = TestBed.createComponent(PaymentResult);
        component = fixture.componentInstance;
    });

    it('should create component', () => {
        expect(component).toBeTruthy();
    });

    it('should handle null input without errors', () => {
        component.response = null;
        fixture.detectChanges();

        expect(component.response).toBeNull();
        const compiled = fixture.nativeElement as HTMLElement;
        // expect(compiled.textContent?.trim()).toBe('');
    });

    it('should display payment result details correctly', () => {
        component.response = mockResponse;
        fixture.detectChanges();

        const compiled = fixture.nativeElement as HTMLElement;

        expect(compiled.textContent).toContain(mockResponse.message);
        expect(compiled.textContent).toContain(mockResponse.status);
        expect(compiled.textContent).toContain(mockResponse.paymentId);
    });
});
