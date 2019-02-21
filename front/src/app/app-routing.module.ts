import { RecuperacaoComponent } from './login/recuperacao/recuperacao.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { ConcorrenteComponent } from './negociacoes/concorrente/concorrente.component';
import { AuthGuard } from './guards/auth.guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { RegisterComponent } from './login/register/register.component';
import { EnviaCodigoComponent } from './login/envia-codigo/envia-codigo.component';


const appRoutes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'registro', component: RegisterComponent },
    { path: 'envia-codigo', component: EnviaCodigoComponent },
    { path: 'recuperacao', component: RecuperacaoComponent },
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'concorrente/:id', component: ConcorrenteComponent, outlet: 'popup' },
    { path: 'not-found', component: PageNotFoundComponent },
    { path: 'forbidden', component: ForbiddenComponent },
    {
        path: 'dashboard',
        loadChildren: './dashboard/dashboard.module#DashboardModule',
        canActivate: [AuthGuard],
        data: {
            breadcrumb: 'Dashboard'
        }
    },
    {
        path: 'sindicatos',
        loadChildren: './sindicatos/sindicatos.module#SindicatosModule',
        canActivate: [AuthGuard],
        data: {
            breadcrumb: 'Sindicatos'
        }
    },
    {
        path: 'empresas',
        canActivate: [AuthGuard],
        loadChildren: './empresas/empresas.module#EmpresasModule',
        data: {
            breadcrumb: 'Empresas'
        }
    },
    {
        path: 'negociacoes',
        canActivate: [AuthGuard],
        loadChildren: './negociacoes/negociacoes.module#NegociacoesModule',
        data: {
            breadcrumb: 'Negociações'
        }
    },
    {
        path: 'usuarios',
        canActivate: [AuthGuard],
        loadChildren: './usuarios/usuarios.module#UsuariosModule'
    },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: '/not-found' }
];

@NgModule({
    declarations: [],
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [
        RouterModule
    ],
    providers: [],
})
export class AppRoutingModule { }
