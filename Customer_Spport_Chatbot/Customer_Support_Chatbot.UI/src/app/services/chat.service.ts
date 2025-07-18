import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

@Injectable({ providedIn: 'root' })
export class ChatService {
    private baseUrl = environment.apiUrl;

    constructor(private http: HttpClient) {}

    sendMessage(data: { ticketId: string; senderId: string; content: string }): Observable<any> {
        return this.http.post(`${this.baseUrl}/messages`, data);
    }

    getMessages(ticketId: string): Observable<{ sender: 'bot' | 'user' | 'agent'; text?: string; fileUrl?: string; isImage?: boolean; timestamp?: string }[]> {
        return this.http.get(`${this.baseUrl}/messages/${ticketId}`).pipe(
            map(response => {
                // Handle $values wrapper from backend
                return Array.isArray(response) ? response : (response as any)?.$values || [];
            })
        );
    }

    uploadFile(file: File, ticketId: string): Observable<any> {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('ticketId', ticketId);
        return this.http.post(`${this.baseUrl}/files/upload`, formData);
    }
}