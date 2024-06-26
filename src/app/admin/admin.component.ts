import { Component, OnInit } from '@angular/core';
import { ArticleService } from '../../services/article-service/article.service';
import { Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { Article } from '../../app/models/article';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';


@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule,MatCardModule,MatButtonModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit{

  articles: Article[] = [];
  ngOnInit(): void {
    console.log('Admin component initialized');
    this.loadArticles();  // load articles when component is initialized
  }
  constructor(private articleService:ArticleService,private router:Router) { }


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

  aggregateArticles():void{
    this.articleService.aggregateArticles().subscribe(
      (response:any)  => {
        console.log('Articles aggregated');
        this.loadArticles(); // reload articles after aggregation
      },
      (error:any) => {
        console.error('Aggregation failed');
      });
  }

  deleteArticle(id:Guid):void{
    this.articleService.deleteArticle(id).subscribe(
      (response:any) => {
        console.log('Article deleted');
        this.loadArticles();
      },
      (error:any) => {
        console.error('Delete failed');
      });
  }

}
