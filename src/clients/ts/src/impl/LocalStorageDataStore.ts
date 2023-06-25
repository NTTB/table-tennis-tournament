import { DataStoreKey, AllDataStorageKeys } from "../DataStoreType";
import { IDataStore, } from "../IDataStore";

export class LocalStorageDataStore implements IDataStore {
  setItem(key: DataStoreKey, value: string): void {
    window.localStorage.setItem(key, value);
  }
  getItem(key: DataStoreKey): string | null {
    return window.localStorage.getItem(key);
  }
  removeItem(key: DataStoreKey): void {
    window.localStorage.removeItem(key);
  }
  clear(): void {
    window.localStorage.clear();
  }
  getKeys(): DataStoreKey[] {
    var localStorageKeys: string[] = [];
    for (var i = 0; i < window.localStorage.length; i++) {
      localStorageKeys.push(window.localStorage.key(i)!);
    }

    return AllDataStorageKeys.filter(x => localStorageKeys.includes(x));
  }
}
