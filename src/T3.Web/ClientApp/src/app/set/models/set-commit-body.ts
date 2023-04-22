import {Score} from "./score";

interface SetCommitBodyBase<T extends string> {
  type: T;
}

export interface SetCommitBodyNoOp extends SetCommitBodyBase<'NoOp'> {
  type: 'NoOp';
}

export interface SetCommitBodySetScoreChange extends SetCommitBodyBase<'SetScoreChange'> {
  type: 'SetScoreChange';
  setScoreDelta: Score;
}

export type SetCommitBody = SetCommitBodyNoOp
  | SetCommitBodySetScoreChange
