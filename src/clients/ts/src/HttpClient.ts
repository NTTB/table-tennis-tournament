import { IJwtTokenProvider } from "./IJwtTokenProvider";

export interface IHttpClient {
  get<TResponse = any>(url: string): Promise<TResponse>;
  post<T>(url: string, body: any | null): Promise<T>;
  put<T>(url: string, body: any | null): Promise<T>;
  patch<T>(url: string, body: any | null): Promise<T>;
  delete<T = any>(url: string): Promise<T>;
}

export class HttpClient implements IHttpClient {

  constructor(
    private readonly baseUrl: string,
    private readonly jwtTokenProvider: IJwtTokenProvider) {
  }

  async get<TResponse = any>(url: string): Promise<TResponse> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'GET',
      headers: await this.createRequestHeader(),
    });

    return await response.json() as TResponse;
  }

  private async createRequestHeader() {
    const requestHeader: HeadersInit = { 'Content-Type': 'application/json', };
    const jwtToken = await this.jwtTokenProvider.getJwtToken();
    if (jwtToken) {
      requestHeader['Authorization'] = `Bearer ${jwtToken}`;
    }

    return requestHeader as HeadersInit;
  }

  private getRequestInfo(url: string): RequestInfo | URL {
    return this.baseUrl + url;
  }

  async post<T>(url: string, body: any | null): Promise<T> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'POST',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });
    return await response.json() as T;
  }

  async put<T>(url: string, body: any | null): Promise<T> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'PUT',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });
    return await response.json() as T;
  }

  async patch<T>(url: string, body: any | null): Promise<T> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'PATCH',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });
    return await response.json() as T;
  }

  async delete<T = any>(url: string): Promise<T> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'DELETE',
      headers: await this.createRequestHeader(),
    });
    return await response.json();
  }
}