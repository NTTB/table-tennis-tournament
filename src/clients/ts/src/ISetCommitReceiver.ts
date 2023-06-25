import { SetCommitMessage } from "./models/SetCommitMessage";


export interface ISetCommitReceiver {
  onCommitReceived(message: SetCommitMessage): void;
}
