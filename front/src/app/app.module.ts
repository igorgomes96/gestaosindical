import { UsuariosModule } from './usuarios/usuarios.module';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import pt from '@angular/common/locales/pt';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { NavModule } from './nav/nav.module';
import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from './login/auth.module';
import { InterceptorModule } from './interceptor/interceptor.module';
import { SharedModule } from './shared/shared.module';
import { NegociacoesModule } from './negociacoes/negociacoes.module';
import { ForbiddenComponent } from './forbidden/forbidden.component';

registerLocaleData(pt);

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PageNotFoundComponent,
    ForbiddenComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    NavModule,
    AuthModule,
    AppRoutingModule,
    HttpModule,
    HttpClientModule,
    InterceptorModule,
    SharedModule,
    NegociacoesModule,
    UsuariosModule
  ],
  providers: [
    {
      provide: LOCALE_ID,
      useValue: 'pt-BR'
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
