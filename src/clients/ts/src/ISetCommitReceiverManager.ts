import { ISetCommitFilter } from "./ISetCommitFilter";
import { ISetCommitReceiver } from "./ISetCommitReceiver";

export interface ISetCommitReceiverCollection {
  setFilter(filter: ISetCommitFilter): void;
  add(receiver: ISetCommitReceiver): void;
  remove(receiver: ISetCommitReceiver): void;
}
