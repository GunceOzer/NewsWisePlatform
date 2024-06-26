import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Guid } from 'guid-typescript';
import { Observable } from 'rxjs';
import { UserService } from '../user/user.service';

@Injectable({
  providedIn: 'root'
})
export class LikeService {

  private baseUrl: string = 'https://localhost:7122/api/Likes';

  constructor(private httpClient:HttpClient,private userService:UserService) { }


  private getAuthHeaders(): HttpHeaders {
    const token = this.userService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }



  likeArticle(articleId: Guid): Observable<any> {
    return this.httpClient.post(
      `${this.baseUrl}/like`,
      { articleId: articleId.toString() },
      { headers: this.getAuthHeaders() }
    );
  }

  unlikeArticle(articleId: Guid): Observable<any> {
    return this.httpClient.post(
      `${this.baseUrl}/unlike`,
      { articleId: articleId.toString() },
      { headers: this.getAuthHeaders() }
    );
  }




}
