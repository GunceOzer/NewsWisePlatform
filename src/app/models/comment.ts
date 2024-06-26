import { Guid } from "guid-typescript";

export class Comment {
  id: Guid ;
  userId: Guid;
  content: string;
  fullName: string;
  createdAt: string;
  articleId: Guid;
}
