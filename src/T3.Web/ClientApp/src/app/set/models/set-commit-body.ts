interface SetCommitBodyBase<T extends string> {
  type: T;
}

export interface SetCommitBodyNoOp extends SetCommitBodyBase<'NoOp'> {
  type: 'NoOp';
}

export type SetCommitBody = SetCommitBodyNoOp;
