import { ToastsService } from 'src/app/shared/toasts.service';
import { Estado } from './../../../model/estado';
import { EstadosApiService } from './../../../shared/estados-api.service';
import { Referente } from './../../../model/litigio';
import { PlanoAcao, StatusPlanoAcao } from './../../../model/plano-acao';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { PlanoAcaoApiService } from '../plano-acao-api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Arquivo } from 'src/app/model/arquivo';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { switchMap, filter, distinctUntilChanged, tap, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

@Component({
  selector: 'app-plano-acao-form',
  templateUrl: './plano-acao-form.component.html',
  styleUrls: ['./plano-acao-form.component.css']
})
export class PlanoAcaoFormComponent implements OnInit {

  planoAcao: PlanoAcao;
  arquivos: Arquivo[];
  form: FormGroup;
  estados$: Observable<Estado[]>;
  StatusPlanoAcao: typeof StatusPlanoAcao = StatusPlanoAcao;
  Referente: typeof Referente = Referente;
  urlEmpresaList = environment.api + endpoints.empresa;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;

  constructor(private formBuilder: FormBuilder,
    private service: PlanoAcaoApiService,
    private route: ActivatedRoute,
    private estadosApi: EstadosApiService,
    private router: Router,
    private toasts: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      referente: ['MTE', Validators.required],
      laboral: [''],
      patronal: [''],
      data: ['', Validators.required],
      estado: ['', Validators.required],
      reclamacoes: [''],
      reclamante: [''],
      procedencia: [''],
      responsavelAcao: [''],
      dataSolucao: [''],
      status: ['']
    });

    this.estados$ = this.estadosApi.getEstados();

    this.form.get('referente').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(v => {
        if (Referente[v] === Referente.Laboral) {
          this.form.get('patronal').reset();
        } else if (Referente[v] === Referente.Patronal) {
          this.form.get('laboral').reset();
        } else {
          this.form.get('patronal').reset();
          this.form.get('laboral').reset();
        }
      });

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('planoAcao')),
        switchMap(d => {
          this.planoAcao = d['planoAcao'];
          this.updateForm(this.planoAcao);
          return this.service.getArquivos(this.planoAcao.id);
        })
      ).subscribe(value => {
        this.arquivos = value;
      });
  }

  linkLaboral(valor) {
    return `/sindicatos/laborais/${valor}`;
  }

  linkPatronal(valor) {
    return `/sindicatos/patronais/${valor}`;
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  sindicatoInvalido() {
    return (Referente[this.form.get('referente').value] === Referente.Laboral && !this.form.get('laboral').value) ||
      (Referente[this.form.get('referente').value] === Referente.Patronal && !this.form.get('patronal').value);
  }

  classStatus(status: StatusPlanoAcao) {
    if (!status) { return ''; }
    switch (StatusPlanoAcao[status]) {
      case StatusPlanoAcao.Vencido:
        return 'label-danger';
      case StatusPlanoAcao.AVencer:
        return 'label-warning';
      case StatusPlanoAcao.Solucionado:
        return 'label-primary';
      case StatusPlanoAcao.NoPrazo:
        return 'label-success';
    }
    return '';
  }

  updateForm(planoAcao: PlanoAcao) {
    this.form.patchValue(planoAcao);
    if (planoAcao.data) {
      this.form.get('data').setValue(planoAcao.data.substring(0, 10));
    }
    if (planoAcao.dataSolucao) {
      this.form.get('dataSolucao').setValue(planoAcao.dataSolucao.substring(0, 10));
    }
  }

  upload(files: FileList) {
    this.service.uploadFiles(this.planoAcao.id, files)
      .pipe(switchMap(_ => this.service.getArquivos(this.planoAcao.id)))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.planoAcao.id)
      .subscribe(d => this.arquivos = d);
  }

  post(plano: PlanoAcao) {
    this.service.post(plano)
      .subscribe(p => this.router.navigate(['/negociacoes/planosacao', p.id]));
  }

  put(plano: PlanoAcao) {
    this.service.put(this.planoAcao.id, plano)
      .subscribe(p => this.router.navigate(['/negociacoes/planosacao']));
  }

  salvar() {
    if (this.sindicatoInvalido()) {
      this.toasts.showMessage({ message: 'O Sindicato deve ser informado!', title: 'Fomulário Inválido', type: ToastType.warning });
      return;
    }
    const plano = this.form.getRawValue();
    plano.laboralId = plano.laboral;
    plano.patronalId = plano.patronal;
    delete plano.laboral;
    delete plano.patronal;
    if (this.planoAcao) {
      this.put(plano);
    } else {
      this.post(plano);
    }
  }

  mapValue(obj: any) {
    return obj && obj['id'];
  }

}
