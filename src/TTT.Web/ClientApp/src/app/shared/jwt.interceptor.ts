import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpHeaders } from '@angular/common/http';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<any>, next: HttpHandler) {

    // Get the JWT token from wherever you are storing it
    const token = localStorage.getItem('token');

    // If the token is present, add it to the headers of the request
    if (token) {
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });
      request = request.clone({ headers });
    }

    // Return the modified request
    return next.handle(request);
  }
}
