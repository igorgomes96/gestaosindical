import { queryParams } from './../../../environments/queryparams';
import { Pesquisa, EntityType } from './../../model/pesquisa';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from './../../login/auth.service';
import { Component, OnInit } from '@angular/core';
import { smoothlyMenu } from '../../../app.helpers';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { distinctUntilChanged, filter, tap } from 'rxjs/operators';

declare var $: any;


@Component({
  selector: 'app-topnav',
  templateUrl: './topnav.component.html',
  styleUrls: ['./topnav.component.css']
})
export class TopnavComponent implements OnInit {

  url = environment.api + endpoints.pesquisa;
  form: FormGroup;
  EntityType: typeof EntityType = EntityType;
  constructor(private authService: AuthService,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      pesquisa: ['']
    });

    this.form.get('pesquisa').valueChanges
    .pipe(
      distinctUntilChanged(), filter(v => v), tap(_ => this.form.get('pesquisa').setValue(''))
    ).subscribe(v => {
      switch (v.entityType) {
        case EntityType[EntityType.Empresa]:
          this.router.navigate(['/empresas', v.obj['id']]);
          break;
        case EntityType[EntityType.SindicatoLaboral]:
          this.router.navigate(['/sindicatos/laborais', v.obj['id']]);
          break;
        case EntityType[EntityType.SindicatoPatronal]:
          this.router.navigate(['/sindicatos/patronais', v.obj['id']]);
          break;
      }
    });
  }

  getClassIcon(result: Pesquisa) {
    switch (result.entityType) {
      case EntityType[EntityType.Empresa]:
        return 'fa-users';
      case EntityType[EntityType.SindicatoLaboral]:
        return 'fa-building-o';
      case EntityType[EntityType.SindicatoPatronal]:
        return 'fa-building';
      default:
        return 'fa-check';
    }
  }

  getClassColor(result: Pesquisa) {
    switch (result.entityType) {
      case EntityType[EntityType.Empresa]:
        return 'empresa-bg';
      case EntityType[EntityType.SindicatoLaboral]:
        return 'laboral-bg';
      case EntityType[EntityType.SindicatoPatronal]:
        return 'patronal-bg';
      default:
        return '';
    }
  }

  getQueryParam(result: Pesquisa): any {
    switch (result.entityType) {
      case EntityType[EntityType.Empresa]:
        return { empresaId: result.obj['id'] };
      case EntityType[EntityType.SindicatoLaboral]:
        return { laboralId: result.obj['id'] };
      case EntityType[EntityType.SindicatoPatronal]:
        return { patronalId: result.obj['id'] };
      default:
        return '';
    }
  }

  navigate(url: string, obj: Pesquisa, ev: MouseEvent = null) {
    this.router.navigate([url], { queryParams: this.getQueryParam(obj) });
    this.form.reset();
    if (ev !== null) {
      ev.stopPropagation();
    }
  }

  mapShowFunction(obj: any) {
    if (!obj || !obj.hasOwnProperty('obj') || !obj['obj'].hasOwnProperty('nome')) {
      return null;
    }
    const pesquisa = <Pesquisa>obj;
    return pesquisa.obj['nome'];
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  toggleNavigation(): void {
    $('body').toggleClass('mini-navbar');
    smoothlyMenu();
  }

}
