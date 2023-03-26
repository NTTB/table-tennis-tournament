import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";


@Injectable()
export class JwtService {
  private readonly _jwt$ = new BehaviorSubject<string | null>(localStorage.getItem('token'));
  public readonly jwt$ = this._jwt$.asObservable();

  public getToken(): string | null {
    return this._jwt$.getValue();
  }

  public setToken(token: string): void {
    localStorage.setItem('token', token);
    this._jwt$.next(token);
  }

  public destroyToken(): void {
    localStorage.removeItem('token');
    this._jwt$.next(null);
  }
}
