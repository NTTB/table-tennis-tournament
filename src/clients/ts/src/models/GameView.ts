import { Score } from "./Score";
import { PlayerId } from "./typed-ids";
import { WatchView } from "./WatchView";


export interface GameView {
  initialServer?: PlayerId;
  initialReceiver?: PlayerId;
  currentServer?: PlayerId;
  currentReceiver?: PlayerId;
  points: Score;
  watches: WatchView[];
}
