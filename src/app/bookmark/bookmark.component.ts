import { BookmarkService } from './../../services/bookmark/bookmark.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthGuard } from '../guards/auth.guard';
import { MatListModule } from '@angular/material/list';
import { Bookmark } from '../models/bookmark';
import { MatCardModule } from '@angular/material/card';
import { Guid } from 'guid-typescript';
import { MatGridListModule } from '@angular/material/grid-list';
import { ArticlePreviewComponent } from "../article-preview/article-preview.component";
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-bookmark',
    standalone: true,
    templateUrl: './bookmark.component.html',
    styleUrl: './bookmark.component.css',
    imports: [CommonModule, RouterLink, MatListModule, MatCardModule, MatGridListModule, ArticlePreviewComponent,MatButtonModule]
})
export class BookmarkComponent implements OnInit{

  ngOnInit(): void {

    this.loadBookmarks();
  }
  constructor(private authGuard:AuthGuard,private bookmarkService:BookmarkService){}

  bookmarks:Bookmark[] = [];
  isAuthenticated():boolean{
    return this.authGuard.isAuthenticated();
  }


  loadBookmarks(): void {
    this.bookmarkService.getBookmarks().subscribe(
      (response: Bookmark[]) => {
        this.bookmarks = response.map(bookmark => Object.assign(new Bookmark(), bookmark));
      },
      (error: any) => {
        console.error('Failed to load bookmarks', error);
      }
    );
  }
  removeBookmark(bookmarkId: Guid): void {
    this.bookmarkService.removeBookmark(bookmarkId).subscribe(
      () => {
        this.bookmarks = this.bookmarks.filter(bookmark => bookmark.id !== bookmarkId);
        console.log('Bookmark removed');
      },
      (error: any) => {
        console.error('Failed to remove bookmark', error);
      }
    );
  }

}
