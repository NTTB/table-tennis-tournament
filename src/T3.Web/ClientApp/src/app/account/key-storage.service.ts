import { Injectable } from '@angular/core';
import { CryptoKeyService } from './crypto-key.service';

const PublicKeyPath = "t3-publicKey";
const PrivateKeyPath = "t3-privateKey";

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
    this.setInMemory(keys.privateKey, keys.publicKey);

    var privateKeyJwk = await this.cryptoKeyService.serialize(keys.privateKey);
    var publicKeyJwk = await this.cryptoKeyService.serialize(keys.publicKey);
    localStorage.setItem(PublicKeyPath, JSON.stringify(publicKeyJwk));
    localStorage.setItem(PrivateKeyPath, JSON.stringify(privateKeyJwk));
  }

  setInMemory(privateKey: CryptoKey, publicKey: CryptoKey) {
    this.privateKey = privateKey;
    this.publicKey = publicKey;
  }

  async getPrivateKey() {
    if (!this.privateKey) {
      var pkJson = localStorage.getItem(PrivateKeyPath);
      if (pkJson) {
        var pkJwk = JSON.parse(pkJson);
        this.privateKey = await this.cryptoKeyService.parse(pkJwk, 'sign');
      }
    }

    return this.privateKey;
  }

  getPublicKeyJwk() {
    if (!this.publicKey) {
      var pkJson = localStorage.getItem(PublicKeyPath);
      if (pkJson) {
        return JSON.parse(pkJson);
      }
    }
  }

  async getPublicKey() {
    var pkJwk = this.getPublicKeyJwk();
    if (pkJwk) {
      this.publicKey = await this.cryptoKeyService.parse(pkJwk, 'verify');
    }

    return this.publicKey;
  }

}
