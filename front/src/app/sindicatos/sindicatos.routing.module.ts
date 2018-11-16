import { PatronalResolverService } from './patronais/patronal-resolver.service';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';

import { LaboralFormComponent } from './laborais/laboral-form/laboral-form.component';
import { LaboralListComponent } from './laborais/laboral-list/laboral-list.component';
import { PatronalFormComponent } from './patronais/patronal-form/patronal-form.component';
import { PatronalListComponent } from './patronais/patronal-list/patronal-list.component';
import { LaboralResolverService } from './laborais/laboral-resolver.service';

const routes: Routes = [
    {
        path: 'laborais',
        data: {
            breadcrumb: 'Laborais',
            title: 'Sindicatos Laborais'
        },
        children: [
            {
                path: '',
                component: LaboralListComponent
            },
            {
                path: 'novo',
                component: LaboralFormComponent,
            },
            {
                path: ':id',
                component: LaboralFormComponent,
                resolve: {
                    sindicatoLaboral: LaboralResolverService
                }
            }
        ]
    },
    {
        path: 'patronais',
        data: {
            breadcrumb: 'Patronais',
            title: 'Sindicatos Patronais'
        },
        children: [
            {
                path: '',
                component: PatronalListComponent
            },
            {
                path: 'novo',
                component: PatronalFormComponent,
            },
            {
                path: ':id',
                component: PatronalFormComponent,
                resolve: {
                    sindicatoPatronal: PatronalResolverService
                }
            }
        ]
    }
];

@NgModule({
    imports: [CommonModule, RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SindicatosRoutingModule {}
