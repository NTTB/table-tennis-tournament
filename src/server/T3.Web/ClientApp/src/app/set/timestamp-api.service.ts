import {Injectable} from '@angular/core';
import {ApiService} from "../shared/api.service";
import {Observable} from "rxjs";
import {ServerTimestamp} from "./models/server-timestamp";

@Injectable({
  providedIn: 'root'
})
export class TimestampApiService {
  constructor(
    private readonly api: ApiService,
  ) {
  }

  getLatestTimestamp(): Observable<ServerTimestamp> {
    return this.api.post<ServerTimestamp>('api/timestamp/latest', null);
  }
}
