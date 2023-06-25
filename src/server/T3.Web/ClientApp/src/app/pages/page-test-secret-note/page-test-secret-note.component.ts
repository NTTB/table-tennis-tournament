import {Component, OnInit} from '@angular/core';
import {SecretNoteService} from "../../secret-notes/secret-note.service";
import {SecretNoteApiService} from "../../secret-notes/secret-note-api.service";

@Component({
  selector: 'app-page-test-secret-note',
  templateUrl: './page-test-secret-note.component.html',
  styleUrls: ['./page-test-secret-note.component.css']
})
export class PageTestSecretNoteComponent implements OnInit {
  constructor(
    private readonly secretNoteService: SecretNoteService,
    private readonly secretNoteApiService: SecretNoteApiService,
  ) {
  }

  content = "";
  response: object | undefined = undefined;

  ngOnInit(): void {
  }

  async onSubmit() {
    const secretNote = await this.secretNoteService.createNewSecretNotePlain(this.content);
    this.secretNoteApiService.create(secretNote).subscribe(response => {
      this.response = response;
    });
  }

}
