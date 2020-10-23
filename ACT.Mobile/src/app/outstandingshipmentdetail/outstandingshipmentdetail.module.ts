import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { OutstandingshipmentdetailPageRoutingModule } from './outstandingshipmentdetail-routing.module';

import { OutstandingshipmentdetailPage } from './outstandingshipmentdetail.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    OutstandingshipmentdetailPageRoutingModule
  ],
  declarations: [OutstandingshipmentdetailPage]
})
export class OutstandingshipmentdetailPageModule {}
