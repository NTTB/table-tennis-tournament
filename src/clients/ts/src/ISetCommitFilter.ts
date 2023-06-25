import { ISetCommitReceiver } from "./ISetCommitReceiver";

export interface ISetCommitFilter {
  isAllowed(receiver: ISetCommitReceiver): boolean;
}
