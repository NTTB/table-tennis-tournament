import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";

export interface SetFormData {
  displayName: string;
}

@Component({
  selector: 'app-set-form',
  templateUrl: './set-form.component.html',
  styleUrls: ['./set-form.component.css']
})
export class SetFormComponent implements OnInit {
  constructor() {
  }

  @Input() public initialData: SetFormData = {
    displayName: ''
  };

  @Output() submitClicked = new EventEmitter<SetFormData>();

  form = new FormGroup({
    displayName: new FormControl(this.initialData.displayName, [Validators.required]),
  });

  ngOnInit(): void {
    this.form.setValue({
      displayName: this.initialData.displayName
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    const data: SetFormData = {
      displayName: this.form.get('displayName')?.value!
    };

    this.submitClicked.emit(data);
  }
}
