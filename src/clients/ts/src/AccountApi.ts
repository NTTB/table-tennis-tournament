import { IHttpClient } from "./HttpClient";

export interface AccountKeysResponse {
  publicKey: string;
  createdAtUtc: Date;
  revokedAtUtc: Date;
}

export interface AccountRevokeKeyRequest {
  publicKey: string;
}
export interface AccountAddKeyRequest {
  publicKey: string;
}

export interface IAccountApi {
  create(username: string, password: string): Promise<void>;
  login(username: string, password: string): Promise<{ jwtToken: string }>;
  getKeys(): Promise<AccountKeysResponse[]>;
  revokeKey(request: AccountRevokeKeyRequest): Promise<void>;
  addKey(request: AccountAddKeyRequest): Promise<void>;
}

export class AccountApi implements IAccountApi {
  constructor(
    private readonly http: IHttpClient,
  ) {
  }

  public create(username: string, password: string): Promise<void> {
    return this.http.post<void>('api/account/create', {
      username,
      password
    });
  }

  public login(username: string, password: string): Promise<{ jwtToken: string }> {
    return this.http.post<{ jwtToken: string }>('api/account/login', {
      username,
      password
    });
  }

  public getKeys(): Promise<AccountKeysResponse[]> {
    return this.http.get<AccountKeysResponse[]>('api/account/keys');
  }

  public revokeKey(request: AccountRevokeKeyRequest): Promise<void> {
    return this.http.post<void>('api/account/revoke-key', request);
  }

  public addKey(request: AccountAddKeyRequest): Promise<void> {
    return this.http.post<void>('api/account/add-key', request);
  }
}
