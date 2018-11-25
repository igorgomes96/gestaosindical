import { empty } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, Input, OnDestroy, EventEmitter, Output } from '@angular/core';

declare var $: any;

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent implements OnInit, OnDestroy {

  @Input() title: string;
  @Input() sizeClass: string;
  @Output() close = new EventEmitter<boolean>();

  constructor(private router: Router, private route: ActivatedRoute) { }

  closeModal() {
    $('#modal-component').modal('hide');
    this.close.emit(true);
  }

  ngOnInit() {
    $('#modal-component').modal('show');
    $('#modal-component').on('hidden.bs.modal', () => {
      this.router.navigate(['../', { outlets: { popup: null }}]);
      this.close.emit(true);
    });
  }

  ngOnDestroy(): void {
    $('#modal-component').modal('hide');
  }
}
