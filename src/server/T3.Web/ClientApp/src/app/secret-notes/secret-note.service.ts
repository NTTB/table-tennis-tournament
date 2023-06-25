import {Injectable} from "@angular/core";
import {SecretNote} from "./models/secret-note";
import {SecretNoteApiService} from "./secret-note-api.service";
import {IdentityService} from "../account/identity.service";
import {TimestampService} from "../set/timestamp.service";
import {v4 as uuidv4} from 'uuid';
import {KeyStorageService} from "../account/key-storage.service";
import {CryptoKeyService} from "../account/crypto-key.service";
import {SecretNoteContent, SecretNoteContentPlain} from "./models/secret-note-content";
import {SecretNoteSignature} from "./models/secret-note-signature";

@Injectable({providedIn: 'root'})
export class SecretNoteService {
  constructor(
    private readonly api: SecretNoteApiService,
    private readonly identityService: IdentityService,
    private readonly timestampService: TimestampService,
    private readonly keyStorageService: KeyStorageService,
    private readonly cryptoKeyService: CryptoKeyService,
  ) {
  }

  public async createNewSecretNotePlain(content: string): Promise<SecretNote> {
    const secretNoteContent = await this.createNewPlainContent(content);
    return this.createNew(secretNoteContent);
  }

  private async createNew(content: SecretNoteContent): Promise<SecretNote> {
    const privateKey = await this.keyStorageService.getPrivateKey();
    const publicKey = await this.keyStorageService.getPublicKeyJwk();

    if (!privateKey) throw new Error("No private key known on client side");
    if (!publicKey) throw new Error("No public key known on client side");

    const signature = await this.createSignature(privateKey, publicKey, content);

    return {
      content,
      signature,
    };
  }

  private async createSignature(privateKey: CryptoKey, publicKey: JsonWebKey, content: SecretNoteContent): Promise<SecretNoteSignature> {
    const payload = JSON.stringify(content);
    const signature = await this.cryptoKeyService.sign(privateKey, payload);
    const signatureBase64 = btoa(String.fromCharCode(...new Uint8Array(signature)));
    return {
      version: 'v1',
      payload,
      signature: signatureBase64,
      publicKey: JSON.stringify(publicKey),
    };
  }

  private async createNewPlainContent(content: string): Promise<SecretNoteContentPlain> {
    const identity = this.identityService.userId;
    if (identity == undefined) throw new Error("Identity not set");

    const timestamp = await this.timestampService.getTimestamp();

    return {
      id: {value: uuidv4()},
      versionId: {value: uuidv4()},
      type: "Plain",
      createdAt: timestamp,
      userId: {value: identity},
      content: content,
    };
  }
}
