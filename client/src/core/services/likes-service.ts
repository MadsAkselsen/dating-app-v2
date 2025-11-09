import {inject, Injectable, signal} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Member, MemberParams} from '../../types/member';
import {PaginatedResult} from '../../types/pagination';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<string[]>([])

  toggleLike(targetMemberId: string) {
    return this.http.post(`${this.baseUrl}likes/${targetMemberId}`, {})
  }

  getLikes(memberParams: MemberParams, predicate: string) {
    let params = new HttpParams();

    params = params.append('predicate', predicate);
    params = params.append('pageSize', memberParams.pageSize);
    params = params.append('pageNumber', memberParams.pageNumber);

    return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'likes', {params});
  }

  getLikeIds() {
    return this.http.get<string[]>(this.baseUrl + 'likes/list').subscribe({
      next: ids => this.likeIds.set(ids)
    })
  }

  clearLikeIds() {
    this.likeIds.set([]);
  }
}
