import { Component, inject, OnInit } from '@angular/core';
import { Article } from '../models/article';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ArticlePreviewComponent } from '../article-preview/article-preview.component';
import { ArticleService } from '../../services/article-service/article.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; // Import the MatProgressSpinnerModule from the appropriate package
import { AuthGuard } from '../guards/auth.guard';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatGridListModule } from '@angular/material/grid-list';
@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ArticlePreviewComponent,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatFormFieldModule,
    MatOptionModule,
    MatSelectModule,
    MatGridListModule
  ],
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css'],
})
export class ArticleListComponent implements OnInit {
  articles: Article[] | null = []; // after injecting service we equaled it to an empty array
  hasAccessToEdit: boolean = true; //when it is true it shows the edit textbox
  selectedArticle: Article | null = null; //when we click on an article it will be selected


  showSpinner: boolean = false;

  constructor(
    private articleService: ArticleService,
    private authGuard: AuthGuard
  ) {
  }

  ngOnInit() {
    this.showSpinner = true;

    console.log('Article List Component Init');
    //out article during our initialization will be equal to the articles we get from our service
    this.articleService
      .getArticles()
      .subscribe((articles) => (this.articles = articles))
      .add(() => (this.showSpinner = false));

    // get articles is a method from ArticleService is being called(it basically makes an HTTP request to our API and gets the articles)
    // and it returns an obsevable(which is a stream of data that can be processed over time)
    // and we are subscribing to that stream of data

    // A pipe is a method that we can use on Obsrevables to apply one or more operators.
    //Operaters are functions that can be used to manipulate the data in the stream
  }

  isAuthenticated(): boolean {
    return this.authGuard.isAuthenticated();
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

  sortArticles(criteria: string): void {
    switch (criteria) {
      case 'newest':
        this.articleService.getSortedArticles(true).subscribe(
          (response: Article[]) => {
            this.articles = response;
          },
          (error: any) => {
            console.error('Failed to sort articles', error);
          }
        );
        break;
      case 'oldest':
        this.articleService.getSortedArticles(false).subscribe(
          (response: Article[]) => {
            this.articles = response;
          },
          (error: any) => {
            console.error('Failed to sort articles', error);
          }
        );
        break;
      case 'positive':
        this.articleService.getSortedArticlesByPositivity(true).subscribe(
          (response: Article[]) => {
            this.articles = response;
          },
          (error: any) => {
            console.error('Failed to sort articles', error);
          }
        );
        break;
      case 'negative':
        this.articleService.getSortedArticlesByPositivity(false).subscribe(
          (response: Article[]) => {
            this.articles = response;
          },
          (error: any) => {
            console.error('Failed to sort articles', error);
          }
        );
        break;
      default:
        this.loadArticles();
        break;
    }
  }
}
