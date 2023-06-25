import { Timestamp } from "./Timestamp";
import { WatchState } from "./WatchState";

export interface WatchChange {
  state: WatchState;
  timestamp: Timestamp;
}
