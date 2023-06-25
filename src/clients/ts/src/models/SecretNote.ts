import { Timestamp } from "./Timestamp";
import { UserId, SecretNoteId, SecretNoteVersionId } from "./typed-ids";

interface TypedSecretNote<T extends string> {
  id: SecretNoteId;
  versionId: SecretNoteVersionId;
  type: T;
  createdAt: Timestamp;
  userId: UserId;
}

export interface SecretNotePlain extends TypedSecretNote<"Plain"> {
  content: string;
}

export type SecretNote = SecretNotePlain;
