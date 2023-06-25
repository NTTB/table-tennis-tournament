import { DataStoreKey } from "./DataStoreType";

export interface IDataStore {
  setItem(key: DataStoreKey, value: string): void;
  getItem(key: DataStoreKey): string | null;
  removeItem(key: DataStoreKey): void;
  clear(): void;
  getKeys(): DataStoreKey[];
}
