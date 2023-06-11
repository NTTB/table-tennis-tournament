import {Timestamp} from "../../set/models/timestamp";
import {UserId} from "../../set/models/typed-ids";
import {SecretNoteId, SecretNoteVersionId} from "./typed-ids";

interface TypedSecretNoteContent<T extends string> {
  id: SecretNoteId;
  versionId: SecretNoteVersionId;
  type: T;
  createdAt: Timestamp;
  userId: UserId;
}

export interface SecretNoteContentPlain extends TypedSecretNoteContent<"Plain"> {
  content: string;
}

export type SecretNoteContent = SecretNoteContentPlain;
