import { IJwtTokenProvider } from "./IJwtTokenProvider";

export interface IHttpClient {
  get(url: string): Promise<void>;
  get<T>(url: string): Promise<T>;

  post(url: string, body: any | null): Promise<void>;
  post<T>(url: string, body: any | null): Promise<T>;

  put(url: string, body: any | null): Promise<void>;
  put<T>(url: string, body: any | null): Promise<T>;

  patch(url: string, body: any | null): Promise<void>;
  patch<T>(url: string, body: any | null): Promise<T>;

  delete(url: string): Promise<void>;
  delete<T = any>(url: string): Promise<T>;
}

export class HttpClient implements IHttpClient {

  constructor(
    private readonly baseUrl: string,
    private readonly jwtTokenProvider: IJwtTokenProvider) {
    if (baseUrl == null) throw new Error("baseUrl cannot be null");
    if (jwtTokenProvider == null) throw new Error("jwtTokenProvider cannot be null");
  }

  async get<T = any>(url: string): Promise<T | void> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'GET',
      headers: await this.createRequestHeader(),
    });

    if (response.ok) {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const jsonData = await response.json() as T;
        return jsonData;
      } else {
        return;
      }
    } else {
      throw new Error("Response failed");
    }

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

  async post<T = any>(url: string, body: any | null): Promise<T | void> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'POST',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });

    if (response.ok) {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const jsonData = await response.json() as T;
        return jsonData;
      } else {
        return;
      }
    } else {
      throw new Error("Response failed");
    }

  }

  async put<T = any>(url: string, body: any | null): Promise<T | void> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'PUT',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });

    if (response.ok) {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const jsonData = await response.json() as T;
        return jsonData;
      } else {
        return;
      }
    } else {
      throw new Error("Response failed");
    }
  }

  async patch<T = any>(url: string, body: any | null): Promise<T | void> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'PATCH',
      headers: await this.createRequestHeader(),
      body: JSON.stringify(body),
    });

    if (response.ok) {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const jsonData = await response.json() as T;
        return jsonData;
      } else {
        return;
      }
    } else {
      throw new Error("Response failed");
    }
  }

  async delete<T = any>(url: string): Promise<T | void> {
    const response = await fetch(this.getRequestInfo(url), {
      method: 'DELETE',
      headers: await this.createRequestHeader(),
    });
    if (response.ok) {
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        const jsonData = await response.json() as T;
        return jsonData;
      } else {
        return;
      }
    } else {
      throw new Error("Response failed");
    }
  }
}