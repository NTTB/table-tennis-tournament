import { IHttpConnectionOptions } from "@microsoft/signalr";
import { IDataStore } from "./IDataStore";

export interface ConfigOverrides {
  signalR?: IHttpConnectionOptions;
  dataStore?: IDataStore;
}

export interface Config {
  urlPrefix: string; // Can be "" if local

  /**
   * Service that returns the JWT token for the current user.
   * @returns {Promise<string|undefined>} A promise that resolves to the JWT token, or null if not authenticated.
   */
  getJwtToken: () => Promise<string|undefined>;

  overrides?: ConfigOverrides;
}