import {Injectable} from '@angular/core';
import {SetCommitAuthor} from "./models/set-commit-author";
import {SetCommit} from "./models/set-commit";
import {SetCommitBody} from "./models/set-commit-body";
import {SetView} from "./models/set-view";
import {v4 as uuidv4} from 'uuid';
import {TimestampService} from "./timestamp.service";
import {KeyStorageService} from "../account/key-storage.service";
import {CryptoKeyService} from "../account/crypto-key.service";


/**
 * A reusable service to build commits
 */
@Injectable({
  providedIn: 'root'
})
export class SetCommitBuilderService {
  constructor(
    private readonly timestampService: TimestampService,
    private readonly keyStorageService: KeyStorageService,
    private readonly cryptoKeyService: CryptoKeyService,
  ) {
  }

  private _author: SetCommitAuthor = {
    clientApp: {
      name: "T3.Web",
      version: "0.0.0",
    },
    userId: {value: "00000000-0000-0000-0000-000000000000"},
    sessionId: {value: "00000000-0000-0000-0000-000000000000"},
    displayName: "",
  };


  // Should be called after login
  updateAuthor(author: Partial<SetCommitAuthor>) {
    this._author = {...this._author, ...author};

    // Delete any values that are undefined
    for (const keyx in this._author) {
      const key = keyx as keyof SetCommitAuthor;
      if (Object.prototype.hasOwnProperty.call(this._author, key)) {
        if (this._author[key] === undefined) {
          delete this._author[key];
        }
      }
    }
  }

  get author(): SetCommitAuthor {
    return this._author;
  }

  async create(setId: string, body: SetCommitBody, view: SetView, previousCommitId?: string): Promise<SetCommit> {

    const commitWithoutSignature: Omit<SetCommit, 'signature'> = {
      view,
      body,
      header: {
        author: this.author,
        commitId: {value: uuidv4()},
        createdAt: await this.timestampService.getTimestamp(),
        previousCommitId: previousCommitId ? {value: previousCommitId} : undefined,
        setId: {value: setId}
      },
    };

    const privateKey = await this.keyStorageService.getPrivateKey();
    const publicKey = await this.keyStorageService.getPublicKeyJwk();

    if (!privateKey) throw new Error("No private key known on client side");
    if (!publicKey) throw new Error("No public key known on client side");

    const signature = await this.cryptoKeyService.createSignature(privateKey, publicKey, commitWithoutSignature);

    return {
      ...commitWithoutSignature,
      signature
    };

  }
}
