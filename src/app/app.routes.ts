import { Routes } from '@angular/router';
import { ArticleListComponent } from './article-list/article-list.component';
import { ArticleDetailsComponent } from './article-details/article-details.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { AdminComponent } from './admin/admin.component';
import { AuthGuard } from './guards/auth.guard';
import { BookmarkComponent } from './bookmark/bookmark.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { RegisterComponent } from './register/register.component';
import { ResetPasswordRequestComponent } from './reset-password-request/reset-password-request.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
export const routes: Routes = [
  {
    path:'',
    redirectTo: '/home',
    pathMatch: 'full',

  },
  {
    path:'home',
    component: ArticleListComponent,
    title: 'Home Page'
  },
  {
    path:'article-details/:id',
    component:ArticleDetailsComponent,
    title: 'Article Details'
  }
  ,
  {
    path:'article-a',
    redirectTo:'/home',
    pathMatch:'prefix'

  },

  {
    path:'login',
    component:LoginComponent,
    title:':Login'

  },
  { path: 'unauthorized', component: UnauthorizedComponent, title: 'Unauthorized' },
  { path: 'admin', component: AdminComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  {
    path: 'bookmarks',
      component: BookmarkComponent,
      title: 'Bookmarks',
      canActivate: [AuthGuard]
      ,data:{role:['User','Admin']}
    },
    { path: 'change-password',
      component: ChangePasswordComponent ,
      title: 'Change Password',
  },
  {path:'register',
    component:RegisterComponent,
    title:'Register'
  },
  {path:'reset-password-request',
    component:ResetPasswordRequestComponent,
    title:'Reset Password Request'
  },
  {path:'reset-password',
    component:ResetPasswordComponent,
    title:'Reset Password'
  }
];
