import {Injectable} from '@angular/core';
import {SetCommit} from "../set/models/set-commit";
import {SetCommitSignature} from "../set/models/set-commit-signature";

/**
 * A service for generating and restoring the cryptographic key
 */
@Injectable({
  providedIn: 'root'
})
export class CryptoKeyService {
  /**
   * Serializes a CryptoKey into a JsonWebKey.
   * @param key The CryptoKey to serialize.
   * @returns The serialized key
   */
  public async serialize(key: CryptoKey): Promise<JsonWebKey> {
    return await window.crypto.subtle.exportKey('jwk', key);
  }

  /**
   * Parses a JsonWebKey into a CryptoKey.
   * @param key The JsonWebKey to parse.
   * @param usage The usage of the key. Either 'sign' or 'verify', use 'sign' for private keys and 'verify' for public keys.
   */
  public async parse(key: JsonWebKey, usage: 'sign' | 'verify'): Promise<CryptoKey> {
    return await window.crypto.subtle.importKey(
      'jwk',
      key,
      {name: 'RSASSA-PKCS1-v1_5', hash: {name: 'SHA-256'},},
      true,
      [usage]
    );
  }

  public async createSignature(privateKey: CryptoKey, publicKey: JsonWebKey, commit: Omit<SetCommit, 'signature'>): Promise<SetCommitSignature> {
    var clone = JSON.parse(JSON.stringify(commit));
    delete clone.signature; // Remove the signature from the commit, since we are signing the commit.
    const payload = JSON.stringify(clone);
    const signature = await this.sign(privateKey, payload);
    const signatureBase64 = btoa(String.fromCharCode(...new Uint8Array(signature)));
    return {
      version: 'v1',
      payload,
      signature: signatureBase64,
      publicKey: JSON.stringify(publicKey),
    };
  }

  public async generateKey(): Promise<CryptoKeyPair> {
    // Generate a new key pair.
    return await window.crypto.subtle.generateKey(
      {
        name: 'RSASSA-PKCS1-v1_5',
        modulusLength: 2048, //can be 1024, 2048, or 4096
        publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
        hash: {name: 'SHA-256'}, //can be "SHA-1", "SHA-256", "SHA-384", or "SHA-512"
      },
      true, //whether the key is extractable (i.e. can be used in exportKey)
      ['sign', 'verify'] //can be any combination of "sign" and "verify"
    );
  }

  public  async sign(privateKey: CryptoKey, message: string): Promise<ArrayBuffer> {
    // Sign the message with the private key.
    return await window.crypto.subtle.sign(
      {
        name: 'RSASSA-PKCS1-v1_5',
      },
      privateKey,
      new TextEncoder().encode(message)
    );
  }
}
