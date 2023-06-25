import { TeamId } from "./typed-ids";
import { PlayerView } from "./PlayerView";

export interface TeamView {
  teamId: TeamId;
  displayName: string;
  players: PlayerView[];
}
