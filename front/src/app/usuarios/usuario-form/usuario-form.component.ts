import { ToastsService } from './../../shared/toasts.service';
import { Usuario } from 'src/app/model/usuario';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { filter, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { endpoints } from 'src/environments/endpoints';
import { environment } from 'src/environments/environment';
import { Empresa } from 'src/app/model/empresa';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { UsuariosApiService } from 'src/app/shared/api/usuarios-api.service';
import { EmpresasApiService } from 'src/app/shared/api/empresas-api.service';

@Component({
  selector: 'app-usuario-form',
  templateUrl: './usuario-form.component.html',
  styleUrls: ['./usuario-form.component.css']
})
export class UsuarioFormComponent implements OnInit {

  usuario: Usuario;
  form: FormGroup;
  formSearch: FormGroup;
  empresas: Empresa[];

  urlEmpresasList = environment.api + endpoints.empresa + 'list';
  constructor(private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private api: UsuariosApiService,
    private empresasApi: EmpresasApiService,
    private toasts: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      login: [''],
      nome: [''],
      perfil: ['']
    });

    this.form.get('perfil').valueChanges
    .pipe(distinctUntilChanged(), filter(v => v != null && this.usuario != null),
    switchMap(v => {
      this.usuario.perfil = v;
      return this.api.put(this.usuario.login, this.usuario);
    })).subscribe(_ => this.toasts.showMessage({
      message: 'Perfil alterado com sucesso!',
      title: 'Sucesso!',
      type: ToastType.success
    }));

    this.formSearch = this.formBuilder.group({
      search: ['']
    });

    this.formSearch.get('search').valueChanges
    .pipe(distinctUntilChanged(), filter(v => v != null))
    .subscribe(v => {
      this.empresasApi.allowUser(+v, this.usuario).subscribe(_ => {
        this.loadEmpresas();
        this.formSearch.get('search').reset();
      });
    });

    this.route.data
    .pipe(
      filter(d => d.hasOwnProperty('usuario')),
      switchMap(d => {
        if (d.hasOwnProperty('usuario')) {
          this.updateForm(d['usuario']);
          this.usuario = d['usuario'];
        }
        return this.empresasApi.getRelated(this.usuario.login);
      })
    ).subscribe(e => {
      this.empresas = e;
    });
  }

  loadEmpresas() {
    this.empresasApi.getRelated(this.usuario.login).subscribe(e => {
      this.empresas = e;
    });
  }

  updateForm(usuario: Usuario) {
    this.form.patchValue(usuario);
  }

  disallow(idEmpresa: number) {
    this.empresasApi.disallowUser(idEmpresa, this.usuario)
    .subscribe(_ => this.loadEmpresas());
  }

  mapValueFunction(empresa: Empresa) {
    return empresa && empresa.id;
  }

}
