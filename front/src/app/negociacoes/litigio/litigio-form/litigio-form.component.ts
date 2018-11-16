import { Arquivo } from 'src/app/model/arquivo';
import { LitigiosApiService } from './../litigios-api.service';
import { ProcedimentoLitigio, Litigio, Referente } from './../../../model/litigio';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { endpoints } from 'src/environments/endpoints';
import { Router, ActivatedRoute } from '@angular/router';
import { filter, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { FormValidators } from 'src/app/shared/form-validators';

@Component({
  selector: 'app-litigio-form',
  templateUrl: './litigio-form.component.html',
  styleUrls: ['./litigio-form.component.css']
})
export class LitigioFormComponent implements OnInit {

  litigio: Litigio;
  arquivos: Arquivo[];
  form: FormGroup;
  ProcedimentoLitigio: typeof ProcedimentoLitigio = ProcedimentoLitigio;
  Referente: typeof Referente = Referente;
  urlEmpresaList = environment.api + endpoints.empresa;
  urlLaboralList = environment.api + endpoints.sindicatosLaborais;
  urlPatronalList = environment.api + endpoints.sindicatosPatronais;

  constructor(private formBuilder: FormBuilder,
    private service: LitigiosApiService,
    private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{value: '', disabled: true}],
      empresa: ['', Validators.required],
      referente: ['', Validators.required],
      laboral: ['', FormValidators.referenteA('referente')],
      patronal: ['', FormValidators.referenteA('referente')],
      data: ['', Validators.required],
      procedimento: ['', Validators.required],
      assuntos: [''],
      resumoAssuntos: [''],
      participantes: ['']
    });

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
          this.updateForm(this.litigio);
          return this.service.getArquivos(this.litigio.id);
      })
    ).subscribe(value => {
      this.arquivos = value;
    });
  }

  hasError(formControl: AbstractControl) {
    return formControl.dirty && formControl.invalid;
  }

  updateForm(litigio: Litigio) {
    this.form.patchValue(litigio);
    if (litigio.data) {
      this.form.get('data').setValue(litigio.data.substring(0, 10));
    }
  }

  upload(files: FileList) {
    this.service.uploadFiles(this.litigio.id, files)
    .pipe(switchMap(_ => this.service.getArquivos(this.litigio.id)))
    .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.litigio.id).subscribe(d => this.arquivos = d);
  }

  put(litigio: Litigio) {
    this.service.put(litigio.id, litigio)
    .subscribe(_ => this.router.navigate(['negociacoes/litigios']));
  }

  post(litigio: Litigio) {
    this.service.post(litigio)
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

}
