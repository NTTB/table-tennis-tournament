export interface SetCommitSignature {
  /**
   * The way the signature is encoded.
   */
  version: setCommitSignatureVersion;

  /**
   * The payload that was signed, when decoded, it should match the actual (except the signature).
   * @see SetCommit
   */
  payload: string;

  /**
   * The signature of the payload.
   */
  signature: string;

  /**
   * The public key used to sign the payload.
   * This way the server can reject the entire commit if the public key is not known.
   * Since the user is logged in we might not even need it
   */
  publicKey: string;
}


export type setCommitSignatureVersion = 'v1';
