import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private accountService = inject(AccountService);

  init() {
    const user = localStorage.getItem('user');
    if (!user) return of(null);
    this.accountService.setCurrentUser(JSON.parse(user));

    return of(null);
  }
}
