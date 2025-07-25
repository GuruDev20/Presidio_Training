import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class VideoService {
    private api = 'http://localhost:5081/api/videos';

    constructor(private http: HttpClient) {}

    uploadVideo(data: FormData): Observable<any> {
        return this.http.post(`${this.api}/upload`, data);
    }

    getAllVideos(): Observable<any[]> {
        return this.http.get<any[]>(this.api);
    }
}
