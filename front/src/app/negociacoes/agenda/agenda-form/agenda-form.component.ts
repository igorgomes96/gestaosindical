import { EmpresasApiService } from './../../../empresas/empresas-api.service';
import { Concorrente } from './../../../model/negociacao';
import { ActivatedRoute, Router, NavigationEnd, ActivationStart } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';

import { NegociacoesApiService } from './../../negociacoes-api.service';
import { Arquivo } from 'src/app/model/arquivo';
import { switchMap, filter, tap, distinctUntilChanged } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { forkJoin, Observable } from 'rxjs';
import { Negociacao, RodadaNegociacao } from 'src/app/model/negociacao';
import { PatronaisApiService } from './../../../sindicatos/patronais/patronais-api.service';
import { LaboraisApiService } from './../../../sindicatos/laborais/laborais-api.service';
import { Empresa } from 'src/app/model/empresa';

@Component({
  selector: 'app-agenda-form',
  templateUrl: './agenda-form.component.html',
  styleUrls: ['./agenda-form.component.css']
})
export class AgendaFormComponent implements OnInit {

  form: FormGroup;
  negociacao: Negociacao;
  arquivos: Arquivo[];
  rodadas: RodadaNegociacao[];
  concorrentes: Concorrente[];
  urlEmpresaList = environment.api + endpoints.empresa;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;

  constructor(private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private service: NegociacoesApiService,
    private laboraisApi: LaboraisApiService,
    private patronaisApi: PatronaisApiService,
    private empresasApi: EmpresasApiService,
    private router: Router) { }

  ngOnInit() {

    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      ano: ['', Validators.required],
      empresa: ['', Validators.required],
      sindicatoLaboral: ['', Validators.required],
      sindicatoPatronal: ['', Validators.required],
      qtdaTrabalhadores: ['', Validators.required],
      massaSalarial: ['', Validators.required],
      statusACT_CCT: ['NaoIniciada'],
      statusPLR: ['NaoIniciada'],
      qtdaRodadas: [''],
      taxaLaboral: [''],
      taxaPatronal: [''],
      orcado: this.formBuilder.group({
        salario: [''],
        piso: [''],
        auxCreche: [''],
        vaVr: [''],
        vaVrFerias: [''],
        descontoVt: ['']
      }),
      negociado: this.formBuilder.group({
        salario: [''],
        piso: [''],
        auxCreche: [''],
        vaVr: [''],
        vaVrFerias: [''],
        descontoVt: ['']
      }),
      plr1Sem: [''],
      plr2Sem: ['']
    });

    this.router.events
    .pipe(filter(e => e instanceof NavigationEnd), tap(_ => this.loadConcorrentes()))
    .subscribe();

    this.form.get('empresa').valueChanges
      .pipe(filter(_ => this.negociacao == null), tap(_ => {
        this.form.get('sindicatoLaboral').reset();
        this.form.get('sindicatoPatronal').reset();
        this.form.get('massaSalarial').reset();
        this.form.get('qtdaTrabalhadores').reset();
      }),
        distinctUntilChanged(), filter(v => v && !(v instanceof Object)),
        switchMap(v => this.empresasApi.get(v)),
        tap((empresa: Empresa) => {
          this.form.get('massaSalarial').setValue(empresa.massaSalarial);
          this.form.get('qtdaTrabalhadores').setValue(empresa.qtdaTrabalhadores);
        }),
        switchMap(v => forkJoin(this.laboraisApi.get(v.sindicatoLaboralId), this.patronaisApi.get(v.sindicatoPatronalId))))
      .subscribe(v => {
        this.form.get('sindicatoLaboral').setValue(v[0]);
        this.form.get('sindicatoPatronal').setValue(v[1]);
      });

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('negociacao')),
        switchMap(d => {
          this.negociacao = d['negociacao'];
          this.form.patchValue(this.negociacao);
          return forkJoin(this.service.getRodadas(this.negociacao.id),
            this.service.getArquivos(this.negociacao.id),
            this.service.getConcorrentes(this.negociacao.id));
        })
      ).subscribe(value => {
        this.rodadas = value[0];
        this.arquivos = value[1];
        this.concorrentes = value[2];
      });
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  novaRodada() {
    this.service.postRodada(this.negociacao.id)
      .subscribe(r => this.rodadas.push(r));
  }

  put(negociacao: Negociacao) {
    this.service.put(negociacao.id, negociacao)
      .subscribe(_ => this.router.navigate(['/negociacoes/agenda']));
  }

  post(negociacao: Negociacao) {
    this.service.post(negociacao)
      .subscribe((neg: Negociacao) => this.router.navigate(['/negociacoes/agenda', neg.id]));
  }

  salvar() {
    let neg: Negociacao = this.form.getRawValue();
    neg = Object.assign(neg, {
      empresaId: neg.empresa,
      sindicatoLaboralId: neg.sindicatoLaboral,
      sindicatoPatronalId: neg.sindicatoPatronal,
      sindicatoLaboral: null,
      sindicatoPatronal: null,
      empresa: null
    });
    if (this.negociacao) {
      neg.orcado.id = this.negociacao.orcadoId;
      neg.negociado.id = this.negociacao.negociadoId;
      neg.orcadoId = this.negociacao.orcadoId;
      neg.negociadoId = this.negociacao.negociadoId;
      this.put(neg);
    } else {
      this.post(neg);
    }

  }

  deleteRodada() {
    this.service.getRodadas(this.negociacao.id)
      .subscribe(r => this.rodadas = r);
  }

  upload(files: FileList) {
    this.service.uploadFiles(this.negociacao.id, files)
      .pipe(switchMap(_ => this.service.getArquivos(this.negociacao.id)))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.negociacao.id).subscribe(d => this.arquivos = d);
  }

  loadConcorrentes() {
    if (!this.negociacao) {
      return;
    }
    this.service.getConcorrentes(this.negociacao.id)
    .subscribe(x => this.concorrentes = x);
  }

  deleteConcorrente(id: number) {
    this.service.deleteConcorrente(this.negociacao.id, id)
    .pipe(tap(_ => this.loadConcorrentes())).subscribe();
  }

  linkLaboral(valor) {
    return `/sindicatos/laborais/${valor}`;
  }

  linkPatronal(valor) {
    return `/sindicatos/patronais/${valor}`;
  }

  linkEmpresa(valor) {
    return `/empresas/${valor}`;
  }

  mapValue(obj: any) {
    return obj && obj['id'];
  }

}
