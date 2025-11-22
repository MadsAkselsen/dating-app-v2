import { Component, inject, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { ConfirmDialog } from "../shared/confirm-dialog/confirm-dialog";

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, RouterModule, ConfirmDialog],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected router = inject(Router);
}
