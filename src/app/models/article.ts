import {Guid} from "guid-typescript";

export interface Article{
  id: Guid;
  title :string;
  description : string;
  content: string;
  publishedDate:Date;
  likesCount: 0;
  sourceUrl:string;
  urlToImage:string;
  isBookmarked:boolean;

}

