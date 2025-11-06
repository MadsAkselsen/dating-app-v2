import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';
import {LikesService} from './likes-service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private accountService = inject(AccountService);
  private likesService = inject(LikesService);

  init() {
    const user = localStorage.getItem('user');
    if (!user) return of(null);
    this.accountService.setCurrentUser(JSON.parse(user));
    this.likesService.getLikeIds();

    return of(null);
  }
}
