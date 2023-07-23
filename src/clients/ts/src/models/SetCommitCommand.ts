import { PenaltyEvent } from "./PenaltyEvent";
import { Score } from "./Score";
import { TeamView } from "./TeamView";
import { Timestamp } from "./Timestamp";
import { WatchState } from "./WatchState";
import { PlayerId, WatchId, PenaltyEventId } from "./typed-ids";

interface TypedSetCommitCommand<T extends string> {
  type: T;
}

export interface NoOpCommand extends TypedSetCommitCommand<'NoOp'> {
  type: 'NoOp';
}

export interface SetHomeTeamCommand extends TypedSetCommitCommand<'SetHomeTeam'> {
  type: 'SetHomeTeam';
  homeTeam?: TeamView;
}

export interface SetAwayTeamCommand extends TypedSetCommitCommand<'SetAwayTeam'> {
  type: 'SetAwayTeam';
  awayTeam?: TeamView;
}

export interface UpdateSetScoreCommand extends TypedSetCommitCommand<'UpdateSetScore'> {
  type: 'UpdateSetScore';
  setScore: Score;
}

export interface SetInitialServerCommand extends TypedSetCommitCommand<'SetInitialServer'> {
  type: 'SetInitialServer';
  gameIndex: number;
  servingPlayer: PlayerId;
  receivingPlayer: PlayerId;
}

export interface SetCurrentServerCommand extends TypedSetCommitCommand<'SetCurrentServer'> {
  type: 'SetCurrentServer';
  gameIndex: number;
  servingPlayer: PlayerId;
  receivingPlayer: PlayerId;
}

export interface UpdateGameScoreCommand extends TypedSetCommitCommand<'UpdateGameScore'> {
  type: 'UpdateGameScore';
  gameIndex: number;
  gameScore: Score;
}

export interface AddGameCommand extends TypedSetCommitCommand<'AddGame'> {
  type: 'AddGame';
  position: number; // The position in the list of games where the new game should be inserted 0 = first, 1 = second, etc.
  amount: number; // The amount of games to add
}

export interface AddWatchCommand extends TypedSetCommitCommand<'AddWatch'> {
  type: 'AddWatch';
  watchId: WatchId;
  gameIndex?: number;
  key: string;
  maxMilliseconds?: number;
}
export interface UpdateWatchCommand extends TypedSetCommitCommand<'UpdateWatch'> {
  type: 'UpdateWatch';
  watchId: WatchId;
  timestamp: Timestamp;
  newState: WatchState;
}

export interface RemoveWatchCommand extends TypedSetCommitCommand<'RemoveWatch'> {
  type: 'RemoveWatch';
  watchId: WatchId;
}

export interface AddPenaltyEventCommand extends TypedSetCommitCommand<'AddPenaltyEvent'> {
  type: 'AddPenaltyEvent';
  penaltyEvent: PenaltyEvent;
}

export interface UpdatePenaltyEventCommand extends TypedSetCommitCommand<'UpdatePenaltyEvent'> {
  type: 'UpdatePenaltyEvent';
  penaltyEvent: PenaltyEvent;
}

export interface RemovePenaltyEventCommand extends TypedSetCommitCommand<'RemovePenaltyEvent'> {
  type: 'RemovePenaltyEvent';
  penaltyEventId: PenaltyEventId;
}

export type SetCommitCommand = NoOpCommand
  | SetHomeTeamCommand
  | SetAwayTeamCommand
  | UpdateSetScoreCommand
  | SetInitialServerCommand
  | SetCurrentServerCommand
  | UpdateGameScoreCommand
  | AddGameCommand
  | AddWatchCommand
  | UpdateWatchCommand
  | RemoveWatchCommand
  | AddPenaltyEventCommand
  | UpdatePenaltyEventCommand
  | RemovePenaltyEventCommand
  ;
