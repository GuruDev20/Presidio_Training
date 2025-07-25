import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VideoModal } from './video-modal';

describe('VideoModal', () => {
  let component: VideoModal;
  let fixture: ComponentFixture<VideoModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VideoModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
