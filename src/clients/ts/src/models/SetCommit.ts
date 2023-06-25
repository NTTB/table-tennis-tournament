import { SetCommitCommand } from "./SetCommitCommand";
import { SetCommitHeader } from "./SetCommitHeader";
import { SetView } from "./SetView";

export interface SetCommit {
  header: SetCommitHeader;
  commands: SetCommitCommand[];
  view: SetView;
}

