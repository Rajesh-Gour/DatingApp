import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';
import { TestComponent } from '.c:/Users/rajmc/Rajesh Gour/Reading Material/UDeMy/DatingApp-SPA/src/test/test.component';

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      TestComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule
   ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
