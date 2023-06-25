import {Injectable} from "@angular/core";
import {SecretNoteContent} from "./models/secret-note";

export interface SecretNoteKeyChain {
  wrappedKey: string; // This is the key (wrapped
  digestUnw: string;
}

/**
 * Responsible for generating and storing the keys used to encrypt and decrypt secret notes.
 */
@Injectable()
export class SecretNoteKeysService {
  constructor(
  ) {}


  /**
   * Generates a new key chain, the key chain
   */
  public generateNewKeyChain(): SecretNoteKeyChain {
  }
}
