import { Component,Input, OnInit } from '@angular/core';
import { Article } from '../models/article';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLinkActive, RouterModule, RouterOutlet ,RouterLink} from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { AuthGuard } from '../guards/auth.guard';
import { ArticleService } from '../../services/article-service/article.service';
import { Guid } from 'guid-typescript';
import { BookmarkService } from '../../services/bookmark/bookmark.service';
import { Bookmark } from '../models/bookmark';
import { LikeService } from '../../services/like/like.service';
import { UserService } from '../../services/user/user.service';
import { BookmarkComponent } from '../bookmark/bookmark.component';

@Component({
  selector: 'app-article-preview',
  standalone: true,
  imports: [CommonModule,FormsModule,RouterLink,RouterModule,RouterOutlet,MatButtonModule,MatCardModule,MatGridListModule,BookmarkComponent],
  templateUrl: './article-preview.component.html',
  styleUrl: './article-preview.component.css'
})
export class ArticlePreviewComponent {
   @Input() article?: Article;
   isEditOpened:boolean = false;
    articles: Article[] = [];



   constructor(private authGuard:AuthGuard,private articleService:ArticleService,
    private bookmarkService:BookmarkService,  private likeService: LikeService,private userService:UserService
   ){
   }


   isAuthenticated():boolean{
      return this.authGuard.isAuthenticated();
   }

   getCurrentUserId():Guid|null{
      return this.userService.getCurrentUserId();
   }
   deleteArticle(id:Guid):void{
      if(this.article){
         this.articleService.deleteArticle(this.article.id).subscribe(
            (response:any) => {
               console.log('Article deleted');
               this.loadArticles();
            },
            (error:any) => {
               console.error('Delete failed');
            });
      }
   }

  loadArticles() {
    this.articleService.getArticles().subscribe(
      (response: Article[]) => {
        this.articles = response;
      },
      (error: any) => {
        console.error('Failed to load articles', error);
      }
    );
  }
   isAdmin():boolean
    {
        return this.authGuard.isAdmin();
    }

    toggleEditor(){
      this.isEditOpened = !this.isEditOpened;
      console.log(this.isEditOpened);
   }

  addBookmark(): void {
    if (this.article) {
      this.bookmarkService.addBookmark(this.article.id).subscribe(
        (response: Bookmark) => {
          this.article!.isBookmarked = true;
          console.log('Bookmark added');
        },
        (error: any) => {
          console.error('Failed to add bookmark', error);
        }
      );
    }
  }

  removeBookmark(): void {


  }
  toggleBookmark(): void {
    if (this.article?.isBookmarked) {
      this.removeBookmark();
    } else {
      this.addBookmark();
    }
  }


  likeArticle(): void {
    console.log('Like article');
    if (this.article) {
      this.likeService.likeArticle(this.article.id).subscribe(
        () => {
          this.article!.likesCount += 1;
          console.log('Article liked');
        },
        (error: any) => {
          console.error('Failed to like article', error);
        }
      );
    }
  }
  unlikeArticle(): void {
    console.log('Like article');
    if (this.article) {
      this.likeService.unlikeArticle(this.article.id).subscribe(
        () => {
          this.article!.likesCount -= 1;
          console.log('Article liked');
        },
        (error: any) => {
          console.error('Failed to like article', error);
        }
      );
    }
  }


  toggleLike(): void {
    if (this.article) {
      if (this.article.likesCount > 0) {
        console.log('Calling unlikeArticle');
        this.unlikeArticle();
      } else {
        console.log('Calling likeArticle');
        this.likeArticle();
      }
    } else {
      console.error('Article not found');
    }
  }





}
