import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDashboard } from './overview';
import { AdminService } from '../../../services/admin.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';

describe('AdminDashboard Component', () => {
  let component: AdminDashboard;
  let fixture: ComponentFixture<AdminDashboard>;
  let adminServiceSpy: jasmine.SpyObj<AdminService>;
  let router: Router;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('AdminService', [
      'getOverview',
      'getTicketGrowth',
      'getDeactivationRequests',
      'getAgentDetails',
      'getTicketDetails'
    ]);

    await TestBed.configureTestingModule({
      imports: [AdminDashboard, RouterTestingModule],
      providers: [{ provide: AdminService, useValue: spy }]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboard);
    component = fixture.componentInstance;
    adminServiceSpy = TestBed.inject(AdminService) as jasmine.SpyObj<AdminService>;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });


  it('should populate overview data on success', () => {
    const mockResponse = {
      success: true,
      message: 'Overview fetched',
      data: {
        totalUsers: 10,
        activeUsers: 5,
        totalAgents: 4,
        activeAgents: 2,
        totalTickets: 20
      }
    };

    adminServiceSpy.getOverview.and.returnValue(of(mockResponse));
    component.fetchOverviewData();

    expect(adminServiceSpy.getOverview).toHaveBeenCalled();
    expect(component.cards[0].count).toBe(10);
    expect(component.pieChartData.datasets[0].data).toEqual([10, 5, 4, 2]);
  });

  it('should handle error on overview fetch failure', () => {
    const error = { error: 'fail' };
    adminServiceSpy.getOverview.and.returnValue(throwError(() => error));
    spyOn(console, 'error');

    component.fetchOverviewData();

    expect(console.error).toHaveBeenCalled();
  });

  it('should update line chart data', () => {
    const mockLineResponse = {
      success: true,
      message: 'Line chart data fetched',
      data: {
        labels: { $values: ['Aug 1', 'Aug 2'] },
        values: { $values: [3, 7] }
      }
    };

    adminServiceSpy.getTicketGrowth.and.returnValue(of(mockLineResponse));
    component.updateLineChartData('last24hours');

    expect(component.lineChartData.labels).toEqual(['Aug 1', 'Aug 2']);
    expect(component.lineChartData.datasets[0].data).toEqual([3, 7]);
  });

  it('should handle error on ticket growth fetch failure', () => {
    const error = { error: 'fail' };
    adminServiceSpy.getTicketGrowth.and.returnValue(throwError(() => error));
    spyOn(console, 'error');

    component.updateLineChartData('last24hours');

    expect(console.error).toHaveBeenCalled();
  });

  it('should call router.navigate when manage() is triggered', () => {
    const navSpy = spyOn(router, 'navigate');
    component.manage();
    expect(navSpy).toHaveBeenCalledWith(['/admin/dashboard/workspace/manage-agents']);
  });

  it('should call router.navigate when viewAllDeactivationRequests() is triggered', () => {
    const navSpy = spyOn(router, 'navigate');
    component.viewAllDeactivationRequests();
    expect(navSpy).toHaveBeenCalledWith(['/admin/dashboard/workspace/manage-users']);
  });

  it('should unsubscribe from all subscriptions on destroy', () => {
    const sub1 = of().subscribe();
    const sub2 = of().subscribe();

    const unsubSpy1 = spyOn(sub1, 'unsubscribe');
    const unsubSpy2 = spyOn(sub2, 'unsubscribe');

    component['subscriptions'] = [sub1, sub2];
    component.ngOnDestroy();

    expect(unsubSpy1).toHaveBeenCalled();
    expect(unsubSpy2).toHaveBeenCalled();
  });
});
