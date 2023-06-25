import {Injectable} from '@angular/core';
import {CryptoKeyService} from './crypto-key.service';

const StorageKey = "t3-key";

interface KeyStorage {
  publicKey: JsonWebKey;
  privateKey: JsonWebKey;
}

@Injectable({
  providedIn: 'root'
})
export class KeyStorageService {
  constructor(
    private readonly cryptoKeyService: CryptoKeyService,
  ) {
  }

  private privateKey?: CryptoKey;
  private publicKey?: CryptoKey;

  async set(keys: CryptoKeyPair) {
    this.privateKey = keys.privateKey;
    this.publicKey = keys.publicKey;

    const privateKeyJwk = await this.cryptoKeyService.serialize(keys.privateKey);
    const publicKeyJwk = await this.cryptoKeyService.serialize(keys.publicKey);

    const storage: KeyStorage = {
      publicKey: publicKeyJwk,
      privateKey: privateKeyJwk,
    };
    localStorage.setItem(StorageKey, JSON.stringify(storage));
  }

  getFromStorage() {
    const storageJson = localStorage.getItem(StorageKey);
    if (storageJson) {
      return JSON.parse(storageJson) as KeyStorage;
    }
    return undefined;
  }

  async getPrivateKey() {
    if (!this.privateKey) {
      var pkJwk = this.getFromStorage()?.privateKey;
      if (pkJwk) {
        this.privateKey = await this.cryptoKeyService.parse(pkJwk, 'sign');
      }
    }

    return this.privateKey;
  }

  getPublicKeyJwk() {
    return this.getFromStorage()?.publicKey;
  }

  hasKeys() {
    return this.getFromStorage() != undefined;
  }
}
