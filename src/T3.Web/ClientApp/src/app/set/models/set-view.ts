import {Score} from "./score";
import {PlayerView} from "./player-view";
import {PlayerId} from "./typed-ids";

export interface SetView {
  gamesWon: Score;

  homePlayers: PlayerView[];
  awayPlayers: PlayerView[];

  games: GameView[];
}

export interface GameView {
  initialServer?: PlayerId;
  initialReceiver?: PlayerId;

  currentServer?: PlayerId;
  currentReceiver?: PlayerId;

  points: Score;
}

