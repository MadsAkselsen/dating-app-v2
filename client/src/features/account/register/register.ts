import { Component, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds;
  private accountService = inject(AccountService);

  register() {
    this.accountService.register(this.creds).subscribe({
      next: () => {
        this.cancel();
      },
      error: () => {
        console.log("error");
      }
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  // protected registerForm = new FormGroup({
  //   displayName: new FormControl('', [Validators.required]),
  //   username: new FormControl('', [Validators.required]),
  //   password: new FormControl('', [Validators.required]),
  // });
}
