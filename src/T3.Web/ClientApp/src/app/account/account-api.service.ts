import {ApiService} from "../shared/api.service";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";
export interface AccountKeysResponse {
  publicKey: string;
  createdAtUtc: Date;
  revokedAtUtc: Date;
}

export interface AccountRevokeKeyRequest{
  publicKey: string;
}
export interface AccountAddKeyRequest
{
  publicKey: string;
}
@Injectable()
export class AccountApiService {
  constructor(
    private readonly apiService: ApiService,
  ) {
  }

  public create(username: string, password: string): Observable<void> {
    return this.apiService.post<void>('api/account/create', {
      username,
      password
    });
  }

  public login(username: string, password: string): Observable<{ jwtToken: string }> {
    return this.apiService.post<{ jwtToken: string }>('api/account/login', {
      username,
      password
    });
  }

  public getKeys(): Observable<AccountKeysResponse[]> {
    return this.apiService.get<AccountKeysResponse[]>('api/account/keys');
  }

  public revokeKey(request: AccountRevokeKeyRequest): Observable<void> {
    return this.apiService.post<void>('api/account/revoke-key', request);
  }

  public addKey(request: AccountAddKeyRequest): Observable<void> {
    return this.apiService.post<void>('api/account/add-key', request);
  }
}
