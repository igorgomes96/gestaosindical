import { ToastsService } from 'src/app/shared/toasts.service';
import { Concorrente, TipoReajuste, ParcelaReajuste } from '../../../model/negociacao';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Component, OnInit, AfterViewInit } from '@angular/core';

import { Arquivo } from 'src/app/model/arquivo';
import { switchMap, filter, tap, distinctUntilChanged, finalize } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { forkJoin, of } from 'rxjs';
import { Negociacao, RodadaNegociacao } from 'src/app/model/negociacao';
import { Empresa } from 'src/app/model/empresa';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { Mes } from 'src/app/model/sindicato-laboral';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';
import { LaboraisApiService } from 'src/app/shared/api/laborais-api.service';
import { PatronaisApiService } from 'src/app/shared/api/patronais-api.service';
import { EmpresasApiService } from 'src/app/shared/api/empresas-api.service';
import * as Ladda from 'ladda';


@Component({
  selector: 'app-agenda-form',
  templateUrl: './agenda-form.component.html',
  styleUrls: ['./agenda-form.component.css']
})
export class AgendaFormComponent implements OnInit, AfterViewInit {

  form: FormGroup;
  parcelasForm: FormGroup;
  negociacao: Negociacao;
  arquivos: Arquivo[];
  rodadas: RodadaNegociacao[];
  concorrentes: Concorrente[];
  Mes: typeof Mes = Mes;
  TipoReajuste: typeof TipoReajuste = TipoReajuste;
  urlEmpresaList = environment.api + endpoints.empresa;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;
  spinnerArquivos = false;
  tipoReajuste = null;
  reajuste = null;

  btnSalvarLoad: Ladda.LaddaButton;
  btnNovaRodadaLoad: Ladda.LaddaButton;


  constructor(private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private service: NegociacoesApiService,
    private laboraisApi: LaboraisApiService,
    private patronaisApi: PatronaisApiService,
    private empresasApi: EmpresasApiService,
    private router: Router,
    private toast: ToastsService) { }

