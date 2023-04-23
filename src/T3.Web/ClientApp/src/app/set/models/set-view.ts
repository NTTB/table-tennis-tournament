import {Score} from "./score";
import {PlayerView} from "./player-view";

export interface SetView {
  gamesWon: Score;

  homePlayers: PlayerView[];
  awayPlayers: PlayerView[];

  currentServer?: PlayerView;
  currentReceiver?: PlayerView;

  initialServer?: PlayerView;
  initialReceiver?: PlayerView;
}
