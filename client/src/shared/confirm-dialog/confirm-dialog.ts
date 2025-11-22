import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ConfirmDialogService } from '../../core/services/confirm-dialog-service';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.css'
})
export class ConfirmDialog {
  @ViewChild('dialogRef') dialogRef!: ElementRef<HTMLDialogElement>;
  message = 'Are you sure?';
  private resolveFn: ((value: boolean) => void) | null = null;

  constructor() { 
    inject(ConfirmDialogService).register(this);
  }

  open(message: string): Promise<boolean> {
    this.message = message;
    this.dialogRef.nativeElement.showModal();

    return new Promise<boolean>((resolve) => {
      this.resolveFn = resolve;
    });
  }

  confirm() {
    this.dialogRef.nativeElement.close();
    this.resolveFn?.(true);
    this.resolveFn = null;
  }

  cancel() {
    this.dialogRef.nativeElement.close();
    this.resolveFn?.(false);
    this.resolveFn = null;
  }
}
