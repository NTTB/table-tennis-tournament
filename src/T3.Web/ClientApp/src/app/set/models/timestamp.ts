import {ClientOffset} from "./client-offset";
import {ServerTimestamp} from "./server-timestamp";

export interface Timestamp {
  server: ServerTimestamp;
  client: ClientOffset;
}
