import { ActivatedRoute, Router } from '@angular/router';
import { map, tap } from 'rxjs/operators';
import { Component, OnInit } from '@angular/core';
import { CalendarView, CalendarEvent, CalendarEventAction, CalendarEventTimesChangedEvent } from 'angular-calendar';
import { Subject } from 'rxjs';
import {
  startOfDay,
  endOfDay,
  subDays,
  addDays,
  endOfMonth,
  isSameDay,
  isSameMonth,
  addHours
} from 'date-fns';
import { NegociacoesApiService } from 'src/app/shared/api/negociacoes-api.service';

const colors: any = {
  red: {
    primary: '#ad2121',
    secondary: '#FAE3E3'
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF'
  },
  yellow: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  }
};

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {

  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();
  events: CalendarEvent[] = [];


  activeDayIsOpen = false;

  constructor(private api: NegociacoesApiService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.load(this.viewDate.getMonth() + 1);
  }

  load(mes: number) {
    this.api.getCalendar(mes)
    .pipe(
      map((evs: CalendarEvent[]) => evs.map(ev => Object.assign(ev, { start: new Date(ev.start), end: new Date(ev.end) })))
    ).subscribe((ev: CalendarEvent[]) => this.events = ev);
  }

  handleEvent($event: any) {
    if ($event && $event.hasOwnProperty('title')) {
      const title = $event['title'];
      const pattern = /\d.+? /;
      if (pattern.test(title)) {
        const id = title.match(/\d.+? /)[0].toString().trim();
        this.router.navigate(['../', 'gestao', id], { relativeTo: this.route });
      }
    }
  }

  viewDateChange($event: Date) {
    this.activeDayIsOpen = false;
    this.load((<Date>$event).getMonth() + 1);
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      this.viewDate = date;
      if ((isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) || events.length === 0) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
    }
  }

  eventTimesChanged({ event, newStart, newEnd }: CalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    this.refresh.next();
  }


}
