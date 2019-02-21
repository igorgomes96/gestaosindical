import { Concorrente } from './../../model/negociacao';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';

@Component({
  selector: 'app-concorrente',
  templateUrl: './concorrente.component.html',
  styleUrls: ['./concorrente.component.css']
})
export class ConcorrenteComponent implements OnInit {

  negociacaoId: number;
  formConcorrente: FormGroup;
  constructor(private formBuilder: FormBuilder, private route: ActivatedRoute,
    private api: NegociacoesApiService,
    private router: Router) { }

  ngOnInit() {
    this.formConcorrente = this.formBuilder.group({
      nome: ['', Validators.required],
      reajuste: this.formBuilder.group({
        salario: [''],
        piso: [''],
        auxCreche: [''],
        vaVr: [''],
        vaVrFerias: [''],
        descontoVt: ['']
      })
    });

    this.route.paramMap.subscribe(x => this.negociacaoId = +x.get('id'));

  }

  salvar() {
    const conc: Concorrente = this.formConcorrente.value;
    conc.negociacaoId = this.negociacaoId;
    this.api.postConcorrente(this.negociacaoId, conc)
      .subscribe(_ => this.router.navigate(['', { outlets: { popup: null } }]));
  }

}
