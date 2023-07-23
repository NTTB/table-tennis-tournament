import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AccountModule } from "./account/account.module";
import { JwtInterceptor } from "./shared/jwt.interceptor";
import { JwtService } from "./shared/jwt.service";
import { userRoutes } from "./pages/user-routes";
import { PagesModule } from "./pages/pages.module";
import { IJwtTokenProvider, HttpClient, SetApi, AccountApi, ClockService, SetsHub, CryptoKeyService } from '@nttb/t3-api-client';

class JwtTokenProviderWrapper implements IJwtTokenProvider {
  constructor(private readonly jwtService: JwtService) { }
  getJwtToken() {
    return Promise.resolve(this.jwtService.getToken());
  }
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    AccountModule,
    PagesModule,
    RouterModule.forRoot([
      { path: '', children: userRoutes },
    ])
  ],
  providers: [
    JwtService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: JwtTokenProviderWrapper, useFactory: (jwtService: JwtService) => new JwtTokenProviderWrapper(jwtService), deps: [JwtService] },
    { provide: HttpClient, useFactory: (tokenProvider: JwtTokenProviderWrapper) => new HttpClient("", tokenProvider), deps: [JwtTokenProviderWrapper] },
    { provide: SetApi, useFactory: (httpClient: HttpClient) => new SetApi(httpClient), deps: [HttpClient] },
    { provide: AccountApi, useFactory: (httpClient: HttpClient) => new AccountApi(httpClient), deps: [HttpClient] },
    { provide: ClockService, useFactory: (httpClient: HttpClient) => new ClockService(httpClient), deps: [HttpClient] },
    { provide: CryptoKeyService, useFactory: () => new CryptoKeyService() },
    { provide: SetsHub, useFactory: (tokenProvider: JwtTokenProviderWrapper) => new SetsHub("hubs/set", tokenProvider), deps: [JwtTokenProviderWrapper] },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
