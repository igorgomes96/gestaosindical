import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';

import { EmpresaListComponent } from './empresa-list/empresa-list.component';
import { EmpresaFormComponent } from './empresa-form/empresa-form.component';
import { EmpresaResolverService } from './empresa-resolver.service';

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'Empresas'
        },
        children: [
            {
                path: '',
                component: EmpresaListComponent,
            },
            {
                path: 'nova',
                component: EmpresaFormComponent
            },
            {
                path: ':id',
                component: EmpresaFormComponent,
                resolve: {
                    empresa: EmpresaResolverService
                }
            }
        ]
    }
];

@NgModule({
    imports: [CommonModule, RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class EmpresasRoutingModule { }
