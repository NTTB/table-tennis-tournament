import {Score} from "./score";
import {PlayerView} from "./player-view";
import {PenaltyEventId, PlayerId, WatchId} from "./typed-ids";
import {Timestamp} from "./timestamp";
import {SecretNoteId} from "../../secret-notes/models/typed-ids";

export interface SetView {
  gamesWon: Score;

  homePlayers: PlayerView[];
  awayPlayers: PlayerView[];

  games: GameView[];

  setWatches: WatchView[];

  penaltyEvents: PenaltyEvent[];
}

export interface GameView {
  initialServer?: PlayerId;
  initialReceiver?: PlayerId;

  currentServer?: PlayerId;
  currentReceiver?: PlayerId;

  points: Score;
  watches: WatchView[];
}

export interface WatchView {
  watchId: WatchId;
  key: string;
  changes: WatchChange[];
  maxMilliseconds?: number;
}

export interface WatchChange {
  state: WatchState;
  timestamp: Timestamp;
}

export interface PenaltyEvent {
  penaltyEventId: PenaltyEventId;
  timestamp: Timestamp;
  playerId: PlayerId;
  penaltyCard: PenaltyCard;
  offenses: Offense[];
}

export interface Offense {
  type: OffenseType;

  /**
   * Optional details, is only required when the offense type requires it.
   * Since the details can't be freely shared they are put in a secret note.
   */
  secretNoteId?: SecretNoteId;
}

export interface OffenseType {
  code:string;
  description:string;
  detailsRequired: boolean;
}

export type WatchState = 'Ticking' | 'Pausing';
export type PenaltyCard = 'Yellow' |'YellowAndRed'| 'Red';
