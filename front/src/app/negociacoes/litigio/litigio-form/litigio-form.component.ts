import { ToastsService } from './../../../shared/toasts.service';
import { Arquivo } from 'src/app/model/arquivo';
import { ProcedimentoLitigio, Litigio, Referente, ItemLitigio } from './../../../model/litigio';
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { Router, ActivatedRoute, ɵangular_packages_router_router_b } from '@angular/router';
import { filter, switchMap, distinctUntilChanged, finalize, tap } from 'rxjs/operators';
import { FormValidators } from 'src/app/shared/form-validators';
import { Observable } from 'rxjs';
import { Estado } from 'src/app/model/estado';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { EstadosApiService } from 'src/app/shared/api/estados-api.service';
import { ItensLitigiosApiService } from 'src/app/shared/api/itens-litigios-api.service';
import { LitigiosApiService } from 'src/app/shared/api/litigios-api.service';
import * as Ladda from 'ladda';

@Component({
  selector: 'app-litigio-form',
  templateUrl: './litigio-form.component.html',
  styleUrls: ['./litigio-form.component.css']
})
export class LitigioFormComponent implements OnInit, AfterViewInit {

  litigio: Litigio;
  arquivos: Arquivo[];
  form: FormGroup;
  estados$: Observable<Estado[]>;
  ProcedimentoLitigio: typeof ProcedimentoLitigio = ProcedimentoLitigio;
  Referente: typeof Referente = Referente;
  urlEmpresaList = environment.api + endpoints.empresa;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;
  spinnerArquivos = false;

  btnSalvarLoad: Ladda.LaddaButton;
  btnAddReclamacao: Ladda.LaddaButton;

  constructor(private formBuilder: FormBuilder,
    private service: LitigiosApiService,
    private route: ActivatedRoute, private router: Router,
    private estadosApi: EstadosApiService,
    private itensApi: ItensLitigiosApiService,
    private toasts: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      empresa: ['', Validators.required],
      referente: ['', Validators.required],
      laboral: ['', FormValidators.referenteA('referente')],
      patronal: ['', FormValidators.referenteA('referente')],
      data: ['', [FormValidators.date, Validators.required]],
      estado: ['', Validators.required],
      procedimento: ['', Validators.required],
      resumoAssuntos: [''],
      participantes: ['']
    });

    this.btnSalvarLoad = Ladda.create(document.querySelector('#btnSalvar'));

    this.estados$ = this.estadosApi.getEstados();
    this.form.get('referente').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(v => {
        if (Referente[v] === Referente.Laboral) {
          this.form.get('patronal').reset();
          this.form.get('patronal').markAsPristine();
        } else if (Referente[v] === Referente.Patronal) {
          this.form.get('laboral').reset();
          this.form.get('laboral').markAsPristine();
        } else {
          this.form.get('patronal').reset();
          this.form.get('laboral').reset();
          this.form.get('patronal').markAsPristine();
          this.form.get('laboral').markAsPristine();
        }
      });

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('litigio')),
        switchMap(d => {
          this.litigio = d['litigio'];
          // this.litigio.data = new Date(this.litigio.data);
          this.updateForm(this.litigio);
          return this.service.getArquivos(this.litigio.id);
        })
      ).subscribe(value => {
        this.arquivos = value;
      });
  }

  ngAfterViewInit() {
    if (this.litigio) {
      this.btnAddReclamacao = Ladda.create(document.querySelector('#btnAddReclamacao'));
    }
  }

  hasError(formControl: AbstractControl) {
    return formControl.dirty && formControl.invalid;
  }

  addItem() {
    this.btnAddReclamacao.start();
    const novoItem = {
      litigioId: this.litigio.id,
      assuntos: '',
      possuiPlano: false
    };
    if (this.litigio) {
      this.itensApi.post(<ItemLitigio>novoItem)
        .pipe(finalize(() => this.btnAddReclamacao.stop()))
        .subscribe((item: ItemLitigio) => {
          if (!this.litigio.itens) {
            this.litigio.itens = [];
          }
          this.litigio.itens.push(item);
          this.toasts.showMessage({
            message: 'Reclamação adicionada com sucesso!',
            title: 'Sucesso!',
            type: ToastType.success
          });
        });
    }
  }

  updateForm(litigio: Litigio) {
    this.form.patchValue(litigio);
    // if (litigio.data) {
    //   this.form.get('data').setValue(litigio.data.substring(0, 10));
    // }
  }

  upload(files: FileList) {
    this.spinnerArquivos = true;
    this.service.uploadFiles(this.litigio.id, files)
      .pipe(switchMap(_ => this.service.getArquivos(this.litigio.id)),
        finalize(() => this.spinnerArquivos = false))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.litigio.id).subscribe(d => this.arquivos = d);
  }

  put(litigio: Litigio) {
    this.btnSalvarLoad.start();
    this.service.put(litigio.id, litigio)
      .pipe(tap(_ => this.toasts.showMessage({
        message: 'Litígio salvo com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })), finalize(() => this.btnSalvarLoad.stop()))
      .subscribe(_ => this.router.navigate(['negociacoes/litigios', litigio.id]));
  }

  post(litigio: Litigio) {
    this.btnSalvarLoad.start();
    this.service.post(litigio)
      .pipe(tap(_ => this.toasts.showMessage({
        message: 'Litígio salvo com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })), finalize(() => this.btnSalvarLoad.stop()))
      .subscribe(l => this.router.navigate(['negociacoes/litigios', l.id]));
  }

  salvar() {
    let litigio = <Litigio>this.form.getRawValue();
    litigio = Object.assign(litigio, {
      patronalId: litigio.patronal ? litigio.patronal.id : null,
      laboralId: litigio.laboral ? litigio.laboral.id : null,
      empresaId: litigio.empresa.id,
      patronal: null,
      laboral: null,
      empresa: null
    });
    if (this.litigio) {
      this.put(litigio);
    } else {
      this.post(litigio);
    }
  }

  linkLaboral(valor) {
    return `/sindicatos/laborais/${valor.id}`;
  }

  linkPatronal(valor) {
    return `/sindicatos/patronais/${valor.id}`;
  }

  salvarItem(item: ItemLitigio) {
    item.litigioId = this.litigio.id;
    this.itensApi.put(item.id, item)
      .pipe(switchMap(_ => this.service.get(this.litigio.id)))
      .subscribe((li: Litigio) => {
        this.litigio = li;
        this.toasts.showMessage({
          message: 'Reclamação atualizada!',
          title: 'Sucesso!',
          type: ToastType.success
        });
      });
  }

  deleteItem(item: ItemLitigio) {
    this.itensApi.delete(item.id)
      .subscribe(_ => {
        const itemRemovido = this.litigio.itens.find(i => i.id === item.id);
        this.litigio.itens.splice(this.litigio.itens.indexOf(itemRemovido));
        this.toasts.showMessage({
          message: 'Reclamação excluída!',
          title: 'Sucesso!',
          type: ToastType.success
        });
      });
  }

}
