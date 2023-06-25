import { Timestamp } from "./Timestamp";
import { PenaltyEventId, PlayerId } from "./typed-ids";
import { PenaltyCard } from "./PenaltyCard";
import { Offense } from "./Offense";

export interface PenaltyEvent {
  penaltyEventId: PenaltyEventId;
  timestamp: Timestamp;
  playerId: PlayerId;
  penaltyCard: PenaltyCard;
  offenses: Offense[];
}
