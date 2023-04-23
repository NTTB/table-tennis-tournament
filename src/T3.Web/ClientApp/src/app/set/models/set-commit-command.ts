import {Score} from "./score";
import {PlayerId} from "./typed-ids";
import {PlayerView} from "./player-view";

interface TypedSetCommitCommand<T extends string> {
  type: T;
}

export interface NoOpCommand extends TypedSetCommitCommand<'NoOp'> {
  type: 'NoOp';
}

export interface SetHomePlayersCommand extends TypedSetCommitCommand<'SetHomePlayers'> {
  type: 'SetHomePlayers';
  homePlayers: PlayerView[];
}

export interface SetAwayPlayersCommand extends TypedSetCommitCommand<'SetAwayPlayers'> {
  type: 'SetAwayPlayers';
  awayPlayers: PlayerView[];
}

export interface SetInitialServiceCommand extends TypedSetCommitCommand<'SetInitialService'> {
  type: 'SetInitialService';
  servingPlayer: PlayerId;
  receivingPlayer: PlayerId;
}

export interface SetCurrentServiceCommand extends TypedSetCommitCommand<'SetCurrentService'> {
  type: 'SetCurrentService';
  servingPlayer: PlayerId;
  receivingPlayer: PlayerId;
}

export interface ChangeSetScoreCommand extends TypedSetCommitCommand<'SetScoreChange'> {
  type: 'SetScoreChange';
  setScoreDelta: Score;
}

export type SetCommitCommand = NoOpCommand
  | SetHomePlayersCommand
  | SetAwayPlayersCommand
  | SetInitialServiceCommand
  | SetCurrentServiceCommand
  | ChangeSetScoreCommand
  ;
