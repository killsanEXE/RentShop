import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() label: string;
  currentYear = new Date();
  maxDate: Date = new Date(this.currentYear.getFullYear() - 16, this.currentYear.getMonth(), this.currentYear.getDay());
  minDate: Date = new Date(this.currentYear.getFullYear() - 90, this.currentYear.getMonth(), this.currentYear.getDay());

  constructor(@Self() public ngControl: NgControl){
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {

  }

  registerOnChange(fn: any): void {

  }

  registerOnTouched(fn: any): void {

  }

}