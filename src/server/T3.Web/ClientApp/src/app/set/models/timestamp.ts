import {ClientOffset} from "./client-offset";
import {ServerTimestamp} from "./server-timestamp";

export interface Timestamp {
  serverTimestamp: ServerTimestamp;
  clientOffset: ClientOffset;
}
