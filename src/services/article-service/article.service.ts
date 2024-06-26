import { ApiService } from './../api/api.service';
import { Injectable } from '@angular/core';
import { Article } from '../../app/models/article';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, take, tap } from 'rxjs';
import { Guid } from 'guid-typescript';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  //this is our getArticles method url
  private  articleApiUrl: string ='https://localhost:7122/api/Articles';

  //after creating url service we deleted the httpclient from the constructor
  constructor(private apiService:ApiService) {
   }

  getArticles():Observable<Article[]>{
    return this.apiService.get('Articles').pipe(take(30));

  }

  getArticleById(id:Guid):Observable<Article|undefined>{
    return this.apiService.get(`Articles/${id}`).pipe();

  }

  aggregateArticles():Observable<any>{
    return this.apiService.post('Articles/aggregate',{}).pipe();
  }

  deleteArticle(id:Guid):Observable<Article>{
    return this.apiService.delete(`Articles/${id}`).pipe();
  }


  getSortedArticles(sortByNewest:boolean):Observable<Article[]>{
    return this.apiService.get(`Articles/sortedByNewest`,{sortByNewest}).pipe();
  }

  getSortedArticlesByPositivity(sortByPositive:boolean):Observable<Article[]>{
    return this.apiService.get(`Articles/sortedByPositivity`,{sortByPositive}).pipe();
  }

}
