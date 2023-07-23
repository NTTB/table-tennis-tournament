import { GameView } from "./GameView";
import { PenaltyEvent } from "./PenaltyEvent";
import { Score } from "./Score";
import { TeamView } from "./TeamView";
import { WatchView } from "./WatchView";

export interface SetView {
  gamesWon: Score;
  homeTeam?: TeamView;
  awayTeam?: TeamView;
  games: GameView[];
  setWatches: WatchView[];
  penaltyEvents: PenaltyEvent[];
}
