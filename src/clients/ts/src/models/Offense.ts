import { SecretNoteId } from "./typed-ids";
import { OffenseType } from "./OffenseType";

export interface Offense {
  type: OffenseType;

  /**
   * Optional details, is only required when the offense type requires it.
   * Since the details can't be freely shared they are put in a secret note.
   */
  secretNoteId?: SecretNoteId;
}