  ngOnInit() {

    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      ano: ['', Validators.required],
      empresa: ['', Validators.required],
      sindicatoLaboral: [''],
      sindicatoPatronal: [''],
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

    this.btnSalvarLoad = Ladda.create(document.querySelector('#btnSalvar')); 

    this.parcelasForm = this.formBuilder.group({
      id: [''],
      mes: ['Janeiro', Validators.required],
      tipoReajuste: ['', Validators.required],
      reajusteId: ['', Validators.required],
      valor: ['', Validators.required],
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
        switchMap(v =>
          forkJoin(v.sindicatoLaboralId ? this.laboraisApi.get(v.sindicatoLaboralId) : of(null),
            v.sindicatoPatronalId ? this.patronaisApi.get(v.sindicatoPatronalId) : of(null))))
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

  ngAfterViewInit() {
    if (this.negociacao) {
      this.btnNovaRodadaLoad = Ladda.create(document.querySelector('#btnNovaRodada'));  
    }
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  showModal(_, tipo: string, reajuste: string) {
    this.reajuste = reajuste;
    switch (tipo) {
      case ('Salário'):
        this.tipoReajuste = TipoReajuste.Salario;
        break;
      case ('Piso'):
        this.tipoReajuste = TipoReajuste.Piso;
        break;
      case ('VT'):
        this.tipoReajuste = TipoReajuste.VT;
        break;
      case ('VA/VR'):
        this.tipoReajuste = TipoReajuste.VaVr;
        break;
      case ('VA/VR Férias'):
        this.tipoReajuste = TipoReajuste.VaVrFerias;
        break;
      case ('Creche'):
        this.tipoReajuste = TipoReajuste.Creche;
        break;
    }
    this.parcelasForm.get('tipoReajuste').setValue(this.tipoReajuste);
    this.parcelasForm.get('reajusteId').setValue(this.negociacao[this.reajuste].id);
  }

  adicionarParcela() {
    const parcela = (<ParcelaReajuste>this.parcelasForm.getRawValue());
    this.service.postParcela(this.negociacao.id, parcela).subscribe((p: ParcelaReajuste) => {
      this.negociacao[this.reajuste].parcelas.push(p);
      this.toast.showMessage({
        message: 'Parcela adicionada com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    });
  }

  closeModal(_: any) {
    this.tipoReajuste = null;
    this.reajuste = null;
    this.parcelasForm.reset();
    this.parcelasForm.get('mes').setValue(Mes.Janeiro);
  }

  get parcelas(): ParcelaReajuste[] {
    return this.negociacao[this.reajuste].parcelas.filter(p => TipoReajuste[p.tipoReajuste] === this.tipoReajuste);
  }

  parcelasPorTipo(reajuste: string, tipo: TipoReajuste): ParcelaReajuste[] {
    const parcelas = this.negociacao[reajuste].parcelas.filter(p => {
      return p.tipoReajuste === TipoReajuste[tipo];
    });
    return parcelas;
  }

  excluirParcela(parcela: ParcelaReajuste) {
    this.service.deleteParcela(this.negociacao.id, parcela.id)
      .subscribe(p => {
        const index = this.negociacao[this.reajuste].parcelas.indexOf(
          this.negociacao[this.reajuste].parcelas.filter(x => x.id === parcela.id)[0]
        );
        this.negociacao[this.reajuste].parcelas.splice(index, 1);
        this.toast.showMessage({
          message: 'Parcela excluída com sucesso!',
          title: 'Sucesso!',
          type: ToastType.success
        });
      });
  }

  novaRodada() {
    this.btnNovaRodadaLoad.start();
    this.service.put(this.negociacao.id, this.negociacaoToSave)
      .pipe(switchMap(_ => this.service.postRodada(this.negociacao.id)), finalize(() => this.btnNovaRodadaLoad.stop()))
      .subscribe(r => {
        this.rodadas.push(r);
        this.toast.showMessage({
          message: 'Rodada aberta com sucesso!',
          title: 'Sucesso!',
          type: ToastType.success
        });
        setTimeout(() => {
          const element = document.querySelector('#rodada' + this.rodadas.length.toString());
          if (element) {
            element.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
          }
        }, 500);
      });
  }

  put(negociacao: Negociacao) {
    this.btnSalvarLoad.start();
    this.service.put(negociacao.id, negociacao)
      .pipe(tap(_ => this.toast.showMessage({
        message: 'Negociação salva com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })), finalize(() => this.btnSalvarLoad.stop()))
      .subscribe(_ => this.router.navigate(['/negociacoes/gestao', negociacao.id]));
  }

  post(negociacao: Negociacao) {
    this.btnSalvarLoad.start();
    this.service.post(negociacao)
      .pipe(tap(_ => this.toast.showMessage({
        message: 'Negociação salva com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })), finalize(() => this.btnSalvarLoad.stop()))
      .subscribe((neg: Negociacao) => this.router.navigate(['/negociacoes/gestao', neg.id]));
  }

  salvar() {
    const neg: Negociacao = this.negociacaoToSave;
    if (this.negociacao) {
      this.put(neg);
    } else {
      this.post(neg);
    }
  }

  get negociacaoToSave(): Negociacao {
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
    }
    return neg;
  }

  deleteRodada() {
    this.service.getRodadas(this.negociacao.id)
      .subscribe(r => this.rodadas = r);
  }

  upload(files: FileList) {
    this.spinnerArquivos = true;
    this.service.uploadFiles(this.negociacao.id, files)
      .pipe(switchMap(_ => this.service.getArquivos(this.negociacao.id)),
        finalize(() => this.spinnerArquivos = false))
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
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Essa ação não poderá ser desfeita!',
      type: ToastType.warning
    }, () => this.service.deleteConcorrente(this.negociacao.id, id).subscribe(d => {
      this.loadConcorrentes();
      this.toast.showMessage({
        message: 'Concorrente excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
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
