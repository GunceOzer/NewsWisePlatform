import { AdminComponent } from './../admin/admin.component';
import { AuthGuard } from './../guards/auth.guard';
import { ArticleService } from './../../services/article-service/article.service';
import { Component, inject, Input, OnInit } from '@angular/core';
import { Article } from '../models/article';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Guid } from 'guid-typescript';
import { MatCardModule } from '@angular/material/card';
import { MatGridList, MatGridTile } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { CommentComponent } from '../comment/comment.component';
import { Comment } from '../models/comment';
import { CommentService } from '../../services/comment/comment.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { UserService } from '../../services/user/user.service';
import { LikeService } from '../../services/like/like.service';
import { BookmarkService } from '../../services/bookmark/bookmark.service';
@Component({
    selector: 'app-article-details',
    standalone: true,
    templateUrl: './article-details.component.html',
    styleUrl: './article-details.component.css',
    imports: [
        RouterLink,
        MatCardModule,
        MatGridList,
        MatGridTile,
        MatButtonModule,
        MatCardModule, CommonModule,
        MatFormFieldModule,
        CommentComponent
    ]
})
export class ArticleDetailsComponent implements OnInit {
  //route:ActivatedRoute = inject(ActivatedRoute); // we will comment this and use the constructor
  id?: Guid;
  article?: Article;
  articles :Article[] = [];
  // selectedArticle: Article | null = null;
  // comments: Comment[] = [];
  // newCommentContent: string = '';

  constructor(
    private route: ActivatedRoute,
    private articleService: ArticleService,
    private authGuard:AuthGuard,
    private router: Router,private commentService: CommentService,
    private likeService: LikeService,
    private userService: UserService,
    private bookmarkService: BookmarkService

  ) {}
  // @Input() article?: Article;

  ngOnInit() {
    //and we move the code below to ngOnInit from constructor
    this.id = Guid.parse(this.route.snapshot.params['id']);
    if (this.id) {
      this.articleService.getArticleById(this.id).subscribe((article) => {
        this.article = article;
       // this.loadComments(this.id); working just commented out
      }); //this code will get the article by id

    }
  }

  isAdmin(): boolean {
    return this.authGuard.isAdmin();
  }

  isAuthenticated(): boolean {
    return this.authGuard.isAuthenticated();
  }
  deleteArticle(id:Guid):void{
    if(this.article){
       this.articleService.deleteArticle(this.article.id).subscribe(
          (response:any) => {
             console.log('Article deleted');
             this.router.navigate(['/home']);
          },
          (error:any) => {
             console.error('Delete failed');
          });
    }
 }

 loadArticles(): void {
  this.articleService.getArticles().subscribe(
    (articles: Article[]) => {
      this.articles = articles;
    },
    (error: any) => {
      console.error('Failed to load articles', error);
    }
  );
}

addBookmark(): void {
  if(this.article){
    this.bookmarkService.addBookmark(this.article.id).subscribe(
      (response: any) => {
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
  if (this.article) {
    this.bookmarkService.removeBookmark(this.article.id).subscribe(
      () => {
        this.article!.isBookmarked = false;
        console.log('Bookmark removed');
      },
      (error: any) => {
        console.error('Failed to remove bookmark', error);
      }
    );
  }

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



///JUST COMMMETNED OUT THE CODE BELOWWAS WORKING

// loadComments(articleId: Guid): void {
//   this.commentService.getCommentsByArticleId(articleId).subscribe(
//     (comments: Comment[]) => {
//       this.comments = comments;
//     },
//     (error: any) => {
//       console.error('Failed to load comments', error);
//     }
//   );
// }
///====================
//  loadArticle(id: Guid): void {
//   this.articleService.getArticleById(id).subscribe(
//     (response: Article | undefined) => {
//       if (response) {
//         this.article = response;
//       } else {
//         console.error('Article not found');
//       }
//     },
//     (error: any) => {
//       console.error('Failed to load article', error);
//     }
//   );
// }

}
