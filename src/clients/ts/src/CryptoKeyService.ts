export interface ICryptoKeyService {
  /**
   * Serializes a CryptoKey into a JsonWebKey.
   * @param key The CryptoKey to serialize.
   * @returns The serialized key
   */
  serialize(key: CryptoKey): Promise<JsonWebKey>;

  /**
   * Parses a JsonWebKey into a CryptoKey.
   * @param key The JsonWebKey to parse.
   * @param usage The usage of the key. Either 'sign' or 'verify', use 'sign' for private keys and 'verify' for public keys.
   */
  parse(key: JsonWebKey, usage: 'sign' | 'verify'): Promise<CryptoKey>;

  /**
   * Generates a new key pair for signing and verifying.
   * @returns The generated key pair.
   * @remarks The key pair is generated using the RSASSA-PKCS1-v1_5 algorithm with SHA-256.
   */
  generateKey(): Promise<CryptoKeyPair>;

  /**
   * Signs a message using a private key.
   * @param privateKey The private key to sign the message with.
   * @param message The message to sign.
   * @returns The signature.
   * @remarks The message is signed using the RSASSA-PKCS1-v1_5 algorithm with SHA-256.
   */
  sign(privateKey: CryptoKey, message: string): Promise<ArrayBuffer>;
}

export class CryptoKeyService implements ICryptoKeyService {
  public async serialize(key: CryptoKey): Promise<JsonWebKey> {
    return await window.crypto.subtle.exportKey('jwk', key);
  }

  public async parse(key: JsonWebKey, usage: 'sign' | 'verify'): Promise<CryptoKey> {
    return await window.crypto.subtle.importKey(
      'jwk',
      key,
      { name: 'RSASSA-PKCS1-v1_5', hash: { name: 'SHA-256' }, },
      true,
      [usage]
    );
  }

  public async generateKey(): Promise<CryptoKeyPair> {
    // Generate a new key pair.
    return await window.crypto.subtle.generateKey(
      {
        name: 'RSASSA-PKCS1-v1_5',
        modulusLength: 2048, //can be 1024, 2048, or 4096
        publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
        hash: { name: 'SHA-256' }, //can be "SHA-1", "SHA-256", "SHA-384", or "SHA-512"
      },
      true, //whether the key is extractable (i.e. can be used in exportKey)
      ['sign', 'verify'] //can be any combination of "sign" and "verify"
    );
  }

  public async sign(privateKey: CryptoKey, message: string): Promise<ArrayBuffer> {
    return await window.crypto.subtle.sign(
      {
        name: 'RSASSA-PKCS1-v1_5',
      },
      privateKey,
      new TextEncoder().encode(message)
    );
  }
}
