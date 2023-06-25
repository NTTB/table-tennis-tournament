import { WatchId } from "./typed-ids";
import { WatchChange } from "./WatchChange";

export interface WatchView {
  watchId: WatchId;
  key: string;
  changes: WatchChange[];
  maxMilliseconds?: number;
}