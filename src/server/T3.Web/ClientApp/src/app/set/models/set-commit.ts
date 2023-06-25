import {SetCommitHeader} from './set-commit-header';
import {SetCommitCommand} from "./set-commit-command";
import {SetCommitSignature} from "./set-commit-signature";
import {SetView} from "./set-view";

export interface SetCommit {
  header: SetCommitHeader;
  commands: SetCommitCommand[];
  view: SetView;
  signature: SetCommitSignature;
}
