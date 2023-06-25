import { MessageSignature } from "./MessageSignature";

export interface Message<T> {
  content: T;
  signature: MessageSignature;
}
