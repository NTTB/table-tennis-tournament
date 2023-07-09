import { ServerTimestamp } from "./models/ServerTimestamp";
import { Timestamp } from "./models/Timestamp";
import { IHttpClient } from "./HttpClient";
import { IStartable } from "./IStartable";

export interface IClockService {
  /**
   * Will retrieve the current timestamp from the server (and block if not yet started)
   */
  getTimestamp(): Promise<Timestamp>;
}

export class ClockService implements IClockService, IStartable {
  private latest?: {
    serverTimestamp: ServerTimestamp,
    fetchAt: Date,
  };

  constructor(
    private readonly http: IHttpClient,
  ) { }

  /**
   * Get the latest that is locally available (or refresh from the server)
   */
  public async getTimestamp(): Promise<Timestamp> {
    await this.ensureServerIsRecent();
    if (!this.latest) {
      throw new Error("No timestamp available");
    }

    return {
      serverTimestamp: this.latest.serverTimestamp,
      clientOffset: {
        milliseconds: this.differenceInMilliseconds(new Date(), this.latest.fetchAt)
      }
    };
  }

  public async start() {
    await this.ensureServerIsRecent();
  }

  private getLatestTimestamp(): Promise<ServerTimestamp> {
    return this.http.post<ServerTimestamp>('api/timestamp/latest', null);
  }

  private async ensureServerIsRecent() {
    // Check if we have a timestamp that is recent enough
    if (this.latest != null && this.differenceInHours(new Date(), this.latest.fetchAt) < 2) {
      return;
    }

    // Wrap in performance
    const start = performance.now();
    try {
      const result = await this.getLatestTimestamp();
      this.latest = {
        serverTimestamp: result,
        fetchAt: new Date(),
      };
    } catch {
      // If we fail to get a timestamp, we remove the latest timestamp and throw an error
      this.latest = undefined;
      throw new Error("Failed to get a timestamp from the server");
    }

    const end = performance.now();
    const measure = performance.measure('TimestampService.refreshTimestamp', { start, end });
    if (measure.duration > 1000) {
      console.warn("The refresh to the server took more than 1000 milliseconds", measure);
    }
  }

  private differenceInMilliseconds(a: Date, b: Date) {
    return a.getTime() - b.getTime();
  }

  private differenceInHours(a: Date, b: Date) {
    return (a.getTime() - b.getTime()) / 36e5;
  }
}