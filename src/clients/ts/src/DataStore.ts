/** @file
 * This file contains the DataStore interface and its implementation.
 */

/**
 * The keys that are used in the data store.
 */
export type DataStoreKey = 'keys' | 'identity';
export const AllDataStorageKeys = Object.freeze<DataStoreKey[]>(['keys', 'identity']);

/**
 * Used to store data.
 */
export interface IDataStore {

  /**
   * Sets the item in the store.
   * @param key The key to set in the store.
   * @param value The value to set in the store.
   */
  setItem(key: DataStoreKey, value: string): void;

  /**
   * Returns the item from the store if it exists.
   * @param key The key to get from the store.
   */
  getItem(key: DataStoreKey): string | null;
  
  /**
   * Removes the item from the store if it exists.
   * @param key The key to remove from the store.
   */
  removeItem(key: DataStoreKey): void;
}

/**
 * The default implementation of the IDataStore interface.
 */
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
}
