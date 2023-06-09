import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {AccountModule} from "./account/account.module";
import {JwtInterceptor} from "./shared/jwt.interceptor";
import {JwtService} from "./shared/jwt.service";
import {userRoutes} from "./pages/user-routes";
import {PagesModule} from "./pages/pages.module";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    AccountModule,
    PagesModule,
    RouterModule.forRoot([
      {path: '', children: userRoutes},
    ])
  ],
  providers: [
    JwtService,
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule {
}
