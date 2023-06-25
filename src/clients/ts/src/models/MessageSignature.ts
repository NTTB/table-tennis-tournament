export type MessageSignatureVersion = "V1";

export interface MessageSignature {
  version: MessageSignatureVersion;
  payload: string;
  signature: string;
  publicKey: string;
}
