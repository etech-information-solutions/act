import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { AddsiteauditPageRoutingModule } from './addsiteaudit-routing.module';

import { AddsiteauditPage } from './addsiteaudit.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    AddsiteauditPageRoutingModule
  ],
  declarations: [AddsiteauditPage]
})
export class AddsiteauditPageModule {}
