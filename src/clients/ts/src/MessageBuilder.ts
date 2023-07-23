import { Message } from "./models/Message";

export class MessageBuilder {
  public async buildMessage<T>(content: T, keyPair: CryptoKeyPair): Promise<Message<T>> {
    const payload = JSON.stringify(content);

    const signatureArrayBufferPromise = MessageBuilder.sign(keyPair.privateKey, payload);
    const publicKeyPromise = window.crypto.subtle.exportKey('jwk', keyPair.publicKey);

    const signatureArrayBuffer = await signatureArrayBufferPromise;
    var publicKey = await publicKeyPromise;

    const signature = btoa(String.fromCharCode(...new Uint8Array(signatureArrayBuffer)));
    
    return {
      content: content,
      signature: {
        version: "V1",
        payload,
        signature,
        publicKey: JSON.stringify(publicKey),
      }
    };
  }

  private static async sign(privateKey: CryptoKey, message: string): Promise<ArrayBuffer> {
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