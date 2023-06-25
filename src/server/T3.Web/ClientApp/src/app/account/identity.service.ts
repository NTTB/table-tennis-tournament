import {Injectable} from '@angular/core';
import {map, Observable} from "rxjs";
import {decodeJwt, JWTPayload} from "jose";
import {JwtService} from "../shared/jwt.service";

const StorageKey = 't3-identity';

interface IdentityStorage {
  displayName?: string;
  sessionId?: string;
  deviceName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  private _jwt$: Observable<JWTPayload | undefined>;
  private _userId$: Observable<string | undefined>;

  private _userId: string | undefined;

  public get userId(): string | undefined {
    return this._userId;
  }

  public get stored(): IdentityStorage {
    return this.getStorage();
  }

  constructor(
    private readonly jwtService: JwtService
  ) {
    this._jwt$ = this.jwtService.jwt$.pipe(
      map(x => x ? decodeJwt(x) : undefined)
    );

    this._userId$ = this._jwt$.pipe(
      map(x => x ? x.sub : undefined)
    );

    this._userId$.subscribe(x => this._userId = x);
  }

  private getStorage(): IdentityStorage {
    const identity = localStorage.getItem(StorageKey);
    if (identity) {
      return JSON.parse(identity);
    }

    return {};
  }

  updateIdentity(param: Partial<IdentityStorage>) {
    let newIdentity = { ...this.getStorage(), ...param } as IdentityStorage;
    // Delete any values that are undefined
    for (const keyx in newIdentity) {
      const key = keyx as keyof IdentityStorage;
      if (Object.prototype.hasOwnProperty.call(newIdentity, key)) {
        if (newIdentity[key] === undefined) {
          delete newIdentity[key];
        }
      }
    }

    localStorage.setItem(StorageKey, JSON.stringify(newIdentity));
  }
}
