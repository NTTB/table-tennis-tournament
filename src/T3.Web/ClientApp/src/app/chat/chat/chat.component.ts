import {Component, OnDestroy, OnInit} from '@angular/core';
import {ChatService} from "../chat.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {
  message: string = "";
  username: string = "";
  messages: { username: string, message: string }[] = [];
  private subs?: Subscription;

  constructor(
    private chatService: ChatService
  ) {
  }

  ngOnDestroy(): void {
    this.subs?.unsubscribe();
  }

  ngOnInit(): void {
    this.subs = this.chatService.messages$.subscribe((message) => {
      this.messages.push(message);
    });
  }

  OnSendClick() {
    if (this.username == "") return;
    if (this.message == "") return;

    this.chatService.Send(this.username, this.message);

    this.message = "";
  }

}
