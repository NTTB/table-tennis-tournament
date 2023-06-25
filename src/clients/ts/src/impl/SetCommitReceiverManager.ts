import { ISetCommitFilter } from "../ISetCommitFilter";
import { ISetCommitReceiver } from "../ISetCommitReceiver";
import { ISetCommitReceiverCollection } from "../ISetCommitReceiverManager";
import { SetCommitMessage } from "../models/SetCommitMessage";


export class SetCommitReceiverCollection implements ISetCommitReceiverCollection, ISetCommitReceiver {
  private readonly receivers: ISetCommitReceiver[] = [];
  private filter: ISetCommitFilter = { isAllowed: () => true };

  setFilter(filter: ISetCommitFilter): void {
    this.filter = filter;
  }

  add(receiver: ISetCommitReceiver): void {
    this.receivers.push(receiver);
  }

  remove(receiver: ISetCommitReceiver): void {
    const index = this.receivers.indexOf(receiver);
    if (index >= 0) {
      this.receivers.splice(index, 1);
    }
  }

  onCommitReceived(message: SetCommitMessage): void {
    if (!this.filter.isAllowed(this)) {
      return;
    }

    for (const receiver of this.receivers) {
      receiver.onCommitReceived(message);
    }
  }
}
