import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AdminWorkspaceComponent } from './workspace.component';

describe('AdminWorkspaceComponent', () => {
  let component: AdminWorkspaceComponent;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', [], {
      url: '/admin/dashboard/workspace' // default
    });

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerSpy }
      ]
    });

    component = new AdminWorkspaceComponent(routerSpy, {} as any);
  });

  it('should return true if at base workspace URL', () => {
    expect(component.isAtBaseTickets).toBeTrue();
  });

  it('should return false if at nested workspace URL', () => {
    Object.defineProperty(routerSpy, 'url', {
      get: () => '/admin/dashboard/workspace/manage-users'
    });

    expect(component.isAtBaseTickets).toBeFalse();
  });
});
