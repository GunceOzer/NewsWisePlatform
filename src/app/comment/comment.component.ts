import { Component, Input } from '@angular/core';
import { Guid } from 'guid-typescript';
import { AuthGuard } from '../guards/auth.guard';
import { CommentService } from '../../services/comment/comment.service';
import { UserService } from '../../services/user/user.service';
import { Router, RouterModule } from '@angular/router';
import { Comment } from '../models/comment';
import { Article } from '../models/article';
import { ArticleService } from '../../services/article-service/article.service';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [MatCardModule,CommonModule,FormsModule,RouterModule,MatButtonModule ],
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.css'
})
export class CommentComponent {
  @Input() articleId: Guid;
  comments: Comment[] = [];
  newCommentContent: string = '';
  editMode: boolean = false;
  commentToEdit: Comment | null = null;


  constructor(private commentService:CommentService,private authGuard:AuthGuard,private router:Router,private userService:UserService,private articleService:ArticleService){}


  ngOnInit(): void {
    this.loadComments();


  }

  isAdmin(): boolean {

    return this.authGuard.isAdmin();
  }

  isAuthenticated(): boolean {
    return this.authGuard.isAuthenticated();
  }

  isCommentOwner(commentUserId: Guid): boolean {
    const currentUserId = this.userService.getCurrentUserId();
    const isOwner = currentUserId?.equals(commentUserId);
    return isOwner;
  }

  loadComments(): void {
    this.commentService.getCommentsByArticleId(this.articleId).subscribe(
      (comments: Comment[]) => {
        this.comments = comments;
      },
      (error: any) => {
        console.error('Failed to load comments', error);
      }
    );
  }




addComment(): void {
  if (this.newCommentContent.trim()) {
    const commentData = { content: this.newCommentContent };
    console.log('Comment data:', commentData);

    this.commentService.addComment(this.articleId, this.newCommentContent).subscribe(
      (newComment: Comment) => {
        this.loadComments();

        this.comments.push(newComment);
        this.newCommentContent = '';
      },
      (error: any) => {
        console.error('Failed to add comment', error);
      }
    );
  }
}



editComment(comment: Comment): void {
  this.commentToEdit = { ...comment };
  this.editMode = true;
  this.newCommentContent = comment.content;
}



saveEditComment(): void {
  if (this.commentToEdit && this.newCommentContent.trim()) {
    this.commentService.editComment(this.articleId, this.commentToEdit.id, this.newCommentContent).subscribe(
      (updatedComment: Comment) => {
        this.loadComments();
        const index = this.comments.findIndex(c => c.id === updatedComment.id);
        if (index !== -1) {
          this.comments[index] = updatedComment;
        }
        this.newCommentContent = '';
        this.editMode = false;
        this.commentToEdit = null;
      },
      (error: any) => {
        console.error('Failed to update comment', error);
      }
    );
  }
}

cancelEdit(): void {
  this.editMode = false;
  this.commentToEdit = null;
  this.newCommentContent = '';
}


deleteComment(commentId: Guid): void {
  this.commentService.deleteComment(this.articleId, commentId).subscribe(
    () => {
      this.comments = this.comments.filter(c => c.id !== commentId);
    },
    (error: any) => {
      console.error('Failed to delete comment', error);
    }
  );
}
  redirectToLogin(): void {
    this.router.navigate(['/login']);
  }

}
