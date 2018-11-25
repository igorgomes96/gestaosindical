import { ToastsService } from 'src/app/shared/toasts.service';
import { EstadosApiService } from './../../shared/estados-api.service';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

import { switchMap, tap, filter, flatMap, map } from 'rxjs/operators';
import { Observable, forkJoin } from 'rxjs';
import { Contato } from './../../model/contato';
import { Arquivo } from './../../model/arquivo';
import { SindicatoLaboral } from './../../model/sindicato-laboral';
import { SindicatoPatronal } from './../../model/sindicato-patronal';
import { endpoints } from '../../../environments/endpoints';
import { environment } from './../../../environments/environment';
import { EmpresasApiService } from './../empresas-api.service';
import { Empresa } from './../../model/empresa';
import { LaboraisApiService } from 'src/app/sindicatos/laborais/laborais-api.service';
import { PatronaisApiService } from './../../sindicatos/patronais/patronais-api.service';
import { RelatedLink } from 'src/app/shared/related-link/related-link';
import { Estado } from 'src/app/model/estado';
import { ToastType } from 'src/app/shared/toasts/toasts.component';


@Component({
  selector: 'app-empresa-form',
  templateUrl: './empresa-form.component.html',
  styleUrls: ['./empresa-form.component.css']
})
export class EmpresaFormComponent implements OnInit {

  form: FormGroup;
  empresa: Empresa;
  contatos$: Observable<Contato[]>;
  arquivos: Arquivo[];
  patronal: SindicatoPatronal;
  laboral: SindicatoLaboral;
  relatedLinks: RelatedLink[];
  estados$: Observable<Estado[]>;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;

  constructor(private route: ActivatedRoute,
    private formBuilder: FormBuilder, private router: Router,
    private empresasApi: EmpresasApiService,
    private laboraisApi: LaboraisApiService,
    private patronaisApi: PatronaisApiService,
    private estadosApi: EstadosApiService,
    private toast: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      nome: ['', Validators.required],
      cnpj: ['', [Validators.required, Validators.minLength(14), Validators.maxLength(14)]],
      endereco: this.formBuilder.group({
        id: [{ value: '', disabled: true }],
        cidade: ['', Validators.required],
        uf: ['', Validators.required],
        logradouro: ['', Validators.required],
        numero: ['']
      }),
      qtdaTrabalhadores: [''],
      massaSalarial: [''],
      sindicatoLaboral: ['', Validators.required],
      sindicatoPatronal: ['', Validators.required]
    });

    this.estados$ = this.estadosApi.getEstados();

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('empresa')),
        tap(d => {
          this.empresa = d['empresa'];
          this.relatedLinks = [
            {
              label: 'Negociações',
              link: '/negociacoes/agenda',
              queryParams: { empresaId: this.empresa.id }
            },
            {
              label: 'Litígios',
              link: '/negociacoes/litigios',
              queryParams: { empresaId: this.empresa.id }
            },
            {
              label: 'Planos de Ação',
              link: '/negociacoes/planosacao',
              queryParams: { empresaId: this.empresa.id }
            }
          ];
          this.form.patchValue(this.empresa);
          this.contatos$ = this.empresasApi.getContatos(this.empresa.id);
        }),
        flatMap(d => {
          return forkJoin(
            this.patronaisApi.get(this.empresa.sindicatoPatronalId),
            this.laboraisApi.get(this.empresa.sindicatoLaboralId),
            this.empresasApi.getArquivos(this.empresa.id),
            );
        })
      ).subscribe(d => {
        this.form.get('sindicatoPatronal').setValue(d[0]);
        this.form.get('sindicatoLaboral').setValue(d[1]);
        this.arquivos = d[2];
      });
  }

  linkLaboral(valor) {
    return `/sindicatos/laborais/${valor.id}`;
  }

  linkPatronal(valor) {
    return `/sindicatos/patronais/${valor.id}`;
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  preventDefault($event: KeyboardEvent) {
    if ($event.charCode === 13) {
      $event.preventDefault();
    }
  }

  put(empresa: Empresa) {
    this.empresasApi.put(empresa.id, empresa)
      .subscribe(_ => this.router.navigate(['/empresas']));
  }

  post(empresa: Empresa) {
    this.empresasApi.post(empresa)
      .subscribe((e: Empresa) => this.router.navigate(['/empresas', e.id]));
  }

  salvar() {
    const empresa: Empresa = this.form.getRawValue();
    empresa.sindicatoLaboralId = empresa.sindicatoLaboral.id;
    empresa.sindicatoPatronalId = empresa.sindicatoPatronal.id;
    empresa.sindicatoLaboral = null;
    empresa.sindicatoPatronal = null;
    if (this.empresa) {
      this.put(empresa);
    } else {
      this.post(empresa);
    }
  }

  novoContato(contato: Contato) {
    this.contatos$ = this.empresasApi.addContato(this.empresa.id, contato)
      .pipe(switchMap(_ => this.empresasApi.getContatos(this.empresa.id)));
  }

  removeContato(idContato: number) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Essa ação não poderá ser desfeita!',
      type: ToastType.warning
    }, () => this.empresasApi.deleteContato(this.empresa.id, idContato).subscribe(_ => {
      this.contatos$ = this.empresasApi.getContatos(this.empresa.id);
      this.toast.showMessage({
        message: 'Contato excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  upload(files: FileList) {
    this.empresasApi.uploadFiles(this.empresa.id, files)
      .pipe(switchMap(_ => this.empresasApi.getArquivos(this.empresa.id)))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.empresasApi.getArquivos(this.empresa.id).subscribe(d => this.arquivos = d);
  }

  mapShow(value: any) {
    return value['nome'];
  }

  mapValue(value: any) {
    return value && value['id'];
  }

}
