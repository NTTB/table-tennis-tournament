export interface SecretNoteSignature {
  /** The way the signature is encoded. */
  version: SecretNoteSignatureVersion;

  /** The payload that was signed, when decoded, it should match the actual (except the signature). */
  payload: string;

  /** The signature of the payload. */
  signature: string;

  /** The public key used to sign the payload. */
  publicKey: string;
}

export type SecretNoteSignatureVersion = 'v1';
