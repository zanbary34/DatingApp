import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  standalone: true,
  imports: [BsDatepickerModule, NgIf, ReactiveFormsModule],
  templateUrl: './date-picker.component.html',
  styleUrl: './date-picker.component.css'
})
export class DatePickerComponent implements ControlValueAccessor {
  label = input<string>('');
  maxDate = input<Date>();
  bsconfig?: Partial<BsDatepickerConfig>;

  constructor(@Self() public ngcontrol: NgControl) {
    this.ngcontrol.valueAccessor = this;
    this.bsconfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMMM YYYY'
    }
  }

  writeValue(obj: any): void {
  }
  
  registerOnChange(fn: any): void {
  }
  
  registerOnTouched(fn: any): void {
  }
  
  setDisabledState?(isDisabled: boolean): void {
  }

  get control(): FormControl {
    return this.ngcontrol.control as FormControl
  }

}
