import {SetCommitHeader} from './set-commit-header';
import {SetCommitBody} from "./set-commit-body";
import {SetCommitSignature} from "./set-commit-signature";
import {SetView} from "./set-view";

export interface SetCommit {
  header: SetCommitHeader;
  body: SetCommitBody;
  view: SetView;
  signature: SetCommitSignature;
}
