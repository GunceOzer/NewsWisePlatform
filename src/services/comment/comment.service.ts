import { UserService } from './../user/user.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../../app/models/user';
import { catchError, Observable, throwError } from 'rxjs';
import { Guid } from 'guid-typescript';
import { Comment } from '../../app/models/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  private baseUrl: string = 'https://localhost:7122/api/Comments';

  constructor(private httpClient:HttpClient , private userService:UserService) { }

  private getAuthHeaders(): HttpHeaders {
    const token = this.userService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }


  getCommentsByArticleId(articleId: Guid): Observable<Comment[]> {
    return this.httpClient.get<Comment[]>(`${this.baseUrl}/${articleId}/comments`);

  }

  addComment(articleId: Guid, content: string): Observable<Comment> {
    const headers = this.getAuthHeaders();
    const commentData = { content };
    console.log('Sending comment data:', commentData);

    return this.httpClient.post<Comment>(
      `${this.baseUrl}/add?articleId=${articleId}`,
      commentData,
      { headers }
    ).pipe(
      catchError(error => {
        console.error('Error response:', error);
        return throwError(error);
      })
    );
  }

  editComment(articleId: Guid, commentId: Guid, content: string): Observable<Comment> {
    const headers = this.getAuthHeaders();
    const editCommentData = { id: commentId, articleId, content };
    return this.httpClient.put<Comment>(
      `${this.baseUrl}/edit?articleId=${articleId}`,
      editCommentData,
      { headers }
    ).pipe(
      catchError(error => {
        console.error('Error response:', error);
        return throwError(error);
      })
    );
  }



deleteComment(articleId: Guid, commentId: Guid): Observable<void> {
  const headers = this.getAuthHeaders();
  return this.httpClient.delete<void>(
    `${this.baseUrl}/delete?articleId=${articleId}&commentId=${commentId}`,
    { headers }
  );
}

}
