import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import queryString from 'query-string';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http:HttpClient) { }

  //it prepends the url with the api url
  private getUrl(url:string){
    return environment.apiUrl + url;
  }

  //this method makes a get request to the provided url
  get(url:string,data:object={}):Observable<any|null>{
    if(Object.keys(data).length>0)//check is any parameter is there/passed
    url+= '?'+ queryString.stringify(data);

    //OLD
  //return this.http.get(this.getUrl(url)).pipe((data)=> {return data});
  return this.http.get(this.getUrl(url), { headers: this.getHeaders() });


  }

  //this method makes a post request to the provided url , options can be used to pass additional options such as HTTP headers
  post(url:string,data:object={},options: object={}):Observable<any>{
    return this.http.post(this.getUrl(url),data,{ ...options, headers: this.getHeaders() });

  }

  //this method makes an http request to the provided url with the provided method(GET,PUT,POST,DELETE), Url data and options
  //can be used to get mpre control over the request.
  //it creates and httprequest object and passes it to the http.request method

  request(methods: string, url: string, data: object = {}, options: object = {}): Observable<any> {
    const request = new HttpRequest(methods, this.getUrl(url), data, { ...options, headers: this.getHeaders() });
    return this.http.request(request);
  }
  delete(url:string,data:object={},options:object={}):Observable<any>{
    return this.http.delete(this.getUrl(url),{ ...options, headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

}
