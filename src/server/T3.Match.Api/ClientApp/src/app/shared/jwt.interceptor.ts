import {Injectable} from '@angular/core';
import {HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {JwtService} from "./jwt.service";

@Injectable({providedIn: 'root'})
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private readonly jwtService: JwtService
  ) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    const token = this.jwtService.getToken();

    // If the token is present, add it to the headers of the request
    if (token) {
      // Clone the headers by setting the authorization
      const headers = request.headers.set('Authorization', `Bearer ${token}`);
      // Clone the request with the new headers.
      request = request.clone({headers});
    }

    // Return the modified request
    return next.handle(request);
  }
}


