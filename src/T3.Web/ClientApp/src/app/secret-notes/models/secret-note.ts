import {SecretNoteContent} from "./secret-note-content";
import {SecretNoteSignature} from "./secret-note-signature";

export interface SecretNote {
  content: SecretNoteContent;
  signature: SecretNoteSignature;
}
