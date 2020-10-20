import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { OutstandingpalletspersitePageRoutingModule } from './outstandingpalletspersite-routing.module';

import { OutstandingpalletspersitePage } from './outstandingpalletspersite.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    OutstandingpalletspersitePageRoutingModule
  ],
  declarations: [OutstandingpalletspersitePage]
})
export class OutstandingpalletspersitePageModule {}
