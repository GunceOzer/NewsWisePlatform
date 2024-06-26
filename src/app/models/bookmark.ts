import { Guid } from 'guid-typescript';
import { Article } from './article';

export class Bookmark implements Article {
  id: Guid;
  title: string;
  description: string;
  content: string;
  publishedDate: Date;
  likesCount:  0;
  sourceUrl: string;
  urlToImage: string;
  isBookmarked: boolean;

  constructor(
    id?: Guid,
    title: string = "",
    description: string = "",
    content: string = "",
    publishedDate: Date = new Date(),
    likesCount: number = 0,
    sourceUrl: string = "",
    urlToImage: string = "",
    isBookmarked: boolean = false
  ) {

    this.id = id ?? Guid.create();
    this.title = title;
    this.description = description;
    this.content = content;
    this.publishedDate = publishedDate;
    this.likesCount = 0;
    this.sourceUrl = sourceUrl;
    this.urlToImage = urlToImage;
    this.isBookmarked = isBookmarked;
  }


}
