import { ClientInfo } from "./ClientInfo";
import { SessionId, UserId } from "./typed-ids";

export interface SetCommitAuthor {
  userId: UserId;
  sessionId: SessionId;
  displayName: string;
  deviceName?: string;
  clientInfo?: ClientInfo;
}
