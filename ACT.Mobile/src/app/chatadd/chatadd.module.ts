import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { ChataddPageRoutingModule } from './chatadd-routing.module';

import { ChataddPage } from './chatadd.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ChataddPageRoutingModule
  ],
  declarations: [ChataddPage]
})
export class ChataddPageModule {}
