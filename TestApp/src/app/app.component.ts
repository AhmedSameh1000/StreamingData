import { Component, OnInit } from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  weight: string = '';

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.StartStream();

    setInterval(() => {
      this.StartStream();
    }, 2000);
  }

  StartStream() {
    const eventSource = new EventSource(
      'https://localhost:7097/api/cmd/stream'
    );

    eventSource.onmessage = (event) => {
      console.log('New Data:', event.data);

      const match = event.data.match(/([\d,.]+)/);
      if (match) {
        this.weight = match[1];
      } else {
        this.weight = '0.00';
      }

      this.cdr.detectChanges();
    };

    eventSource.onerror = (err) => {
      console.error('Stream connection error, trying to reconnect...', err);
      eventSource.close();
      setTimeout(() => {
        this.StartStream();
      }, 5000);
    };
  }
}
