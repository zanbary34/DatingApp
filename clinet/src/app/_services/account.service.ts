import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiurl;
  currentUser = signal<User | null>(null);
  

  login(model: any) {
    return this.http .post<User>(this.baseUrl + '/account/login', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.setCurrentUser(user);
          console.log(this.currentUser()?.username)
        }
      })
    )
  }

  
  register(model: any) {
    return this.http.post<User>(this.baseUrl + '/account/register', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }

  setCurrentUser(user: User | null) {
    this.currentUser.set(user);
  }
  

  logout() {
    localStorage.removeItem('user');
    this.setCurrentUser(null);
  }
}
