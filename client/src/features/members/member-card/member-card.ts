import {Component, computed, inject, input} from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import {LikesService} from '../../../core/services/likes-service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css'
})
export class MemberCard {
  member = input.required<Member>();
  private likeService = inject(LikesService);
  protected hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id))
}
