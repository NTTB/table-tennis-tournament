import { Timestamp } from "./models/Timestamp";

export interface IClockService {
  /**
   * Will retrieve the current timestamp from the server.
   */
  start(): Promise<void>;

  /**
   * Will stop the clock service from updating the timestamp.
   */
  stop(): Promise<void>;

  /**
   * Will retrieve the current timestamp from the server (and block if not yet started)
   */
  getTimestamp(): Promise<Timestamp>;
}
