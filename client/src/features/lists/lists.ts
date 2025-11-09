import {Component, inject, OnInit, signal} from '@angular/core';
import {LikesService} from '../../core/services/likes-service';
import {Member, MemberParams} from "../../types/member";
import {MemberCard} from '../members/member-card/member-card';
import {PaginatedResult} from '../../types/pagination';
import {MemberService} from '../../core/services/member-service';
import {Paginator} from '../../shared/paginator/paginator';

@Component({
  selector: 'app-lists',
  imports: [
    MemberCard,
    Paginator
  ],
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists implements OnInit {
  private likesService = inject(LikesService);
  protected members = signal<Member[]>([])
  protected predicate = 'liked';
  protected memberParams = new MemberParams();
  protected paginatedLikes = signal<PaginatedResult<Member> | null>(null);

  tabs = [
    {label: 'Liked', value: 'liked'},
    {label: 'Liked me', value: 'likedBy'},
    {label: 'Mutual', value: 'mutual'},
  ]

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.predicate != predicate) {
      this.predicate = predicate;
      this.memberParams.pageNumber = 1;
      this.loadLikes();
    }
  }

  loadLikes() {
    this.likesService.getLikes(this.memberParams, this.predicate).subscribe({
      next: members => this.paginatedLikes.set(members)
    })
  }

  onPageChange(event: {pageNumber: number, pageSize: number}) {
    this.memberParams.pageSize = event.pageSize;
    this.memberParams.pageNumber = event.pageNumber;
    this.loadLikes();
  }
}
