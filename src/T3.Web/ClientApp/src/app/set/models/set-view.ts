import {Score} from "./score";
import {PlayerView} from "./player-view";
import {PlayerId, WatchId} from "./typed-ids";
import {Timestamp} from "./timestamp";

export interface SetView {
  gamesWon: Score;

  homePlayers: PlayerView[];
  awayPlayers: PlayerView[];

  games: GameView[];

  setWatches: WatchView[];
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

export type WatchState = 'Ticking' | 'Pausing';
