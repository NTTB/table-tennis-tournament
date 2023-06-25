import { ServerTimestamp } from "./ServerTimestamp";
import { ClientOffset } from "./ClientOffset";

export interface Timestamp {
  serverTimestamp: ServerTimestamp;
  clientOffset: ClientOffset;
}
