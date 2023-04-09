import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { TimestampApiService } from "./timestamp-api.service";
import { DateTime } from 'luxon';
import { Timestamp } from "./models/timestamp";
import { ServerTimestamp } from "./models/server-timestamp";

@Injectable({
  providedIn: 'root'
})
export class TimestampService {
  private latest?: {
    serverTimestamp: ServerTimestamp,
    fetchAt: DateTime,
  };

  constructor(
    private readonly api: TimestampApiService,
  ) {
  }

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
        milliseconds: -this.latest!.fetchAt.diffNow().milliseconds,
      }
    };
  }

  private async ensureServerIsRecent() {
    // Check if we have a timestamp that is recent enough
    if (this.latest != null && (-this.latest.fetchAt.diffNow().hours) < 2) {
      return;
    }

    // Wrap in performance
    const start = performance.now();
    try {
      const result = await lastValueFrom(this.api.getLatestTimestamp());
      this.latest = {
        serverTimestamp: result,
        fetchAt: DateTime.utc(),
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
}
