import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { VideoService } from '../../service/video.service';
import { VideoModal } from "../video-modal/video-modal";

@Component({
    selector: 'app-view',
    standalone: true,
    imports: [CommonModule, VideoModal],
    templateUrl: './list.html',
    styleUrls: ['./list.css']
})
export class List implements OnInit {
    videos: any[] = [];
    selectedVideo: any = null;

    constructor(private videoService: VideoService,private cdRef: ChangeDetectorRef){}

    ngOnInit() {
        this.videoService.getAllVideos().subscribe({
            next: (data) => {
                console.log('Videos fetched successfully', data);
                this.videos = data;
                this.cdRef.detectChanges();
            },
            error: (err) => {
                console.error('Error fetching videos', err);
            }
        });
    }

    play(video: any) {
        this.selectedVideo = video;
    }
}
