import { ChangeDetectorRef, Component } from '@angular/core';
import { RouterModule,RouterOutlet,RouterLink,RouterLinkActive } from '@angular/router';
import { ArticleListComponent } from './article-list/article-list.component';
import { HttpClient } from '@angular/common/http';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbar, MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../services/user/user.service';
import { CommonModule } from '@angular/common';
import { AuthGuard } from './guards/auth.guard';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule,RouterOutlet, RouterModule ,RouterLink,RouterLinkActive,MatSidenavModule,MatToolbarModule,MatButtonModule,MatListModule,MatIconModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  // title = 'NewWise App';

  constructor(private authGuard:AuthGuard, private userService: UserService){}
  //private changeDetectorRef:ChangeDetectorRef
  logout(){
    this.userService.logout().subscribe(
      (response:any)=>{
        console.log('Logout successful');
      },
      (error:any)=>{
        console.error('Logout failed');
      }


    );
  }
  isAuthenticated():boolean{
    return this.authGuard.isAuthenticated();
  }

  isAdmin():boolean{
    return this.authGuard.isAdmin();
  }

}
