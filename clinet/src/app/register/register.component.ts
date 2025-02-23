import {
  Component,inject,OnInit, output,} from '@angular/core';
import {
  AbstractControl,FormBuilder,FormGroup,
  ReactiveFormsModule,ValidatorFn,Validators,} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder)
  cancelRegister = output<boolean>();
  maxdate = new Date;
  registerForm: FormGroup = new FormGroup({});
  private router = inject(Router);
  validationErrors: string[] | undefined

  ngOnInit(): void {
    this.initializeform();
    this.maxdate.setFullYear(this.maxdate.getFullYear() - 18)
  }

  initializeform() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      country: ['', Validators.required],
      city: ['', Validators.required],
      password: ['', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(8),
      ]],
      confirmPassword: ['', [
        Validators.required,
        this.matchValues('password'),
      ]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value 
        ? null
        : { isMatching: true };
    };
  }
  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value)
    this.registerForm.patchValue({dateOfBirth: dob});
    this.accountService.register(this.registerForm.value).subscribe({
      next: _ => this.router.navigateByUrl('/members') ,
      error: (error) => this.validationErrors = error,
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob:string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0, 10);
  }
}
