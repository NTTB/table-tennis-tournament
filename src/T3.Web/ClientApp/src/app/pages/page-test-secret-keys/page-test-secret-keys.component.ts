import { Component, OnInit } from '@angular/core';
import {SecretNoteService} from "../../secret-notes/secret-note.service";

@Component({
  selector: 'app-page-test-secret-keys',
  templateUrl: './page-test-secret-keys.component.html',
  styleUrls: ['./page-test-secret-keys.component.css']
})
export class PageTestSecretKeysComponent implements OnInit {
  text = "Hello world";
  output:any = "Output";
  constructor(
    private readonly secretNoteService: SecretNoteService,
  ) { }

  ngOnInit(): void {
  }

  async send(){
    const encryptIv = window.crypto.getRandomValues(new Uint8Array(16));
    const wrapIv = window.crypto.getRandomValues(new Uint8Array(16));

    const signKeys = await createSignKeys();
    const wrappingKeys = await createWrappingKeys();
    const encryptKey = await createEncryptionKey();
    const encryptedData = await encrypt(this.text, encryptKey,encryptIv);

    const wrappedKey = await wrapKeys(encryptKey, wrappingKeys, wrapIv);
    const unwrappedKey = await unwrapKeys(wrappedKey, wrappingKeys, wrapIv);

    const decryptData  = await decrypt(encryptedData, unwrappedKey, encryptIv);

    this.output = arr2str(decryptData);
  }
}


async function createEncryptionKey(){
  const iv = window.crypto.getRandomValues(new Uint8Array(16));

  return await window.crypto.subtle.generateKey(
    {
      name: "AES-CBC",
      length: 256,
    },
    true,
    ["encrypt", "decrypt"]
  );
}
async function createWrappingKeys() {
  return await window.crypto.subtle.generateKey({
    name: "AES-CBC",
    length: 256,
  }, true,
    ["wrapKey", "unwrapKey"]);
}
async function createSignKeys(){
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

async function sign(message:string, keys: CryptoKeyPair){
  return await window.crypto.subtle.sign(
    { name: 'RSASSA-PKCS1-v1_5' },
    keys.privateKey,
    new TextEncoder().encode(message)
  );
}

async function encrypt(message:string, key: CryptoKey, iv: Uint8Array){
  return (await window.crypto.subtle.encrypt(
    {name: "AES-CBC", iv},
    key,
    new TextEncoder().encode(message)
  )) as ArrayBuffer;
}
async function decrypt(message:BufferSource, key:CryptoKey, iv: Uint8Array){
  return (await window.crypto.subtle.decrypt(
    {name: "AES-CBC", iv  },
    key,
    message
  )) as ArrayBuffer;
}

async function wrapKeys(key: CryptoKey, wrappingkey:CryptoKey, iv: Uint8Array){
  return await window.crypto.subtle.wrapKey(
    "jwk",
    key,
    wrappingkey,
    {
      name: "AES-CBC",
       iv,
    }
  );
}

async function unwrapKeys(key: BufferSource, keys:CryptoKey, iv: Uint8Array){
  return await window.crypto.subtle.unwrapKey(
    "jwk",
    key,
    keys,
    {name: "AES-CBC", iv},
    {
      name: "AES-CBC",
      length: 256,

    },
    true,
    [ "encrypt","decrypt"]
  );
}


function arr2str(arr:ArrayBuffer) {
  return String.fromCharCode(...new Uint8Array(arr));
}
