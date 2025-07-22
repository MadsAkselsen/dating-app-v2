import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [CommonModule],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css'
})
export class ServerError {
  protected error: ApiError;
  private router = inject(Router);
  protected showDetails = false;

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.['error']
  }

  detailsToggle() {
    this.showDetails = !this.showDetails;
  }

}
