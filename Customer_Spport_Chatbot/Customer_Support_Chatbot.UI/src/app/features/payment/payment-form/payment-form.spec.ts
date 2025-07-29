import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentFormComponent } from './payment-form';
import { ReactiveFormsModule } from '@angular/forms';
import { RazorpayService } from '../../../services/razorpay.service';
import { OrderService } from '../../../services/order.service';
import { of } from 'rxjs';

class MockRazorpayService {
  initiateTransaction = jasmine
    .createSpy('initiateTransaction')
    .and.returnValue(Promise.resolve(true));
}
class MockOrderService {
  createOrder = jasmine
    .createSpy('createOrder')
    .and.returnValue(of({ id: 'order_123' }));
}

describe('PaymentFormComponent', () => {
  let component: PaymentFormComponent;
  let fixture: ComponentFixture<PaymentFormComponent>;
  let razorpayService: MockRazorpayService;
  let orderService: MockOrderService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, PaymentFormComponent],
      providers: [
        { provide: RazorpayService, useClass: MockRazorpayService },
        { provide: OrderService, useClass: MockOrderService },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(PaymentFormComponent);
    component = fixture.componentInstance;
    razorpayService = TestBed.inject(RazorpayService) as any;
    orderService = TestBed.inject(OrderService) as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should mark form as touched if invalid on pay', async () => {
    spyOn(component.paymentForm, 'markAllAsTouched');
    component.paymentForm.patchValue({ amount: 0 });
    await component.pay();
    expect(component.paymentForm.markAllAsTouched).toHaveBeenCalled();
  });

  it('should call createOrder and initiateTransaction on valid pay', async () => {
    component.paymentForm.setValue({
      amount: 100,
      customerName: 'Test User',
      email: 'test@example.com',
      contactNumber: '1234567890',
    });
    await component.pay();
    expect(orderService.createOrder).toHaveBeenCalled();
    expect(razorpayService.initiateTransaction).toHaveBeenCalled();
  });
});
