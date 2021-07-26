import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { OutstandingpalletsperclientPageRoutingModule } from './outstandingpalletsperclient-routing.module';

import { OutstandingpalletsperclientPage } from './outstandingpalletsperclient.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    OutstandingpalletsperclientPageRoutingModule
  ],
  declarations: [OutstandingpalletsperclientPage]
})
export class OutstandingpalletsperclientPageModule {}
