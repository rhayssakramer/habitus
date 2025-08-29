import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import { User } from './models/user.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Habitus';
  currentUser: User | null = null;
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    // Observar mudanças no usuário atual
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  logout(): void {
    this.authService.logout();
  }
}
