import {Inject, Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

@Injectable()
export class ApiService {
  constructor(
    private readonly http: HttpClient,
    @Inject('BASE_URL') private readonly baseUrl: string
  ) {
  }

  public get<T>(path: string, params?: any): Observable<T> {
    return this.http.get<T>(this.sanitizePath(path), {params});
  }

  public post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(this.sanitizePath(path), body);
  }

  public put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(this.sanitizePath(path), body);
  }

  public delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(this.sanitizePath(path));
  }

  public patch<T>(path: string, body: any): Observable<T> {
    return this.http.patch<T>(this.sanitizePath(path), body);
  }

  private sanitizePath(path: string): string {
    if (path.startsWith("/")) {
      throw new Error("Path cannot start with a slash, this is handled automatically.");
    }

    return this.baseUrl + path;
  }
}
