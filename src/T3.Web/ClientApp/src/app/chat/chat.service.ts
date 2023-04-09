import {Injectable} from '@angular/core';
import {Observable, TeardownLogic} from 'rxjs';
import * as signalR from "@microsoft/signalr";
import {LogLevel} from "@microsoft/signalr";

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private connection = new signalR.HubConnectionBuilder()
    .configureLogging(LogLevel.Trace)
    .withUrl("/hubs/chat")
    .build();

  public messages$ = new Observable<{ username: string, message: string }>((observer): TeardownLogic => {
    console.log("Subscribing to chat hub");
    this.connection.on('ReceiveMessage', (username, message) => {
      observer.next({username, message});
    });

    this.connection.onclose((error) => {
      observer.error(error);
    });

    console.log("Starting connection to chat hub");
    this.connection.start().then(() => {
      console.log("Connected to chat hub");
    }).catch((e) => {
      console.error(e);
      observer.error(e);
    });

    console.log("Returning unsubscribe function from chat hub");
    return () => {
      console.log("Unsubscribing from chat hub");
      this.connection.stop();
    }
  });

  constructor() {
  }

  Send(user: string, message: string) {
    this.connection.invoke("SendMessage", user, message).catch((e) => {
      console.error(e);
    });
  }
}
