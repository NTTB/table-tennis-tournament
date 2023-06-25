import { SetCommitAuthor } from "./SetCommitAuthor";
import { Timestamp } from "./Timestamp";
import { SetCommitId, SetId } from "./typed-ids";

export interface SetCommitHeader {
  commitId: SetCommitId;
  setId: SetId;
  previousCommitId?: SetCommitId;
  author: SetCommitAuthor;
  createdAt: Timestamp;
}
