import { Injectable } from '@angular/core';
import { Guid } from 'guid-typescript';
import { ApiService } from '../api/api.service';
import { Observable } from 'rxjs';
import { Bookmark } from '../../app/models/bookmark';

@Injectable({
  providedIn: 'root'
})
export class BookmarkService {

  private bookmarkApiUrl: string = 'https://localhost:7122/api/Bookmarks';
  constructor(private apiService: ApiService) { }

  getBookmarks(): Observable<Bookmark[]> {
    return this.apiService.get('Bookmarks').pipe();
  }

  addBookmark(articleId: Guid): Observable<Bookmark> {
    return this.apiService.post(`Bookmarks/${articleId}`, {});
  }

  removeBookmark(articleId: Guid): Observable<void> {
    return this.apiService.delete(`Bookmarks/${articleId}`);
  }


}
