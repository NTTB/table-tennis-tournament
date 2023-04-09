import { Component, Input, OnInit } from '@angular/core';
import { SetCommitService } from "../set-commit.service";
import { SetCommit } from "../models/set-commit";
import { TimestampService } from "../timestamp.service";
import { v4 as uuidv4, NIL as guidZero } from 'uuid';
@Component({
  selector: 'app-set-view',
  templateUrl: './set-view.component.html',
  styleUrls: ['./set-view.component.css']
})
export class SetViewComponent implements OnInit {

  constructor(
    private readonly setCommitService: SetCommitService,
    private readonly timestampService: TimestampService) {
  }

  @Input() setId?: string;

  lastMessage$ = this.setCommitService.messages$;
  lastState$ = this.setCommitService.state$;


  ngOnInit(): void {
    this.setCommitService.start().then(()=>{
      this.setCommitService.addSetWatch({value: this.setId!}).then(()=> console.log("Subscribed"));

    });
  }

  async sendUpdate() {
    if(!this.setId) return;

    const commit: SetCommit = {
      header: {
        commitId: { value: uuidv4() },
        setId: { value: this.setId },
        previousCommitId: undefined,
        author: {
          userId: { value: guidZero },
          sessionId: { value: guidZero },
          displayName: "test",
          clientApp: undefined,
          deviceName: undefined
        },
        createdAt: await this.timestampService.getTimestamp(),
      },
      body: { type: 'NoOp' },
      view: {
        gamesWon: { home: 0, away: 0 },
      },
      signature: { value: 'temp' }
    };

    await this.setCommitService.push(commit);
  }

}
